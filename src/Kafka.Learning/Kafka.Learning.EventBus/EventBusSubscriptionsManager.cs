using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kafka.Learning.EventBus
{
    public class EventBusSubscriptionManager : IEventBusSubscriptionManager
    {
        public EventHandler<EventBusSubscriptionEventArgs> OnSubscribe { get; set; }
        public EventHandler<EventBusSubscriptionEventArgs> OnUnsubscribe { get; set; }

        private List<Type> _eventTypes;
        private Dictionary<string, List<Type>> _eventHandlers;
        public EventBusSubscriptionManager()
        {
            _eventTypes = new List<Type>();
            _eventHandlers = new Dictionary<string, List<Type>>();
        }
        public void Clear()
        {
            _eventHandlers.Clear();
        }

        public string GetEventKey<T>() where T : EventBase
        {
            return typeof(T).FullName;
        }

        public string GetEventKey(Type handlerType)
        {
            if (_eventTypes.Contains(handlerType))
                return string.Empty;

            foreach (var handler in _eventHandlers)
            {
                if (handler.Value.Any() && handler.Value.Contains(handlerType))
                    return handler.Key;
            }

            return string.Empty;
        }


        public Type GetEventTypeByName(string eventName)
        {
            return _eventTypes.FirstOrDefault(x => x.FullName == eventName);
        }

        public IEnumerable<Type> GetHandlersForEvent<T>()
            where T : EventBase
        {
            var eventName = GetEventKey<T>();
            if (_eventHandlers.ContainsKey(eventName))
                return _eventHandlers[eventName];
            return new List<Type>();
        }

        public IEnumerable<Type> GetHandlersForEvent(string eventName)
        {
            if (_eventHandlers.ContainsKey(eventName))
                return _eventHandlers[eventName];
            return new List<Type>();
        }

        public bool IsEventSubscribed<T>() where T : EventBase
        {
            var eventName = GetEventKey<T>();
            return _eventHandlers.ContainsKey(eventName);
        }

        public bool IsEventSubscribed(string eventName)
        {
            return _eventHandlers.ContainsKey(eventName);
        }

        public void Subscribe<T, TH>()
            where T : EventBase
            where TH : IEventHandler<T>
        {
            var eventName = GetEventKey<T>();
            if (_eventHandlers.ContainsKey(eventName) && !_eventHandlers[eventName].Any(x => x == typeof(TH)))
            {
                _eventHandlers[eventName].Add(typeof(TH));
            }
            else
            {
                _eventHandlers[eventName] = new List<Type>() { typeof(TH) };
                _eventTypes.Add(typeof(T));
            }
            if (OnSubscribe != null)
                OnSubscribe(this, new EventBusSubscriptionEventArgs() { EvenType = typeof(T), HandlerType = typeof(TH) });
        }

        public void Unsubscribe<T, TH>()
            where T : EventBase
            where TH : IEventHandler<T>
        {
            var eventName = GetEventKey<T>();
            if (_eventHandlers.ContainsKey(eventName) && _eventHandlers[eventName].Any(x => x == typeof(TH)))
            {
                _eventHandlers[eventName].Remove(typeof(TH));
            }
            if (_eventHandlers.ContainsKey(eventName) && !_eventHandlers[eventName].Any())
            {
                _eventHandlers.Remove(eventName);
                _eventTypes.RemoveAll(x => x.FullName == eventName);
            }
            if (OnUnsubscribe != null && !GetHandlersForEvent<T>().Any())
                OnSubscribe(this, new EventBusSubscriptionEventArgs() { EvenType = typeof(T), HandlerType = typeof(TH) });
        }
    }
}
