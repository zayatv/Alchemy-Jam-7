using System;
using Core.Systems.ServiceLocator;
using UnityEngine;

namespace Core.Systems.UI
{
    [Serializable]
    public class UIServiceConfig : ServiceConfig
    {
        [Header("Menu Management")] 
        [SerializeField]
        private Menu[] allMenus;
        
        private IUIService _uiService;

        public override void Install(IServiceInstallHelper helper)
        {
            _uiService = new UIService(allMenus);
            
            helper.Register(_uiService);
        }

        public override void OnUninstalled(IServiceInstallHelper helper)
        {
            base.OnUninstalled(helper);
            
            _uiService.CloseAllMenus(false);
        }
    }
}