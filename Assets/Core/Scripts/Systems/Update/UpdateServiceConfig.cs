using System;
using Core.Systems.ServiceLocator;
using UnityEngine;

namespace Core.Systems.Update
{
    /// <summary>
    /// Configuration for UpdateService.
    /// Gets or creates and registers the UpdateService which manages all update callbacks.
    /// </summary>
    [Serializable]
    public class UpdateServiceConfig : ServiceConfig
    {
        [Header("Update Service Configuration")]
        [SerializeField] private UpdateService updateService;

        public override void Install(IServiceInstallHelper helper)
        {
            // Create UpdateService component
            if (updateService == null)
            {
                Debug.Log("[UpdateServiceConfig] Creating UpdateService component");
                
                GameObject gameObject = new GameObject("UpdateService");
                
                updateService = gameObject.AddComponent<UpdateService>();
            }

            // Register the service
            helper.Register<IUpdateService>(updateService);

            Debug.Log("[UpdateServiceConfig] UpdateService created and registered");
        }

        public override void OnInstalled(IServiceInstallHelper helper)
        {
            Debug.Log("[UpdateServiceConfig] UpdateService ready");
        }

        public override void OnUninstalled(IServiceInstallHelper helper)
        {
            Debug.Log("[UpdateServiceConfig] UpdateService uninstalled");
        }
    }
}
