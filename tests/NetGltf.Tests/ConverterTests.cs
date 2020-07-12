using NetGltf.Json;
using Newtonsoft.Json;
using Xunit;

namespace NetGltf.Tests
{
    public class ConverterTests
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

        [Fact]
        public void Test_Checked_Enums()
        {
            var converter = new CheckedEnumConverter();
            var obj = new EnumClassTest();
            obj.Component = new CheckedValue<ComponentType, int>(5123);
            obj.Accessor = new CheckedValue<AccessorType, string>("VEC3");
            obj.Accessor2 = new CheckedValue<AccessorType, string>(1);
            obj.TargetProperty = new CheckedValue<TargetProperty, string>("translation");
            obj.Target = new CheckedValue<TargetType,int>(34963);

            var json = JsonConvert.SerializeObject(obj, converter);

            var obj2 = JsonConvert.DeserializeObject<EnumClassTest>(json, converter);

            Assert.True(obj2.Component.IsValid);
            Assert.True(obj2.Accessor.IsValid);
            Assert.True(!obj2.Accessor2.IsValid);
            Assert.True(obj2.TargetProperty.IsValid);
            Assert.True(obj2.Target.HasValue && obj2.Target.Value.IsValid);
        }

        public class EnumClassTest
        {
            public CheckedValue<ComponentType, int> Component {get;set;}

            public CheckedValue<AccessorType, string> Accessor {get;set;}

            public CheckedValue<AccessorType, string> Accessor2 {get;set;}

            public CheckedValue<TargetProperty,string> TargetProperty {get;set;}

            public CheckedValue<TargetType,int>? Target {get;set;}
        }
    }
}
