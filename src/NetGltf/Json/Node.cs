using System.Collections.Generic;
using Newtonsoft.Json;

namespace NetGltf.Json
{
    public class Node
    {
        [JsonProperty("name")]
        public string Name {get;set;}
        [JsonProperty("children")]
        public List<int> ChildrenIndices {get;} = new List<int>();

        [JsonProperty("mesh")]
        public int? Mesh {get;set;}

        [JsonProperty("weights")]
        public float[] Weights {get;set;}
    }

    public class MatrixNode : Node
    {
        [JsonProperty("camera")]
        public int? Camera {get;set;}
        /// <summary>
        /// float[16]
        /// </summary>
        /// <value></value>
        [JsonProperty("matrix")]
        public float[] Matrix {get;set;}
    }

    public class RstNode : Node
    {
        /// <summary>
        /// float[4]
        /// </summary>
        /// <value></value>
        [JsonProperty("rotation")]
        public float[] Rotation {get;set;}
        /// <summary>
        /// float[3]
        /// </summary>
        /// <value></value>
        [JsonProperty("scale")]
        public float[] Scale {get;set;}
        /// <summary>
        /// float[3]
        /// </summary>
        /// <value></value>
        [JsonProperty("translation")]
        public float[] Translation {get;set;}
    }
}