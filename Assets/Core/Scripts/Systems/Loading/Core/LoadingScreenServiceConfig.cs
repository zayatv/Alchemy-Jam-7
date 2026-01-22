using System;
using System.Collections.Generic;
using Core.Systems.ServiceLocator;
using UnityEngine;

namespace Core.Systems.Loading
{
    [Serializable]
    public class LoadingScreenServiceConfig : ServiceConfig
    {
        [SerializeField] private List<MonoBehaviour> loadingScreens;
        
        private ILoadingScreenService _loadingScreenService;
        
        public override void Install(IServiceInstallHelper helper)
        {
            _loadingScreenService = new LoadingScreenService();

            foreach (var loadingScreen in loadingScreens)
            {
                if (loadingScreen.TryGetComponent(out ILoadingScreen screen))
                    _loadingScreenService.RegisterLoadingScreen(screen);
            }
            
            helper.Register(_loadingScreenService);
        }
    }
}