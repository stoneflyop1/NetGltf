using NetGltf.Json;
using Newtonsoft.Json;
using Xunit;

namespace NetGltf.Tests
{
    public class Converter_Tests
    {
        [Fact]
        public void FloatArrayConverter_Tests()
        {
            var arr = new[] { 1.0f, 2.0f };
            var str = JsonConvert.SerializeObject(arr, new FloatArrayJsonConverter());
            Assert.Equal("[1,2]", str);
            var arr2 = JsonConvert.DeserializeObject<float[]>(str, new FloatArrayJsonConverter());
            Assert.Equal(1.0, arr2[0]);
            Assert.Equal(2.0, arr2[1]);
        }

        [Fact]
        public void Index_Tests()
        {
            // var index = JsonConvert.DeserializeObject<Index<Scene>>("1");
            // Assert.Equal(1, index.Value);
            // var str = JsonConvert.SerializeObject(index, new IndexConverter());
            // Assert.Equal("1", str);

            var ti = new TestIndex
            {
                Scene = new Index<Scene> { Value = 1 }
            };
            var str2 = JsonConvert.SerializeObject(ti, Formatting.None);
            Assert.Equal("{\"scene\":1}", str2);
            var ti2 = JsonConvert.DeserializeObject<TestIndex>(str2);
            Assert.Equal(1, ti2.Scene.Value);

        }

        public class TestIndex
        {
            [JsonProperty("scene")]
            [JsonConverter(typeof(IndexConverter))]
            public Index<Scene> Scene { get; set; }
        }
    }
}