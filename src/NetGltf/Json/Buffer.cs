using Newtonsoft.Json;

namespace NetGltf.Json
{
    public class Buffer
    {
        [JsonProperty("bytelength")]
        public int Bytes {get;set;}
        /// <summary>
        /// If this is null, this buffer is in glb format
        /// </summary>
        /// <value></value>
        [JsonProperty("uri")]
        public string Uri {get;set;}
    }
}