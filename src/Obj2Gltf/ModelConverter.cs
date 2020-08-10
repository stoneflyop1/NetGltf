using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using NetGltf.Json;
using System.Linq;
using Obj2Gltf.WaveFront;
using Material = NetGltf.Json.Material;

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
        private readonly Dictionary<string, int> _matDict = new Dictionary<string, int>();
        private readonly Buffers _buffers = new Buffers();

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

            model.Samplers.Add(new Sampler
            {
                MagFilter = new CheckedValue<MagFilter, int>(MagFilter.Linear),
                MinFilter = new CheckedValue<MinFilter, int>(MinFilter.NearestMipmapLinear),
                WrapS = new CheckedValue<WrappingMode, int>(WrappingMode.Repeat),
                WrapT = new CheckedValue<WrappingMode, int>(WrappingMode.Repeat)
            });

            // parse obj files
            var objModel = ObjParser.Parse(_objFile);
            
            var mtlModels = new List<MtlModel>();
            foreach(var mtl in objModel.MaterialLibaries)
            {
                var mtlM = MtlParser.Parse(Path.Combine(_objFolder, mtl));
                mtlModels.Add(mtlM);
                foreach(var m in mtlM.Materials)
                {
                    var matName = m.Key;
                    var mm = m.Value;
                    var matIndex = AddMaterial(model, matName, mm);
                    _matDict.Add(matName, matIndex);
                }
            }

            AddNodes(model, objModel);

            return model;
        }

        private void AddNodes(Model model, ObjModel objModel)
        {
            var u32Indices = RequiresU32Indices(objModel);

            var scene = new Scene();
            model.Scenes.Add(scene);
            var nodeName = objModel.Name;
            if (String.IsNullOrEmpty(nodeName))
            {
                nodeName = "default";
            }
            var node = new Node { Name = nodeName };
            model.Nodes.Add(node);
            var polyMatDict = GetPolygonMatDict(objModel);
            foreach (var gd in objModel.Groups)
            {
                var key = gd.Key;
                var g = gd.Value;
                var mesh = new Mesh { Name = key };
                var faceList = new SortedList<int, List<int>>();
                foreach(var pp in g.Polygons)
                {
                    for (var i = pp.Start; i < pp.End; i++)
                    {
                        var pIndex = (int)i;
                        var matName = polyMatDict[pIndex];
                        var matId = _matDict[matName];
                        if (!faceList.ContainsKey(matId))
                        {
                            faceList.Add(matId, new List<int>());
                        }
                        faceList[matId].Add(pIndex);
                    }
                }
                var faceIndex = 0;
                foreach(var fd in faceList)
                {
                    var matIndex = fd.Key;
                    var polygons = fd.Value;
                    string faceName;
                    if (faceIndex > 0)
                    {
                        faceName = key + "_" + faceIndex;
                    }
                    else
                    {
                        faceName = key;
                    }
                    var p = new Primitive { Material = matIndex, Mode = new CheckedValue<Mode, int>(Mode.Triangles) };
                    p.Attributes = new Dictionary<Semantic, int>();
                    var state = new PrimitiveState();

                    foreach (var pIndex in polygons)
                    {
                        var poly = objModel.Polygons[pIndex];
                        if (poly.Vertices.Count == 3)
                        {
                            AddPolygon(p, state, model, objModel, poly);
                        }
                        else if (poly.Vertices.Count > 3)
                        {
                            //TODO:
                        }
                    }

                    // Accessors
                    // Position Accessors
                    var accessorIndex = model.Accessors.Count;
                    var accessorVertex = new Accessor
                    {
                        Min = new float[] { state.VertexXMM.Min, state.VertexYMM.Min, state.VertexZMM.Min },
                        Max = new float[] { state.VertexXMM.Max, state.VertexYMM.Max, state.VertexZMM.Max },
                        AccessorType = new CheckedValue<AccessorType, string>(AccessorType.VEC3),
                        Count = state.VertexCount,
                        ComponentType = new CheckedValue<ComponentType, int>(ComponentType.F32)
                    };
                    p.Attributes.Add(Semantic.POSITION, accessorIndex);
                    model.Accessors.Add(accessorVertex);
                    _buffers.Positions.Add(state.VertexBuffers);
                    _buffers.PositionAccessors.Add(accessorIndex);
                    // Normal Accessors
                    if (state.NormalCount > 0)
                    {
                        accessorIndex = model.Accessors.Count;
                        var accessorNormal = new Accessor
                        {
                            Min = new float[] { state.NormalXMM.Min, state.NormalYMM.Min, state.NormalZMM.Min },
                            Max = new float[] { state.NormalXMM.Max, state.NormalYMM.Max, state.NormalZMM.Max },
                            AccessorType = new CheckedValue<AccessorType, string>(AccessorType.VEC3),
                            Count = state.NormalCount,
                            ComponentType = new CheckedValue<ComponentType, int>(ComponentType.F32)
                        };
                        p.Attributes.Add(Semantic.NORMAL, accessorIndex);
                        model.Accessors.Add(accessorNormal);
                        _buffers.Normals.Add(state.NormalBuffers);
                        _buffers.NormalAccessors.Add(accessorIndex);
                    }
                    // UV Accessors
                    if (state.UvCount > 0)
                    {
                        accessorIndex = model.Accessors.Count;
                        var accessorUV = new Accessor
                        {
                            Min = new float[] { state.UvUMM.Min, state.UvVMM.Min },
                            Max = new float[] { state.UvUMM.Max, state.UvVMM.Max },
                            AccessorType = new CheckedValue<AccessorType, string>(AccessorType.VEC2),
                            ComponentType = new CheckedValue<ComponentType, int>(ComponentType.F32)
                        };
                        p.Attributes.Add(Semantic.TEXCOORD_0, accessorIndex);
                        model.Accessors.Add(accessorUV);
                        _buffers.Uvs.Add(state.TextureBuffers);
                        _buffers.UvAccessors.Add(accessorIndex);
                    }
                    else
                    {
                        model.Materials[matIndex].PbrMetallicRoughness.BaseColorTexture = null;
                    }
                    // Index Accessors
                    accessorIndex = model.Accessors.Count;
                    var componentType = u32Indices ? ComponentType.U32 : ComponentType.U16;
                    var mm = new MinMax();
                    UpdateMinMax(mm, state.Indices);
                    var accessor = new Accessor
                    {
                        Min = new float[] { mm.Min },
                        Max = new float[] { mm.Max },
                        AccessorType = new CheckedValue<AccessorType, string>(AccessorType.SCALAR),
                        ComponentType = new CheckedValue<ComponentType, int>(componentType)
                    };
                    _buffers.Indices.Add(GetBytes(state.Indices, u32Indices));
                    _buffers.IndexAccessors.Add(accessorIndex);
                    p.Indices = accessorIndex;
                    mesh.Primitives.Add(p);
                }
                var meshIndex = model.Meshes.Count;
                model.Meshes.Add(mesh);
                var nodeIndex = model.Nodes.Count;
                var cNode = new Node { Name = key, Mesh = meshIndex };
                model.Nodes.Add(cNode);
                model.Scenes[0].Nodes.Add(nodeIndex);
            }
        }

        private void AddPolygon(Primitive p, PrimitiveState state, Model model, ObjModel objModel, Polygon poly)
        {
            var v1 = poly.Vertices[0];
            var v2 = poly.Vertices[1];
            var v3 = poly.Vertices[2];
            var v1Index = (int)(v1.V - 1);
            var v2Index = (int)(v2.V - 1);
            var v3Index = (int)(v3.V - 1);
            var v1v = objModel.Positions[v1Index];
            var v2v = objModel.Positions[v2Index];
            var v3v = objModel.Positions[v3Index];
            UpdateMinMax(state.VertexXMM, v1v.X, v2v.X, v3v.X);
            UpdateMinMax(state.VertexYMM, v1v.Y, v2v.Y, v3v.Y);
            UpdateMinMax(state.VertexZMM, v1v.Z, v2v.Z, v3v.Z);

            var hasNormal = v1.VN.HasValue;
            var hasUV = v1.VT.HasValue;

            Normal n1, n2, n3;

            if (hasNormal)
            {
                var n1Index = (int)(v1.VN.Value - 1);
                var n2Index = (int)(v2.VN.Value - 1);
                var n3Index = (int)(v3.VN.Value - 1);
                n1 = objModel.Normals[n1Index];
                n2 = objModel.Normals[n2Index];
                n3 = objModel.Normals[n3Index];
                UpdateMinMax(state.NormalXMM, n1.X, n2.X, n3.X);
            }
            else
            {
                n1 = new Normal();
                n2 = n1;
                n3 = n1;
            }

            Uv t1, t2, t3;
            if (hasUV)
            {
                var t1Index = (int)(v1.VT.Value - 1);
                var t2Index = (int)(v2.VT.Value - 1);
                var t3Index = (int)(v3.VT.Value - 1);
                t1 = objModel.TextureCoords[t1Index];
                t2 = objModel.TextureCoords[t2Index];
                t3 = objModel.TextureCoords[t3Index];
                UpdateMinMax(state.UvUMM, t1.U, t2.U, t3.U);
                UpdateMinMax(state.UvVMM, 1 - t1.V, 1 - t2.V, 1 - t3.V);
            }
            else
            {
                t1 = new Uv();
                t2 = t1;
                t3 = t1;
            }

            if (state.AddVertex(v1))
            {
                state.VertexCount++;
                FillBytes(state.VertexBuffers, v1v.ToArray());
                if (hasNormal)
                {
                    state.NormalCount++;
                    FillBytes(state.NormalBuffers, n1.ToArray());
                }
                if (hasUV)
                {
                    state.UvCount++;
                    FillBytes(state.TextureBuffers, new Uv(t1.U, 1-t1.V).ToArray());
                }
            }

            if (state.AddVertex(v2))
            {
                state.VertexCount++;
                FillBytes(state.VertexBuffers, v2v.ToArray());
                if (hasNormal)
                {
                    state.NormalCount++;
                    FillBytes(state.NormalBuffers, n2.ToArray());
                }
                if (hasUV)
                {
                    state.UvCount++;
                    FillBytes(state.TextureBuffers, new Uv(t2.U, 1 - t2.V).ToArray());
                }
            }

            if (state.AddVertex(v3))
            {
                state.VertexCount++;
                FillBytes(state.VertexBuffers, v3v.ToArray());
                if (hasNormal)
                {
                    state.NormalCount++;
                    FillBytes(state.NormalBuffers, n3.ToArray());
                }
                if (hasUV)
                {
                    state.UvCount++;
                    FillBytes(state.TextureBuffers, new Uv(t3.U, 1 - t3.V).ToArray());
                }
            }

            var correctWinding = CheckWindingCorrect(v1v, v2v, v3v, n1);
            if (correctWinding)
            {
                state.Indices.AddRange(new[] { state.PolyVertexCache[v1], state.PolyVertexCache[v2], state.PolyVertexCache[v3] });
            }
            else
            {
                state.Indices.AddRange(new[] { state.PolyVertexCache[v1], state.PolyVertexCache[v3], state.PolyVertexCache[v2] });
            }

        }

        private bool CheckWindingCorrect(Point a, Point b, Point c, Normal normal)
        {
            var (leftX, leftY, leftZ) = (b.X - a.X, b.Y - a.Y, b.Z - a.Z);
            var (rightX, rightY, rightZ) = (c.X - a.X, c.Y - a.Y, c.Z - a.Z);
            var cc = Cross(leftX, leftY, leftZ, rightX, rightY, rightZ);
            return Dot(cc.x, cc.y, cc.z, normal.X, normal.Y, normal.Z) > 0;
        }

        private static (float x, float y, float z) Cross(float leftX, float leftY, float leftZ, float rightX, float rightY, float rightZ)
        {
            var x = leftY * rightZ - leftZ * rightY;
            var y = leftZ * rightX - leftX * rightZ;
            var z = leftX * rightY - leftY * rightX;
            return (x,y,z);
        }

        private static float Dot(float leftX, float leftY, float leftZ, float rightX, float rightY, float rightZ)
        {
            return leftX * rightX + leftY * rightY + leftZ * rightZ;
        }

        private static List<byte> GetBytes(IList<int> indices, bool useU32)
        {
            var bytes = new List<byte>();
            foreach(var i in indices)
            {
                if (useU32)
                {
                    bytes.AddRange(BitConverter.GetBytes((uint)i));
                }
                else
                {
                    bytes.AddRange(BitConverter.GetBytes((ushort)i));
                }
            }
            return bytes;
        }

        private static void FillBytes(List<byte> res, IList<float> vals)
        {
            foreach(var f in vals)
            {
                res.AddRange(BitConverter.GetBytes(f));
            }
        }

        private void UpdateMinMax(MinMax mm, float v1, float v2, float? v3)
        {
            UpdateMinMax(mm, v1);
            UpdateMinMax(mm, v2);
            if (v3.HasValue)
            {
                UpdateMinMax(mm, v3.Value);
            }
        }

        private void UpdateMinMax(MinMax mm, IList<int> indices)
        {
            foreach(var i in indices)
            {
                if (mm.Min > i)
                {
                    mm.Min = i;
                }
                if (mm.Max < i)
                {
                    mm.Max = i;
                }
            }
        }

        private void UpdateMinMax(MinMax mm, float v)
        {
            if (mm.Min > v)
            {
                mm.Min = v;
            }
            if (mm.Max < v)
            {
                mm.Max = v;
            }
        }

        private Dictionary<int, string> GetPolygonMatDict(ObjModel objModel)
        {
            var dict = new Dictionary<int, string>();
            foreach(var gd in objModel.Meshes)
            {
                var matName = gd.Key;
                foreach(var rs in gd.Value.Polygons)
                {
                    for(var i = rs.Start; i < rs.End;i++)
                    {
                        dict.Add((int)i, matName);
                    }
                }
            }
            return dict;
        }

        private class MinMax
        {
            public float Min { get; set; } = float.MaxValue;

            public float Max { get; set; } = float.MinValue;
        }

        private class PrimitiveState
        {
            public List<byte> VertexBuffers { get; } = new List<byte>();
            public List<byte> NormalBuffers { get; } = new List<byte>();
            public List<byte> TextureBuffers { get; } = new List<byte>();
            public List<int> Indices { get; } = new List<int>();

            public int VertexCount { get; set; }
            public int NormalCount { get; set; }
            public int UvCount { get; set; }

            public bool ContainsVertex(PolygonVertex v)
            {
                return PolyVertexCache.ContainsKey(v);
            }

            public bool AddVertex(PolygonVertex v)
            {
                if (PolyVertexCache.ContainsKey(v))
                {
                    return false;
                }
                PolyVertexCache.Add(v, PolyVertextCount++);
                return true;
            }

            public int PolyVertextCount { get; set; }
            public Dictionary<PolygonVertex, int> PolyVertexCache { get; } = new Dictionary<PolygonVertex, int>();

            public MinMax VertexXMM { get; } = new MinMax();
            public MinMax VertexYMM { get; } = new MinMax();
            public MinMax VertexZMM { get; } = new MinMax();

            public MinMax NormalXMM { get; } = new MinMax();
            public MinMax NormalYMM { get; } = new MinMax();
            public MinMax NormalZMM { get; } = new MinMax();

            public MinMax UvUMM { get; } = new MinMax();
            public MinMax UvVMM { get; } = new MinMax();
        }

        private class Buffers
        {
            public List<List<byte>> Positions { get; } = new List<List<byte>>();
            public List<List<byte>> Normals { get; } = new List<List<byte>>();
            public List<List<byte>> Uvs { get; } = new List<List<byte>>();
            public List<List<byte>> Indices { get; } = new List<List<byte>>();

            public List<int> PositionAccessors { get; } = new List<int>();
            public List<int> NormalAccessors { get; } = new List<int>();
            public List<int> UvAccessors { get; } = new List<int>();
            public List<int> IndexAccessors { get; } = new List<int>();

        }

        private bool RequiresU32Indices(ObjModel objModel)
        {
            foreach (var g in objModel.Groups)
            {
                var node = g.Value;
                foreach (var p in node.Polygons)
                {
                    for (var i = p.Start; i < p.End; i++)
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

    }
}
