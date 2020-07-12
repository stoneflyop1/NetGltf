using Newtonsoft.Json;

namespace NetGltf.Json
{
    public class BufferView
    {
        /// <summary>
        /// Optional user-defined name for this object.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }
        /// <summary>
        /// The parent `Buffer`.
        /// </summary>
        [JsonProperty("buffer")]
        public Index<Buffer> Buffer { get; set; }
        /// <summary>
        /// The length of the `BufferView` in bytes.
        /// </summary>
        [JsonProperty("byteLength")]
        public int ByteLength { get; set; }
        /// <summary>
        /// Offset into the parent buffer in bytes.
        /// </summary>
        [JsonProperty("byteOffset")]
        public int ByteOffset { get; set; }
        /// <summary>
        /// The stride in bytes between vertex attributes or other interleavable data.
        /// When zero, data is assumed to be tightly packed.
        /// </summary>
        [JsonProperty("byteStride")]
        public int? ByetStride { get; set; }
        /// <summary>
        /// Optional target the buffer should be bound to.
        /// When bufferView.target is defined, 
        /// runtime must use it to determine data usage, 
        /// otherwise it could be inferred from mesh' accessor objects.
        /// </summary>
        [JsonProperty("target")]
        public CheckedValue<TargetType,int>? Target { get; set; }
    }

    public enum TargetType
    {
        ArrayBuffer = 34962,
        ElementArrayBuffer = 34963
    }
}
