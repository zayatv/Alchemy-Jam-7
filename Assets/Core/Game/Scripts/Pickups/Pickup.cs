using Core.Systems.Audio;
using Core.Systems.Logging;
using Core.Systems.Pooling;
using Core.Systems.ServiceLocator;
using Core.Systems.VFX;
using UnityEngine;

namespace Core.Game.Pickups
{
    public abstract class Pickup : MonoBehaviour, IPoolable
    {
        #region Constants

        private const int MaxOverlapResults = 8;

        #endregion

        #region Serialized Fields

        [Header("Pickup Settings")]
        [Tooltip("Layer mask for entities that can pick up this item")]
        [SerializeField] private LayerMask pickupLayers;

        [Tooltip("Radius for detecting collectors")]
        [SerializeField] private float pickupRadius = 0.5f;

        [Header("Effects")]
        [Tooltip("VFX to spawn when pickup is collected")]
        [SerializeField] private VFXCue collectVFX;

        [Tooltip("Sound to play when pickup is collected")]
        [SerializeField] private AudioCue collectSound;

        #endregion

        #region Private Fields

        private static readonly Collider[] OverlapResults = new Collider[MaxOverlapResults];

        private IObjectPoolService _poolService;
        private IVFXService _vfxService;
        private IAudioService _audioService;
        private bool _isCollected;

        #endregion

        #region IPoolable Implementation

        public GameObject GameObject => gameObject;

        public void OnSpawnFromPool()
        {
            _isCollected = false;
            InitializeServices();
            OnPickupSpawned();
        }

        public void OnReturnToPool()
        {
            _isCollected = false;
            OnPickupDespawned();
        }

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            InitializeServices();
        }

        private void OnEnable()
        {
            _isCollected = false;
        }

        private void FixedUpdate()
        {
            if (_isCollected)
                return;

            CheckForCollector();
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Override to apply the pickup effect to the collector.
        /// </summary>
        /// <param name="collector">The GameObject that collected this pickup.</param>
        /// <returns>True if the pickup was successfully applied, false otherwise.</returns>
        protected abstract bool TryApplyPickup(GameObject collector);

        /// <summary>
        /// Called when the pickup is spawned from the pool.
        /// Override to perform additional initialization.
        /// </summary>
        protected virtual void OnPickupSpawned() { }

        /// <summary>
        /// Called when the pickup is returned to the pool.
        /// Override to perform additional cleanup.
        /// </summary>
        protected virtual void OnPickupDespawned() { }

        /// <summary>
        /// Called after the pickup has been successfully collected.
        /// Override to play effects, sounds, etc.
        /// </summary>
        /// <param name="collector">The GameObject that collected this pickup.</param>
        protected virtual void OnPickupCollected(GameObject collector) { }

        #endregion

        #region Private Methods

        private void InitializeServices()
        {
            GameLogger.Log(LogLevel.Debug, $"Initializing services for {gameObject.name}");
            
            if (_poolService == null)
            {
                ServiceLocator.TryGet(out _poolService);
                
                GameLogger.Log(LogLevel.Debug, $"Found pool service: {_poolService != null}");
            }

            if (_vfxService == null)
            {
                ServiceLocator.TryGet(out _vfxService);
                
                GameLogger.Log(LogLevel.Debug, $"Found VFX service: {_vfxService != null}");
            }

            if (_audioService == null)
            {
                ServiceLocator.TryGet(out _audioService);
                
                GameLogger.Log(LogLevel.Debug, $"Found audio service: {_audioService != null}");
            }
        }

        private void CheckForCollector()
        {
            int count = Physics.OverlapSphereNonAlloc(
                transform.position,
                pickupRadius,
                OverlapResults,
                pickupLayers
            );

            for (int i = 0; i < count; i++)
            {
                var collector = OverlapResults[i].gameObject;
                
                InitializeServices();

                if (TryApplyPickup(collector))
                {
                    _isCollected = true;

                    PlayCollectEffects();
                    OnPickupCollected(collector);
                    ReturnToPool();
                    return;
                }
            }
        }

        private void PlayCollectEffects()
        {
            Vector3 position = transform.position;

            if (collectVFX != null && _vfxService != null)
            {
                _vfxService.Spawn(collectVFX, position);
                
                GameLogger.Log(LogLevel.Debug, $"Playing collect VFX at {position}");
            }

            if (collectSound != null && _audioService != null)
            {
                _audioService.PlayAtPosition(collectSound, position);
            }
        }

        private void ReturnToPool()
        {
            if (_poolService != null)
            {
                _poolService.Despawn(this);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        #endregion
    }
}
