using Xunit;
using Newtonsoft.Json;
using NetGltf.Json;

namespace NetGltf.Tests
{
    public class Converter_Tests
    {
        [Fact]
        public void FloatArrayConverter_Tests()
        {
            var arr = new[] {1.0f,2.0f};
            var str = JsonConvert.SerializeObject(arr, new FloatArrayJsonConverter());
            Assert.Equal("[1,2]", str);
            var arr2 = JsonConvert.DeserializeObject<float[]>(str, new FloatArrayJsonConverter());
            Assert.Equal(1.0, arr2[0]);
            Assert.Equal(2.0, arr2[1]);
        }
    }
}