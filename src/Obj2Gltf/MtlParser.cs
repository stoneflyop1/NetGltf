using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Obj2Gltf.WaveFront;

namespace Obj2Gltf
{
    /// <summary>
    /// parse mtl file
    /// </summary>
    public class MtlParser
    {
        private static MtlColor ParseColor(IList<string> args)
        {
            if (args.Count == 0)
            {
                throw new ArgumentException("not enough argments", nameof(args));
            }
            args = TextParser.ParseArgs(args);
            switch (args[0])
            {
                case "xyz":
                    if (args.Count == 2)
                    {
                        return new ColorXyz(float.Parse(args[1]));
                    }
                    if (args.Count >= 4)
                    {
                        return new ColorXyz(float.Parse(args[1]), float.Parse(args[2]), float.Parse(args[3]));
                    }
                    throw new ArgumentException("not enough argments, need 2 or 4", nameof(args));
                case "spectral":
                    if (args.Count == 2)
                    {
                        return new ColorSpectral(args[1]);
                    }
                    if (args.Count >= 3)
                    {
                        return new ColorSpectral(args[1], float.Parse(args[2]));
                    }
                    throw new ArgumentException("not enough argments, need 2 or 3", nameof(args));
                default:
                    if (args.Count == 1)
                    {
                        return new ColorRgb(float.Parse(args[0]));
                    }
                    if (args.Count == 3)
                    {
                        return new ColorRgb(float.Parse(args[0]), float.Parse(args[1]), float.Parse(args[2]));
                    }
                    throw new ArgumentException("not enough argments, need 1 or 3", nameof(args));
            }
        }

        private static string ParseMap(IList<string> args)
        {
            if (args.Count == 0)
            {
                throw new ArgumentException("not enough argments", nameof(args));
            }
            return args[0];
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputFile"></param>
        /// <param name="encoding">default utf-8</param>
        /// <returns></returns>
        public static MtlModel Parse(string inputFile, Encoding encoding = null)
        {
            encoding = encoding ?? TextParser.DefaultEncoding;

            using (var fs = new FileStream(inputFile, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var bs = new BufferedStream(fs))
            using (var sr = new StreamReader(bs, encoding))
            {
                var model = new MtlModel();
                string name = null;
                var mat = new Material();

                TextParser.Lex(sr, (key, args) =>
                {
                    switch (key)
                    {
                        case "newmtl":
                            if (!String.IsNullOrEmpty(name))
                            {
                                model.Materials.Add(name, mat);
                                mat = new Material();
                            }
                            name = args[0];
                            break;
                        case "Ka":
                            mat.Ambient = ParseColor(args);
                            break;
                        case "Kd":
                            mat.Diffuse = ParseColor(args);
                            break;
                        case "Ks":
                            mat.Specular = ParseColor(args);
                            break;
                        case "Ke":
                            mat.Emissive = ParseColor(args);
                            break;
                        case "Tf":
                            mat.TransmissionFilter = ParseColor(args);
                            break;
                        case "Ns":
                            if (float.TryParse(args[0], out var ns))
                            {
                                mat.SpecularExponent = ns;
                            }
                            break;
                        case "Ni":
                            if (float.TryParse(args[0], out var ni))
                            {
                                mat.OpticalDensity = ni;
                            }
                            break;
                        case "illum":
                            if (uint.TryParse(args[0], out var illum))
                            {
                                mat.Illumination = illum;
                            }
                            break;
                        case "d":
                            if (float.TryParse(args[0], out var d))
                            {
                                mat.Dissolve = d;
                            }
                            break;
                        case "map_Ka":
                            mat.AmbientMap = ParseMap(args);
                            break;
                        case "map_Kd":
                            mat.DiffuseMap = ParseMap(args);
                            break;
                        case "map_Ks":
                            mat.SpecularMap = ParseMap(args);
                            break;
                        case "map_Ke":
                            mat.EmissiveMap = ParseMap(args);
                            break;
                        case "bump":
                        case "map_bump":
                        case "map_Bump":
                            mat.BumpMap = ParseMap(args);
                            break;
                    }
                });

                if (!String.IsNullOrEmpty(name))
                {
                    model.Materials.Add(name, mat);
                }
                return model;
            }
        }
    }
}
