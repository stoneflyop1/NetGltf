using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace NetGltf.Json
{
    public class Image
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("uri")]
        public string Uri { get; set; }

        [JsonProperty("bufferView")]
        public Index<BufferView> BufferView { get; set; }

        [JsonProperty("mimeType")]
        public string MimeType { get; set; }
    }

    public static class ImageExtensions
    {
        public static readonly ISet<string> ValidTypes = new HashSet<string>(new[] {
            "image/jpeg",
            "image/png",
        });

        public static bool IsValidImageType(this Image img)
        {
            if (img == null) throw new ArgumentNullException(nameof(img));
            if (!String.IsNullOrEmpty(img.Uri) && String.IsNullOrEmpty(img.MimeType))
            {
                return true;
            }
            if (String.IsNullOrEmpty(img.MimeType)) return false;
            return ValidTypes.Contains(img.MimeType.ToLower());
        }
    }
}
