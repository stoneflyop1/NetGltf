using System.Collections.Generic;
using Newtonsoft.Json;

namespace NetGltf.Json {
    public class Model {
        [JsonProperty ("asset")]
        public Asset Asset { get; set; }

        [JsonProperty ("buffers")]
        public List<Buffer> Buffers { get; set; } = new List<Buffer> ();

        [JsonProperty ("bufferViews")]
        public List<BufferView> Views { get; set; } = new List<BufferView> ();

        [JsonProperty ("accessors")]
        public List<Accessor> Accessors { get; set; } = new List<Accessor> ();

        [JsonProperty ("meshes")]
        public List<Mesh> Meshes { get; set; } = new List<Mesh> ();

        [JsonProperty ("nodes")]
        public List<Node> Nodes { get; set; } = new List<Node> ();

        [JsonProperty ("scenes")]
        public List<Scene> Scenes { get; set; } = new List<Scene> ();

        [JsonConverter(typeof(IndexConverter))]
        [JsonProperty ("scene")]
        public Index<Scene> Scene { get; set; }

        [JsonProperty ("materials")]
        public List<Material> Materials { get; set; } = new List<Material> ();

        [JsonProperty ("textures")]
        public List<Texture> Textures { get; set; } = new List<Texture> ();

        [JsonProperty ("samplers")]
        public List<Sampler> Samplers { get; set; } = new List<Sampler> ();

        [JsonProperty ("images")]
        public List<Image> Images { get; set; } = new List<Image> ();
    }
}