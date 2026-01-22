using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Core.Systems.ServiceLocator
{
    /// <summary>
    /// Unified service installer for a scene.
    /// Supports configurable services via ServiceConfig.
    /// Use Odin's SerializeReference to configure services in the inspector.
    /// </summary>
    public class SceneServiceInstaller : MonoBehaviour, IServiceInstallHelper
    {
        [SerializeField] private bool installOnAwake = true;
        [SerializeReference] private List<ServiceConfig> serviceConfigs = new();
        [SerializeField] private UnityEvent onInstalled = new();
        [SerializeField] private UnityEvent onUninstalled = new();

        private readonly List<Type> _registeredServiceTypes = new();

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
            // Install all services
            foreach (var config in serviceConfigs)
            {
                if (config != null)
                {
                    config.Install(this);
                }
            }

            foreach (var config in serviceConfigs)
            {
                if (config != null)
                {
                    config.OnInstalled(this);
                }
            }
            
            onInstalled?.Invoke();

            Debug.Log($"[SceneServiceInstaller] Installed {serviceConfigs.Count} services");
        }

        /// <summary>
        /// Uninstall all services registered by this installer.
        /// </summary>
        public void Uninstall()
        {
            foreach (var config in serviceConfigs)
            {
                if (config != null)
                {
                    config.OnUninstalled(this);
                }
            }

            foreach (var serviceType in _registeredServiceTypes)
            {
                serviceType.Unregister();
            }

            _registeredServiceTypes.Clear();
            
            onUninstalled?.Invoke();

            Debug.Log($"[SceneServiceInstaller] Uninstalled services");
        }

        // IServiceInstallHelper implementation
        public void Register<T>(T service) where T : IService
        {
            ServiceLocator.Register(service);
            
            _registeredServiceTypes.Add(typeof(T));
        }

        public T Get<T>() where T : IService
        {
            return ServiceLocator.Get<T>();
        }

        public bool TryGet<T>(out T service) where T : IService
        {
            return ServiceLocator.TryGet(out service);
        }
    }
}
