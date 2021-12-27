using Grpc.Core;
using Grpc.Net.Client;
using System;

namespace GrpcJwt.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new Greeter.GreeterClient(channel);

            var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoi566h55CG5ZGYIiwiaHR0cDovL3NjaGVtYXMubWljcm9zb2Z0LmNvbS93cy8yMDA4LzA2L2lkZW50aXR5L2NsYWltcy9yb2xlIjoiQWRtaW4iLCJleHAiOjE2NDA2NzY3MDksImlzcyI6IkdycGNKd3QiLCJhdWQiOiJHcnBjSnd0In0.76a8zURaZdhxwiBodwLaxpwIV19AQBsyu-o3uH-lRFs";
            var metadata = new Metadata();
            metadata.Add("Authorization", $"Bearer {token}");

            var reply = client.SayHello(new HelloRequest(), metadata);
            Console.ReadKey();
        }
    }
}
