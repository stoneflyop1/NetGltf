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
                var doc = Document.Create();
                var path = Path.GetFullPath("testglb.gltf");
                doc.WriteModel(model, path);
            }
        }
    }
}
