using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace NetGltf.Json
{
    /// <summary>
    /// Gltf Json Model
    /// </summary>
    public class Model
    {
        /// <summary>
        /// Gltf filepath
        /// </summary>
        [JsonIgnore]
        public string Uri {get;set;}
        [JsonProperty("asset")]
        public Asset Asset { get; set; }

        [JsonProperty("buffers")]
        public List<Buffer> Buffers { get; set; } = new List<Buffer>();

        [JsonProperty("bufferViews")]
        public List<BufferView> BufferViews { get; set; } = new List<BufferView>();

        [JsonProperty("accessors")]
        public List<Accessor> Accessors { get; set; } = new List<Accessor>();

        [JsonProperty("meshes")]
        public List<Mesh> Meshes { get; set; } = new List<Mesh>();

        [JsonProperty("nodes")]
        public List<Node> Nodes { get; set; } = new List<Node>();

        [JsonProperty("skins")]
        public List<Skin> Skins { get; set; } = new List<Skin>();

        [JsonProperty("scenes")]
        public List<Scene> Scenes { get; set; } = new List<Scene>();

        [JsonProperty("scene")]
        public Index<Scene> Scene { get; set; }

        [JsonProperty("materials")]
        public List<Material> Materials { get; set; } = new List<Material>();

        [JsonProperty("textures")]
        public List<Texture> Textures { get; set; } = new List<Texture>();

        [JsonProperty("samplers")]
        public List<Sampler> Samplers { get; set; } = new List<Sampler>();

        [JsonProperty("images")]
        public List<Image> Images { get; set; } = new List<Image>();

        [JsonProperty("animations")]
        public List<Animation> Animations { get; set; } = new List<Animation>();

        [JsonProperty("cameras")]
        public List<Camera> Cameras {get;set;} = new List<Camera>();
    }

    public static class ModelExtensions
    {
        private static bool IsFileUri(string url)
        {
            if (String.IsNullOrEmpty(url)) return false;
                if (url.StartsWith("data:")) return false;
                if (url.StartsWith("http://") || 
                    url.StartsWith("https://")) return false;
            return true;
        }

        private static List<string> GetFileUris(Model model)
        {
            var files = new List<string>();
            foreach(var img in model.Images)
            {
                var url = img.Uri;
                if (!IsFileUri(url)) continue;
                files.Add(url);
            }
            foreach(var buf in model.Buffers)
            {
                var url = buf.Uri;
                if (!IsFileUri(url)) continue;
                files.Add(url);
            }
            return files;
        }

        private static bool CopyFile(string oldFolder, string fileNameWithPath, string newFolder)
        {
            var oldFile = Path.Combine(oldFolder, fileNameWithPath);
            if (!File.Exists(oldFile)) return false;
            var newFile = Path.Combine(newFolder, fileNameWithPath);
            var newFileDir = Path.GetDirectoryName(newFile);
            if (!Directory.Exists(newFileDir)) Directory.CreateDirectory(newFileDir);
            File.Copy(oldFile, newFile, true);
            return true;
        }

        public static void WriteUriFiles(this Model model, string newFilePath)
        {
            if (String.IsNullOrEmpty(newFilePath)) return;
            var files = GetFileUris(model);
            if (files.Count == 0) return;
            if (String.IsNullOrEmpty(model.Uri))
            {
                throw new GltfException(new[]{new GltfError{
                    Kind = ErrorKind.Uri,
                    Message = "Must provide gltf model filepath"
                }});
            }
            var newDir = Path.GetDirectoryName(newFilePath);
            var modelUri = new Uri(model.Uri);
            var mName = modelUri.Segments[modelUri.Segments.Length-1];
            var parentUri = model.Uri.Substring(0, model.Uri.Length - mName.Length);
            foreach(var url in files)
            {
                CopyFile(parentUri, url, newDir);
            }
        }
    }
}
