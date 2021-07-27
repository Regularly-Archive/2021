using Caching.AOP.Core;
using Caching.AOP.Core.Serialization;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace Caching.AOP.Test
{
    public class CacheTest
    {
        [Fact]
        public void Test_AOP()
        {
            var services = new ServiceCollection();
            services.AddTransient<IFakeService, FakeService>();
            services.AddTransient<ICacheSerializer, JsonCacheSerializer>();
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = "localhost:6379";
                options.InstanceName = "Caching.AOP.Test";
            });

            var fakeServiceProxy = DispatchProxy.Create<IFakeService, CacheInterceptor>();
            (fakeServiceProxy as CacheInterceptor).RealProxy = services.BuildServiceProvider().GetService<IFakeService>();
            (fakeServiceProxy as CacheInterceptor).CacheSerializer = services.BuildServiceProvider().GetService<ICacheSerializer>();
            (fakeServiceProxy as CacheInterceptor).DistributedCache = services.BuildServiceProvider().GetService<IDistributedCache>();

            for (var i = 0; i < 3; i++)
            {
                var colors = fakeServiceProxy.GetColors();
                var student = fakeServiceProxy.GetStudentById(1);
                var isPassed = fakeServiceProxy.IsGradePassed(1);
            }
        }

        [Fact]
        public void Test_Void_Task_ReturnType()
        {
            var methodAnyVoid = typeof(FakeService).GetMethod("AnyVoid");
            var methodAnyTask = typeof(FakeService).GetMethod("AnyTask");

            Assert.True(methodAnyVoid.ReturnType == typeof(void));
            Assert.True(methodAnyTask.ReturnType == typeof(Task));
        }
    }
}
