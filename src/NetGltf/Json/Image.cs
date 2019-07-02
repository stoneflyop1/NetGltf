using System.Collections.Generic;
using Newtonsoft.Json;

namespace NetGltf.Json {
    public class Image {
        [JsonProperty ("name")]
        public string Name { get; set; }

        [JsonProperty ("uri")]
        public string Uri { get; set; }

        [JsonProperty ("bufferView")]
        [JsonConverter(typeof(IndexConverter))]
        public Index<BufferView> BufferView { get; set; }

        [JsonProperty ("mimeType")]
        public string MimeType { get; set; }
    }

    public static class ImageExtensions {
        public static readonly ISet<string> ValidTypes = new HashSet<string> (new [] {
            "image/jpeg",
            "image/png",
        });
    }
}