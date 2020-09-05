using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace NetGltf.Json
{
    /// <summary>
    /// checked enum value from string or integer
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    /// <typeparam name="TVal"></typeparam>
    public struct CheckedValue<TEnum, TVal> where TEnum : struct, Enum
    {
        /// <summary>
        /// from enum
        /// </summary>
        /// <param name="enum"></param>
        public CheckedValue(TEnum @enum)
        {
            _value = @enum;
            if (typeof(TVal) == typeof(String))
            {
                Value = @enum.ToString();
            }
            else
            {
                Value = Convert.ChangeType(@enum, typeof(int));
            }
            IsValid = true;
        }
        /// <summary>
        /// from string or integer
        /// </summary>
        /// <param name="val"></param>
        public CheckedValue(object val)
        {
            Value = val;
            IsValid = val is TVal;
            if (IsValid) {
                var str = val as string;
                if (str != null) {
                    IsValid = Enum.TryParse(str, true, out _value);
                } else {
                    IsValid = Enum.IsDefined(typeof(TEnum), val);
                    _value = (TEnum)Enum.ToObject(typeof(TEnum), val);
                }
            }
            else {
                _value = default;
            }
        }
        /// <summary>
        /// string/integer value
        /// </summary>
        public object Value {get; private set;}
        /// <summary>
        /// If not valid, throws InvalidOperationException
        /// </summary>
        /// <exception cref="InvalidOperationException">value is not valid!</exception>
        /// <returns></returns>
        public TEnum GetValidValue()
        {
            if (!IsValid) {
                throw new InvalidOperationException("value is not valid!");
            }
            return _value;
        }
        /// <summary>
        /// whether value can be converted to a valid enum
        /// </summary>
        public bool IsValid {get; private set;}

        private TEnum _value;
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Value.ToString();
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public enum ValidationErrorKind
    {
        /// <summary>
        /// 
        /// </summary>
        IndexOutOfBounds,
        /// <summary>
        /// Invalid value found
        /// </summary>
        Invalid,
        /// <summary>
        /// 
        /// </summary>
        Missing
    }
    /// <summary>
    /// 
    /// </summary>
    public class ValidationError
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="kind"></param>
        /// <param name="path"></param>
        public ValidationError(ValidationErrorKind kind, string path)
        {
            Kind = kind;
            Path = path;
        }
        /// <summary>
        /// 
        /// </summary>
        public ValidationErrorKind Kind { get;  }
        /// <summary>
        /// json path
        /// </summary>
        public string Path { get;  }

    }
    /// <summary>
    /// 
    /// </summary>
    public static class ModelValidationExtensions
    {
        private static bool IsDefined<TEnum>(TEnum @enum) where TEnum: struct
        {
            return Enum.IsDefined(typeof(TEnum), @enum);
        }
        private static void ValidateBufferViews(Model model, List<ValidationError> errors)
        {
            if (model.BufferViews != null && model.Buffers != null)
            {
                for (var i = 0; i < model.BufferViews.Count; i++)
                {
                    var view = model.BufferViews[i];
                    var index = view.Buffer;
                    if (index != null)
                    {
                        if (index.Value < 0 || index.Value >= model.Buffers.Count)
                        {
                            errors.Add(new ValidationError(
                                ValidationErrorKind.IndexOutOfBounds, $"bufferView_{i}_buffer"));
                        }
                    }

                    if (view.Target != null && !view.Target.Value.IsValid)
                    {
                        errors.Add(new ValidationError(
                                ValidationErrorKind.Invalid, $"bufferView_{i}_target"));
                    }
                }
            }
        }

        private static void ValidateAccessors(Model model, List<ValidationError> errors)
        {
            if (model.Accessors != null && model.BufferViews != null)
            {
                for (var i = 0; i < model.Accessors.Count; i++)
                {
                    var acc = model.Accessors[i];
                    var index = acc.BufferView;
                    if (index != null)
                    {
                        if (index.Value < 0 || index.Value >= model.BufferViews.Count)
                        {
                            errors.Add(new ValidationError(
                                ValidationErrorKind.IndexOutOfBounds, $"accessor_{i}_bufferview"));
                        }
                    }

                    if (!acc.ComponentType.IsValid)
                    {
                        errors.Add(new ValidationError(
                                ValidationErrorKind.Invalid, $"accessor_{i}_componenttype"));
                    }

                    if (!acc.AccessorType.IsValid)
                    {
                        errors.Add(new ValidationError(
                                ValidationErrorKind.Invalid, $"accessor_{i}_accessortype"));
                    }

                    if (acc.Sparse != null)
                    {
                        if (acc.Sparse.Indices != null)
                        {
                            var bvIndex = acc.Sparse.Indices.BufferView;
                            if (bvIndex != null)
                            {
                                if (bvIndex.Value < 0 || bvIndex.Value >= model.BufferViews.Count)
                                {
                                    errors.Add(new ValidationError(
                                        ValidationErrorKind.IndexOutOfBounds, $"accessor_{i}_sparse_index_bufferview"));
                                }
                            }
                            if (!acc.Sparse.Indices.ComponentType.IsValid)
                            {
                                errors.Add(new ValidationError(
                                ValidationErrorKind.Invalid, $"accessor_{i}_sparse_index_componenttype"));
                            }
                        }

                        if (acc.Sparse.Values != null)
                        {
                            var bvIndex = acc.Sparse.Values.BufferView;
                            if (bvIndex != null)
                            {
                                if (bvIndex.Value < 0 || bvIndex.Value >= model.BufferViews.Count)
                                {
                                    errors.Add(new ValidationError(
                                        ValidationErrorKind.IndexOutOfBounds, $"accessor_{i}_sparse_value_bufferview"));
                                }
                            }
                            
                        }
                    }
                }
            }
        }

        private static void ValidateMeshes(Model model, List<ValidationError> errors)
        {
            if (model.Meshes != null)
            {
                for (var i = 0; i < model.Meshes.Count; i++)
                {
                    var mesh = model.Meshes[i];
                    if (mesh.Primitives != null)
                    {
                        for (var j = 0; j < mesh.Primitives.Count; j++)
                        {
                            var p = mesh.Primitives[j];
                            if (p.Indices != null)
                            {
                                if (model.Accessors != null)
                                {
                                    var index = p.Indices;
                                    if (index.Value < 0 || index.Value >= model.Accessors.Count)
                                    {
                                        errors.Add(
                                            new ValidationError(ValidationErrorKind.IndexOutOfBounds,
                                                $"mesh_{i}_primitive_{j}_accessor"));
                                    }
                                }
                            }
                            if (p.Material != null)
                            {
                                if (model.Materials != null)
                                {
                                    var index = p.Material;
                                    if (index.Value < 0 || index.Value >= model.Materials.Count)
                                    {
                                        errors.Add(new ValidationError(
                                            ValidationErrorKind.IndexOutOfBounds,
                                            $"mesh_{i}_primitive_{j}_material"));
                                    }
                                }
                            }

                            if (p.Targets != null)
                            {
                                if (model.Accessors != null)
                                {
                                    for (var k = 0; k < p.Targets.Count; k++)
                                    {
                                        var tt = p.Targets[k];
                                        if (tt.Normals != null)
                                        {
                                            var index = tt.Normals;
                                            if (index.Value < 0 || index.Value >= model.Accessors.Count)
                                            {
                                                errors.Add(new ValidationError(
                                                    ValidationErrorKind.IndexOutOfBounds,
                                                    $"mesh_{i}_primitive_{j}_target_{k}_normal"));
                                            }
                                        }
                                        if (tt.Positions != null)
                                        {
                                            var index = tt.Positions;
                                            if (index.Value < 0 || index.Value >= model.Accessors.Count)
                                            {
                                                errors.Add(new ValidationError(
                                                    ValidationErrorKind.IndexOutOfBounds,
                                                    $"mesh_{i}_primitive_{j}_target_{k}_position"));
                                            }
                                        }
                                        if (tt.Tangents != null)
                                        {
                                            var index = tt.Tangents;
                                            if (index.Value < 0 || index.Value >= model.Accessors.Count)
                                            {
                                                errors.Add(new ValidationError(
                                                    ValidationErrorKind.IndexOutOfBounds,
                                                    $"mesh_{i}_primitive_{j}_target_{k}_tangent"));
                                            }
                                        }
                                    }
                                }
                                
                            }

                            if (p.Mode != null && !p.Mode.Value.IsValid)
                            {
                                errors.Add(new ValidationError(
                                            ValidationErrorKind.Invalid,
                                                $"mesh_{i}_primitive_{j}_mode"));
                            }

                            if (p.Attributes != null)
                            {
                                foreach(var k in p.Attributes.Keys)
                                {
                                    if (!IsDefined(k))
                                    {
                                        errors.Add(new ValidationError(
                                            ValidationErrorKind.Invalid,
                                                $"mesh_{i}_primitive_{j}_attirbute_{k.ToString()}"));
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private static void ValidateNodes(Model model, List<ValidationError> errors)
        {
            if (model.Nodes != null)
            {
                for(var i = 0; i < model.Nodes.Count;i++)
                {
                    var node = model.Nodes[i];
                    if (node.Children != null)
                    {
                        for(var j = 0; j < node.Children.Count;j++)
                        {
                            var index = node.Children[j];
                            if (index == null)
                            {
                                errors.Add(new ValidationError(
                                    ValidationErrorKind.Invalid, $"node_{i}_children_{j}"));
                            }
                            else
                            {
                                if (index.Value < 0 || index.Value >= model.Nodes.Count)
                                {
                                    errors.Add(new ValidationError(
                                        ValidationErrorKind.IndexOutOfBounds, $"node_{i}_children_{j}"));
                                }
                            }
                        }
                    }
                    if (node.Mesh != null)
                    {
                        if (model.Meshes != null)
                        {
                            var index = node.Mesh;
                            if (index != null)
                            {
                                if (index.Value < 0 || index.Value >= model.Meshes.Count)
                                {
                                    errors.Add(new ValidationError(
                                        ValidationErrorKind.IndexOutOfBounds, $"node_{i}_mesh"));
                                }
                            }
                        }
                        
                    }
                    if (node.Camera != null)
                    {
                        if (model.Cameras != null)
                        {
                            var index = node.Camera;
                            if (index.Value < 0 || index.Value >= model.Cameras.Count)
                            {
                                errors.Add(new ValidationError(
                                    ValidationErrorKind.IndexOutOfBounds, $"node_{i}_camera"));
                            }
                        }
                    }
                    if (node.Skin != null)
                    {
                        if (model.Skins != null)
                        {
                            var index = node.Skin;
                            if (index != null)
                            {
                                if (index.Value < 0 || index.Value >= model.Skins.Count)
                                {
                                    errors.Add(new ValidationError(
                                        ValidationErrorKind.IndexOutOfBounds, $"node_{i}_skin"));
                                }
                            }
                        }

                    }
                }
            }
        }

        private static void ValidateSkins(Model model, List<ValidationError> errors)
        {
            if (model.Skins != null)
            {
                for(var i = 0; i < model.Skins.Count; i++)
                {
                    var s = model.Skins[i];
                    if (s.InverseBindMatrices != null)
                    {
                        if (model.Accessors != null)
                        {
                            var index = s.InverseBindMatrices;
                            if (index.Value < 0 || index.Value >= model.Accessors.Count)
                            {
                                errors.Add(new ValidationError(
                                    ValidationErrorKind.IndexOutOfBounds, $"skin_{i}_accessor"));
                            }
                        }
                    }

                    if (model.Nodes != null)
                    {
                        if (s.Joints != null)
                        {
                            for (var j = 0; j < s.Joints.Count; j++)
                            {
                                var index = s.Joints[j];
                                if (index == null)
                                {
                                    errors.Add(new ValidationError(
                                            ValidationErrorKind.Invalid, $"skin_{i}_joint_{j}"));
                                }
                                else
                                {
                                    if (index.Value < 0 || index.Value >= model.Nodes.Count)
                                    {
                                        errors.Add(new ValidationError(
                                            ValidationErrorKind.IndexOutOfBounds, $"skin_{i}_joint_{j}"));
                                    }
                                }
                                
                            }
                        }

                        if (s.Skeleton != null)
                        {
                            var index = s.Skeleton;
                            if (index.Value < 0 || index.Value >= model.Nodes.Count)
                            {
                                errors.Add(new ValidationError(
                                    ValidationErrorKind.IndexOutOfBounds, $"skin_{i}_skeleton"));
                            }
                        }
                    }

                    
                }
            }
        }

        private static void ValidateScenes(Model model, List<ValidationError> errors)
        {

            if (model.Scenes != null)
            {
                if (model.Nodes != null)
                {
                    for(var i = 0; i < model.Scenes.Count; i++)
                    {
                        var s = model.Scenes[i];
                        if (s.Nodes != null)
                        {
                            for(var j = 0; j < s.Nodes.Count; j++)
                            {
                                var index = s.Nodes[j];
                                if (index == null)
                                {
                                    errors.Add(new ValidationError(
                                            ValidationErrorKind.Invalid, $"scene_{i}_node_{j}"));
                                }
                                else
                                {
                                    if (index.Value < 0 || index.Value >= model.Nodes.Count)
                                    {
                                        errors.Add(new ValidationError(
                                            ValidationErrorKind.IndexOutOfBounds, $"scene_{i}_node_{j}"));
                                    }
                                }
                            }
                        }
                    }
                }

                if (model.Scene != null)
                {
                    var index = model.Scene;
                    if (index.Value < 0 || index.Value >= model.Scenes.Count)
                    {
                        errors.Add(new ValidationError(
                            ValidationErrorKind.IndexOutOfBounds, $"scene"));
                    }
                }
            }
        }

        private static void ValidateMaterials(Model model, List<ValidationError> errors)
        {
            if (model.Materials != null)
            {
                for(var i = 0; i < model.Materials.Count;i++)
                {
                    var mat = model.Materials[i];
                    if (mat.PbrMetallicRoughness == null) continue;
                    if (mat.PbrMetallicRoughness.BaseColorTexture == null) continue;
                    var index = mat.PbrMetallicRoughness.BaseColorTexture.Index;
                    if (index != null)
                    {
                        if (model.Textures != null)
                        {
                            if (index.Value < 0 || index.Value >= model.Textures.Count)
                            {
                                errors.Add(new ValidationError(
                                    ValidationErrorKind.IndexOutOfBounds, $"material_{i}_basecolor_texture"));
                            }
                        }
                    }

                    if (mat.AlphaMode != null && !mat.AlphaMode.Value.IsValid)
                    {
                        errors.Add(new ValidationError(
                                    ValidationErrorKind.Invalid, $"material_{i}_alphamode_{mat.AlphaMode.ToString()}"));
                    }
                }
            }
        }

        private static void ValidateTextures(Model model, List<ValidationError> errors)
        {
            if (model.Textures != null)
            {
                for(var i = 0; i < model.Textures.Count;i++)
                {
                    var t = model.Textures[i];

                    if (t.Sampler != null)
                    {
                        if (model.Samplers != null)
                        {
                            var index = t.Sampler;
                            if (index.Value < 0 || index.Value >= model.Samplers.Count)
                            {
                                errors.Add(new ValidationError(
                                    ValidationErrorKind.IndexOutOfBounds, $"texture_{i}_sampler"));
                            }
                        }
                    }
                    if (t.Source != null)
                    {
                        if (model.Images != null)
                        {
                            var index = t.Source;
                            if (index.Value < 0 || index.Value >= model.Images.Count)
                            {
                                errors.Add(new ValidationError(
                                    ValidationErrorKind.IndexOutOfBounds, $"texture_{i}_image"));
                            }
                        }
                    }
                }
            }
        }

        private static void ValidateImages(Model model, List<ValidationError> errors)
        {
            if (model.Images != null)
            {
                for(var i = 0; i < model.Images.Count; i++)
                {
                    var img = model.Images[i];
                    if (img.BufferView != null)
                    {
                        if (model.BufferViews != null)
                        {
                            var index = img.BufferView;
                            if (index.Value < 0 || index.Value >= model.BufferViews.Count)
                            {
                                errors.Add(new ValidationError(
                                    ValidationErrorKind.IndexOutOfBounds, $"image_{i}_bufferview"));
                            }
                        }
                        
                    }
                    if (!img.IsValidImageType())
                    {
                        errors.Add(new ValidationError(
                                    ValidationErrorKind.Invalid, $"image_{i}_mimetype_{img.MimeType}"));
                    }
                }
            }
        }

        private static void ValidateAnimations(Model model, List<ValidationError> errors)
        {
            if (model.Animations != null)
            {
                for(var i = 0; i < model.Animations.Count; i++)
                {
                    var a = model.Animations[i];
                    if (a.Channels != null)
                    {
                        for(var j = 0; j < a.Channels.Count; j++)
                        {
                            var c = a.Channels[j];
                            if (c.Sampler != null)
                            {
                                var index = c.Sampler;
                                if (index != null)
                                {
                                    
                                    if (index.Value < 0 || index.Value >= a.Samplers.Count)
                                    {
                                        errors.Add(new ValidationError(
                                            ValidationErrorKind.IndexOutOfBounds, 
                                            $"animation_{i}_channel_{j}_sampler"));
                                    }
                                }
                            }
                            if (c.Target != null)
                            {
                                var index = c.Target.Node;
                                if (index != null)
                                {
                                    if (model.Nodes != null)
                                    {
                                        if (index.Value < 0 || index.Value >= model.Nodes.Count)
                                        {
                                            errors.Add(new ValidationError(
                                                ValidationErrorKind.IndexOutOfBounds, 
                                                $"animation_{i}_channel_{j}_target_node"));
                                        }
                                    }
                                }
                                if (!c.Target.Path.IsValid)
                                {
                                    errors.Add(new ValidationError(
                                                ValidationErrorKind.Invalid,
                                                $"animation_{i}_channel_{j}_target_path_{c.Target.Path.ToString()}"));
                                }
                            }
                        }
                    }

                    if (a.Samplers != null)
                    {
                        if (model.Accessors != null)
                        {
                            for (var j = 0; j < a.Samplers.Count; j++)
                            {
                                var s = a.Samplers[j];
                                if (s.Input != null)
                                {
                                    var index = s.Input;
                                    if (index.Value < 0 || index.Value >= model.Accessors.Count)
                                    {
                                        errors.Add(new ValidationError(
                                            ValidationErrorKind.IndexOutOfBounds,
                                            $"animation_{i}_sampler_{j}_input_accessor"));
                                    }
                                }
                                if (s.Output != null)
                                {
                                    var index = s.Output;
                                    if (index.Value < 0 || index.Value >= model.Accessors.Count)
                                    {
                                        errors.Add(new ValidationError(
                                            ValidationErrorKind.IndexOutOfBounds,
                                            $"animation_{i}_sampler_{j}_output_accessor"));
                                    }
                                }
                                if (!s.Interpolation.IsValid)
                                {
                                    errors.Add(new ValidationError(
                                            ValidationErrorKind.IndexOutOfBounds,
                                            $"animation_{i}_sampler_{j}_interpolation_{s.Interpolation.ToString()}"));
                                }
                            }
                        }
                        
                    }
                }
            }
        }
        /// <summary>
        /// validate glTF model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static List<ValidationError> Validate(this Model model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

            var errors = new List<ValidationError>();

            ValidateBufferViews(model, errors);
            ValidateAccessors(model, errors);
            ValidateMeshes(model, errors);
            ValidateNodes(model, errors);
            ValidateSkins(model, errors);
            ValidateScenes(model, errors);
            ValidateMaterials(model, errors);
            ValidateTextures(model, errors);
            ValidateImages(model, errors);
            ValidateAnimations(model, errors);

            return errors;
        }
    }
}
