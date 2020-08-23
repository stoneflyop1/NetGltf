using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace NetGltf.Json
{
    internal static class JsonUtil
    {
        private static readonly JsonSerializer _serde;

        internal static readonly Encoding StrEncoding = new UTF8Encoding(false);

        static JsonUtil()
        {
            _serde = new JsonSerializer
            {
                ContractResolver = new ArrayContractResolver(),
                NullValueHandling = NullValueHandling.Ignore,
#if DEBUG
                Formatting = Formatting.Indented
#endif
            };
            _serde.Converters.Add(new IndexConverter());
            _serde.Converters.Add(new CheckedEnumConverter());
        }

        public static T DeserializeString<T>(string json)
        {
            using(var sr = new StringReader(json))
            using(var jr = new JsonTextReader(sr))
            {
                return _serde.Deserialize<T>(jr);
            }
        }

        public static T Deserialize<T>(string jsonFile)
        {
            using(var sr = new StreamReader(jsonFile, Encoding.UTF8))
            using(var jr = new JsonTextReader(sr))
            {
                return _serde.Deserialize<T>(jr);
            }
        }

        public static string ToJson(object obj)
        {
            using(var sw = new StringWriter())
            {
                _serde.Serialize(sw, obj);
                sw.Flush();
                return sw.ToString();
            }
        }

        public static void Serialize(object obj, string jsonFile)
        {
            using (var sw = new StreamWriter(jsonFile, false, StrEncoding))
            {
                _serde.Serialize(sw, obj);
                sw.Flush();
            }
        }
    }
}
