using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Systems.ServiceLocator
{
    /// <summary>
    /// Base class for service installers. Inherit from this to create installers that register services globally.
    /// Services are registered when the scene loads and unregistered when the scene unloads.
    /// </summary>
    public abstract class ServiceInstaller : MonoBehaviour
    {
        [SerializeField] private bool installOnAwake = true;

        private readonly List<Type> _registeredServiceTypes = new List<Type>();

        private void Awake()
        {
            if (installOnAwake)
                Install();
        }

        private void OnDestroy()
        {
            Uninstall();
        }

        /// <summary>
        /// Manually trigger installation. Useful if installOnAwake is false.
        /// </summary>
        public void Install()
        {
            InstallServices();
            OnInstalled();
        }

        /// <summary>
        /// Uninstall all services registered by this installer.
        /// </summary>
        public void Uninstall()
        {
            foreach (var serviceType in _registeredServiceTypes)
            {
                serviceType.Unregister();
            }

            _registeredServiceTypes.Clear();
            
            OnUninstalled();
        }

        /// <summary>
        /// Override this to install your services.
        /// Use Register() to add services globally.
        /// </summary>
        protected abstract void InstallServices();

        /// <summary>
        /// Called after services have been installed.
        /// </summary>
        protected virtual void OnInstalled() { }

        /// <summary>
        /// Called after services have been uninstalled (scene unload).
        /// </summary>
        protected virtual void OnUninstalled() { }

        /// <summary>
        /// Helper method to register a service globally and track it for cleanup.
        /// </summary>
        protected void Register<T>(T service) where T : IService
        {
            ServiceLocator.Register(service);
            
            _registeredServiceTypes.Add(typeof(T));
        }

        /// <summary>
        /// Helper method to get a service.
        /// </summary>
        protected T Get<T>() where T : IService
        {
            return ServiceLocator.Get<T>();
        }

        /// <summary>
        /// Helper method to try getting a service.
        /// </summary>
        protected bool TryGet<T>(out T service) where T : IService
        {
            return ServiceLocator.TryGet(out service);
        }
    }

    // Extension for ServiceLocator to support type-based unregistration (internal use only)
    internal static class ServiceLocatorExtensions
    {
        public static void Unregister(this Type serviceType)
        {
            var method = typeof(ServiceLocator).GetMethod("Unregister");
            var genericMethod = method?.MakeGenericMethod(serviceType);
            
            genericMethod?.Invoke(null, null);
        }
    }
}
