using Core.Game.Combat;
using Core.Game.Destructibles.Events;
using Core.Game.Pickups;
using Core.Game.Upgrades;
using Core.Systems.Audio;
using Core.Systems.Events;
using Core.Systems.ServiceLocator;
using Core.Systems.VFX;
using UnityEngine;

namespace Core.Game.Destructibles
{
    /// <summary>
    /// Specialized dropper for Item Cages that always drops an upgrade.
    /// Can be configured with a specific upgrade or use a random one.
    /// </summary>
    [RequireComponent(typeof(HealthComponent))]
    public class UpgradeDropper : MonoBehaviour
    {
        #region Serialized Fields

        [Header("Upgrade Configuration")]
        [Tooltip("Specific upgrade to drop. If null, a random upgrade will be selected.")]
        [SerializeField] private UpgradeDefinition specificUpgrade;

        [Tooltip("Chance for special upgrade when selecting randomly (0-1)")]
        [SerializeField] [Range(0f, 1f)] private float specialUpgradeChance = 0.3f;

        [Header("Spawn Settings")]
        [Tooltip("Vertical offset for spawned pickup")]
        [SerializeField] private float dropHeight = 0.5f;

        [Header("Destruction Effects")]
        [Tooltip("VFX to spawn when destroyed")]
        [SerializeField] private VFXCue destroyVFX;

        [Tooltip("Sound to play when destroyed")]
        [SerializeField] private AudioCue destroySound;

        #endregion

        #region Private Fields

        private HealthComponent _healthComponent;
        private IPickupService _pickupService;
        private IVFXService _vfxService;
        private IAudioService _audioService;
        private UpgradeDefinition _assignedUpgrade;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the upgrade that will be dropped when this cage is destroyed.
        /// </summary>
        public UpgradeDefinition AssignedUpgrade => _assignedUpgrade ?? specificUpgrade;

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
        /// Sets the specific upgrade this cage will drop.
        /// Call this when spawning the cage to assign a particular upgrade.
        /// </summary>
        public void SetUpgrade(UpgradeDefinition upgrade)
        {
            _assignedUpgrade = upgrade;
        }

        #endregion

        #region Private Methods

        private void HandleDeath()
        {
            int dropsSpawned = 0;

            PlayDestroyEffects();

            if (_pickupService == null)
            {
                ServiceLocator.TryGet(out _pickupService);
            }

            if (_pickupService != null)
            {
                Vector3 spawnPosition = transform.position;
                spawnPosition.y += dropHeight;

                var upgradeToSpawn = _assignedUpgrade ?? specificUpgrade;

                if (upgradeToSpawn != null)
                {
                    _pickupService.SpawnUpgradePickup(spawnPosition, Quaternion.identity, upgradeToSpawn);
                    dropsSpawned = 1;
                }
                else
                {
                    _pickupService.SpawnUpgradePickup(spawnPosition, Quaternion.identity);
                    dropsSpawned = 1;
                }
            }

            EventBus.Raise(new DestructibleDestroyedEvent
            {
                Type = DestructibleType.ItemCage,
                Position = transform.position,
                DropsSpawned = dropsSpawned
            });
        }

        private void PlayDestroyEffects()
        {
            Vector3 position = transform.position;

            if (destroyVFX != null)
            {
                if (_vfxService == null)
                {
                    ServiceLocator.TryGet(out _vfxService);
                }

                _vfxService?.Spawn(destroyVFX, position);
            }

            if (destroySound != null)
            {
                if (_audioService == null)
                {
                    ServiceLocator.TryGet(out _audioService);
                }

                _audioService?.PlayAtPosition(destroySound, position);
            }
        }

        #endregion
    }
}
