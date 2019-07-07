using System.IO;
using Xunit;

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
            }
        }
    }
}
