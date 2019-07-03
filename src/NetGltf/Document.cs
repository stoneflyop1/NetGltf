using System.IO;
using NetGltf.Json;
using Newtonsoft.Json;

namespace NetGltf
{
    public class Document
    {
        private readonly string _filePath;

        internal Document() : this(null) { }

        internal Document(string filePath)
        {
            _filePath = filePath;
            _serializer = new JsonSerializer
            {
                ContractResolver = new ArrayContractResolver(),
                NullValueHandling = NullValueHandling.Ignore
            };
            _serializer.Converters.Add(new IndexConverter());
        }

        public static Document Create()
        {
            return new Document();
        }

        public static Document FromPath(string filePath)
        {
            if (filePath == null)
            {
                throw new System.ArgumentNullException(
                    nameof(filePath), "Must provide gltf filePath");
            }
            return new Document(filePath);
        }

        private readonly JsonSerializer _serializer;

        public GltfResult Parse()
        {
            try
            {
                using (var sr = new StreamReader(_filePath))
                using (var jsonReader = new JsonTextReader(sr))
                {
                    var model = _serializer.Deserialize<Model>(jsonReader);
                    return Result.Ok(model);
                }
            }
            catch (System.Exception ex)
            {
                return Result.Error(ex);
            }
        }

        public void WriteModel(Model model, string filePath)
        {
            using (var sw = new StreamWriter(filePath))
            {
                _serializer.Serialize(sw, model);
                sw.Flush();
            }
        }
    }
}