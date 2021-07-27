using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caching.AOP.Core.Serialization
{
    public interface ICacheSerializer
    {
        T Deserialize<T>(byte[] bytes);

        dynamic Deserialize(byte[] bytes, Type type);

        byte[] Serialize<T>(T value);

        byte[] Serialize(object value);
    }
}
