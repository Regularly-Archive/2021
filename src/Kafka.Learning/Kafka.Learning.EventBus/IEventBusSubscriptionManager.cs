using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kafka.Learning.EventBus
{
    public interface IEventBusSubscriptionManager
    {
        EventHandler<EventBusSubscriptionEventArgs> OnSubscribe { get; set; }
        EventHandler<EventBusSubscriptionEventArgs> OnUnsubscribe { get; set; }

        void Subscribe<T, TH>()
           where T : EventBase
           where TH : IEventHandler<T>;

        void Unsubscribe<T, TH>()
           where T : EventBase
           where TH : IEventHandler<T>;

        bool IsEventSubscribed<T>() where T : EventBase;

        bool IsEventSubscribed(string eventName);

        Type GetEventTypeByName(string eventName);

        void Clear();

        IEnumerable<Type> GetHandlersForEvent<T>() where T : EventBase;

        IEnumerable<Type> GetHandlersForEvent(string eventName);

        string GetEventKey<T>() where T : EventBase;
    }
}
