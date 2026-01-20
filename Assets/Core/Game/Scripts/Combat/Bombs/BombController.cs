using System;
using System.Collections.Generic;
using Core.Systems.Audio;
using Core.Systems.Combat;
using Core.Systems.Grid;
using Core.Systems.Pooling;
using Core.Systems.Update;
using Core.Systems.VFX;
using UnityEngine;

namespace Core.Game.Combat.Bombs
{
    public class BombController : MonoBehaviour, IPoolable, IUpdatable
    {
        #region Fields

        private BombDefinition _definition;
        private BombStats _stats;
        private BombEffectOrchestrator _effectOrchestrator;
        private IGridService _gridService;
        private ITargetRegistry _targetRegistry;
        private IUpdateService _updateService;
        private Action<BombController> _onDetonateCallback;

        private float _timeRemaining;
        private bool _isActive;
        private bool _waitingForManualDetonation;
        private TileCoordinate _tile;
        private TeamType _sourceTeam;
        private int _bombId;

        private VFXHandle _fuseVFXHandle;
        private AudioHandle _fuseSoundHandle;
        private readonly List<VFXHandle> _indicatorHandles = new List<VFXHandle>();

        #endregion

        #region Properties

        public int UpdatePriority => 50;
        public TileCoordinate Tile => _tile;
        public int BombId => _bombId;
        public GameObject GameObject => gameObject;

        #endregion

        #region Public Methods

        /// <summary>
        /// Initializes the bomb with all required dependencies and configuration.
        /// </summary>
        /// <param name="definition">The bomb definition containing visual and audio assets.</param>
        /// <param name="stats">The bomb statistics defining behavior.</param>
        /// <param name="tile">The tile coordinate where the bomb is placed.</param>
        /// <param name="sourceTeam">The team that placed the bomb.</param>
        /// <param name="bombId">The unique identifier for this bomb.</param>
        /// <param name="effectOrchestrator">The effect orchestrator for visual and audio effects.</param>
        /// <param name="gridService">The grid service for tile operations.</param>
        /// <param name="targetRegistry">The target registry for finding damageable targets.</param>
        /// <param name="updateService">The update service to register the updatable.</param>
        /// <param name="onDetonateCallback">Callback invoked when the bomb detonates.</param>
        public void Initialize(
            BombDefinition definition,
            BombStats stats,
            TileCoordinate tile,
            TeamType sourceTeam,
            int bombId,
            BombEffectOrchestrator effectOrchestrator,
            IGridService gridService,
            ITargetRegistry targetRegistry,
            IUpdateService updateService,
            Action<BombController> onDetonateCallback)
        {
            _definition = definition;
            _stats = stats;
            _tile = tile;
            _sourceTeam = sourceTeam;
            _bombId = bombId;
            _effectOrchestrator = effectOrchestrator;
            _gridService = gridService;
            _targetRegistry = targetRegistry;
            _updateService = updateService;
            _onDetonateCallback = onDetonateCallback;
            
            _updateService.Register(this);

            _timeRemaining = _stats.FuseTime;
            _isActive = true;
            _waitingForManualDetonation = _stats.RequiresManualDetonation;

            Vector3 worldPos = _gridService.TileToCenteredWorld(_tile);
            transform.position = worldPos;

            SpawnPlacementEffects();
            SpawnIndicatorVFX();
        }

        /// <summary>
        /// Forces the bomb to detonate immediately, bypassing the fuse timer.
        /// </summary>
        public void ForceDetonate()
        {
            if (!_isActive)
                return;

            Detonate();
        }

        #endregion

        #region IUpdatable Implementation

        public void OnUpdate(float deltaTime)
        {
            if (!_isActive)
                return;

            if (_waitingForManualDetonation)
                return;

            _timeRemaining -= deltaTime;

            if (_timeRemaining <= 0f)
                Detonate();
        }

        #endregion

        #region IPoolable Implementation

        public void OnSpawnFromPool()
        {
            _isActive = false;
            _waitingForManualDetonation = false;
            _indicatorHandles.Clear();
        }

        public void OnReturnToPool()
        {
            _isActive = false;
            _waitingForManualDetonation = false;

            CleanupFuseEffects();
            CleanupIndicatorVFX();

            _definition = null;
            _stats = null;
            _onDetonateCallback = null;
        }

        #endregion

        #region Private Methods

        private void Detonate()
        {
            _isActive = false;

            CleanupFuseEffects();
            CleanupIndicatorVFX();

            var centerPosition = _tile.ToCenteredWorldPosition(_gridService.TileSize);

            SpawnExplosionVFX();
            ApplyDamageToTargets(centerPosition);

            _onDetonateCallback?.Invoke(this);
        }

        private void SpawnPlacementEffects()
        {
            Vector3 worldPos = transform.position;

            _effectOrchestrator.PlayPlacementSound(_definition, worldPos);

            _fuseVFXHandle = _effectOrchestrator.SpawnFuseVFX(_definition, transform);
            _fuseSoundHandle = _effectOrchestrator.PlayFuseSound(_definition, transform);
        }

        private void SpawnIndicatorVFX()
        {
            _indicatorHandles.Clear();

            var radius = _stats.GetExplosionRadius();
            var handle = _effectOrchestrator.SpawnIndicatorVFX(_definition, _tile, radius);

            if (handle.IsValid)
                _indicatorHandles.Add(handle);
        }

        private void SpawnExplosionVFX()
        {
            _effectOrchestrator.SpawnExplosionVFX(_definition, _tile, _stats.GetExplosionRadius());
            _effectOrchestrator.PlayExplosionSound(_definition, transform.position);
        }

        private void ApplyDamageToTargets(Vector3 centerPosition)
        {
            var radius = _stats.GetExplosionRadius();
            var targets = _targetRegistry.GetTargetsInRadius(centerPosition, radius);
            var blockingLayers = _definition.ExplosionBlockingLayers;

            foreach (var target in targets)
            {
                if (!target.IsTargetable)
                    continue;

                if (!ShouldDamageTarget(target))
                    continue;

                if (IsExplosionBlocked(centerPosition, target.Transform.position, blockingLayers))
                    continue;

                if (target is IDamageable damageable)
                {
                    var damageInfo = new DamageInfo(
                        _stats.Damage,
                        _sourceTeam,
                        DamageSource.Bomb,
                        _tile,
                        transform.position,
                        this
                    );

                    damageable.TakeDamage(damageInfo);
                }
            }
        }

        private bool IsExplosionBlocked(Vector3 explosionCenter, Vector3 targetPosition, LayerMask blockingLayers)
        {
            if (blockingLayers == 0)
                return false;

            Vector3 direction = targetPosition - explosionCenter;
            float distance = direction.magnitude;

            return Physics.Raycast(explosionCenter, direction.normalized, distance, blockingLayers);
        }

        private bool ShouldDamageTarget(ITargetable target)
        {
            if (_sourceTeam == TeamType.Player)
            {
                if (target.TargetType == TargetType.Player && _stats.PlayerImmuneToBombs)
                    return false;

                return true;
            }

            if (_sourceTeam == TeamType.Enemy)
            {
                return target.TargetType == TargetType.Player;
            }

            return true;
        }

        private void CleanupFuseEffects()
        {
            _effectOrchestrator.StopFuseEffects(_fuseVFXHandle, _fuseSoundHandle);
            _fuseVFXHandle = VFXHandle.Invalid;
            _fuseSoundHandle = AudioHandle.Invalid;
        }

        private void CleanupIndicatorVFX()
        {
            foreach (var handle in _indicatorHandles)
            {
                _effectOrchestrator.StopVFX(handle);
            }

            _indicatorHandles.Clear();
        }

        #endregion
    }
}
