using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace NetGltf.Json
{
    /// <summary>
    /// Image data used to create a texture.
    /// </summary>
    public class Image
    {
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }
        /// <summary>
        /// The uri of the image.  Relative paths are relative to the .gltf file.
        /// Instead of referencing an external file, the uri can also be a data-uri.
        /// The image format must be jpg or png.
        /// </summary>

        [JsonProperty("uri")]
        public string Uri { get; set; }
        /// <summary>
        /// The index of the buffer view that contains the image. Use this instead of
        /// the image's uri property.
        /// </summary>
        [JsonProperty("bufferView")]
        public Index<BufferView> BufferView { get; set; }
        /// <summary>
        /// The image's MIME type.
        /// </summary>
        [JsonProperty("mimeType")]
        public string MimeType { get; set; }
    }

    internal static class ImageExtensions
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
