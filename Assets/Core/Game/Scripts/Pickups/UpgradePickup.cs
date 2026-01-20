using Core.Game.Pickups.Events;
using Core.Game.Upgrades;
using Core.Systems.Events;
using Core.Systems.ServiceLocator;
using UnityEngine;

namespace Core.Game.Pickups
{
    public class UpgradePickup : Pickup
    {
        #region Serialized Fields

        [Header("Upgrade Pickup Settings")]
        [Tooltip("Specific upgrade to grant. If null, a random upgrade will be selected.")]
        [SerializeField] private UpgradeDefinition specificUpgrade;

        [Tooltip("Chance for special upgrade when selecting randomly (0-1)")]
        [SerializeField] [Range(0f, 1f)] private float specialUpgradeChance = 0.3f;

        #endregion

        #region Private Fields

        private IUpgradeService _upgradeService;
        private UpgradeDefinition _assignedUpgrade;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the upgrade that will be granted when this pickup is collected.
        /// </summary>
        public UpgradeDefinition AssignedUpgrade => _assignedUpgrade;

        #endregion

        #region Public Methods

        /// <summary>
        /// Sets the specific upgrade this pickup will grant.
        /// Call this after spawning to assign a particular upgrade.
        /// </summary>
        /// <param name="upgrade">The upgrade to assign.</param>
        public void SetUpgrade(UpgradeDefinition upgrade)
        {
            _assignedUpgrade = upgrade;
        }

        #endregion

        #region Protected Methods

        protected override void OnPickupSpawned()
        {
            if (_upgradeService == null)
            {
                ServiceLocator.TryGet(out _upgradeService);
            }

            if (specificUpgrade != null)
            {
                _assignedUpgrade = specificUpgrade;
            }
            else if (_upgradeService != null)
            {
                _assignedUpgrade = _upgradeService.GetRandomUpgrade(specialUpgradeChance);
            }
        }

        protected override void OnPickupDespawned()
        {
            _assignedUpgrade = null;
        }

        protected override bool TryApplyPickup(GameObject collector)
        {
            if (_upgradeService == null)
            {
                if (!ServiceLocator.TryGet(out _upgradeService))
                    return false;
            }

            if (_assignedUpgrade == null)
                return false;

            if (!_assignedUpgrade.CanSpawn(_upgradeService))
                return false;

            _upgradeService.AcquireUpgrade(_assignedUpgrade);
            return true;
        }

        protected override void OnPickupCollected(GameObject collector)
        {
            EventBus.Raise(new PickupCollectedEvent
            {
                Type = PickupType.Upgrade,
                Position = transform.position
            });

            if (_assignedUpgrade != null)
            {
                EventBus.Raise(new UpgradePickupCollectedEvent
                {
                    Upgrade = _assignedUpgrade
                });
            }
        }

        #endregion
    }
}
