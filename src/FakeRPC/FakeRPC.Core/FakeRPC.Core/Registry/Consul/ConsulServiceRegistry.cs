using Consul;
using System;
using System.Collections.Generic;
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
            var serviceHost = serviceRegistration.ServiceUri.Host;
            var servicePort = serviceRegistration.ServiceUri.Port;
            var serviceName = serviceRegistration.ServiceName;
            var serviceGroup = serviceRegistration.ServiceGroup;
            var serviceRegitryKey = GetServiceRegistryKey(serviceGroup, serviceName);
            var registerID = $"{serviceRegitryKey}:{serviceHost}:{servicePort}";
            var writeResult = GetAsyncResult<WriteResult>(() => _consulClient.Agent.ServiceDeregister(registerID));
            writeResult = GetAsyncResult<WriteResult>(() => _consulClient.Agent.ServiceRegister(new AgentServiceRegistration()
            {
                ID = registerID,
                Name = serviceName,
                Address = serviceRegistration.ServiceUri.Host,
                Port = serviceRegistration.ServiceUri.Port,
                Check = new AgentServiceCheck
                {
                    TCP = $"{serviceHost}:5001",
                    Status = HealthStatus.Passing,
                    TLSSkipVerify = true,
                    DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(10),
                    Interval = TimeSpan.FromSeconds(10),
                    Timeout = TimeSpan.FromSeconds(5)
                },
                Tags = new string[] { "FakeRpc" }
            }));
        }

        public override void Unregister(ServiceRegistration serviceRegistration)
        {
            
        }
    }
}
