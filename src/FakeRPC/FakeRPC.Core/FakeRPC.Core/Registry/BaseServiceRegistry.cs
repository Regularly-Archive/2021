using FakeRpc.Core.Discovery;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FakeRpc.Core.Registry
{
    public class BaseServiceRegistry : IServiceRegistry
    {
        public virtual void Register(ServiceRegistration serviceRegistryEntry)
        {
            
        }

        public void Register<TService>(Uri serviceUri, string serviceGroup)
        {
            var serviceRegistration = new ServiceRegistration()
            {
                ServiceUri = serviceUri,
                ServiceId = Guid.NewGuid(),
                ServiceName = typeof(TService).GetServiceName(),
                ServiceGroup = serviceGroup
            };

            Register(serviceRegistration);
        }

        public Task RegisterAsync<TService>(Uri serviceUri, string serviceGroup)
        {
            Register<TService>(serviceUri, serviceGroup);
            return Task.CompletedTask;
        }

        public Task RegisterAsync(ServiceRegistration serviceRegistration)
        {
            Register(serviceRegistration);
            return Task.CompletedTask;
        }

        public virtual void Unregister(ServiceRegistration serviceRegistration)
        {
            
        }

        public void Unregister<TService>(Uri serviceUri, string serviceGroup)
        {
            var serviceRegistration = new ServiceRegistration()
            {
                ServiceUri = serviceUri,
                ServiceId = Guid.NewGuid(),
                ServiceName = typeof(TService).GetServiceName(),
                ServiceGroup = serviceGroup
            };

            Unregister(serviceRegistration);
        }

        public Task UnregisterAsync(ServiceRegistration serviceRegistration)
        {
            Unregister(serviceRegistration);
            return Task.CompletedTask;
        }

        public Task UnregisterAsync<TService>(Uri serviceUri, string serviceGroup)
        {
            var serviceRegistration = new ServiceRegistration()
            {
                ServiceUri = serviceUri,
                ServiceId = Guid.NewGuid(),
                ServiceName = typeof(TService).GetServiceName(),
                ServiceGroup = serviceGroup
            };

            return UnregisterAsync(serviceRegistration);
        }

        protected string GetServiceRegistryKey(string serviceGroup, string serviceName)
        {
            var firstLetter = serviceName.AsSpan().Slice(0, 1).ToString().ToLower();
            var otherLetters = serviceName.AsSpan().Slice(1).ToString();
            return $"rpc:services:{serviceGroup}:{firstLetter}{otherLetters}";
        }
    }
}
