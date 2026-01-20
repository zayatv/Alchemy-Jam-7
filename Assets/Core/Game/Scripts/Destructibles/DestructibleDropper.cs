using System.Collections.Generic;
using Core.Game.Combat;
using Core.Game.Pickups;
using Core.Systems.Audio;
using Core.Systems.ServiceLocator;
using Core.Systems.VFX;
using UnityEngine;

namespace Core.Game.Destructibles
{
    [RequireComponent(typeof(HealthComponent))]
    public class DestructibleDropper : MonoBehaviour
    {
        #region Serialized Fields

        [Header("Drop Configuration")]
        [Tooltip("The drop table defining what this destructible can drop")]
        [SerializeField] private DropTable dropTable;

        [Header("Spawn Settings")]
        [Tooltip("Vertical offset for spawned pickups")]
        [SerializeField] private float dropHeight = 0.5f;

        #endregion

        #region Private Fields

        private HealthComponent _healthComponent;
        private IPickupService _pickupService;
        private IVFXService _vfxService;
        private IAudioService _audioService;

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            _healthComponent = GetComponent<HealthComponent>();
        }

        private void OnEnable()
        {
            if (_healthComponent != null)
            {
                _healthComponent.OnDeath += HandleDeath;
            }
        }

        private void OnDisable()
        {
            if (_healthComponent != null)
            {
                _healthComponent.OnDeath -= HandleDeath;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Sets the drop table at runtime.
        /// </summary>
        public void SetDropTable(DropTable table)
        {
            dropTable = table;
        }

        #endregion

        #region Private Methods

        private void HandleDeath()
        {
            int dropsSpawned = 0;

            if (dropTable != null)
            {
                if (_pickupService == null)
                {
                    ServiceLocator.TryGet(out _pickupService);
                }

                if (_pickupService != null)
                {
                    var drops = dropTable.GetAllDrops();
                    
                    dropsSpawned = SpawnDrops(drops);
                }
            }
        }

        private int SpawnDrops(List<DropItem> drops)
        {
            int count = 0;

            foreach (var drop in drops)
            {
                if (drop.DropType == DropType.None)
                    continue;

                for (int i = 0; i < drop.Amount; i++)
                {
                    Vector3 spawnPosition = GetSpawnPosition();
                    SpawnDrop(drop, spawnPosition);
                    count++;
                }
            }

            return count;
        }

        private void SpawnDrop(DropItem drop, Vector3 position)
        {
            switch (drop.DropType)
            {
                case DropType.Health:
                    _pickupService.SpawnHealthPickup(position, Quaternion.identity);
                    break;

                case DropType.Bomb:
                    _pickupService.SpawnBombPickup(position, Quaternion.identity);
                    break;

                case DropType.Upgrade:
                    if (drop.RandomUpgrade)
                    {
                        _pickupService.SpawnUpgradePickup(position, Quaternion.identity);
                    }
                    else if (drop.SpecificUpgrade != null)
                    {
                        _pickupService.SpawnUpgradePickup(position, Quaternion.identity, drop.SpecificUpgrade);
                    }
                    break;
            }
        }

        private Vector3 GetSpawnPosition()
        {
            Vector3 basePosition = transform.position;
            basePosition.y += dropHeight;

            if (dropTable.DropSpreadRadius > 0f)
            {
                Vector2 randomOffset = Random.insideUnitCircle * dropTable.DropSpreadRadius;
                basePosition.x += randomOffset.x;
                basePosition.z += randomOffset.y;
            }

            return basePosition;
        }

        #endregion
    }
}
