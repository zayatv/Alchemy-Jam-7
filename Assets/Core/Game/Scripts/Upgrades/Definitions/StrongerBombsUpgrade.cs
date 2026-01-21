using Core.Game.Combat.Bombs;
using Core.Systems.ServiceLocator;
using UnityEngine;

namespace Core.Game.Upgrades.Definitions
{
    [CreateAssetMenu(fileName = "StrongerBombs", menuName = "Core/Game/Upgrades/Stackable/Stronger Bombs")]
    public class StrongerBombsUpgrade : StackableUpgradeDefinition
    {
        #region Serialized Fields

        [Header("Effect")]
        [SerializeField] private int damageIncrease = 1;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the amount of damage increase per stack.
        /// </summary>
        public int DamageIncrease => damageIncrease;

        #endregion

        #region UpgradeDefinition Overrides

        public override void Apply(IUpgradeService upgradeService)
        {
            var bombService = ServiceLocator.Get<IBombService>();
            bombService.ModifyStats(stats => stats.Damage += damageIncrease);
        }

        public override void Remove(IUpgradeService upgradeService)
        {
            var bombService = ServiceLocator.Get<IBombService>();
            int totalIncrease = damageIncrease * GetStackCount(upgradeService);
            bombService.ModifyStats(stats => stats.Damage -= totalIncrease);
        }

        #endregion
    }
}
