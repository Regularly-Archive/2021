using FakeRpc.Core.Mvc;
using FakeRpc.Core.Mvc.MessagePack;
using FakeRpc.Core.Mvc.Protobuf;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Text;

namespace FakeRpc.Core
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddFakeRpc(this IServiceCollection services)
        {
            var partManager = services.BuildServiceProvider().GetService<ApplicationPartManager>();
            if (partManager == null)
                throw new InvalidOperationException("请在AddMvc()方法后调用AddFakeRpc()");

            partManager.FeatureProviders.Add(new FakeRpcFeatureProvider());

            services.Configure<KestrelServerOptions>(x => x.AllowSynchronousIO = true);
            services.Configure<IISServerOptions>(x => x.AllowSynchronousIO = true);

            services.Configure<MvcOptions>(o => o.Conventions.Add(new FakeRpcModelConvention()));
            return services;
        }

        public static void AddFakeRpcClient<TClient>(this IServiceCollection services, Action<HttpClient> configureClient)
        {
            services.AddHttpClient(typeof(TClient).Name.AsSpan().Slice(1).ToString(), configureClient);
            services.AddSingleton<FakeRpcClientFactory>();
        }

        public static IServiceCollection UseProtobuf(this IServiceCollection services)
        {
            services.Configure<MvcOptions>(options =>
            {
                options.InputFormatters.Add(new ProtobufInputFormatter());
                options.OutputFormatters.Add(new ProtobufOutputFormatter());
                options.FormatterMappings.SetMediaTypeMappingForFormat("protobuf", MediaTypeHeaderValue.Parse(FakeRpcMediaTypes.Protobuf));
            });

            return services;
        }

        public static IServiceCollection UseMessagePack(this IServiceCollection services)
        {
            services.Configure<MvcOptions>(options =>
            {
                options.InputFormatters.Add(new MessagePackInputFormatter());
                options.OutputFormatters.Add(new MessagePackOutputFormatter());
                options.FormatterMappings.SetMediaTypeMappingForFormat("msgpack", MediaTypeHeaderValue.Parse(FakeRpcMediaTypes.MessagePack));
            });

            return services;
        }
    }
}
