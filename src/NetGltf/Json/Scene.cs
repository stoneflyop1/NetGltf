using System.Collections.Generic;
using Newtonsoft.Json;

namespace NetGltf.Json {
    public class Scene {
        [JsonProperty ("name")]
        public string Name { get; set; }

        [JsonProperty ("nodes")]
        public List<Index<Node>> NodeIndices { get; set; }
    }
}