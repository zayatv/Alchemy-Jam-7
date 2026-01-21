using System;
using Core.Systems.Logging;
using Core.Systems.ServiceLocator;
using UnityEngine;

namespace Core.Systems.Animations
{
    [Serializable]
    public class DOTweenAnimationServiceConfig : ServiceConfig
    {
        [SerializeField] private DOTweenAnimationService _service;
        
        public override void Install(IServiceInstallHelper helper)
        {
            if (_service == null)
            {
                GameLogger.Log(LogLevel.Error, "[DOTweenAnimationServiceConfig] DOTweenAnimationService is null! Cannot install DOTweenAnimationService.");

                return;
            }
            
            helper.Register(_service);
        }
    }
}