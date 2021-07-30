using Consul;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FakeRpc.Core.Registry.Consul
{
    public class ConsulServiceRegistry : BaseServiceRegistry
    {
        private IConsulClient _consulClient;
        public ConsulServiceRegistry(IConsulClient consulClient)
        {
            _consulClient = consulClient;
        }

        public override void Register(ServiceRegistration serviceRegistration)
        {
            var services = AsyncHelper.RunSync<QueryResult<ServiceEntry[]>>(() => _consulClient.Health.Service(serviceRegistration.ServiceName));
            var sb = services.Response.ToList();

            var registerID = GetConsulRegisterID(serviceRegistration);
            AsyncHelper.RunSync<WriteResult>(() => _consulClient.Agent.ServiceDeregister(registerID));
            AsyncHelper.RunSync<WriteResult>(() => _consulClient.Agent.ServiceRegister(new AgentServiceRegistration()
            {
                ID = registerID,
                Name = serviceRegistration.ServiceName,
                Address = serviceRegistration.ServiceUri.Host,
                Port = serviceRegistration.ServiceUri.Port,
                Check = new AgentServiceCheck
                {
                    TCP = $"{serviceRegistration.ServiceUri.Host}:{serviceRegistration.ServiceUri.Port}",
                    Status = HealthStatus.Passing,
                    TLSSkipVerify = true,
                    DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(10),
                    Interval = TimeSpan.FromSeconds(10),
                    Timeout = TimeSpan.FromSeconds(5)
                },
                Tags = new string[] { "FakeRpc", serviceRegistration.ServiceGroup }
            })) ;
        }

        public override void Unregister(ServiceRegistration serviceRegistration)
        {
            var registerID = GetConsulRegisterID(serviceRegistration);
            var writeResult = AsyncHelper.RunSync<WriteResult>(() => _consulClient.Agent.ServiceDeregister(registerID));
        }

        public override string GetServiceRegistryKey(string serviceGroup, string serviceName)
        {
            return $"{serviceGroup}/{serviceName}";
        }

        private string GetConsulRegisterID(ServiceRegistration serviceRegistration)
        {
            var serviceHost = serviceRegistration.ServiceUri.Host;
            var servicePort = serviceRegistration.ServiceUri.Port;
            var serviceName = serviceRegistration.ServiceName;
            var serviceGroup = serviceRegistration.ServiceGroup;
            var serviceRegitryKey = GetServiceRegistryKey(serviceGroup, serviceName);
            var registerID = $"{serviceRegitryKey}/{serviceHost}:{servicePort}";
            return registerID;
        }
    }
}
