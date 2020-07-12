using System;
using Newtonsoft.Json;

namespace NetGltf.Json
{
    public class Index<T>
    {
        public int Value { get; set; }

        public override string ToString()
        {
            return Value.ToString();
        }

        public static implicit operator int(Index<T> index)
        {
            return index.Value;
        }

        public static implicit operator Index<T>(int val)
        {
            return new Index<T> { Value = val };
        }

        public static implicit operator Index<T>(long val)
        {
            return new Index<T> { Value = (int)val };
        }
    }

}
