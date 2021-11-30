using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtobufAny.Extensions
{
    public static class MyAnyExtension
    {
        public static MyAny Pack(this object obj, string typeUrlPrefix = "")
        {
            var any = new MyAny();
            any.TypeUrl = $"{typeUrlPrefix}/{obj.GetType().FullName}";
            var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj));
            any.Value = Google.Protobuf.ByteString.CopyFrom(bytes);
            return any;
        }

        public static T Unpack<T>(this MyAny any, string typeUrlPrefix = "")
        {
            var typeUrl = $"{typeUrlPrefix}/{typeof(T).FullName}";
            if (typeUrl == any.TypeUrl)
            {
                var json = Encoding.UTF8.GetString(any.Value.ToByteArray());
                return JsonConvert.DeserializeObject<T>(json);
            }

            return default(T);
        }

        public static bool Is<T>(this MyAny any, string typeUrlPrefix = "")
        {
            var typeUrl = $"{typeUrlPrefix}/{typeof(T).FullName}";
            return typeUrl == any.TypeUrl;
        }
    }
}
