using System;
using System.Collections.Generic;
using Newtonsoft.Json.Serialization;

namespace NetGltf.Json
{
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
}