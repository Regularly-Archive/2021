using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProtobufAny.Extensions
{
    public static class ObjectExtension
    {
        public static Any ToAny(this IMessage message, string typeUrlPrefix = "")
        {
            return new Any() { TypeUrl = $"{typeUrlPrefix}/{message.Descriptor.FullName}", Value = message.ToByteString() };
        }

        public static TMessage ToObject<TMessage>(this Any any) where TMessage : IMessage, new()
        {
            return any.Unpack<TMessage>();
        }
    }
}
