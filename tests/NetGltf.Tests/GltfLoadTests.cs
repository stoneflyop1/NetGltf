using System;
using System.IO;
using NetGltf.Json;
using Newtonsoft.Json;
using Xunit;

namespace NetGltf.Tests {
    public class GltfLoadTests {
        /// <summary>
        /// relative to bin/Debug/netcoreapp2.2
        /// </summary>
        private static readonly string SampleRootPath = @"../../../../../gltfsamples";

        [Fact]
        public void LoadGltf_Test () {
            var rootPath = Path.GetFullPath (SampleRootPath);
            var gltfFile = Path.Combine (rootPath, "CesiumMan.gltf");
            var content = File.ReadAllText (gltfFile);
            var model = JsonConvert.DeserializeObject<Model> (content);
            Assert.True (model.Scenes.Count > 0);
            var str = JsonConvert.SerializeObject(model, new JsonSerializerSettings{NullValueHandling = NullValueHandling.Ignore});
            var path = Path.GetFullPath("test.gltf");
            File.WriteAllText(path, str);
        }
    }
}