using ProtoBuf;
using ProtoBuf.Meta;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caching.AOP.Core.Serialization
{
    public class ProtobufCacheSerializer : ICacheSerializer
    {
        public T Deserialize<T>(byte[] bytes)
        {
            using (MemoryStream ms = new MemoryStream(bytes))
                return Serializer.Deserialize<T>(ms);
        }

        public dynamic Deserialize(byte[] bytes, Type type)
        {
            using (MemoryStream ms = new MemoryStream(bytes))
                return Serializer.Deserialize(type, ms);
        }

        public byte[] Serialize<T>(T value)
        {
            using (var ms = new MemoryStream())
            {
                Serializer.Serialize<T>(ms, value);
                return ms.ToArray();
            }
        }

        public byte[] Serialize(object value)
        {
            using (var ms = new MemoryStream())
            {
                var typeModel = RuntimeTypeModel.Create();
                typeModel.UseImplicitZeroDefaults = false;
                typeModel.Add(typeof(DateTimeOffset), false).SetSurrogate(typeof(DateTimeOffsetSurrogate));
                typeModel.Serialize(ms, value);
                return ms.ToArray();
            }
        }
    }

    [ProtoContract]
    public class DateTimeOffsetSurrogate
    {
        [ProtoMember(1)]
        public long DateTimeTicks { get; set; }

        [ProtoMember(2)]
        public short OffsetMinutes { get; set; }

        public static implicit operator DateTimeOffsetSurrogate(DateTimeOffset value)
        {
            return new DateTimeOffsetSurrogate
            {
                DateTimeTicks = value.Ticks,
                OffsetMinutes = (short)value.Offset.TotalMinutes
            };
        }

        public static implicit operator DateTimeOffset(DateTimeOffsetSurrogate value)
        {
            return new DateTimeOffset(value.DateTimeTicks, TimeSpan.FromMinutes(value.OffsetMinutes));
        }
    }
}
