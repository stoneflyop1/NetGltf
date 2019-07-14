using System;
using NetGltf.Json;

namespace NetGltf
{
    public class Document
    {
        private readonly string _filePath;

        internal Document() : this(null) { }

        internal Document(string filePath)
        {
            _filePath = filePath;
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

        public GltfResult<Model> Parse()
        {
            try
            {
                var model = JsonUtil.Deserialize<Model>(_filePath);
                model.Uri = _filePath;
                return Result.Ok(model);
            }
            catch (Exception ex)
            {
                return Result.Error<Model>(ex);
            }
        }

        public void WriteModel(Model model, string filePath)
        {
            model.WriteUriFiles(filePath);
            JsonUtil.Serialize(model, filePath);
        }
    }
}
