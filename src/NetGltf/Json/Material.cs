using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NetGltf.Json
{
    public class Material
    {

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("pbrMetallicRoughness")]
        public PbrMetallicRoughness PbrMetallicRoughness { get; set; }

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
        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty("alphaMode")]
        public AlphaMode AlphaMode { get; set; } = AlphaMode.OPAQUE;
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

    public class PbrMetallicRoughness
    {
        /// <summary>
        /// The base color of the material
        /// </summary>
        [JsonProperty("baseColorFactor")]
        public float[] BaseColorFactor { get; set; }

        [JsonProperty("baseColorTexture")]
        public TextureInfo BaseColorTexture { get; set; }

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
        OPAQUE = 1,

        /// The rendered output is either fully opaque or fully transparent depending on
        /// the alpha value and the specified alpha cutoff value.
        MASK,

        /// The rendered output is either fully opaque or fully transparent depending on
        /// the alpha value and the specified alpha cutoff value.
        BLEND,
    }
}
