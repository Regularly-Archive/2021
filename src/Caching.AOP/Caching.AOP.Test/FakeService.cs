using Caching.AOP.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Caching.AOP.Test
{
    public class FakeService : IFakeService
    {
        [Cacheable(CacheKeyPrefix = "Fake")]
        public List<string> GetColors()
        {
            return new List<string> { "Red", "Yellow", "Green" };
        }

        [Cacheable(CacheKeyPrefix = "Fake")]
        public Student GetStudentById(int id)
        {
            return new Student { Id = id, Name = "飞鸿踏雪" };
        }

        [Cacheable(CacheKeyPrefix = "Fake")]
        public Task<bool> IsGradePassed(int id)
        {
            return Task.FromResult(id % 2 == 0);
        }
    }
}
