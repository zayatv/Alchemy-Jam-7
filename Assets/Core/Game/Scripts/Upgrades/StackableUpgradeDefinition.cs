using UnityEngine;

namespace Core.Game.Upgrades
{
    public abstract class StackableUpgradeDefinition : UpgradeDefinition
    {
        #region Serialized Fields

        [Header("Stacking")]
        [SerializeField] private int maxStacks = 99;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the maximum number of times this upgrade can be stacked.
        /// </summary>
        public int MaxStacks => maxStacks;

        public override UpgradeRarity Rarity => UpgradeRarity.Stackable;

        #endregion

        #region UpgradeDefinition Overrides

        public override int GetStackCount(IUpgradeService upgradeService)
        {
            return upgradeService.GetStackCount(this);
        }

        public override bool CanSpawn(IUpgradeService upgradeService)
        {
            return GetStackCount(upgradeService) < maxStacks;
        }

        #endregion
    }
}
