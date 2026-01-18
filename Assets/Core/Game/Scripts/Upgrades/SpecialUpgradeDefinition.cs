using UnityEngine;

namespace Core.Game.Upgrades
{
    public abstract class SpecialUpgradeDefinition : UpgradeDefinition
    {
        #region Serialized Fields

        [Header("Exclusivity")]
        [SerializeField] private SpecialUpgradeDefinition[] excludedUpgrades;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the array of upgrades that are mutually exclusive with this upgrade.
        /// </summary>
        public SpecialUpgradeDefinition[] ExcludedUpgrades => excludedUpgrades;

        public override UpgradeRarity Rarity => UpgradeRarity.Special;

        #endregion

        #region UpgradeDefinition Overrides

        public override bool CanSpawn(IUpgradeService upgradeService)
        {
            if (upgradeService.HasUpgrade(this))
                return false;

            if (excludedUpgrades != null)
            {
                foreach (var excluded in excludedUpgrades)
                {
                    if (excluded != null && upgradeService.HasUpgrade(excluded))
                        return false;
                }
            }

            return true;
        }

        #endregion
    }
}
