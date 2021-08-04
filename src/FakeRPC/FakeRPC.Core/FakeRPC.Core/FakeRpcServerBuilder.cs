using Consul;
using CSRedis;
using FakeRpc.Core.Discovery;
using FakeRpc.Core.Mvc;
using FakeRpc.Core.Mvc.MessagePack;
using FakeRpc.Core.Mvc.Protobuf;
using FakeRpc.Core.Registry;
using FakeRpc.Core.Registry.Consul;
using FakeRpc.Core.Registry.Redis;
using MessagePack;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace FakeRpc.Core
{
    public class FakeRpcServerBuilder
    {
        private readonly IServiceCollection _services;

        public FakeRpcServerBuilder(IServiceCollection services)
        {
            _services = services;
        }

        public FakeRpcServerBuilder AddFakeRpc()
        {
            var partManager = _services.BuildServiceProvider().GetService<ApplicationPartManager>();
            if (partManager == null)
                throw new InvalidOperationException("请在AddMvc()方法后调用AddFakeRpc()");

            partManager.FeatureProviders.Add(new FakeRpcFeatureProvider());

            _services.Configure<KestrelServerOptions>(x => x.AllowSynchronousIO = true);
            _services.Configure<IISServerOptions>(x => x.AllowSynchronousIO = true);

            _services.Configure<MvcOptions>(o => o.Conventions.Add(new FakeRpcModelConvention()));
            return this;
        }

        public FakeRpcServerBuilder UseMessagePack(Action<MessagePackSerializerOptions> configure = null)
        {
            var defaultSerializerOptions = MessagePackSerializer.DefaultOptions;
            configure?.Invoke(defaultSerializerOptions);

            _services.Configure<MvcOptions>(options =>
            {
                options.InputFormatters.Add(new MessagePackInputFormatter(defaultSerializerOptions));
                options.OutputFormatters.Add(new MessagePackOutputFormatter(defaultSerializerOptions));
                options.FormatterMappings.SetMediaTypeMappingForFormat("msgpack", MediaTypeHeaderValue.Parse(FakeRpcMediaTypes.MessagePack));
            });

            return this;
        }

        public FakeRpcServerBuilder UseUseProtobuf()
        {
            _services.Configure<MvcOptions>(options =>
            {
                options.InputFormatters.Add(new ProtobufInputFormatter());
                options.OutputFormatters.Add(new ProtobufOutputFormatter());
                options.FormatterMappings.SetMediaTypeMappingForFormat("protobuf", MediaTypeHeaderValue.Parse(FakeRpcMediaTypes.Protobuf));
            });

            return this;
        }

        public FakeRpcServerBuilder EnableServiceRegistry<TServiceRegistry>(Func<IServiceProvider, TServiceRegistry> serviceRegistryFactory = null) where TServiceRegistry : class, IServiceRegistry
        {
            if (serviceRegistryFactory != null)
                _services.AddSingleton<TServiceRegistry>(serviceRegistryFactory);
            else
                _services.AddSingleton<IServiceRegistry, TServiceRegistry>();

            return this;
        }

        public FakeRpcServerBuilder EnableRedisServiceRegistry(Action<RedisServiceRegistryOptions> setupAction)
        {
            var options = new RedisServiceRegistryOptions();
            setupAction?.Invoke(options);

            _services.AddSingleton(options);
            _services.AddSingleton<IServiceRegistry, RedisServiceRegistry>();
            return this;
        }

        public FakeRpcServerBuilder EnableConsulServiceRegistry(Action<ConsulServiceRegistryOptions> setupAction)
        {
            var options = new ConsulServiceRegistryOptions();
            setupAction?.Invoke(options);

            _services.AddSingleton(options);
            _services.AddSingleton<IServiceRegistry, ConsulServiceRegistry>();
            return this;
        }

        public FakeRpcServerBuilder EnableSwagger(Action<SwaggerGenOptions> setupAction = null)
        {
            if (setupAction == null)
                setupAction = BuildDefaultSwaggerGenAction();
            _services.AddSwaggerGen(setupAction);
            _services.AddControllers();
            return this;
        }

        public void Build()
        {
            var serviceProvider = _services.BuildServiceProvider();
            var serviceRegistry = serviceProvider.GetService<IServiceRegistry>();
            if (serviceRegistry != null)
            {
                var serviceTypes = FromThis().Where(x => x.GetCustomAttribute<FakeRpcAttribute>() != null);
                foreach (var serviceType in serviceTypes)
                {
                    serviceRegistry.Register(new ServiceRegistration()
                    {
                        ServiceUri = new Uri("https://192.168.50.162:5001"),
                        ServiceName = serviceType.GetServiceName(),
                        ServiceGroup = serviceType.Namespace,
                        ServiceId = Guid.NewGuid()
                    }); ;
                }
            }
        }

        private IEnumerable<Type> FromThis()
        {
            var entryAssembly = Assembly.GetEntryAssembly();
            var feferdAssemblies = entryAssembly.GetReferencedAssemblies().Select(x => Assembly.Load(x));
            var allAssemblies = new List<Assembly> { entryAssembly }.Concat(feferdAssemblies);
            return allAssemblies.SelectMany(x => x.DefinedTypes).ToList();
        }

        private Action<SwaggerGenOptions> BuildDefaultSwaggerGenAction()
        {
            Action<SwaggerGenOptions> setupAction = options =>
            {
                options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo()
                {
                    Title = "FakeRpc Services",
                    Version = "v1",
                    Contact = new Microsoft.OpenApi.Models.OpenApiContact()
                    {
                        Name = "飞鸿踏雪",
                        Email = "qinyuanpei@163.com",
                        Url = new Uri("https://blog.yuanpei.me"),
                    }
                });

                options.DocInclusionPredicate((a, b) => true);
                var assemblyName = Assembly.GetEntryAssembly().GetName().Name;
                var commentFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{assemblyName}.xml");
                options.IncludeXmlComments(commentFile);
            };

            return setupAction;
        }
    }
}
