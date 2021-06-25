using GRPC.Logging.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Consul;
using Winton.Extensions.Configuration.Consul;
using Microsoft.Extensions.Configuration;
using Grpc.Core;
using static GRPC.Logging.Calculator;
using System.Net.Http;
using Polly.Extensions.Http;
using Polly;
using Grpc.Net.Client;

namespace GRPC.Logging
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddGrpc(options => options.Interceptors.Add<GrpcServerLoggingInterceptor>());

            //services.AddHostedService<HostedHealthCheckService>();

            //services.AddGrpcHealthCheck<GreeterService>();
            //services.AddGrpcHealthCheck<CalculatorService>();

            //services.AddScoped<GrpcPollyPolicyOptions>(sp => new GrpcPollyPolicyOptions() { RetryCount = 3, RetryInterval = TimeSpan.FromSeconds(5) });
            //services.AddScoped<CallInvoker, GrpcCallInvoker>();

            //services.AddGrpcClient<Greeter.GreeterClient>(opt => opt.Address = new Uri("https://localhost:5001"));

            //var options = services.BuildServiceProvider().GetService<GrpcPollyPolicyOptions>();
            //var client = (Greeter.GreeterClient)Activator.CreateInstance(typeof(Greeter.GreeterClient), new GrpcCallInvoker(new Channel("localhost", 5001, ChannelCredentials.Insecure), options));
            //client.SayHello(new HelloRequest() { Name = "长安书小妆" });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<GreeterService>();
                endpoints.MapGrpcService<CalculatorService>();
                endpoints.MapGrpcService<HealthCheckService>();

                endpoints.MapGet("/", async context =>
                {
                    context.Response.StatusCode = 200;
                    await context.Response.WriteAsync("Hello World");
                });
            });
        }
    }
}
