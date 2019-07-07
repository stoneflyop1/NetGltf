using System;
using NetGltf.Json;

namespace NetGltf
{
    public class GltfResult<T>
    {
        public T Data { get; internal set; }

        public Exception Error { get; internal set; }
    }

    public static class Result
    {
        public static GltfResult<T> Error<T>(Exception ex)
        {
            if (ex == null) throw new ArgumentNullException(nameof(ex));
            return new GltfResult<T>
            {
                Error = ex
            };
        }

        public static GltfResult<T> Ok<T>(T data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            return new GltfResult<T>
            {
                Data = data
            };
        }
    }
}
