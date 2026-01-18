using System;
using System.Collections.Generic;
using Core.Game.Combat.Bombs;
using Core.Game.Inventory;
using Core.Systems.Logging;
using Core.Systems.ServiceLocator;
using UnityEngine;

namespace Core.Game.Upgrades
{
    [Serializable]
    public class UpgradeServiceConfig : ServiceConfig
    {
        #region Serialized Fields

        [Header("Stackable Upgrades")]
        [SerializeField] private List<StackableUpgradeDefinition> stackableUpgrades = new List<StackableUpgradeDefinition>();

        [Header("Special Upgrades")]
        [SerializeField] private List<SpecialUpgradeDefinition> specialUpgrades = new List<SpecialUpgradeDefinition>();

        [Header("Spawn Settings")]
        [SerializeField] [Range(0f, 1f)] private float specialUpgradeChance = 0.3f;

        #endregion

        #region Fields

        private UpgradeService _upgradeService;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the configured chance for special upgrades to spawn.
        /// </summary>
        public float SpecialUpgradeChance => specialUpgradeChance;

        #endregion

        #region ServiceConfig Overrides

        public override void Install(IServiceInstallHelper helper)
        {
            var bombService = helper.Get<IBombService>();
            var inventoryService = helper.Get<IInventoryService>();

            _upgradeService = new UpgradeService(
                bombService,
                inventoryService,
                stackableUpgrades,
                specialUpgrades
            );

            helper.Register<IUpgradeService>(_upgradeService);

            GameLogger.Log(LogLevel.Debug, $"UpgradeService installed. Stackable: {stackableUpgrades.Count}, Special: {specialUpgrades.Count}");
        }

        #endregion
    }
}
