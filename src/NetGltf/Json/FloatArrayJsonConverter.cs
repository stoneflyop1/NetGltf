
using System;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;
namespace NetGltf.Json
{
    /// <summary>
    /// if element can be int, convert to int array
    /// </summary>
    public class FloatArrayJsonConverter : JsonConverter<float[]>
    {
        public override float[] ReadJson(JsonReader reader, Type objectType, 
            float[] existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var vals = new List<float>();
            double? val;
            while((val = reader.ReadAsDouble()) != null)
            {
                vals.Add((float)val.Value);
            }
            if (vals.Count > 0) {
                return vals.ToArray();
            }
            return existingValue;
        }

        public override void WriteJson(JsonWriter writer, float[] value, 
            JsonSerializer serializer)
        {
            if (value != null)
            {
                var json = "[" + String.Join(",", 
                    value.Select(c => Math.Round(c) - c == 0 ? (int)c : c)) + "]";
                writer.WriteRawValue(json);
            }
        }
    }
}