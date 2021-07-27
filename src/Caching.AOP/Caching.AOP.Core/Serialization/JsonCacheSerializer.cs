using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Caching.AOP.Core.Serialization
{
    public class JsonCacheSerializer : ICacheSerializer
    {
        public T Deserialize<T>(byte[] bytes)
        {
            return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(bytes));
        }

        public dynamic Deserialize(byte[] bytes, Type type)
        {
            return JsonConvert.DeserializeObject(Encoding.UTF8.GetString(bytes), type);
        }

        public byte[] Serialize<T>(T value)
        {
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value));
        }

        public byte[] Serialize(object value)
        {
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value));
        }
    }
}
