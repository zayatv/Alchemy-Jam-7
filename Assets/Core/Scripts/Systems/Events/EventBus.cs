using System;
using System.Collections.Generic;

namespace Core.Systems.Events
{
    /// <summary>
    /// Global event bus for pub/sub messaging.
    /// Subscribe to events, raise events, and automatically cleanup on object destruction.
    /// </summary>
    public static class EventBus
    {
        private static readonly Dictionary<Type, Delegate> _eventHandlers = new Dictionary<Type, Delegate>();
        
        /// <summary>
        /// Get the count of registered event types (useful for debugging).
        /// </summary>
        public static int EventTypeCount => _eventHandlers.Count;

        /// <summary>
        /// Subscribe to an event type.
        /// </summary>
        public static void Subscribe<T>(Action<T> handler) where T : IEvent
        {
            var eventType = typeof(T);

            if (_eventHandlers.TryGetValue(eventType, out var existingHandler))
                _eventHandlers[eventType] = Delegate.Combine(existingHandler, handler);
            else
                _eventHandlers[eventType] = handler;
        }

        /// <summary>
        /// Unsubscribe from an event type.
        /// </summary>
        public static void Unsubscribe<T>(Action<T> handler) where T : IEvent
        {
            var eventType = typeof(T);

            if (_eventHandlers.TryGetValue(eventType, out var existingHandler))
            {
                var newHandler = Delegate.Remove(existingHandler, handler);

                if (newHandler == null)
                    _eventHandlers.Remove(eventType);
                else
                    _eventHandlers[eventType] = newHandler;
            }
        }

        /// <summary>
        /// Raise an event to all subscribers.
        /// </summary>
        public static void Raise<T>(T eventData) where T : IEvent
        {
            var eventType = typeof(T);

            if (_eventHandlers.TryGetValue(eventType, out var handler))
                (handler as Action<T>)?.Invoke(eventData);
        }

        /// <summary>
        /// Clear all event subscriptions. Use with caution.
        /// </summary>
        public static void Clear()
        {
            _eventHandlers.Clear();
        }
    }
}
