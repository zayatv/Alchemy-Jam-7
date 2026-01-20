using System;
using Core.Systems.Logging;
using Core.Systems.Pooling;
using Core.Systems.ServiceLocator;
using UnityEngine;

namespace Core.Game.Pickups
{
    [Serializable]
    public class PickupServiceConfig : ServiceConfig
    {
        [SerializeField] private PickupConfig pickupConfig;
        
        private IPickupService _pickupService;
        
        public override void Install(IServiceInstallHelper helper)
        {
            if (!helper.TryGet(out IObjectPoolService objectPoolService))
            {
                GameLogger.Log(LogLevel.Error, "IObjectPoolService not found! Ensure ObjectPoolService is installed before PickupService.");

                return;
            }
            
            _pickupService = new PickupService(pickupConfig, objectPoolService);
            
            helper.Register(_pickupService);
        }
    }
}