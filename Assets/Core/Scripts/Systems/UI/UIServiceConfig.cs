using System;
using Core.Systems.ServiceLocator;

namespace Core.Systems.UI
{
    [Serializable]
    public class UIServiceConfig : ServiceConfig
    {
        private IUIService _uiService;

        public override void Install(IServiceInstallHelper helper)
        {
            _uiService = new UIService();
            
            helper.Register(_uiService);
        }

        public override void OnUninstalled(IServiceInstallHelper helper)
        {
            base.OnUninstalled(helper);
            
            _uiService.CloseAllMenus(false);
        }
    }
}