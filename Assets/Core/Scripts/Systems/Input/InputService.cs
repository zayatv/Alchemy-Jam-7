using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Systems.Input
{
    /// <summary>
    /// Core input service implementation.
    /// Manages input contexts and their lifecycle.
    /// </summary>
    public class InputService : IInputService
    {
        private readonly Dictionary<Type, IInputContext> _contexts = new Dictionary<Type, IInputContext>();

        /// <summary>
        /// Register an input context.
        /// </summary>
        public void RegisterContext<T>(T context) where T : IInputContext
        {
            var type = typeof(T);

            if (_contexts.ContainsKey(type))
                Debug.LogWarning($"[InputService] Context {type.Name} already registered. Overwriting.");

            _contexts[type] = context;
        }

        /// <summary>
        /// Get an input context.
        /// </summary>
        public T GetContext<T>() where T : IInputContext
        {
            var type = typeof(T);

            if (_contexts.TryGetValue(type, out var context))
                return (T)context;

            Debug.LogError($"[InputService] Context {type.Name} not registered.");
            
            return default;
        }

        /// <summary>
        /// Enable an input context.
        /// </summary>
        public void EnableContext<T>() where T : IInputContext
        {
            var context = GetContext<T>();
            
            context?.Enable();
        }

        /// <summary>
        /// Disable an input context.
        /// </summary>
        public void DisableContext<T>() where T : IInputContext
        {
            var context = GetContext<T>();
            
            context?.Disable();
        }

        /// <summary>
        /// Check if a context is enabled.
        /// </summary>
        public bool IsContextEnabled<T>() where T : IInputContext
        {
            var context = GetContext<T>();
            
            return context?.IsEnabled ?? false;
        }

        /// <summary>
        /// Enable all contexts.
        /// </summary>
        public void EnableAll()
        {
            foreach (var context in _contexts.Values)
            {
                context.Enable();
            }
        }

        /// <summary>
        /// Disable all contexts.
        /// </summary>
        public void DisableAll()
        {
            foreach (var context in _contexts.Values)
            {
                context.Disable();
            }
        }

        /// <summary>
        /// Get all registered contexts.
        /// </summary>
        public IEnumerable<IInputContext> GetAllContexts()
        {
            return _contexts.Values;
        }
    }
}
