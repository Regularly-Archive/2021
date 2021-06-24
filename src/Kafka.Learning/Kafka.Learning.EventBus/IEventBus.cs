using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kafka.Learning.EventBus
{
    public interface IEventBus
    {
        void Publish<TEvent>(TEvent @event) where TEvent : EventBase;

        void Subscribe<T, TH>()
            where T : EventBase
            where TH : IEventHandler<T>;

        void Unsubscribe<T, TH>()
            where TH : IEventHandler<T>
            where T : EventBase;
    }
}
