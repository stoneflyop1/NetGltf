using System.Collections.Generic;
using Newtonsoft.Json;

namespace NetGltf.Json
{
    /// <summary>
    /// A node in the node hierarchy.  When the node contains `skin`, all
    /// `mesh.primitives` must contain `JOINTS_0` and `WEIGHTS_0` attributes.
    /// A node can have either a `matrix` or any combination of
    /// `translation`/`rotation`/`scale` (TRS) properties. TRS properties are converted
    /// to matrices and postmultiplied in the `T * R * S` order to compose the
    /// transformation matrix; first the scale is applied to the vertices, then the
    /// rotation, and then the translation. If none are provided, the transform is the
    /// identity. When a node is targeted for animation (referenced by an
    /// animation.channel.target), only TRS properties may be present; `matrix` will not
    /// be present.
    /// </summary>
    public class Node
    {
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }
        /// <summary>
        /// The indices of this node's children.
        /// </summary>
        [JsonProperty("children")]
        public List<Index<Node>> Children { get; set; } = new List<Index<Node>>();
        /// <summary>
        /// The index of the mesh in this node.
        /// </summary>
        [JsonProperty("mesh")]
        public Index<Mesh> Mesh { get; set; }
        /// <summary>
        /// The index of the skin referenced by this node.
        /// </summary>
        [JsonProperty("skin")]
        public Index<Skin> Skin { get; set; }
        /// <summary>
        /// The weights of the instantiated Morph Target. Number of elements must match
        /// the number of Morph Targets of used mesh.
        /// </summary>
        [JsonProperty("weights")]
        public float[] Weights { get; set; }
        /// <summary>
        /// The index of the camera referenced by this node.
        /// </summary>
        [JsonProperty("camera")]
        public Index<Camera> Camera { get; set; }
        /// <summary>
        /// float[16]
        /// </summary>
        [JsonProperty("matrix")]
        public float[] Matrix { get; set; }
        /// <summary>
        /// float[4]
        /// </summary>
        [JsonProperty("rotation")]
        public float[] Rotation { get; set; }
        /// <summary>
        /// float[3]
        /// </summary>
        [JsonProperty("scale")]
        public float[] Scale { get; set; }
        /// <summary>
        /// float[3]
        /// </summary>
        [JsonProperty("translation")]
        public float[] Translation { get; set; }

    }

}
