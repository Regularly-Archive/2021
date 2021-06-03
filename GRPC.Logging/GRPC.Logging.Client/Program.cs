using Grpc.Core.Interceptors;
using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace GRPC.Logging.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Task.Run(() => RunGreet()).Wait();
            Task.Run(() => RunCalculate()).Wait();
            Console.ReadKey();
        }

        static async Task RunGreet()
        {
            using var channel = GrpcChannel.ForAddress("https://localhost:5001");;
            channel.Intercept(new GrpcClientLoggingInterceptor());
            var client = new Greeter.GreeterClient(channel);
            await client.SayHelloAsync(new HelloRequest() { Name = "长安书小妆" });
        }

        static async Task RunCalculate()
        {
            using var channel = GrpcChannel.ForAddress("https://localhost:5001"); ;
            channel.Intercept(new GrpcClientLoggingInterceptor());
            var client = new Calculator.CalculatorClient(channel);
            await client.CalcAsync(new CalculatorRequest() { Num1 = 10, Op = "+", Num2 = 12 });
            await client.CalcAsync(new CalculatorRequest() { Num1 = 10, Op = "-", Num2 = 12 });
            await client.CalcAsync(new CalculatorRequest() { Num1 = 10, Op = "*", Num2 = 12 });
            await client.CalcAsync(new CalculatorRequest() { Num1 = 20, Op = "/", Num2 = 5 });
        }
    }
}
