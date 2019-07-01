using System.Collections.Generic;
using Newtonsoft.Json;

namespace NetGltf.Json {
    public class Mesh {
        /// <summary>
        /// Optional user-defined name for this object.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }
        /// <summary>
        /// Defines the geometry to be renderered with a material.
        /// </summary>
        [JsonProperty("primitives")]
        public List<Primitive> Primitives { get; set; } = new List<Primitive> ();

        /// <summary>
        /// Defines the weights to be applied to the morph targets.
        /// </summary>
        [JsonProperty("weights")]
        public double[] Weights {get;set;}
    }
    public class Primitive {

        /// <summary>
        /// Maps attribute semantic names to the `Accessor`s containing the corresponding attribute data.
        /// </summary>
        [JsonProperty("attributes")]
        public Dictionary<Semantic, int> Attributes {get;set;}
        /// <summary>
        /// The index of the accessor that contains the indices.
        /// </summary>
        [JsonProperty("indices")]
        public int? Indices { get; set; }
        /// <summary>
        /// The index of the material to apply to this primitive when rendering
        /// </summary>
        [JsonProperty("material")]
        public int? Material { get; set; }
        /// <summary>
        /// The type of primitives to render.
        /// </summary>
        [JsonProperty("mode")]
        public Mode Mode { get; set; }

        [JsonProperty("targets")]
        public List<MorphTarget> Targets {get;set;}
    }

    public class MorphTarget {
        /// <summary>
        /// XYZ vertex position displacements of type `[f32; 3]`.
        /// </summary>
        [JsonProperty("POSITION")]
        public int? Positions {get;set;}
        /// <summary>
        /// YZ vertex normal displacements of type `[f32; 3]`.
        /// </summary>
        [JsonProperty("NORMAL")]
        public int? Normals {get;set;}
        /// <summary>
        /// XYZ vertex tangent displacements of type `[f32; 3]`.
        /// </summary>
        [JsonProperty("TANGENT")]
        public int? Tangents {get;set;}
    }

    /// <summary>
    /// The type of primitives to render.
    /// </summary>
    public enum Mode {
        /// Corresponds to `GL_POINTS`.
        Points = 0,

        /// Corresponds to `GL_LINES`.
        Lines,

        /// Corresponds to `GL_LINE_LOOP`.
        LineLoop,

        /// Corresponds to `GL_LINE_STRIP`.
        LineStrip,

        /// Corresponds to `GL_TRIANGLES`.
        Triangles,

        /// Corresponds to `GL_TRIANGLE_STRIP`.
        TriangleStrip,

        /// Corresponds to `GL_TRIANGLE_FAN`.
        TriangleFan,
    }

    public enum Semantic
    {
        ///<summary>XYZ vertex positions</summary>
        POSITION,
        ///<summary>Normalized XYZ vertex normals</summary>
        NORMAL,
        ///<summary>
        /// XYZW vertex tangents where 
        /// the w component is a sign value (-1 or +1) indicating 
        /// handedness of the tangent basis
        ///</summary>
        TANGENT,
        ///<summary>UV texture coordinates for the first set</summary>
        TEXCOORD_0,
        ///<summary>UV texture coordinates for the second set</summary>
        TEXCOORD_1,
        ///<summary>RGB or RGBA vertex color</summary>
        COLOR_0,
        ///<summary>Skinned Mesh Attributes</summary>
        JOINTS_0,
        ///<summary>Skinned Mesh Attributes</summary>
        WEIGHTS_0

    }
}