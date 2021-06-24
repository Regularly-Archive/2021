using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Kafka.Learning.EventBus
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
            services.AddRazorPages();
            services.AddEventBus();

            var serviceProvider = services.BuildServiceProvider();
            var eventBus = serviceProvider.GetService<IEventBus>();
            eventBus.Publish(new OrderInfoCreateEvent()
            {
                ORDER_ID = "OR001",
            });
            eventBus.Publish(new OrderInfoCreateEvent()
            {
                ORDER_ID = "OR002",
            });
            eventBus.Publish(new WriteLogEvent()
            {
                TRANSACTION_ID = Guid.NewGuid().ToString("N"),
                LOG_LEVEL = "DEBUG",
                HOST_NAME = "localhost",
                HOST_IP = "localhost",
                CONTENT = "起风了，唯有努力生存",
                USER_ID = "飞鸿踏雪",
                TTID = "Default",
                APP_NAMESPACE = "ASP.NET Core"
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
        }
    }
}
