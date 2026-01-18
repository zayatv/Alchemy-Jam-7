using System.Collections.Generic;
using Core.Systems.ServiceLocator;

namespace Core.Game.Upgrades
{
    public interface IUpgradeService : IService
    {
        #region Properties

        /// <summary>
        /// Gets the list of all upgrades acquired during the current run.
        /// </summary>
        IReadOnlyList<UpgradeDefinition> AcquiredUpgrades { get; }

        /// <summary>
        /// Gets the list of all available stackable upgrades.
        /// </summary>
        IReadOnlyList<StackableUpgradeDefinition> AvailableStackableUpgrades { get; }

        /// <summary>
        /// Gets the list of all available special upgrades.
        /// </summary>
        IReadOnlyList<SpecialUpgradeDefinition> AvailableSpecialUpgrades { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Acquires an upgrade and applies its effects.
        /// </summary>
        /// <param name="upgrade">The upgrade to acquire.</param>
        void AcquireUpgrade(UpgradeDefinition upgrade);

        /// <summary>
        /// Checks whether a specific upgrade has been acquired.
        /// </summary>
        /// <param name="upgrade">The upgrade to check for.</param>
        /// <returns>True if the upgrade has been acquired; otherwise, false.</returns>
        bool HasUpgrade(UpgradeDefinition upgrade);

        /// <summary>
        /// Gets the number of times a specific upgrade has been acquired.
        /// </summary>
        /// <param name="upgrade">The upgrade to get the stack count for.</param>
        /// <returns>The number of times the upgrade has been acquired.</returns>
        int GetStackCount(UpgradeDefinition upgrade);

        /// <summary>
        /// Gets a random upgrade from the available pool based on spawn rules.
        /// </summary>
        /// <param name="specialChance">The probability of selecting a special upgrade.</param>
        /// <returns>A randomly selected upgrade, or null if none are available.</returns>
        UpgradeDefinition GetRandomUpgrade(float specialChance = 0.3f);

        /// <summary>
        /// Resets all upgrades and their effects for a new run.
        /// </summary>
        void ResetForNewRun();

        #endregion
    }
}
