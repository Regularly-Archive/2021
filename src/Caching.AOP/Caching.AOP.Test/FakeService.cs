using Caching.AOP.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Caching.AOP.Test
{
    public class FakeService : IFakeService
    {
        public Task AnyTask()
        {
            return Task.CompletedTask;
        }

        public void AnyVoid()
        {
            Console.WriteLine("Method AnyVoid is invoked!");
        }

        public List<string> GetColors()
        {
            Thread.Sleep(500);
            return new List<string> { "Red", "Yellow", "Green" };
        }

        public Student GetStudentById(int id)
        {
            return new Student { Id = id, Name = "飞鸿踏雪" };
        }

        public Task<bool> IsGradePassed(int id)
        {
            return Task.FromResult(id % 2 == 0);
        }
    }
}
