using System;
using System.IO;
using System.Linq;
using NetGltf.Json;
using Newtonsoft.Json;
using Xunit;

namespace NetGltf.Tests
{
    public class GltfLoadTests
    {
        /// <summary>
        /// relative to bin/Debug/netcoreapp2.2
        /// </summary>
        internal static readonly string SampleRootPath = @"../../../../../gltfsamples";

        [Fact]
        public void LoadGltf_Test()
        {
            var rootPath = Path.GetFullPath(SampleRootPath);
            var gltfFile = Path.Combine(rootPath, "CesiumMan.gltf");
            var content = File.ReadAllText(gltfFile);
            var model = JsonConvert.DeserializeObject<Model>(content);
            var node = model.Nodes.FirstOrDefault(c => c.Children.Count > 0);
            var sceneIndex = model.Scene;
            Assert.True(model.Scenes.Count > 0);
            var str = JsonConvert.SerializeObject(model, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new ArrayContractResolver(),
                Converters = new JsonConverter[] { new IndexConverter() }
            });
            var path = Path.GetFullPath("test.gltf");
            File.WriteAllText(path, str);
        }

        [Fact]
        public void LoadDoc_Test()
        {
            var rootPath = Path.GetFullPath(SampleRootPath);
            var gltfFile = Path.Combine(rootPath, "CesiumMan.gltf");
            var doc = Document.FromPath(gltfFile);
            var res = doc.Parse();
            Assert.NotNull(res.Data);
            var model = res.Data;
            var node = model.Nodes.FirstOrDefault(c => c.Children.Count > 0);
            var sceneIndex = model.Scene;
            Assert.Null(res.Error);
            var path = Path.GetFullPath("testdoc.gltf");
            doc.WriteModel(model, path);
        }

        [Fact]
        public void LoadDoc_Test2()
        {
            var rootPath = Path.GetFullPath(SampleRootPath);
            var gltfFile = Path.Combine(rootPath, "sepbins/CesiumMan/CesiumMan.gltf");
            var doc = Document.FromPath(gltfFile);
            var res = doc.Parse();
            Assert.NotNull(res.Data);
            var model = res.Data;
            var node = model.Nodes.FirstOrDefault(c => c.Children.Count > 0);
            var sceneIndex = model.Scene;
            Assert.Null(res.Error);
            var testDir = "test2";
            if (!Directory.Exists(testDir)) Directory.CreateDirectory(testDir);
            var path = Path.GetFullPath(testDir+"/testdoc.gltf");
            doc.WriteModel(model, path);
        }

        [Fact]
        public void LoadCameras_Test2()
        {
            var rootPath = Path.GetFullPath(SampleRootPath);
            var gltfFile = Path.Combine(rootPath, "Cameras.gltf");
            var doc = Document.FromPath(gltfFile);
            var res = doc.Parse();
            Assert.NotNull(res.Data);
            var model = res.Data;
            var node = model.Nodes.FirstOrDefault(c => c.Children.Count > 0);
            var sceneIndex = model.Scene;
            Assert.Null(res.Error);
            var path = Path.GetFullPath("testcameras.gltf");
            doc.WriteModel(model, path);
        }
    }
}
