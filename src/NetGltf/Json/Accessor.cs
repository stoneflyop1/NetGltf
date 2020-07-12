using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NetGltf.Json
{
    public class Accessor
    {
        [JsonProperty("bufferView")]
        public Index<BufferView> BufferView { get; set; }

        [JsonProperty("byteOffset")]
        public int ByteOffset { get; set; }

        [JsonProperty("componentType")]
        public CheckedValue<ComponentType, int> ComponentType { get; set; }

        [JsonProperty("count")]
        public int Count { get; set; }
        /// <summary>
        /// based on ComponentType, float or int
        /// </summary>
        [JsonProperty("min")]
        [JsonConverter(typeof(FloatArrayJsonConverter))]
        public float[] Min { get; set; }
        /// <summary>
        /// based on ComponentType, float or int
        /// </summary>
        [JsonProperty("max")]
        [JsonConverter(typeof(FloatArrayJsonConverter))]
        public float[] Max { get; set; }

        [JsonProperty("type")]
        //[JsonConverter(typeof(StringEnumConverter))]
        public CheckedValue<AccessorType, string> AccessorType { get; set; }

        /// <summary>
        /// for morph targets
        /// </summary>
        [JsonProperty("sparse")]
        public SparsedAccessor Sparse { get; set; }
    }

    public class SparsedAccessor
    {
        /// <summary>
        /// number of displaced elements
        /// </summary>
        [JsonProperty("count")]
        public int Count { get; set; }
        /// <summary>
        /// strictly increasing array of integers of size count and 
        /// specific componentType that stores the indices of those elements that 
        /// deviate from the initialization value.
        /// </summary>
        [JsonProperty("indices")]
        public SparseIndices Indices { get; set; }

        /// <summary>
        /// array of displaced elements corresponding to the indices in the indices array
        /// </summary>
        [JsonProperty("values")]
        public SparseValues Values { get; set; }

        public class SparseValues
        {
            [JsonProperty("bufferView")]
            public Index<BufferView> BufferView { get; set; }

            [JsonProperty("byteOffset")]
            public int ByteOffset { get; set; }
        }

        public class SparseIndices
        {
            [JsonProperty("bufferView")]
            public Index<BufferView> BufferView { get; set; }

            [JsonProperty("byteOffset")]
            public int ByteOffset { get; set; }

            [JsonProperty("componentType")]
            public CheckedValue<ComponentType, int> ComponentType { get; set; }
        }
    }

    /// <summary>
    /// Specifies whether an attribute, vector, or matrix.
    /// </summary>
    public enum AccessorType
    {
        /// Scalar quantity.
        SCALAR = 1,

        /// 2D vector.
        VEC2,

        /// 3D vector.
        VEC3,

        /// 4D vector.
        VEC4,

        /// 2x2 matrix.
        MAT2,

        /// 3x3 matrix.
        MAT3,

        /// 4x4 matrix.
        MAT4
    }

    /// <summary>
    /// The component data type.
    /// </summary>
    public enum ComponentType
    {
        /// Corresponds to `GL_BYTE`.
        I8 = 5120,

        /// Corresponds to `GL_UNSIGNED_BYTE`.
        U8,

        /// Corresponds to `GL_SHORT`.
        I16,

        /// Corresponds to `GL_UNSIGNED_SHORT`.
        U16,

        /// Corresponds to `GL_UNSIGNED_INT`.
        U32 = 5125,

        /// Corresponds to `GL_FLOAT`.
        F32,
    }
}
