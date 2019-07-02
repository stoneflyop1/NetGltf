using System.Collections.Generic;
using Newtonsoft.Json;

namespace NetGltf.Json {
    public class Node {
        [JsonProperty ("name")]
        public string Name { get; set; }

        [JsonProperty ("children")]
        public List<Index<Node>> ChildrenIndices { get; set; }

        [JsonProperty ("mesh")]
        public Index<Mesh> Mesh { get; set; }

        [JsonProperty("skin")]
        public Index<Skin> Skin {get;set;}

        [JsonProperty ("weights")]
        public float[] Weights { get; set; }

        [JsonProperty ("camera")]
        public int? Camera { get; set; }
        /// <summary>
        /// float[16]
        /// </summary>
        /// <value></value>
        [JsonProperty ("matrix")]
        public float[] Matrix { get; set; }

        /// <summary>
        /// float[4]
        /// </summary>
        /// <value></value>
        [JsonProperty ("rotation")]
        public float[] Rotation { get; set; }
        /// <summary>
        /// float[3]
        /// </summary>
        /// <value></value>
        [JsonProperty ("scale")]
        public float[] Scale { get; set; }
        /// <summary>
        /// float[3]
        /// </summary>
        /// <value></value>
        [JsonProperty ("translation")]
        public float[] Translation { get; set; }

    }

}