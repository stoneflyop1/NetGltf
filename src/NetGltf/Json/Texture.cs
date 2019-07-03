using Newtonsoft.Json;

namespace NetGltf.Json
{
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

    public class TextureInfo
    {
        [JsonProperty("index")]
        public Index<Texture> Index { get; set; }

        [JsonProperty("tex_coord")]
        public int TexCoord { get; set; }
    }
}