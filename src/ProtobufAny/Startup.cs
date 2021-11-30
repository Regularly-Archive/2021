﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProtobufAny.Extensions;

namespace ProtobufAny
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddGrpc();
            services.AddGrpcReflection();

            services.AddGrpcClient<ProtobufAny.Greeter.GreeterClient>(opt =>
            {
                opt.Address = new Uri("http://localhost:5000");
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime lifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            lifetime.ApplicationStarted.Register(() =>
            {
                var client = app.ApplicationServices.GetRequiredService<ProtobufAny.Greeter.GreeterClient>();
                var fooResponse = client.Echo(new AnyRequest() { Data = new Foo { Name = "Foo" }.ToAny() });
                Console.WriteLine(fooResponse.Data.ToObject<Foo>().Name);
                var barResponse = client.Echo(new AnyRequest() { Data = new Bar { Name = "Bar" }.ToAny() });
                Console.WriteLine(barResponse.Data.ToObject<Bar>().Name);
            });

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<GreeterService>();
                endpoints.MapGrpcReflectionService();

                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
                });
            });
        }
    }
}
