using System;
using System.Collections.Generic;
using Core.Systems.ServiceLocator;
using UnityEngine;

namespace Core.Systems.UI.HUD
{
    [Serializable]
    public class HUDServiceConfig : ServiceConfig
    {
        private IHUDService _hudService;
        
        public override void Install(IServiceInstallHelper helper)
        {
            _hudService = new HUDService();
            
            helper.Register(_hudService);
        }

        public override void OnInstalled(IServiceInstallHelper helper)
        {
            base.OnInstalled(helper);
            
            _hudService.SubscribeToEvents();
        }

        public override void OnUninstalled(IServiceInstallHelper helper)
        {
            base.OnUninstalled(helper);
            
            _hudService.UnsubscribeFromEvents();
        }
    }
}