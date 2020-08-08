using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using NetGltf.Json;
using System.Linq;

namespace Obj2Gltf
{
    public class ModelConverter
    {
        private readonly string _objFile;
        private readonly string _objFolder;
        private readonly ConverterOptions _options;
        private readonly string _gltfFile;
        private readonly string _gltfFolder;
        private readonly Dictionary<string, int> _textureDict = new Dictionary<string, int>();
        private readonly Dictionary<string, string> _gltfTextureDict = new Dictionary<string, string>();

        public ModelConverter(string objFile, string gltfFile, ConverterOptions options)
        {
            _objFile = objFile;
            _objFolder = Path.GetDirectoryName(_objFile);
            _gltfFile = gltfFile;
            _gltfFolder = Path.GetDirectoryName(gltfFile);
            _options = options ?? new ConverterOptions();
        }

        public Model Run()
        {
            var model = new Model { Asset = new Asset() };
            var scene = new Scene();
            model.Scenes.Add(scene);

            model.Samplers.Add(new Sampler
            {
                MagFilter = new CheckedValue<MagFilter, int>(MagFilter.Linear),
                MinFilter = new CheckedValue<MinFilter, int>(MinFilter.NearestMipmapLinear),
                WrapS = new CheckedValue<WrappingMode, int>(WrappingMode.Repeat),
                WrapT = new CheckedValue<WrappingMode, int>(WrappingMode.Repeat)
            });

            // parse obj files
            var objModel = ObjParser.Parse(_objFile);
            
            var mtlModels = new List<WaveFront.MtlModel>();
            foreach(var mtl in objModel.MaterialLibaries)
            {
                var mtlM = MtlParser.Parse(Path.Combine(_objFolder, mtl));
                mtlModels.Add(mtlM);
                foreach(var m in mtlM.Materials)
                {
                    var matName = m.Key;
                    var mm = m.Value;
                    var matIndex = AddMaterial(model, matName, mm);
                }
            }

            

            return model;
        }

        private int AddMaterial(Model gltf, string name, WaveFront.Material mtl)
        {
            var mat = new Material();
            mat.Name = name;
            mat.PbrMetallicRoughness = new PbrMetallicRoughness();
            
            var matIndex = gltf.Materials.Count;

            float metallicFactor = 0.0f;
            float? roughnessFactor = null;
            // convertTraditionalToMetallicRoughness
            float[] specular = null;
            var specularShininess = mtl.SpecularExponent;
            if (mtl.Specular != null)
            {
                specular = mtl.Specular.ToRGB();
                metallicFactor = specular[0];
                // if from blinn-phong model 
                var specularIntensity = Luminance(specular);
                roughnessFactor = (mtl.SpecularExponent ?? 0.0f) / 1000;
                roughnessFactor = 1.0f - roughnessFactor;
                roughnessFactor = Clamp(roughnessFactor ?? 1.0f, 0.0f, 1.0f);
                metallicFactor = 0.0f;
                specular = new[] { metallicFactor, metallicFactor, metallicFactor, 1.0f };
                specularShininess = roughnessFactor;
            }

            var emissiveTexture = mtl.EmissiveMap;
            var normalTexture = mtl.BumpMap;
            var occlusionTexture = mtl.AmbientMap;
            var baseColorTexture = mtl.DiffuseMap;
            var alphaTexture = mtl.DissolveMap;
            var metallicTexture = mtl.SpecularMap;
            var roughnessTexture = String.Empty;
            //var metallicRoughnessTexture = createMetallicRoughnessTexture(
            //var diffuseAlphaTexture

            var emissiveFactor = mtl.Emissive?.ToRGB();
            var baseColorFactor = mtl.Diffuse?.ToRGB()?.ToList() ?? new List<float> { 0.315f, 0.315f, 0.315f };
            if (IsValidTexture(emissiveTexture))
            {
                var tIndex = ResolveTexture(gltf, emissiveTexture);
                if (tIndex != -1)
                {
                    emissiveFactor = new float[] { 1.0f, 1.0f, 1.0f };
                    mat.EmissiveTexture = new TextureInfo
                    {
                        Index = tIndex
                    };
                }
            }
            if (IsValidTexture(baseColorTexture))
            {
                var tIndex = ResolveTexture(gltf, baseColorTexture);
                if (tIndex != -1)
                {
                    baseColorFactor = new List<float> { 1.0f, 1.0f, 1.0f };
                    mat.PbrMetallicRoughness.BaseColorTexture = new TextureInfo
                    {
                        Index = tIndex
                    };
                }
            }
            if (IsValidTexture(metallicTexture))
            {
                metallicFactor = 1.0f;
            }
            if (IsValidTexture(roughnessTexture))
            {
                roughnessFactor = 1.0f;
            }
            if (IsValidTexture(normalTexture))
            {
                var tIndex = ResolveTexture(gltf, normalTexture);
                if (tIndex != -1)
                {
                    mat.NormalTexture = new NormalTexture
                    {
                        Index = tIndex,
                        Scale = 1
                    };
                }
            }
            if (IsValidTexture(occlusionTexture))
            {
                var tIndex = ResolveTexture(gltf, occlusionTexture);
                if (tIndex != -1)
                {
                    mat.OcclusionTexture = new OcclusionTexture
                    {
                        Index = tIndex
                    };
                }
            }

            baseColorFactor.Add(1.0f);
            var transparent = false;
            if (IsValidTexture(alphaTexture))
            {
                transparent = true;
            } else
            {
                var alpha = mtl.Dissolve ?? 1.0f;
                baseColorFactor[3] = alpha;
                transparent = alpha < 1.0;
            }

            mat.DoubleSided = transparent;
            mat.AlphaMode = new CheckedValue<AlphaMode, string>(transparent ? AlphaMode.BLEND : AlphaMode.OPAQUE);
            mat.PbrMetallicRoughness.BaseColorFactor = baseColorFactor.ToArray();
            mat.PbrMetallicRoughness.MetallicFactor = metallicFactor;
            mat.PbrMetallicRoughness.RoughnessFactor = roughnessFactor;

            gltf.Materials.Add(mat);

            return matIndex;
        }

        private bool IsValidTexture(string t)
        {
            return !String.IsNullOrEmpty(t);
        }

        private static float Clamp(float v, float min, float max)
        {
            if (v < min)
            {
                return min;
            }
            if (v > max) return max;
            return v;
        }
        /// <summary>
        /// Translate the blinn-phong model to the pbr metallic-roughness model
        /// Roughness factor is a combination of specular intensity and shininess
        /// Metallic factor is 0.0
        /// Textures are not converted for now
        /// </summary>
        /// <returns>specularIntensity</returns>
        private static float Luminance(float[] rgbs)
        {
            float r = rgbs[0];
            float g = rgbs[1];
            float b = rgbs[2];
            return r * 0.2125f + g * 0.7154f + b * 0.0721f;
        }

        private int ResolveTexture(Model gltf, string textureFile)
        {
            var txtFile = GetObjTextureFullpath(textureFile);
            if (!File.Exists(txtFile))
            {
                return -1;
            }
            if (_textureDict.TryGetValue(txtFile, out var index))
            {
                return index;
            }
            if (_options.SeparateTextures)
            {
                var uri = CopyTextureFile(txtFile);
                var image = new Image { 
                    Name = Path.GetFileNameWithoutExtension(txtFile), 
                    Uri = uri,
                    MimeType = GetMimeType(txtFile)
                };
                var imageIndex = gltf.Images.Count;
                gltf.Images.Add(image);
                gltf.Textures.Add(new Texture
                {
                    Source = imageIndex,
                    Sampler = 0
                });
                return gltf.Textures.Count - 1;
            }
            else //TODO:
            {

            }
            return -1;
        }

        private static string GetMimeType(string imageFile)
        {
            var ext = Path.GetExtension(imageFile).TrimStart('.').ToUpper();
            switch(ext)
            {
                case "PNG":
                    return "image/png";
                case "JPG":
                case "JPEG":
                default:
                    return "image/jpeg";
            }
        }

        private string GetObjTextureFullpath(string textureFile)
        {
            if (Path.IsPathRooted(textureFile))
            {
                return textureFile;
            }
            return Path.Combine(_objFolder, textureFile);
        }

        private string CopyTextureFile(string txtFile)
        {
            if (_gltfTextureDict.TryGetValue(txtFile, out var val))
            {
                return val;
            }
            var newFilename = Path.GetFileNameWithoutExtension(txtFile) + DateTime.Now.ToString("HHmmssfff");
            var ext = txtFile.Substring(txtFile.LastIndexOf('.'));
            var uri = newFilename + ext;
            var newPath = Path.Combine(_gltfFolder, uri);
            File.Copy(txtFile, newPath);
            _gltfTextureDict.Add(txtFile, uri);
            return uri;
        }

        private bool RequiresU32Indices(WaveFront.ObjModel objModel)
        {
            foreach(var g in objModel.Groups)
            {
                var node = g.Value;
                foreach(var p in node.Polygons)
                {
                    for(var i = p.Start; i < p.End; i++)
                    {
                        var pp = objModel.Polygons[(int)i];
                        var vertexCount = pp.Vertices.Count;
                        if (vertexCount > 65534) // Reserve the 65535 index for primitive restart
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

    }
}
