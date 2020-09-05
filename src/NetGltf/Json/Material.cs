using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NetGltf.Json
{
    /// <summary>
    /// The material appearance of a primitive.
    /// </summary>
    public class Material
    {
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }
        /// <summary>
        /// A set of parameter values that are used to define the metallic-roughness
        /// material model from Physically-Based Rendering (PBR) methodology. When not
        /// specified, all the default values of `pbrMetallicRoughness` apply.
        /// </summary>

        [JsonProperty("pbrMetallicRoughness")]
        public PbrMetallicRoughness PbrMetallicRoughness { get; set; }
        /// <summary>
        /// tangent space normal map. The texture contains RGB components in linear
        /// space. Each texel represents the XYZ components of a normal vector in
        /// tangent space. Red [0 to 255] maps to X [-1 to 1]. Green [0 to 255] maps to
        /// Y [-1 to 1]. Blue [128 to 255] maps to Z [1/255 to 1]. The normal vectors
        /// use OpenGL conventions where +X is right and +Y is up. +Z points toward the
        /// viewer.
        /// </summary>
        [JsonProperty("normalTexture")]
        public NormalTexture NormalTexture { get; set; }
        /// <summary>
        /// The occlusion map texture. The occlusion values are sampled from the R
        /// channel. Higher values indicate areas that should receive full indirect
        /// lighting and lower values indicate no indirect lighting. These values are
        /// linear. If other channels are present (GBA), they are ignored for occlusion
        /// calculations.
        /// </summary>
        [JsonProperty("occlusionTexture")]
        public OcclusionTexture OcclusionTexture { get; set; }
        /// <summary>
        /// The emissive map controls the color and intensity of the light being emitted
        /// by the material. This texture contains RGB components in sRGB color space.
        /// If a fourth component (A) is present, it is ignored.
        /// </summary>
        [JsonProperty("emissiveTexture")]
        public TextureInfo EmissiveTexture {get;set;}
        /// <summary>
        /// The emissive color of a material. f32[3]
        /// </summary>
        [JsonProperty("emissiveFactor")]
        public float[] EmissiveFactor {get;set;}
        /// <summary>
        /// The alpha cutoff value of a material.
        /// </summary>
        [JsonProperty("alphaCutoff")]
        public float? AlphaCutoff {get;set;}
        /// <summary>
        /// The alpha rendering mode of the material.
        ///
        /// The material's alpha rendering mode enumeration specifying the
        /// interpretation of the alpha value of the main factor and texture.
        ///
        /// * In `Opaque` mode (default) the alpha value is ignored and the rendered
        ///   output is fully opaque.
        ///
        /// * In `Mask` mode, the rendered output is either fully opaque or fully
        ///   transparent depending on the alpha value and the specified alpha cutoff
        ///   value.
        ///
        /// * In `Blend` mode, the alpha value is used to composite the source and
        ///   destination areas and the rendered output is combined with the
        ///   background using the normal painting operation (i.e. the Porter and
        ///   Duff over operator).
        /// </summary>
        //[JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty("alphaMode")]
        public CheckedValue<AlphaMode,string>? AlphaMode { get; set; }
        /// <summary>
        /// Specifies whether the material is double-sided.
        ///
        /// * When this value is false, back-face culling is enabled.
        ///
        /// * When this value is true, back-face culling is disabled and double sided
        ///   lighting is enabled.
        ///
        /// The back-face must have its normals reversed before the lighting
        /// equation is evaluated.
        /// </summary>
        [JsonProperty("doubleSided")]
        public bool DoubleSided { get; set; }
    }
    /// <summary>
    /// A set of parameter values that are used to define the metallic-roughness
    /// material model from Physically-Based Rendering (PBR) methodology.
    /// </summary>
    public class PbrMetallicRoughness
    {
        /// <summary>
        /// The base color of the material
        /// </summary>
        [JsonProperty("baseColorFactor")]
        public float[] BaseColorFactor { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("baseColorTexture")]
        public TextureInfo BaseColorTexture { get; set; }
        /// <summary>
        /// The metallic-roughness texture.
        ///
        /// This texture has two components:
        ///
        /// The metalness values are sampled from the B channel.
        /// The roughness values are sampled from the G channel.
        /// These values are linear. If other channels are present (R or A),
        /// they are ignored for metallic-roughness calculations.
        /// </summary>
        [JsonProperty("metallicRoughnessTexture")]
        public TextureInfo MetallicRoughnessTexture { get; set; }

        /// <summary>
        /// The metalness of the material
        /// </summary>
        [JsonProperty("metallicFactor")]
        public float MetallicFactor { get; set; }

        /// <summary>
        /// The roughness of the material
        /// 
        /// * A value of 1.0 means the material is completely rough.
        /// * A value of 0.0 means the material is completely smooth.
        /// </summary>
        [JsonProperty("roughnessFactor")]
        public float? RoughnessFactor { get; set; }
    }

    /// <summary>
    /// The alpha rendering mode of a material.
    /// </summary>
    public enum AlphaMode
    {
        /// The alpha value is ignored and the rendered output is fully opaque.
        OPAQUE,

        /// The rendered output is either fully opaque or fully transparent depending on
        /// the alpha value and the specified alpha cutoff value.
        MASK,

        /// The rendered output is either fully opaque or fully transparent depending on
        /// the alpha value and the specified alpha cutoff value.
        BLEND,
    }
    /// <summary>
    /// Defines the normal texture of a material.
    /// </summary>
    public class NormalTexture 
    {
        /// <summary>
        /// The index of the texture.
        /// </summary>
        [JsonProperty("index")]
        public Index<Texture> Index {get;set;}
        /// <summary>
        /// The scalar multiplier applied to each normal vector of the texture.
        ///
        /// This value is ignored if normalTexture is not specified.
        /// </summary>
        [JsonProperty("scale")]
        public float Scale {get;set;}
        /// <summary>
        /// The set index of the texture's `TEXCOORD` attribute.
        /// </summary>
        [JsonProperty("texCoord")]
        public int TexCoord {get;set;}
    }
    /// <summary>
    /// Defines the occlusion texture of a material.
    /// </summary>
    public class OcclusionTexture
    {
        /// <summary>
        /// The index of the texture.
        /// </summary>
        [JsonProperty("index")]
        public Index<Texture> Index {get;set;}
        /// <summary>
        /// The scalar multiplier controlling the amount of occlusion applied.
        /// </summary>
        [JsonProperty("strength")]
        public float StrengthFactor {get;set;}
        /// <summary>
        /// The set index of the texture's `TEXCOORD` attribute.
        /// </summary>
        [JsonProperty("texCoord")]
        public int TexCoord {get;set;}
    }
}
