using UnityEngine;

namespace Core.Game.Upgrades
{
    public abstract class UpgradeDefinition : ScriptableObject
    {
        #region Serialized Fields

        [Header("Display")]
        [SerializeField] private string upgradeName;
        [SerializeField] [TextArea] private string description;
        [SerializeField] private Sprite icon;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the display name of the upgrade.
        /// </summary>
        public string UpgradeName => upgradeName;

        /// <summary>
        /// Gets the description text for the upgrade.
        /// </summary>
        public string Description => description;

        /// <summary>
        /// Gets the icon sprite for the upgrade.
        /// </summary>
        public Sprite Icon => icon;

        /// <summary>
        /// Gets the rarity category of this upgrade.
        /// </summary>
        public abstract UpgradeRarity Rarity { get; }

        #endregion

        #region Abstract Methods

        /// <summary>
        /// Applies the upgrade's effects when acquired.
        /// </summary>
        /// <param name="upgradeService">The upgrade service managing the upgrade.</param>
        public abstract void Apply(IUpgradeService upgradeService);

        #endregion

        #region Virtual Methods

        /// <summary>
        /// Removes the upgrade's effects when the run is reset.
        /// </summary>
        /// <param name="upgradeService">The upgrade service managing the upgrade.</param>
        public virtual void Remove(IUpgradeService upgradeService) { }

        /// <summary>
        /// Determines whether this upgrade can spawn based on current game state.
        /// </summary>
        /// <param name="upgradeService">The upgrade service to check against.</param>
        /// <returns>True if the upgrade can spawn; otherwise, false.</returns>
        public virtual bool CanSpawn(IUpgradeService upgradeService)
        {
            return true;
        }

        /// <summary>
        /// Gets the current stack count of this upgrade.
        /// </summary>
        /// <param name="upgradeService">The upgrade service to query.</param>
        /// <returns>The number of times this upgrade has been acquired.</returns>
        public virtual int GetStackCount(IUpgradeService upgradeService)
        {
            return 0;
        }

        #endregion
    }
}
