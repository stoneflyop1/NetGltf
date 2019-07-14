using Newtonsoft.Json;

namespace NetGltf.Json
{
    public class Buffer
    {
        [JsonProperty("byteLength")]
        public int ByteLength { get; set; }
        /// <summary>
        /// If this is null, this buffer is in glb format
        /// </summary>
        [JsonProperty("uri")]
        public string Uri { get; set; }
    }
}
