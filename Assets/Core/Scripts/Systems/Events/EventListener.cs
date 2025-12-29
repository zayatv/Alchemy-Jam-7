using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Systems.Events
{
    /// <summary>
    /// Base class for MonoBehaviours that listen to events.
    /// Automatically unsubscribes from all events when destroyed.
    /// </summary>
    public abstract class EventListener : MonoBehaviour
    {
        private readonly List<Action> _unsubscribeActions = new List<Action>();

        /// <summary>
        /// Subscribe to an event and track it for automatic cleanup.
        /// </summary>
        protected void Subscribe<T>(Action<T> handler) where T : IEvent
        {
            EventBus.Subscribe(handler);
            
            _unsubscribeActions.Add(() => EventBus.Unsubscribe(handler));
        }

        /// <summary>
        /// Manually unsubscribe from an event.
        /// </summary>
        protected void Unsubscribe<T>(Action<T> handler) where T : IEvent
        {
            EventBus.Unsubscribe(handler);
            
            _unsubscribeActions.RemoveAll(action => action.Method == handler.Method);
        }

        /// <summary>
        /// Raise an event.
        /// </summary>
        protected void Raise<T>(T eventData) where T : IEvent
        {
            EventBus.Raise(eventData);
        }

        private void OnDestroy()
        {
            // Automatically unsubscribe from all events
            foreach (var unsubscribe in _unsubscribeActions)
            {
                unsubscribe?.Invoke();
            }

            _unsubscribeActions.Clear();
        }
    }
}
