using FakeRpc.Core.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using WebApiContrib.Core.Formatter.MessagePack;

namespace FakeRpc.Core
{
    public static class ServiceCollectionExtension
    {
        public static void AddFakeRpc(this IServiceCollection services)
        {
            var partManager = services.BuildServiceProvider().GetService<ApplicationPartManager>();
            if (partManager == null)
                throw new InvalidOperationException("请在AddMvc()方法后调用AddFakeRpc()");

            partManager.FeatureProviders.Add(new FakeRpcFeatureProvider());
            services.Configure<MvcOptions>(o => o.Conventions.Add(new FakeRpcModelConvention()));
            services.AddMvcCore().AddApiExplorer();
            services.AddSwaggerGen(swagger =>
            {
                swagger.SwaggerDoc("v1", new OpenApiInfo { Title = "ynamic WebApi", Version = "v1.0" });

                swagger.DocInclusionPredicate((docName, description) => true);

                var xmlFile = $"FakeRpc.Web.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                swagger.IncludeXmlComments(xmlPath);
            });
        }
    }
}
