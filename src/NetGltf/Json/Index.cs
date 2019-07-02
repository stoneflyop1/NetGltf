using System;
using Newtonsoft.Json;

namespace NetGltf.Json
{
    public class Index<T>
    {
        public int Value {get;set;}

        public override string ToString()
        {
            return Value.ToString();
        }

        public static implicit operator int(Index<T> index) {
            return index.Value;
        }

        public static implicit operator Index<T>(int val) {
            return new Index<T>{Value = val};
        }

        public static implicit operator Index<T>(long val) {
            return new Index<T>{Value = (int)val};
        }
    }

    public class IndexConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            var indexType = typeof(Index<>);
            return objectType.Name == indexType.Name && 
                objectType.Namespace == indexType.Namespace;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var rawVal = reader.Value;
            int? val = null;
            if (rawVal is int || rawVal is long) {
                val = Convert.ToInt32(rawVal);
            }
            else {
                val = reader.ReadAsInt32();
            }            
            if (val != null)
            {
                dynamic inst = Activator.CreateInstance(objectType);
                inst.Value = val.Value;
                return inst;
            }

            return existingValue;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value != null)
            {
                var type = value.GetType();
                var member = type.GetProperty("Value");
                var val = (int)member.GetValue(value);
                writer.WriteRawValue(val.ToString());
            }
        }
    }
}