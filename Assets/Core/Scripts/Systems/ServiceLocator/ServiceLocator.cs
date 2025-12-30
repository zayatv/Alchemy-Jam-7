using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Systems.ServiceLocator
{
    /// <summary>
    /// Global service locator. Services are registered/unregistered by installers when scenes load/unload.
    /// </summary>
    public static class ServiceLocator
    {
        private static readonly Dictionary<Type, object> _services = new Dictionary<Type, object>();
        
        /// <summary>
        /// Get the count of registered services (useful for debugging).
        /// </summary>
        public static int Count => _services.Count;

        /// <summary>
        /// Register a service implementation for the given interface type.
        /// </summary>
        public static void Register<T>(T service) where T : IService
        {
            var type = typeof(T);

            if (_services.ContainsKey(type))
                Debug.LogWarning($"[ServiceLocator] Service of type {type.Name} is already registered. Overwriting.");

            _services[type] = service;
        }

        /// <summary>
        /// Get a registered service. Throws if not found.
        /// </summary>
        public static T Get<T>() where T : IService
        {
            var type = typeof(T);

            if (_services.TryGetValue(type, out var service))
                return (T)service;

            throw new Exception($"[ServiceLocator] Service of type {type.Name} not registered.");
        }

        /// <summary>
        /// Try to get a registered service. Returns false if not found.
        /// </summary>
        public static bool TryGet<T>(out T service) where T : IService
        {
            var type = typeof(T);

            if (_services.TryGetValue(type, out var obj))
            {
                service = (T)obj;
                
                return true;
            }

            service = default;
            
            return false;
        }

        /// <summary>
        /// Check if a service is registered.
        /// </summary>
        public static bool IsRegistered<T>() where T : IService
        {
            return _services.ContainsKey(typeof(T));
        }

        /// <summary>
        /// Unregister a service.
        /// </summary>
        public static void Unregister<T>() where T : IService
        {
            _services.Remove(typeof(T));
        }

        /// <summary>
        /// Clear all registered services. Use with caution.
        /// </summary>
        public static void Clear()
        {
            _services.Clear();
        }

        public static IEnumerable<KeyValuePair<Type, object>> GetAllServices()
        {
            return _services;
        }
    }
}
