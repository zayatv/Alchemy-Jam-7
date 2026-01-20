using Core.Game.Combat;
using Core.Game.Destructibles.Events;
using Core.Systems.Audio;
using Core.Systems.Combat;
using Core.Systems.Events;
using Core.Systems.ServiceLocator;
using Core.Systems.VFX;
using UnityEngine;

namespace Core.Game.Destructibles
{
    [RequireComponent(typeof(HealthComponent))]
    public class Destructible : MonoBehaviour
    {
        #region Serialized Fields

        [Header("Configuration")]
        [Tooltip("The configuration asset defining this destructible's behavior")]
        [SerializeField] private DestructibleConfig config;

        [Header("Destruction")]
        [Tooltip("Should the GameObject be destroyed when health reaches zero?")]
        [SerializeField] private bool destroyOnDeath = true;

        #endregion

        #region Private Fields

        private HealthComponent _healthComponent;
        private IVFXService _vfxService;
        private IAudioService _audioService;

        #endregion

        #region Properties

        public DestructibleConfig Config => config;

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
                _healthComponent.OnDamageReceived += HandleDamage;
            }
        }

        private void OnDisable()
        {
            if (_healthComponent != null)
            {
                _healthComponent.OnDeath -= HandleDeath;
                _healthComponent.OnDamageReceived -= HandleDamage;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Sets the configuration at runtime.
        /// </summary>
        public void SetConfig(DestructibleConfig newConfig)
        {
            config = newConfig;
        }

        #endregion

        #region Private Methods

        private void HandleDamage(DamageInfo damageInfo)
        {
            if (!_healthComponent.IsAlive)
                return;

            PlayHitEffects();
        }

        private void HandleDeath()
        {
            PlayDestroyEffects();

            var type = config != null ? config.DestructibleType : DestructibleType.Generic;

            EventBus.Raise(new DestructibleDestroyedEvent
            {
                Type = type,
                Position = transform.position,
                DropsSpawned = 0
            });

            if (destroyOnDeath)
            {
                Destroy(gameObject);
            }
        }

        private void PlayHitEffects()
        {
            if (config == null)
                return;

            Vector3 position = transform.position;

            if (config.HitVFX != null)
            {
                EnsureVFXService();
                _vfxService?.Spawn(config.HitVFX, position);
            }

            if (config.HitSound != null)
            {
                EnsureAudioService();
                _audioService?.PlayAtPosition(config.HitSound, position);
            }
        }

        private void PlayDestroyEffects()
        {
            if (config == null)
                return;

            Vector3 position = transform.position;

            if (config.DestroyVFX != null)
            {
                EnsureVFXService();
                _vfxService?.Spawn(config.DestroyVFX, position);
            }

            if (config.DestroySound != null)
            {
                EnsureAudioService();
                _audioService?.PlayAtPosition(config.DestroySound, position);
            }
        }

        private void EnsureVFXService()
        {
            if (_vfxService == null)
            {
                ServiceLocator.TryGet(out _vfxService);
            }
        }

        private void EnsureAudioService()
        {
            if (_audioService == null)
            {
                ServiceLocator.TryGet(out _audioService);
            }
        }

        #endregion
    }
}
