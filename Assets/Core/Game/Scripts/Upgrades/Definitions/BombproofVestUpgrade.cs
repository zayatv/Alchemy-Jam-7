using Core.Game.Combat.Bombs;
using Core.Systems.ServiceLocator;
using UnityEngine;

namespace Core.Game.Upgrades.Definitions
{
    [CreateAssetMenu(fileName = "BombproofVest", menuName = "Game/Upgrades/Special/Bombproof Vest")]
    public class BombproofVestUpgrade : SpecialUpgradeDefinition
    {
        #region UpgradeDefinition Overrides

        public override void Apply(IUpgradeService upgradeService)
        {
            var bombService = ServiceLocator.Get<IBombService>();
            bombService.ModifyStats(stats => stats.PlayerImmuneToBombs = true);
        }

        public override void Remove(IUpgradeService upgradeService)
        {
            var bombService = ServiceLocator.Get<IBombService>();
            bombService.ModifyStats(stats => stats.PlayerImmuneToBombs = false);
        }

        #endregion
    }
}
