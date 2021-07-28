using System;
using System.Collections.Generic;
using System.Text;

namespace FakeRpc.Core.Discovery
{
    public class BaseServiceDiscovey : IServiceDiscovery
    {
        public Uri GetService<TService>(string serviceGroup = null)
        {
            if (string.IsNullOrEmpty(serviceGroup))
                serviceGroup = typeof(TService).Namespace;

            var serviceName = typeof(TService).GetServiceName();
            return GetService(serviceName, serviceGroup);
        }

        public virtual Uri GetService(string serviceName, string serviceGroup)
        {
            throw new NotImplementedException();
        }


        protected string GetServiceDiscoveryKey(string serviceName)
        {
            var firstLetter = serviceName.AsSpan().Slice(0, 1).ToString().ToLower();
            var otherLetters = serviceName.AsSpan().Slice(1).ToString();
            return $"rpc:services:{firstLetter}{otherLetters}";
        }
    }
}
