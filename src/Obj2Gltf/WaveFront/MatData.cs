using System;
using System.Collections.Generic;
using System.Text;

namespace Obj2Gltf.WaveFront
{
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

        public abstract float[] ToRGB();
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

        public override float[] ToRGB()
        {
            return new[] { R, G, B };
        }

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

        // https://stackoverflow.com/questions/43494018/converting-xyz-color-to-rgb
        public override float[] ToRGB()
        {
            var r = 3.2404542f * X - 1.5371385f * Y - 0.4985314f * Z;
            var g = -0.9692660f * X + 1.8760108f * Y + 0.0415560f * Z;
            var b = 0.0556434f * X - 0.2040259f * Y + 1.0572252f * Z;
            return new[] { r, g, b };
        }

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

        public override float[] ToRGB()
        {
            // https://mathematica.stackexchange.com/questions/57389/convert-spectral-distribution-to-rgb-color
            //TODO:
            return new[] { 0.315f, 0.315f, 0.315f };
        }

        public string RflFile { get; }
        public float Factor { get; }

        public override string ToString()
        {
            return $"spectral {RflFile} {Factor}";
        }
    }
}
