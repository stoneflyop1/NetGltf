using System;
using NetGltf.Json;

namespace NetGltf
{
    public class GltfResult
    {
        public Model Data { get; internal set; }

        public Exception Error { get; internal set; }
    }

    public static class Result
    {
        public static GltfResult Error(Exception ex)
        {
            if (ex == null) throw new ArgumentNullException(nameof(ex));
            return new GltfResult
            {
                Error = ex
            };
        }

        public static GltfResult Ok(Model data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            return new GltfResult
            {
                Data = data
            };
        }
    }
}