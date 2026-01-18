using System;
using Core.Systems.ServiceLocator;
using Core.Systems.Grid;
using Core.Systems.Logging;

namespace Core.Game.Combat
{
    [Serializable]
    public class TargetRegistryConfig : ServiceConfig
    {
        private TargetRegistry _targetRegistry;

        public override void Install(IServiceInstallHelper helper)
        {
            if (!helper.TryGet(out IGridService gridService))
            {
                GameLogger.Log(LogLevel.Error, "[TargetRegistryConfig] IGridService not found! Ensure GridService is installed before TargetRegistry.");
                
                return;
            }

            _targetRegistry = new TargetRegistry(gridService);

            helper.Register<ITargetRegistry>(_targetRegistry);

            GameLogger.Log(LogLevel.Info, "[TargetRegistryConfig] TargetRegistry service installed successfully");
        }

        public override void OnUninstalled(IServiceInstallHelper helper)
        {
            GameLogger.Log(LogLevel.Debug, "[TargetRegistryConfig] TargetRegistry service uninstalled");
        }
    }
}
