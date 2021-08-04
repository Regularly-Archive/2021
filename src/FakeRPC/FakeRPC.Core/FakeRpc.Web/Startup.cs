using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FakeRpc.Web;
using FakeRpc.Core;
using System.Net.Http;
using FakeRpc.Web.Services;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using FakeRpc.Core.Registry;
using CSRedis;
using FakeRpc.Core.Registry.Redis;
using FakeRpc.Core.Registry.Consul;
using FakeRPC.XYZ.Services;

namespace FakeRpc.Web
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
            services.AddControllersWithViews();

            services.AddControllers();

            var builder = new FakeRpcServerBuilder(services);
            builder
                .AddFakeRpc()
                .UseMessagePack()
                .UseUseProtobuf()
                .EnableSwagger()
                .AddExternalAssembly(typeof(GreetService).Assembly)
                .EnableConsulServiceRegistry(options => options.BaseUrl = "http://localhost:8500");
            builder.Build();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "FakeRpc Services v1");
            });
        }
    }
}
