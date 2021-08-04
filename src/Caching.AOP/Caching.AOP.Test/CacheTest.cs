using Caching.AOP.Core;
using Caching.AOP.Core.Serialization;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
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

            var serviceProvider = services.BuildServiceProvider();
            var fakeServiceProxy = DispatchProxy.Create<IFakeService, CacheInterceptor<IFakeService>>();
            (fakeServiceProxy as CacheInterceptor<IFakeService>).ServiceProvider = serviceProvider;

            var stopWatch = new Stopwatch();
            stopWatch.Start();

            for (var i = 0; i < 3; i++)
            {
                stopWatch.Restart();
                var colors = fakeServiceProxy.GetColors();
                stopWatch.Stop();
                Trace.WriteLine($"Invoke GetColors used {stopWatch.Elapsed.TotalMilliseconds} ms");
                var student = fakeServiceProxy.GetStudentById(1);
                var isPassed = fakeServiceProxy.IsGradePassed(1);
            }
        }

        [Fact]
        public List<Student> Test_Manual()
        {
            var services = new ServiceCollection();
            services.AddTransient<IRepository<Student>, FakeRepository>();
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = "localhost:6379";
                options.InstanceName = "Caching.AOP.Test";
            });

            var serviceProvider = services.BuildServiceProvider();
            var distributedCache = serviceProvider.GetService<IDistributedCache>();
            var repository = serviceProvider.GetService<IRepository<Student>>();
            var cacheKey = "GetAllStudents";
            var students = new List<Student>();
            var cacheValue = distributedCache.GetString(cacheKey);
            if (string.IsNullOrEmpty(cacheValue))
            {
                students = repository.GetAll().ToList();
                var json = JsonConvert.SerializeObject(students);
                var bytes = Encoding.UTF8.GetBytes(json);
                distributedCache.Set(cacheKey, bytes);
            } 
            else
            {
                students = JsonConvert.DeserializeObject<List<Student>>(cacheValue);
            }

            Assert.True(students.Any());
            return students;
        }

        [Fact]
        public void Test_Void_Task_ReturnType()
        {
            var methodAnyVoid = typeof(FakeService).GetMethod("AnyVoid");
            var methodAnyTask = typeof(FakeService).GetMethod("AnyTask");

            Assert.True(methodAnyVoid.ReturnType == typeof(void));
            Assert.True(methodAnyTask.ReturnType == typeof(Task));
        }

        [Fact]
        public async Task Test_DynamicProxy()
        {
            var services = new ServiceCollection();
            services.AddTransient<IFooService, FooService>();
            services.AddTransient(sp => new DynamicProxyFactory(sp));
            services.AddTransient<IInterceptor, LoggerInterceptor>();

            var serviceProvider = services.BuildServiceProvider();
            var dynamicProxyFactory = serviceProvider.GetService<DynamicProxyFactory>();
            var proxyObject = dynamicProxyFactory.Create<IFooService>(new Type[] { typeof(LoggerInterceptor) });
            await proxyObject.Foo();
        }
    }

    public class DynamicProxy<T> : DispatchProxy
    {
        private T RealObject => ServiceProvider.GetRequiredService<T>();

        public IServiceProvider ServiceProvider { get; set; }

        public IEnumerable<IInterceptor> Interceptors { get; set; }

        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            Interceptors?.ToList().ForEach(x => x.Intercept(targetMethod, args));
            return targetMethod.Invoke(RealObject, args);
        }
    }

    public class DynamicProxyFactory
    {
        private readonly IServiceProvider _serviceProvider;
        public DynamicProxyFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public T Create<T>(Type[] interceptorTypes = null)
        {
            var proxyObject = DispatchProxy.Create<T, DynamicProxy<T>>();

            (proxyObject as DynamicProxy<T>).ServiceProvider = _serviceProvider;

            if (interceptorTypes == null || !interceptorTypes.Any())
                return proxyObject;

            var interceptors = BuildInterceptors(interceptorTypes);
            (proxyObject as DynamicProxy<T>).Interceptors = interceptors;

            return proxyObject;
        }

        public IEnumerable<IInterceptor> BuildInterceptors(Type[] interceptorTypes)
        {
            foreach (var interceptorType in interceptorTypes)
            {
                var interceptor = (IInterceptor)_serviceProvider.GetService(interceptorType);
                if (interceptor != null)
                    yield return interceptor;
            }
        }
    }

    public interface IFooService
    {
        Task<Bar> Foo();
    }

    public interface IInterceptor
    {
        void Intercept(MethodInfo targetMethod, object[] args);
    }

    public class FooService : IFooService
    {
        public Task<Bar> Foo()
        {
            return Task.FromResult(new Bar() { Id = 10, Address = "陕西省西安市雁塔区", Telephone = "12345678901" });
        }
    }

    public class LoggerInterceptor : IInterceptor
    {
        public void Intercept(MethodInfo targetMethod, object[] args)
        {
            Console.WriteLine($"Invoke Method {targetMethod.Name}({JsonConvert.SerializeObject(args)})");
        }
    }

    public class Bar
    {
        public int Id { get; set; }
        public string Address { get; set; }
        public string Telephone { get; set; }
    }

    public class FakeRepository : IRepository<Student>
    {
        IEnumerable<Student> IRepository<Student>.GetAll()
        {
            return new List<Student>()
            {
                new Student() { Id = 1, Name = "张仪" },
                new Student() { Id = 2, Name = "苏秦" },
                new Student() { Id = 3, Name = "孙膑" },
                new Student() { Id = 4, Name = "韩非" },
            };
        }
    }

    public interface IRepository<T>
    {
        IEnumerable<T> GetAll();
    }
}
