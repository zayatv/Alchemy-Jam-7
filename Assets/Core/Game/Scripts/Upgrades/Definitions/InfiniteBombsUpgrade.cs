using Core.Game.Combat.Bombs;
using Core.Game.Inventory;
using Core.Systems.ServiceLocator;
using UnityEngine;

namespace Core.Game.Upgrades.Definitions
{
    [CreateAssetMenu(fileName = "InfiniteBombs", menuName = "Game/Upgrades/Special/Infinite Bombs")]
    public class InfiniteBombsUpgrade : SpecialUpgradeDefinition
    {
        #region UpgradeDefinition Overrides

        public override void Apply(IUpgradeService upgradeService)
        {
            var bombService = ServiceLocator.Get<IBombService>();
            bombService.ModifyStats(stats => stats.InfiniteBombs = true);

            var inventoryService = ServiceLocator.Get<IInventoryService>();
            inventoryService.SetInfiniteBombs(true);
        }

        public override void Remove(IUpgradeService upgradeService)
        {
            var bombService = ServiceLocator.Get<IBombService>();
            bombService.ModifyStats(stats => stats.InfiniteBombs = false);

            var inventoryService = ServiceLocator.Get<IInventoryService>();
            inventoryService.SetInfiniteBombs(false);
        }

        #endregion
    }
}
