using System.Collections.Generic;
using Newtonsoft.Json;

namespace NetGltf.Json
{
    public class Skin
    {

        [JsonProperty("name")]
        public string Name {get;set;}
        /// <summary>
        /// The index of the accessor containing the 4x4 inverse-bind matrices.
        /// 
        /// When `None`,each matrix is assumed to be the 4x4 identity matrix 
        /// which implies that the inverse-bind matrices were pre-applied.
        /// </summary>
        [JsonProperty("inverseBindMatrices")]
        public Index<Accessor> InverseBindMatrices {get;set;}
        /// <summary>
        /// The array length must be the same as the `count` property of the
        ///  `inverse_bind_matrices` `Accessor` (when defined).
        /// </summary>
        [JsonProperty("joints")]
        public List<Index<Node>> Joints {get;set;}
        /// <summary>
        /// The index of the node used as a skeleton root.
        /// 
        /// When `None`, joints transforms resolve to scene root.
        /// </summary>
        [JsonProperty("skeleton")]
        public Index<Node> Skeleton {get;set;}
    }
}