using System;
using System.IO;

namespace NetGltf {
    /// <summary>
    /// 
    /// </summary>
    public class UriUtil
    {
        private static readonly bool _isWin;

        static UriUtil()
        {
            _isWin = Path.DirectorySeparatorChar == '\\';
        }
        /// <summary>
        /// Is Windows Path
        /// </summary>
        public static bool IsWin()
        {
            return _isWin;
        }
        /// <summary>
        /// test whether uri is valid
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static bool IsValidUri(string uri)
        {
            if (String.IsNullOrEmpty(uri))
            {
                return false;
            }
            var wellformed = Uri.IsWellFormedUriString(uri, UriKind.RelativeOrAbsolute);
            if (wellformed)
            {
                return true;
            }
            if (uri.IndexOfAny(Path.GetInvalidPathChars()) != -1)
            {
                return false;
            }
            
            try
            {
                Path.GetFullPath(uri);
                return true;
            }
            catch(Exception)
            {
                return false;
            }
        }
        /// <summary>
        /// add scheme to uri
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static string Normalize(string uri)
        {
            if (String.IsNullOrEmpty(uri)) return uri;
            if (uri.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
                || uri.StartsWith("https://", StringComparison.OrdinalIgnoreCase)
                || uri.StartsWith("data:", StringComparison.OrdinalIgnoreCase)
                || uri.StartsWith("ftp://", StringComparison.OrdinalIgnoreCase))
            {
                return uri;
            }
            var fullPath = Path.GetFullPath(uri).TrimStart('/').TrimEnd('/');
            return $"file:///{fullPath.Replace('\\','/')}";
        }
        /// <summary>
        /// combine two path, with slash
        /// </summary>
        /// <param name="root"></param>
        /// <param name="relativePath"></param>
        /// <returns></returns>
        public static string Combine(string root, string relativePath)
        {
            if (String.IsNullOrEmpty(relativePath))
            {
                return root;
            }
            if (root.StartsWith("data:"))
            {
                throw new NotSupportedException("not support data url for combining");
            }
            relativePath = relativePath.TrimStart('\\', '/');
            root = root.TrimEnd('/', '\\');
            return Path.Combine(root, relativePath).Replace('\\', '/');
        }
    }

}
