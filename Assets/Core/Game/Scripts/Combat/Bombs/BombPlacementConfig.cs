using System;
using Core.Game.Input.Contexts;
using Core.Systems.Grid;
using Core.Systems.Input;
using Core.Systems.Logging;
using Core.Systems.ServiceLocator;
using Core.Systems.Update;
using UnityEngine;

namespace Core.Game.Combat.Bombs
{
    [Serializable]
    public class BombPlacementConfig : ServiceConfig
    {
        #region Fields

        private BombPlacementController _placementController;

        #endregion

        #region ServiceConfig Overrides

        public override void Install(IServiceInstallHelper helper)
        {
        }

        public override void OnInstalled(IServiceInstallHelper helper)
        {
            var bombService = helper.Get<IBombService>();
            var gridService = helper.Get<IGridService>();
            var inputService = helper.Get<IInputService>();

            var bombInput = inputService.GetContext<BombInputContext>();
            if (bombInput == null)
            {
                GameLogger.Log(LogLevel.Error, "BombPlacementConfig: BombInputContext not found.");
                return;
            }

            var player = GameObject.FindGameObjectWithTag("Player");
            if (player == null)
            {
                GameLogger.Log(LogLevel.Error, "BombPlacementConfig: Player not found.");
                return;
            }

            _placementController = new BombPlacementController(
                bombService,
                gridService,
                bombInput,
                player.transform
            );

            var updateService = helper.Get<IUpdateService>();
            updateService.Register(_placementController);

            GameLogger.Log(LogLevel.Debug, "BombPlacementController installed.");
        }

        public override void OnUninstalled(IServiceInstallHelper helper)
        {
            if (_placementController == null)
                return;

            if (helper.TryGet(out IUpdateService updateService))
                updateService.Unregister(_placementController);
        }

        #endregion
    }
}
