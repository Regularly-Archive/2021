using FakeRpc.Core.Discovery;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FakeRpc.Core.Registry
{
    public interface IServiceRegistry
    {
        /// <summary>
        /// Register
        /// </summary>
        /// <param name="serviceRegistryEntry"></param>
        void Register(ServiceRegistration serviceRegistryEntry);

        /// <summary>
        /// Register
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="serviceUri"></param>
        /// <param name="serviceGroup"></param>
        void Register<TService>(Uri serviceUri, string serviceGroup);

        /// <summary>
        /// Register
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="serviceUri"></param>
        /// <param name="serviceGroup"></param>
        Task RegisterAsync<TService>(Uri serviceUri, string serviceGroup);

        /// <summary>
        /// RegisterAsync
        /// </summary>
        /// <param name="serviceRegistryEntry"></param>
        /// <returns></returns>
        Task RegisterAsync(ServiceRegistration serviceRegistration);

        /// <summary>
        /// Unregister
        /// </summary>
        /// <param name="serviceRegistryEntry"></param>
        void Unregister(ServiceRegistration serviceRegistration);

        /// <summary>
        /// UnregisterAsync
        /// </summary>
        /// <param name="serviceRegistryEntry"></param>
        /// <returns></returns>
        Task UnregisterAsync(ServiceRegistration serviceRegistryEntry);

        /// <summary>
        /// Unregister
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="serviceUri"></param>
        /// <param name="serviceGroup"></param>
        void Unregister<TService>(Uri serviceUri, string serviceGroup);

        /// <summary>
        /// Unregister
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="serviceUrl"></param>
        /// <param name="serviceGroup"></param>
        /// <returns></returns>
        Task UnregisterAsync<TService>(Uri serviceUri, string serviceGroup);
    }
}
