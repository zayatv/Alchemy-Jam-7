using System;
using UnityEngine;

namespace Core.Systems.ServiceLocator
{
    /// <summary>
    /// Base class for service configurations.
    /// Implement this to create configurable services that can be installed via SceneServiceInstaller.
    /// Supports Odin's SerializeReference for polymorphic serialization in the inspector.
    /// </summary>
    [Serializable]
    public abstract class ServiceConfig
    {
        /// <summary>
        /// Install this service.
        /// Use helper.Register() to register services.
        /// Use helper.Get() to get other services.
        /// </summary>
        public abstract void Install(IServiceInstallHelper helper);

        /// <summary>
        /// Called after the service has been installed.
        /// Use helper.Get() to access registered services.
        /// </summary>
        public virtual void OnInstalled(IServiceInstallHelper helper) { }

        /// <summary>
        /// Called before the service is uninstalled.
        /// Perform any cleanup here.
        /// </summary>
        public virtual void OnUninstalled(IServiceInstallHelper helper) { }
    }

    /// <summary>
    /// Helper interface for service installation.
    /// Provides access to service registration and retrieval.
    /// </summary>
    public interface IServiceInstallHelper
    {
        /// <summary>
        /// Register a service globally.
        /// </summary>
        void Register<T>(T service) where T : IService;

        /// <summary>
        /// Get a registered service.
        /// </summary>
        T Get<T>() where T : IService;

        /// <summary>
        /// Try to get a registered service.
        /// </summary>
        bool TryGet<T>(out T service) where T : IService;
    }
}
