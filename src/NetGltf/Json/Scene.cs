using System.Collections.Generic;
using Newtonsoft.Json;

namespace NetGltf.Json
{
    /// <summary>
    /// The root `Node`s of a scene.
    /// </summary>
    public class Scene
    {
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }
        /// <summary>
        /// The indices of each root node.
        /// </summary>
        [JsonProperty("nodes")]
        public List<Index<Node>> Nodes { get; set; } = new List<Index<Node>>();
    }
}
