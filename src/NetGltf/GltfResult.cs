using System;
using NetGltf.Json;

namespace NetGltf
{
    /// <summary>
    /// gltf results
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GltfResult<T>
    {
        /// <summary>
        /// result data
        /// </summary>
        public T Data { get; internal set; }
        /// <summary>
        /// exception
        /// </summary>
        public Exception Error { get; internal set; }
    }
    /// <summary>
    /// result helper
    /// </summary>
    public static class Result
    {
        /// <summary>
        /// error from exception
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static GltfResult<T> Error<T>(Exception ex)
        {
            if (ex == null) throw new ArgumentNullException(nameof(ex));
            return new GltfResult<T>
            {
                Error = ex
            };
        }
        /// <summary>
        /// OK Result
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
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
