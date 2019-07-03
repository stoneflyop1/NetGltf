using System;
using Newtonsoft.Json;

namespace NetGltf.Json
{
    /// <summary>
    /// Gltf Asset,
    /// see: https://github.com/KhronosGroup/glTF/blob/master/specification/2.0/README.md#asset
    /// </summary>
    public class Asset
    {
        /// <summary>
        /// Gltf version, default is 2.0
        /// </summary>
        /// <value>2.0</value>
        [JsonProperty("version")]
        public string Version { get; set; } = "2.0";
        /// <summary>
        /// generate from where or what, like obj or collada
        /// </summary>
        [JsonProperty("generator")]
        public string Generator { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [JsonProperty("copyright")]
        public string Copyright { get; set; } = "Jifeng (c) jeff-zhang@outlook.com";
    }
}
