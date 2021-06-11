using Grpc.Core;
using Grpc.Core.Interceptors;
using Grpc.Net.Client;
using Grpc.Net.Client.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace GRPC.Logging.Client
{
    class Program
    {
        async static Task Main(string[] args)
        {
            //await RunGreet();
            //await RunCalculate();
            await RunWithRetey();
            Console.ReadKey();
        }

        static async Task RunGreet()
        {
            using var channel = GrpcChannel.ForAddress("https://localhost:8001");;
            channel.Intercept(new GrpcClientLoggingInterceptor());
            var client = new Greeter.GreeterClient(channel);
            await client.SayHelloAsync(new HelloRequest() { Name = "长安书小妆" });
        }

        static async Task RunCalculate()
        {
            var defaultMethodConfig = new MethodConfig
            {
                Names = { MethodName.Default },
                HedgingPolicy = new HedgingPolicy
                {
                    MaxAttempts = 5,
                    HedgingDelay = TimeSpan.FromSeconds(5),
                    NonFatalStatusCodes = { StatusCode.Unavailable }
                }
            };

            using var channel = GrpcChannel.ForAddress("https://localhost:8001",new GrpcChannelOptions()
            {
                ServiceConfig = new ServiceConfig { MethodConfigs = { defaultMethodConfig } }
            });
            channel.Intercept(new GrpcClientLoggingInterceptor());
            var client = new Calculator.CalculatorClient(channel);
            await client.CalcAsync(new CalculatorRequest() { Num1 = 10, Op = "+", Num2 = 12 });
            await client.CalcAsync(new CalculatorRequest() { Num1 = 10, Op = "-", Num2 = 12 });
            await client.CalcAsync(new CalculatorRequest() { Num1 = 10, Op = "*", Num2 = 12 });
            await client.CalcAsync(new CalculatorRequest() { Num1 = 20, Op = "/", Num2 = 5 });
        }

        static async Task RunWithRetey()
        {
            var services = new ServiceCollection();

            services.AddGrpcClient<Greeter.GreeterClient>(opt =>
            {
                opt.Address = new Uri("https://localhost:8001");
            })
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                ClientCertificateOptions = ClientCertificateOption.Manual,
                ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) => true
            })
            .AddPolicyHandler(
                HttpPolicyExtensions.HandleTransientHttpError()
                .OrResult(res => res.StatusCode != System.Net.HttpStatusCode.OK)
                .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)) + TimeSpan.FromMilliseconds(new Random().Next(0, 100)), (result, timeSpan, current, context)=>
                {
                    Console.WriteLine($"------------------------------------");
                    Console.WriteLine($"StatusCode={result.Result?.StatusCode}");
                    Console.WriteLine($"Exception={result.Exception?.Message}");
                    Console.WriteLine($"正在进行第{current}次重试，间隔{timeSpan.TotalMilliseconds}秒");
                })
            );

            var serviceProvider = services.BuildServiceProvider();
            await serviceProvider.GetService<Greeter.GreeterClient>().SayHelloAsync(new HelloRequest() { Name = "长安书小妆" });
        }
    }
}
