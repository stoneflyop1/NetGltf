using System;
using System.Collections.Generic;
using System.Text;

namespace Obj2Gltf.WaveFront
{
    /// <summary>
    /// represents a mtl file model
    /// </summary>
    public class MtlModel
    {
        /// <summary>
        ///  material list
        /// </summary>
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
    /// <summary>
    /// color in mtl
    /// </summary>
    public abstract class MtlColor
    {
        /// <summary>
        /// 
        /// </summary>
        public abstract ColorType ColorType { get; }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract float[] ToRGB();
    }
    /// <summary>
    /// mtl color type
    /// </summary>
    public enum ColorType
    {
        /// <summary>
        /// 
        /// </summary>
        RGB,
        /// <summary>
        /// CIEXYZ
        /// </summary>
        XYZ,
        /// <summary>
        /// specifies colory using a spectral curve
        /// </summary>
        Spectral
    }
    /// <summary>
    /// 
    /// </summary>
    public class ColorRgb : MtlColor
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="r"></param>
        public ColorRgb(float r) : this(r, r, r) { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        public ColorRgb(float r, float g, float b)
        {
            R = r;
            G = g;
            B = b;
        }
        /// <summary>
        /// 
        /// </summary>
        public override ColorType ColorType => ColorType.RGB;
        /// <summary>
        /// to rgb value array (0~1)
        /// </summary>
        /// <returns></returns>
        public override float[] ToRGB()
        {
            return new[] { R, G, B };
        }
        /// <summary>
        /// 
        /// </summary>
        public float R { get; }
        /// <summary>
        /// 
        /// </summary>
        public float G { get; }
        /// <summary>
        /// 
        /// </summary>
        public float B { get; }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{R} {G} {B}";
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class ColorXyz : MtlColor
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        public ColorXyz(float x) : this(x, x, x) { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public ColorXyz(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }
        /// <summary>
        /// 
        /// </summary>
        public override ColorType ColorType => ColorType.XYZ;

        /// <summary>
        /// https://stackoverflow.com/questions/43494018/converting-xyz-color-to-rgb
        /// </summary>
        /// <returns></returns>
        public override float[] ToRGB()
        {
            var r = 3.2404542f * X - 1.5371385f * Y - 0.4985314f * Z;
            var g = -0.9692660f * X + 1.8760108f * Y + 0.0415560f * Z;
            var b = 0.0556434f * X - 0.2040259f * Y + 1.0572252f * Z;
            return new[] { r, g, b };
        }
        /// <summary>
        /// 
        /// </summary>
        public float X { get; }
        /// <summary>
        /// 
        /// </summary>
        public float Y { get; }
        /// <summary>
        /// 
        /// </summary>
        public float Z { get; }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"xyz {X} {Y} {Z}";
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class ColorSpectral : MtlColor
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rfl"></param>
        public ColorSpectral(string rfl) : this(rfl, 1.0f) { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rfl"></param>
        /// <param name="factor"></param>
        public ColorSpectral(string rfl, float factor)
        {
            RflFile = rfl;
            Factor = factor;
        }
        /// <summary>
        /// 
        /// </summary>
        public override ColorType ColorType => ColorType.Spectral;
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override float[] ToRGB()
        {
            // https://mathematica.stackexchange.com/questions/57389/convert-spectral-distribution-to-rgb-color
            //TODO:
            return new[] { 0.315f, 0.315f, 0.315f };
        }
        /// <summary>
        /// 
        /// </summary>
        public string RflFile { get; }
        /// <summary>
        /// a multiplier for the values in the .rfl file and defaults to 1.0, if not specified
        /// </summary>
        public float Factor { get; }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"spectral {RflFile} {Factor}";
        }
    }
}
