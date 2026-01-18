using System;
using Core.Game.Inventory;
using Core.Systems.Audio;
using Core.Systems.Logging;
using Core.Systems.Pooling;
using Core.Systems.Grid;
using Core.Systems.ServiceLocator;
using Core.Systems.Update;
using Core.Systems.VFX;
using UnityEngine;

namespace Core.Game.Combat.Bombs
{
    [Serializable]
    public class BombServiceConfig : ServiceConfig
    {
        #region Serialized Fields

        [Header("Bomb Configuration")]
        [SerializeField] private BombDefinition defaultBombDefinition;
        [SerializeField] private int poolPrewarmCount = 5;

        #endregion

        #region Fields

        private BombService _bombService;

        #endregion

        #region ServiceConfig Overrides

        public override void Install(IServiceInstallHelper helper)
        {
            if (defaultBombDefinition == null)
            {
                GameLogger.Log(LogLevel.Error, "BombServiceConfig: No default bomb definition assigned.");
                return;
            }

            if (defaultBombDefinition.BombPrefab == null)
            {
                GameLogger.Log(LogLevel.Error, "BombServiceConfig: Bomb definition has no prefab assigned.");
                return;
            }

            var poolService = helper.Get<IObjectPoolService>();
            var gridService = helper.Get<IGridService>();
            var vfxService = helper.Get<IVFXService>();
            var audioService = helper.Get<IAudioService>();
            var targetRegistry = helper.Get<ITargetRegistry>();
            var inventoryService = helper.Get<IInventoryService>();
            var updateService = helper.Get<IUpdateService>();

            _bombService = new BombService(
                defaultBombDefinition,
                poolService,
                gridService,
                vfxService,
                audioService,
                targetRegistry,
                inventoryService,
                updateService
            );

            helper.Register<IBombService>(_bombService);

            poolService.Prewarm(defaultBombDefinition.BombPrefab, poolPrewarmCount);

            GameLogger.Log(LogLevel.Debug, $"BombService installed. Prewarmed {poolPrewarmCount} bomb instances.");
        }

        public override void OnUninstalled(IServiceInstallHelper helper)
        {
            if (_bombService == null)
                return;

            if (helper.TryGet(out IUpdateService updateService))
                updateService.Unregister(_bombService);
        }

        #endregion
    }
}
