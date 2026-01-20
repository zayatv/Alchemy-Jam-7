using Core.Game.Upgrades;
using Core.Systems.Pooling;
using UnityEngine;

namespace Core.Game.Pickups
{
    public class PickupService : IPickupService
    {
        #region Fields

        private readonly PickupConfig _config;
        private readonly IObjectPoolService _poolService;

        #endregion

        #region Constructor

        public PickupService(PickupConfig config, IObjectPoolService poolService)
        {
            _config = config;
            _poolService = poolService;

            PrewarmPools();
        }

        #endregion

        #region IPickupService Implementation

        public Pickup SpawnHealthPickup(Vector3 position, Quaternion rotation)
        {
            if (_config.HealthPickupPrefab == null)
                return null;

            return _poolService.Spawn(_config.HealthPickupPrefab, position, rotation);
        }

        public Pickup SpawnUpgradePickup(Vector3 position, Quaternion rotation)
        {
            if (_config.UpgradePickupPrefab == null)
                return null;

            return _poolService.Spawn(_config.UpgradePickupPrefab, position, rotation);
        }

        public Pickup SpawnUpgradePickup(Vector3 position, Quaternion rotation, UpgradeDefinition upgrade)
        {
            var pickup = SpawnUpgradePickup(position, rotation);

            if (pickup != null && upgrade != null && pickup is UpgradePickup upgradePickup)
            {
                upgradePickup.SetUpgrade(upgrade);
            }

            return pickup;
        }

        public Pickup SpawnBombPickup(Vector3 position, Quaternion rotation)
        {
            if (_config.BombPickupPrefab == null)
                return null;

            return _poolService.Spawn(_config.BombPickupPrefab, position, rotation);
        }

        public void DespawnPickup(Pickup pickup)
        {
            if (pickup == null)
                return;

            switch (pickup)
            {
                case HealthPickup healthPickup:
                    _poolService.Despawn(healthPickup);
                    break;
                case UpgradePickup upgradePickup:
                    _poolService.Despawn(upgradePickup);
                    break;
                case BombPickup bombPickup:
                    _poolService.Despawn(bombPickup);
                    break;
                default:
                    pickup.gameObject.SetActive(false);
                    break;
            }
        }

        #endregion

        #region Private Methods

        private void PrewarmPools()
        {
            if (_config.PrewarmCount <= 0)
                return;

            if (_config.HealthPickupPrefab != null)
            {
                _poolService.Prewarm(_config.HealthPickupPrefab, _config.PrewarmCount);
            }

            if (_config.UpgradePickupPrefab != null)
            {
                _poolService.Prewarm(_config.UpgradePickupPrefab, _config.PrewarmCount);
            }

            if (_config.BombPickupPrefab != null)
            {
                _poolService.Prewarm(_config.BombPickupPrefab, _config.PrewarmCount);
            }
        }

        #endregion
    }
}
