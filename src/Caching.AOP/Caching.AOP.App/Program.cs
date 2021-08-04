using Caching.AOP.Core;
using Caching.AOP.Core.Serialization;
using Caching.AOP.Test;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
using System.Reflection;

namespace Caching.AOP.App
{
    class Program
    {
        static void Main(string[] args)
        {
            var services = new ServiceCollection();
            services.AddTransient<IFakeService, FakeService>();
            services.AddTransient<ICacheSerializer, JsonCacheSerializer>();
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = "localhost:6379";
                options.InstanceName = "Caching.AOP.Test";
            });

            var serviceProvider = services.BuildServiceProvider();
            var fakeServiceProxy = DispatchProxy.Create<IFakeService, CacheInterceptor<IFakeService>>();
            (fakeServiceProxy as CacheInterceptor<IFakeService>).ServiceProvider = serviceProvider;

            Console.WriteLine("");
            for (var i = 0; i < 5; i++)
            {
                var stopWatch = new Stopwatch();
                stopWatch.Start();
                fakeServiceProxy.GetColors();
                stopWatch.Stop();
                Console.WriteLine($" {i} - Invoke GetColors used {stopWatch.Elapsed.TotalMilliseconds} ms");
            }

            Console.ReadKey();
        }
    }
}
