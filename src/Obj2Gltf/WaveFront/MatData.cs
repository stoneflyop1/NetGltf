using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Obj2Gltf.WaveFront
{
    public class MtlParser
    {
        private static MtlColor ParseColor(IList<string> args)
        {
            if (args.Count == 0)
            {
                throw new ArgumentException("not enough argments", nameof(args));
            }
            switch(args[0])
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
                    switch(key)
                    {
                        case "newmtl":
                            if (!String.IsNullOrEmpty(name))
                            {
                                model.Materials.Add(name, mat);
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

    public class MtlModel
    {
        public Dictionary<string, Material> Materials { get; } = new Dictionary<string, Material>();
    }
    /// <summary>
    /// http://paulbourke.net/dataformats/mtl/
    /// </summary>
    public class Material
    {
        /// <summary>
        /// Ka
        /// </summary>
        public MtlColor Ambient { get; set; }
        /// <summary>
        /// Kd
        /// </summary>
        public MtlColor Diffuse { get; set; }
        /// <summary>
        /// Ks
        /// </summary>
        public MtlColor Specular { get; set; }
        /// <summary>
        /// Ke
        /// </summary>
        public MtlColor Emissive { get; set; }
        /// <summary>
        /// Tf
        /// </summary>
        public MtlColor TransmissionFilter { get; set; }
        /// <summary>
        /// illum
        /// </summary>
        public uint? Illumination { get; set; }
        /// <summary>
        /// d
        /// </summary>
        public float? Dissolve { get; set; }
        /// <summary>
        /// Ns
        /// </summary>
        public float? SpecularExponent { get; set; }
        /// <summary>
        /// Ni
        /// </summary>
        public float? OpticalDensity { get; set; }
        /// <summary>
        /// map_Ka
        /// </summary>
        public string AmbientMap { get; set; }
        /// <summary>
        /// map_Kd
        /// </summary>
        public string DiffuseMap { get; set; }
        /// <summary>
        /// map_Ks
        /// </summary>
        public string SpecularMap { get; set; }
        /// <summary>
        /// map_Ke
        /// </summary>
        public string EmissiveMap { get; set; }
        /// <summary>
        /// map_d
        /// </summary>
        public string DissolveMap { get; set; }
        /// <summary>
        /// bump
        /// </summary>
        public string BumpMap { get; set; }
    }

    public abstract class MtlColor
    {
        public abstract ColorType ColorType { get; }
    }

    public enum ColorType
    {
        RGB,
        XYZ,
        Spectral
    }

    public class ColorRgb : MtlColor
    {
        public ColorRgb(float r) : this(r, r, r) { }
        public ColorRgb(float r, float g, float b)
        {
            R = r;
            G = g;
            B = b;
        }
        public override ColorType ColorType => ColorType.RGB;

        public float R { get; }

        public float G { get; }

        public float B { get; }

        public override string ToString()
        {
            return $"{R} {G} {B}";
        }
    }

    public class ColorXyz : MtlColor
    {
        public ColorXyz(float x) : this(x, x, x) { }
        public ColorXyz(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }
        public override ColorType ColorType => ColorType.XYZ;

        public float X { get; }
        public float Y { get; }
        public float Z { get; }

        public override string ToString()
        {
            return $"xyz {X} {Y} {Z}";
        }
    }

    public class ColorSpectral : MtlColor
    {
        public ColorSpectral(string rfl) : this(rfl, 1.0f) { }
        public ColorSpectral(string rfl, float factor)
        {
            RflFile = rfl;
            Factor = factor;
        }
        public override ColorType ColorType => ColorType.Spectral;

        public string RflFile { get; }
        public float Factor { get; }

        public override string ToString()
        {
            return $"spectral {RflFile} {Factor}";
        }
    }
}
