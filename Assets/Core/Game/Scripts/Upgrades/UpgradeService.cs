using System.Collections.Generic;
using Core.Game.Combat.Bombs;
using Core.Game.Inventory;
using UnityEngine;

namespace Core.Game.Upgrades
{
    public class UpgradeService : IUpgradeService
    {
        #region Fields

        private readonly IBombService _bombService;
        private readonly IInventoryService _inventoryService;
        private readonly List<StackableUpgradeDefinition> _allStackableUpgrades;
        private readonly List<SpecialUpgradeDefinition> _allSpecialUpgrades;

        private readonly List<UpgradeDefinition> _acquiredUpgrades = new List<UpgradeDefinition>();
        private readonly Dictionary<UpgradeDefinition, int> _stackCounts = new Dictionary<UpgradeDefinition, int>();

        #endregion

        #region Properties

        public IReadOnlyList<UpgradeDefinition> AcquiredUpgrades => _acquiredUpgrades;
        public IReadOnlyList<StackableUpgradeDefinition> AvailableStackableUpgrades => _allStackableUpgrades;
        public IReadOnlyList<SpecialUpgradeDefinition> AvailableSpecialUpgrades => _allSpecialUpgrades;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="UpgradeService"/> class.
        /// </summary>
        /// <param name="bombService">The bomb service for applying bomb-related upgrades.</param>
        /// <param name="inventoryService">The inventory service for applying inventory-related upgrades.</param>
        /// <param name="stackableUpgrades">The list of available stackable upgrades.</param>
        /// <param name="specialUpgrades">The list of available special upgrades.</param>
        public UpgradeService(
            IBombService bombService,
            IInventoryService inventoryService,
            List<StackableUpgradeDefinition> stackableUpgrades,
            List<SpecialUpgradeDefinition> specialUpgrades)
        {
            _bombService = bombService;
            _inventoryService = inventoryService;
            _allStackableUpgrades = stackableUpgrades ?? new List<StackableUpgradeDefinition>();
            _allSpecialUpgrades = specialUpgrades ?? new List<SpecialUpgradeDefinition>();
        }

        #endregion

        #region IUpgradeService Implementation

        public void AcquireUpgrade(UpgradeDefinition upgrade)
        {
            if (upgrade == null)
                return;

            _acquiredUpgrades.Add(upgrade);

            if (_stackCounts.ContainsKey(upgrade))
                _stackCounts[upgrade]++;
            else
                _stackCounts[upgrade] = 1;

            upgrade.Apply(this);
        }

        public bool HasUpgrade(UpgradeDefinition upgrade)
        {
            if (upgrade == null)
                return false;

            return _stackCounts.ContainsKey(upgrade) && _stackCounts[upgrade] > 0;
        }

        public int GetStackCount(UpgradeDefinition upgrade)
        {
            if (upgrade == null)
                return 0;

            return _stackCounts.TryGetValue(upgrade, out int count) ? count : 0;
        }

        public UpgradeDefinition GetRandomUpgrade(float specialChance = 0.3f)
        {
            bool trySpecial = Random.value < specialChance;

            if (trySpecial)
            {
                var available = GetSpawnableSpecialUpgrades();

                if (available.Count > 0)
                    return available[Random.Range(0, available.Count)];
            }

            var stackable = GetSpawnableStackableUpgrades();

            if (stackable.Count > 0)
                return stackable[Random.Range(0, stackable.Count)];

            if (!trySpecial)
            {
                var available = GetSpawnableSpecialUpgrades();

                if (available.Count > 0)
                    return available[Random.Range(0, available.Count)];
            }

            return null;
        }

        public void ResetForNewRun()
        {
            foreach (var upgrade in _acquiredUpgrades)
            {
                upgrade.Remove(this);
            }

            _acquiredUpgrades.Clear();
            _stackCounts.Clear();

            _bombService.ResetStats();
            _inventoryService.SetInfiniteBombs(false);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the bomb service for use by upgrade definitions.
        /// </summary>
        /// <returns>The bomb service instance.</returns>
        public IBombService GetBombService() => _bombService;

        /// <summary>
        /// Gets the inventory service for use by upgrade definitions.
        /// </summary>
        /// <returns>The inventory service instance.</returns>
        public IInventoryService GetInventoryService() => _inventoryService;

        #endregion

        #region Private Methods

        private List<StackableUpgradeDefinition> GetSpawnableStackableUpgrades()
        {
            var result = new List<StackableUpgradeDefinition>();

            foreach (var upgrade in _allStackableUpgrades)
            {
                if (upgrade != null && upgrade.CanSpawn(this))
                    result.Add(upgrade);
            }

            return result;
        }

        private List<SpecialUpgradeDefinition> GetSpawnableSpecialUpgrades()
        {
            var result = new List<SpecialUpgradeDefinition>();

            foreach (var upgrade in _allSpecialUpgrades)
            {
                if (upgrade != null && upgrade.CanSpawn(this))
                    result.Add(upgrade);
            }

            return result;
        }

        #endregion
    }
}
