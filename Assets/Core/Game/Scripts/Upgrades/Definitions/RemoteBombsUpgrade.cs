using Core.Game.Combat.Bombs;
using Core.Systems.ServiceLocator;
using UnityEngine;

namespace Core.Game.Upgrades.Definitions
{
    [CreateAssetMenu(fileName = "RemoteBombs", menuName = "Game/Upgrades/Special/Remote Bombs")]
    public class RemoteBombsUpgrade : SpecialUpgradeDefinition
    {
        #region UpgradeDefinition Overrides

        public override void Apply(IUpgradeService upgradeService)
        {
            var bombService = ServiceLocator.Get<IBombService>();
            bombService.ModifyStats(stats => stats.RequiresManualDetonation = true);
        }

        public override void Remove(IUpgradeService upgradeService)
        {
            var bombService = ServiceLocator.Get<IBombService>();
            bombService.ModifyStats(stats => stats.RequiresManualDetonation = false);
        }

        #endregion
    }
}
