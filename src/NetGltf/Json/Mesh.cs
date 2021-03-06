using System.Collections.Generic;
using Newtonsoft.Json;

namespace NetGltf.Json
{
    /// <summary>
    /// A set of primitives to be rendered.
    ///
    /// A node can contain one or more meshes and its transform places the meshes in
    /// the scene.
    /// </summary>
    public class Mesh
    {
        /// <summary>
        /// Optional user-defined name for this object.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }
        /// <summary>
        /// Defines the geometry to be renderered with a material.
        /// </summary>
        [JsonProperty("primitives")]
        public List<Primitive> Primitives { get; set; } = new List<Primitive>();

        /// <summary>
        /// Defines the weights to be applied to the morph targets.
        /// </summary>
        [JsonProperty("weights")]
        public double[] Weights { get; set; }
    }
    /// <summary>
    /// Geometry to be rendered with the given material.
    /// </summary>
    public class Primitive
    {
        /// <summary>
        /// Maps attribute semantic names to the `Accessor`s containing the corresponding attribute data.
        /// </summary>
        [JsonProperty("attributes")]
        public Dictionary<Semantic, int> Attributes { get; set; }
        /// <summary>
        /// The index of the accessor that contains the indices.
        /// </summary>
        [JsonProperty("indices")]
        public Index<Accessor> Indices { get; set; }
        /// <summary>
        /// The index of the material to apply to this primitive when rendering
        /// </summary>
        [JsonProperty("material")]
        public Index<Material> Material { get; set; }
        /// <summary>
        /// The type of primitives to render.
        /// </summary>
        [JsonProperty("mode")]
        public CheckedValue<Mode,int>? Mode { get; set; }
        /// <summary>
        /// An array of Morph Targets, each  Morph Target is a dictionary mapping
        /// attributes (only `POSITION`, `NORMAL`, and `TANGENT` supported) to their
        /// deviations in the Morph Target.
        /// </summary>
        [JsonProperty("targets")]
        public List<MorphTarget> Targets { get; set; }
    }
    /// <summary>
    /// A dictionary mapping attributes to their deviations in the Morph Target.
    /// </summary>
    public class MorphTarget
    {
        /// <summary>
        /// XYZ vertex position displacements of type `[f32; 3]`.
        /// </summary>
        [JsonProperty("POSITION")]
        public Index<Accessor> Positions { get; set; }
        /// <summary>
        /// YZ vertex normal displacements of type `[f32; 3]`.
        /// </summary>
        [JsonProperty("NORMAL")]
        public Index<Accessor> Normals { get; set; }
        /// <summary>
        /// XYZ vertex tangent displacements of type `[f32; 3]`.
        /// </summary>
        [JsonProperty("TANGENT")]
        public Index<Accessor> Tangents { get; set; }
    }

    /// <summary>
    /// The type of primitives to render.
    /// </summary>
    public enum Mode
    {
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
    /// <summary>
    /// Vertex attribute semantic name.
    /// </summary>
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
