using Newtonsoft.Json;

namespace NetGltf.Json
{
    /// <summary>
    /// A buffer points to binary data representing geometry, animations, or skins.
    /// </summary>
    public class Buffer
    {
        /// <summary>
        /// The length of the buffer in bytes.
        /// </summary>
        [JsonProperty("byteLength")]
        public int ByteLength { get; set; }
        /// <summary>
        /// If this is null, this buffer is in glb format
        /// </summary>
        [JsonProperty("uri")]
        public string Uri { get; set; }
    }
}
