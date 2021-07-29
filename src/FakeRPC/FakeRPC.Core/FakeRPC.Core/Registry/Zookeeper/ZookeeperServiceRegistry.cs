using Newtonsoft.Json;
using org.apache.zookeeper;
using org.apache.zookeeper.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FakeRpc.Core.Registry.Zookeeper
{
    public class ZookeeperServiceRegistry : BaseServiceRegistry
    {
        private readonly ZooKeeper _zooKeeper;

        public ZookeeperServiceRegistry(ZooKeeper zooKeeper)
        {
            _zooKeeper = zooKeeper;
        }

        public override void Register(ServiceRegistration serviceRegistration)
        {
            var serviceName = serviceRegistration.ServiceName;
            var serviceGroup = serviceRegistration.ServiceGroup;
            var serviceRegisterKey = GetServiceRegistryKey(serviceGroup, serviceName);
            var queryPath = $"/{serviceRegisterKey.Replace(":", "/")}/";
            var groupNode = GetAsyncResult<Stat>(() => _zooKeeper.existsAsync(queryPath, true));
            if (groupNode == null)
            {
                //Zookeeper：/rpc/service/com.team.project/serviceName/serviceId
                queryPath = $"{queryPath}/{serviceRegistration.ServiceId}/";
                var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(serviceRegistration));
                var stat = GetAsyncResult<Stat>(() => _zooKeeper.setDataAsync(queryPath, bytes));
                Console.WriteLine(stat);
            }
            else
            {
                var childrenResult = GetAsyncResult<ChildrenResult>(() => _zooKeeper.getChildrenAsync(queryPath, true));
                var serviceNodes = GetListByZookeeper<ServiceRegistration>(childrenResult);
                if (!serviceNodes.Any(x => x.ServiceUri == serviceRegistration.ServiceUri))
                {
                    //Zookeeper：/rpc/service/com.team.project/serviceName/serviceId/
                    queryPath = $"{queryPath}/{serviceRegistration.ServiceId}/";
                    var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(serviceRegistration));
                    GetAsyncResult<Stat>(() => _zooKeeper.setDataAsync(queryPath, bytes));
                }
            }
        }

        public override void Unregister(ServiceRegistration serviceRegistration)
        {
            var serviceName = serviceRegistration.ServiceName;
            var serviceGroup = serviceRegistration.ServiceGroup;
            var serviceRegisterKey = GetServiceRegistryKey(serviceGroup, serviceName);
            var queryPath = $"/{serviceRegisterKey.Replace(":", "/")}/";
            var groupNode = GetAsyncResult<Stat>(() => _zooKeeper.existsAsync(queryPath, true));
            if (groupNode != null)
            {
                var childrenResult = GetAsyncResult<ChildrenResult>(() => _zooKeeper.getChildrenAsync(queryPath, true));
                var serviceNodes = GetListByZookeeper<ServiceRegistration>(childrenResult);
                var serviceNode = serviceNodes.FirstOrDefault(x => x.ServiceUri == serviceRegistration.ServiceUri);
                if (serviceNode != null)
                {
                    //Zookeeper：/rpc/service/com.team.project/serviceName/serviceId
                    queryPath = queryPath = $"{queryPath}/{serviceNode.ServiceId}/";
                    GetAsyncResult(() => _zooKeeper.deleteAsync(queryPath));
                }
            }
        }

        private IEnumerable<T> GetListByZookeeper<T>(ChildrenResult childrenResult)
        {
            foreach (var child in childrenResult.Children)
                yield return JsonConvert.DeserializeObject<T>(child);
        }
    }

    public class ZookerWatcher : Watcher
    {
        public override Task process(WatchedEvent @event)
        {
            return Task.CompletedTask;
        }
    }
}
