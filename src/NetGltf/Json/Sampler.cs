using Newtonsoft.Json;

namespace NetGltf.Json
{
    /// <summary>
    /// Texture sampler properties for filtering and wrapping modes.
    /// </summary>
    public class Sampler
    {
        /// <summary>
        /// Optional user-defined name for this object.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }
        /// <summary>
        /// Magnification filter.
        /// </summary>
        [JsonProperty("magFilter")]
        public CheckedValue<MagFilter,int>? MagFilter { get; set; }
        /// <summary>
        /// Minification filter.
        /// </summary>
        [JsonProperty("minFilter")]
        public CheckedValue<MinFilter,int>? MinFilter { get; set; }
        /// <summary>
        /// `s` wrapping mode.
        /// </summary>
        [JsonProperty("wrapS")]
        public CheckedValue<WrappingMode,int>? WrapS { get; set; }
        /// <summary>
        /// `t` wrapping mode.
        /// </summary>
        [JsonProperty("wrapT")]
        public CheckedValue<WrappingMode,int>? WrapT { get; set; }
    }

    /// <summary>
    /// Minification filter.
    /// </summary>
    public enum MinFilter
    {
        /// Corresponds to `GL_NEAREST`.
        Nearest = 9728,

        /// Corresponds to `GL_LINEAR`.
        Linear,

        /// Corresponds to `GL_NEAREST_MIPMAP_NEAREST`.
        NearestMipmapNearest = 9984,

        /// Corresponds to `GL_LINEAR_MIPMAP_NEAREST`.
        LinearMipmapNearest,

        /// Corresponds to `GL_NEAREST_MIPMAP_LINEAR`.
        NearestMipmapLinear,

        /// Corresponds to `GL_LINEAR_MIPMAP_LINEAR`.
        LinearMipmapLinear,
    }
    /// <summary>
    /// Magnification filter.
    /// </summary>
    public enum MagFilter
    {
        /// Corresponds to `GL_NEAREST`.
        Nearest = 9728,

        /// Corresponds to `GL_LINEAR`.
        Linear,
    }
    /// <summary>
    /// Texture co-ordinate wrapping mode.
    /// </summary>
    public enum WrappingMode
    {
        /// Corresponds to `GL_CLAMP_TO_EDGE`.
        ClampToEdge = 33071,

        /// Corresponds to `GL_MIRRORED_REPEAT`.
        MirroredRepeat = 33648,

        /// Corresponds to `GL_REPEAT`.
        Repeat = 10497,
    }
}
