using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace NetGltf.Json {
    /// <summary>
    /// only serialize when list is not empty
    /// </summary>
    public class ArrayContractResolver : DefaultContractResolver
    {
        private static readonly Type ListType = typeof(List<>);
        private static bool IsList(Type t)
        {
            return (t.Name == ListType.Name && t.Namespace == ListType.Namespace);
        }
        protected override JsonProperty CreateProperty(
            System.Reflection.MemberInfo member,
            Newtonsoft.Json.MemberSerialization memberSerialization)
        {
            var jProp = base.CreateProperty(member, memberSerialization);

            if (IsList(jProp.PropertyType))
            {
                jProp.ShouldSerialize = inst =>
                {
                    if (inst != null)
                    {
                        dynamic i = jProp.ValueProvider.GetValue(inst);
                        if (i != null && i.Count == 0)
                        {
                            return false;
                        }
                    }
                    return true;
                };
            }
            return jProp;
        }
    }

    public class IndexConverter : JsonConverter
    {
        private static readonly Type indexType = typeof(Index<>);
        public override bool CanConvert(Type objectType)
        {
            return objectType.Name == indexType.Name &&
                objectType.Namespace == indexType.Namespace;
        }

        public override object ReadJson(JsonReader reader, Type objectType, 
            object existingValue, JsonSerializer serializer)
        {
            var rawVal = reader.Value;
            int? val = null;
            if (rawVal is int || rawVal is long)
            {
                val = Convert.ToInt32(rawVal);
            }
            else
            {
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

    public class CheckedEnumConverter : JsonConverter
    {
        private static readonly Type CheckType = typeof(CheckedValue<,>);
        private static readonly Type NullableCheckType = typeof(Nullable<>);

        private static Type GetCheckType(Type objectType)
        {
            var isNullable = objectType.Name == NullableCheckType.Name &&
                objectType.Namespace == NullableCheckType.Namespace;
            if (isNullable)
            {
                var gTypes = objectType.GetGenericArguments();
                if (gTypes.Length == 1)
                {
                    if (IsCheckType(gTypes[0]))
                    {
                        return gTypes[0];
                    }
                }
            }            
            return null;
        }

        private static bool IsCheckType(Type objectType) {
            return objectType.Name == CheckType.Name &&
                objectType.Namespace == CheckType.Namespace;
        }
        public override bool CanConvert(Type objectType)
        {
            if(IsCheckType(objectType)) {
                return true;
            }
            var t = GetCheckType(objectType);
            return t != null;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var rawVal = reader.Value;
            if (rawVal == null) return null;
            if (rawVal is long) { // json.net will get int64 for integer value
                rawVal = (int)(long)rawVal;
            }
            if (IsCheckType(objectType)) {
                return Activator.CreateInstance(objectType, rawVal);
            }
            else {
                var t = GetCheckType(objectType);
                return Activator.CreateInstance(t, rawVal);
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value != null)
            {
                var type = value.GetType();
                if (!IsCheckType(type)) {
                    type = GetCheckType(type);
                }
                var member = type.GetProperty("Value");
                var val = member.GetValue(value);
                var str = val as string;
                if (str != null) {
                    writer.WriteRawValue("\""+str +"\"");
                } else {
                    if (val == null) {
                        
                    }
                    writer.WriteRawValue(val.ToString());
                }
            }
        }
    }

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
            while ((val = reader.ReadAsDouble()) != null)
            {
                vals.Add((float)val.Value);
            }
            if (vals.Count > 0)
            {
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
