using System.IO;
using Xunit;
using Newtonsoft.Json;
using NetGltf.Json;

namespace NetGltf.Tests
{
    public class GlbTests
    {
        [Fact]
        public void LoadGlb_Test()
        {
            var rootPath = Path.GetFullPath(GltfLoadTests.SampleRootPath);
            var glbFile = Path.Combine(rootPath, "CesiumMan.glb");
            using(var fs = File.OpenRead(glbFile))
            {
                var glbRes = GlbFile.Parse(fs);
                Assert.NotNull(glbRes.Data);
                Assert.NotNull(glbRes.Data.Json);

                var model = glbRes.Data.ToGltf();
                Assert.NotNull(model);
                Assert.True(model.Nodes.Count > 0);
                var str = JsonConvert.SerializeObject(model, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    ContractResolver = new ArrayContractResolver(),
                    Converters = new JsonConverter[] { new IndexConverter() }
                });
                var path = Path.GetFullPath("testglb.gltf");
                File.WriteAllText(path, str);
            }
        }
    }
}
