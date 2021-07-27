using Caching.AOP.Core;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Caching.AOP.Test
{
    interface IFakeService
    {
        [Cacheable(CacheKeyPrefix = "Fake", Expiration = 180)]
        List<string> GetColors();

        [Cacheable(CacheKeyPrefix = "Fake", Expiration = 180)]
        Student GetStudentById(int id);

        [Cacheable(CacheKeyPrefix = "Fake", Expiration = 180)]
        Task<bool> IsGradePassed(int id);
    }

    [ProtoContract]
    public class Student
    {
        [ProtoMember(1)]
        public int Id { get; set; }
        [ProtoMember(2)]
        public string Name { get; set; }
    }
}
