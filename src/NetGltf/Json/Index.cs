using System;
using Newtonsoft.Json;

namespace NetGltf.Json
{
    /// <summary>
    /// index of object/asset
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Index<T>
    {
        /// <summary>
        ///  index value
        /// </summary>
        public int Value { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Value.ToString();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        public static implicit operator int(Index<T> index)
        {
            return index.Value;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="val"></param>
        public static implicit operator Index<T>(int val)
        {
            return new Index<T> { Value = val };
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="val"></param>
        public static implicit operator Index<T>(long val)
        {
            return new Index<T> { Value = (int)val };
        }
    }

}
