using System;
using System.IO;
using System.Collections.Generic;
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

        public GltfResult<Model> Parse()
        {
            try
            {
                var model = Util.Deserialize<Model>(_filePath);
                model.Uri = _filePath;
                return Result.Ok(model);
                // using (var sr = new StreamReader(_filePath))
                // using (var jsonReader = new JsonTextReader(sr))
                // {
                //     var model = _serializer.Deserialize<Model>(jsonReader);
                //     model.Uri = _filePath;
                //     return Result.Ok(model);
                // }
            }
            catch (System.Exception ex)
            {
                return Result.Error<Model>(ex);
            }
        }

        private static bool CopyFile(string oldFolder, string fileNameWithPath, string newFolder)
        {
            if (String.IsNullOrEmpty(fileNameWithPath)) return false;
            if (fileNameWithPath.StartsWith("data:")) return false;
            if (fileNameWithPath.StartsWith("http://") || 
                fileNameWithPath.StartsWith("https://")) return false;
            var oldFile = Path.Combine(oldFolder, fileNameWithPath);
            if (!File.Exists(oldFile)) return false;
            var newFile = Path.Combine(newFolder, fileNameWithPath);
            var newFileDir = Path.GetDirectoryName(newFile);
            if (!Directory.Exists(newFileDir)) Directory.CreateDirectory(newFileDir);
            File.Copy(oldFile, newFile, true);
            return true;
        }

        private static void WriteUriFiles(Model model, string newFilePath)
        {
            if (String.IsNullOrEmpty(newFilePath)) return;
            var newDir = Path.GetDirectoryName(newFilePath);
            var modelUri = new Uri(model.Uri);
            var mName = modelUri.Segments[modelUri.Segments.Length-1];
            var parentUri = model.Uri.Substring(0, model.Uri.Length - mName.Length);
            foreach(var img in model.Images)
            {
                var url = img.Uri;
                CopyFile(parentUri, url, newDir);
            }
            foreach(var buf in model.Buffers)
            {
                var url = buf.Uri;
                CopyFile(parentUri, url, newDir);
            }

        }

        public void WriteModel(Model model, string filePath)
        {
            WriteUriFiles(model, filePath);
            Util.Serialize(model, filePath);
            // using (var sw = new StreamWriter(filePath))
            // {
            //     _serializer.Serialize(sw, model);
            //     sw.Flush();
            // }
        }
    }
}
