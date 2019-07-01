using Newtonsoft.Json;

namespace NetGltf.Json
{
    public class BufferView
    {
        [JsonProperty("buffer")]
        public int BufferIndex {get;set;}
        [JsonProperty("bytelength")]
        public int Bytes {get;set;}
        [JsonProperty("byteoffset")]
        public int ByteOffset {get;set;}
        [JsonProperty("byteStride")]
        public int? ByetStride {get;set;}

        [JsonProperty("target")]
        public int Target {get;set;}
    }
}