using Consul;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace GRPC.Logging
{
    public static class ServiceCollectionExtension
    {
        public static void AddGrpcHealthCheck<TService>(this IServiceCollection services)
        {
            var configuration = services.BuildServiceProvider().GetService<IConfiguration>();

            // 注册ConsulClient
            services.AddSingleton<IConsulClient, ConsulClient>(_ => new ConsulClient(consulConfig =>
            {
                var baseUrl = configuration.GetValue<string>("Consul:BaseUrl");
                consulConfig.Address = new Uri(baseUrl);
            }));

            // 注册gRPC服务
            RegisterConsul<TService>(services).Wait();
        }

        private static async Task RegisterConsul<TService>(IServiceCollection services)
        {
            var serverHost = GetLocalIP();
            var serverPort = services.BuildServiceProvider().GetService<IConfiguration>().GetValue<int>("gRPC:Port");
            await RegisterConsul<TService>(services, serverHost, serverPort);
        }

        private static async Task RegisterConsul<TService>(IServiceCollection services, string serverHost, int serverPort)
        {
            var client = services.BuildServiceProvider().GetService<IConsulClient>();
            var registerID = $"{typeof(TService).Name}-{serverHost}:{serverPort}";
            await client.Agent.ServiceDeregister(registerID);
            var result = await client.Agent.ServiceRegister(new AgentServiceRegistration()
            {
                ID = registerID,
                Name = typeof(TService).Name,
                Address = serverHost,
                Port = serverPort,
                Check = new AgentServiceCheck
                {
                    TCP = $"{serverHost}:{serverPort}",
                    Status = HealthStatus.Passing,
                    DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(10),
                    Interval = TimeSpan.FromSeconds(10),
                    Timeout = TimeSpan.FromSeconds(5)
                },
                Tags = new string[] { "gRpc" }
            }) ;
        }

        private static string GetLocalIP()
        {
            var hostName = Dns.GetHostName();
            var ipEntry = Dns.GetHostEntry(hostName);
            for (int i = 0; i < ipEntry.AddressList.Length; i++)
            {
                if (ipEntry.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
                {
                    return ipEntry.AddressList[i].ToString();
                }
            }

            return "127.0.0.1";
        }

        public static async Task<TGrpcClient> GetGrpcClientAsync<TGrpcClient>(this IServiceProvider serviceProvider)
        {
            var consulClient = serviceProvider.GetService<IConsulClient>();
            var serviceName = typeof(TGrpcClient).Name.Replace("Client", "Service");
            var services = await consulClient.Health.Service(serviceName, string.Empty, true);
            var serviceUrls = services.Response.Select(s => $"{s.Service.Address}:{s.Service.Port}").ToList();
            if (serviceUrls == null || !serviceUrls.Any())
                throw new Exception($"Please make sure service {serviceName} is registered in consul");

            var serviceUrl = serviceUrls[new Random().Next(0, serviceUrls.Count - 1)];
            var channel = GrpcChannel.ForAddress($"https://{serviceUrl}");
            var constructorInfo = typeof(TGrpcClient).GetConstructor(new Type[] { typeof(GrpcChannel) });
            if (constructorInfo == null)
                throw new Exception($"Please make sure {typeof(TGrpcClient).Name} is a gRpc client");

            var clientInstance = (TGrpcClient)constructorInfo.Invoke(new object[] { channel });
            return clientInstance;

        }
    }
}
