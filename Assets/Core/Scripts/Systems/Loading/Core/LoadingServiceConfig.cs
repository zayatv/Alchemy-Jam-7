using System;
using Core.Systems.ServiceLocator;

namespace Core.Systems.Loading
{
    [Serializable]
    public class LoadingServiceConfig : ServiceConfig
    {
        private ILoadingService _loadingService;
        
        public override void Install(IServiceInstallHelper helper)
        {
            _loadingService = new LoadingService();
            
            helper.Register(_loadingService);
        }
    }
}