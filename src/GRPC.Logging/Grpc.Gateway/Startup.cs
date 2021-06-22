using Google.Protobuf;
using Grpc.Core;
using GRPC.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grpc.Gateway
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddRazorPages();
            services.AddControllers();
            services.Configure<KestrelServerOptions>(x => x.AllowSynchronousIO = true);
            services.Configure<IISServerOptions>(x => x.AllowSynchronousIO = true);
            services.AddGrpcClient<Greeter.GreeterClient>(opt =>
            {
                opt.Address = new Uri("https://localhost:8001");
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });

            app.AddGrpcGateway<Greeter.GreeterClient, HelloRequest, HelloReply>(
                route: "greet/SayHello",
                requestBuilder: json => new MessageParser<HelloRequest>(() => new HelloRequest()).ParseJson(json),
                responseBuilder: (client, request) => client.SayHelloAsync(request).ResponseAsync.Result
            );
        }
    }
}
