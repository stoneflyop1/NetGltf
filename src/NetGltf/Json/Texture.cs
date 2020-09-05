using Newtonsoft.Json;

namespace NetGltf.Json
{
    /// <summary>
    /// A texture and its sampler.
    /// </summary>
    public class Texture
    {
        /// <summary>
        /// Optional user-defined name for this object.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }
        /// <summary>
        /// The index of the sampler used by this texture.
        /// </summary>
        [JsonProperty("sampler")]
        public Index<Sampler> Sampler { get; set; }
        /// <summary>
        /// The index of the image used by this texture.
        /// </summary>
        [JsonProperty("source")]
        public Index<Image> Source { get; set; }
    }
    /// <summary>
    /// Reference to a `Texture`.
    /// </summary>
    public class TextureInfo
    {
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("index")]
        public Index<Texture> Index { get; set; }
        /// <summary>
        /// The set index of the texture's `TEXCOORD` attribute.
        /// </summary>
        [JsonProperty("tex_coord")]
        public int? TexCoord { get; set; }
    }
}
