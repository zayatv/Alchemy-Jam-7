using System;
using Core.Systems.ServiceLocator;
using Core.Systems.Pooling;
using Core.Systems.Update;

namespace Core.Systems.VFX
{
    [Serializable]
    public class VFXServiceConfig : ServiceConfig
    {
        private IVFXService _vfxService;
        
        public override void Install(IServiceInstallHelper helper)
        {
            IObjectPoolService poolService = helper.Get<IObjectPoolService>();
            IUpdateService updateService = helper.Get<IUpdateService>();

            _vfxService = new VFXService(poolService);

            helper.Register(_vfxService);

            updateService.Register(_vfxService);
        }

        public override void OnUninstalled(IServiceInstallHelper helper)
        {
            if (helper.TryGet(out IUpdateService updateService) && _vfxService != null)
            {
                updateService.Unregister(_vfxService);
            }
        }
    }
}
