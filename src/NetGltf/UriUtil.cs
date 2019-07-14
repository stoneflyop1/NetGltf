using System;
using System.IO;
using System.Text.RegularExpressions;

namespace NetGltf {
    public class UriUtil
    {
        private static readonly bool _isWin;

        static UriUtil()
        {
            _isWin = Path.DirectorySeparatorChar == '\\';
        }

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
            var fullPath = Path.GetFullPath(uri);
            return $"file:///{fullPath.Replace('\\','/')}";
        }

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
            throw new NotImplementedException();
        }
    }

}
