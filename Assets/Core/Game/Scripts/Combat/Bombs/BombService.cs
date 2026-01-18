using System;
using Core.Game.Inventory;
using Core.Systems.Audio;
using Core.Systems.Combat;
using Core.Systems.Events;
using Core.Systems.Grid;
using Core.Systems.Pooling;
using Core.Systems.Update;
using Core.Systems.VFX;
using UnityEngine;

namespace Core.Game.Combat.Bombs
{
    public class BombService : IBombService, IUpdatable
    {
        #region Fields

        private readonly BombDefinition _defaultDefinition;
        private readonly IObjectPoolService _poolService;
        private readonly IGridService _gridService;
        private readonly ITargetRegistry _targetRegistry;
        private readonly IInventoryService _inventoryService;
        private readonly IUpdateService _updateService;
        private readonly BombEffectOrchestrator _effectOrchestrator;

        private BombStats _currentStats;
        private BombStats _baseStats;
        private float _cooldownTimer;
        private BombController _activeBomb;
        private int _nextBombId;

        #endregion

        #region Properties

        public int UpdatePriority => 10;
        public BombStats CurrentStats => _currentStats;
        public bool HasActiveBomb => _activeBomb != null;
        public float CooldownRemaining => Mathf.Max(0f, _cooldownTimer);

        public bool CanPlaceBomb =>
            _cooldownTimer <= 0f &&
            HasBombsAvailable;

        private bool HasBombsAvailable =>
            _currentStats.InfiniteBombs || _inventoryService.BombCount > 0;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="BombService"/> class.
        /// </summary>
        /// <param name="defaultDefinition">The default bomb definition to use.</param>
        /// <param name="poolService">The object pool service for bomb instances.</param>
        /// <param name="gridService">The grid service for tile operations.</param>
        /// <param name="vfxService">The VFX service for visual effects.</param>
        /// <param name="audioService">The audio service for sound effects.</param>
        /// <param name="targetRegistry">The target registry for damage application.</param>
        /// <param name="inventoryService">The inventory service for bomb count management.</param>\
        /// <param name="updateService">The update service to register the updatable.</param>
        public BombService(
            BombDefinition defaultDefinition,
            IObjectPoolService poolService,
            IGridService gridService,
            IVFXService vfxService,
            IAudioService audioService,
            ITargetRegistry targetRegistry,
            IInventoryService inventoryService,
            IUpdateService updateService)
        {
            _defaultDefinition = defaultDefinition;
            _poolService = poolService;
            _gridService = gridService;
            _targetRegistry = targetRegistry;
            _inventoryService = inventoryService;
            _updateService = updateService;

            _effectOrchestrator = new BombEffectOrchestrator(vfxService, audioService, gridService);

            _baseStats = _defaultDefinition.CreateBaseStats();
            _currentStats = _baseStats.Clone();
            _cooldownTimer = 0f;
            _nextBombId = 0;
            
            _updateService.Register(this);
        }

        #endregion

        #region IUpdatable Implementation

        public void OnUpdate(float deltaTime)
        {
            if (_cooldownTimer > 0f)
                _cooldownTimer -= deltaTime;
        }

        #endregion

        #region IBombService Implementation

        public BombHandle PlaceBomb(TileCoordinate tile)
        {
            if (!CanPlaceBomb)
                return BombHandle.Invalid;

            if (!_currentStats.InfiniteBombs)
            {
                if (!_inventoryService.ConsumeBomb())
                    return BombHandle.Invalid;
            }

            int bombId = _nextBombId++;
            Vector3 worldPos = _gridService.TileToCenteredWorld(tile);

            var controller = _poolService.Spawn(
                _defaultDefinition.BombPrefab,
                worldPos,
                Quaternion.identity
            );

            controller.Initialize(
                _defaultDefinition,
                _currentStats.Clone(),
                tile,
                TeamType.Player,
                bombId,
                _effectOrchestrator,
                _gridService,
                _targetRegistry,
                _updateService,
                OnBombDetonated
            );

            _activeBomb = controller;
            _cooldownTimer = _currentStats.PlacementCooldown;

            return new BombHandle(bombId);
        }

        public void DetonateActiveBomb()
        {
            if (_activeBomb == null)
                return;

            if (!_currentStats.RequiresManualDetonation)
                return;

            _activeBomb.ForceDetonate();
        }

        public void ModifyStats(Action<BombStats> modifier)
        {
            modifier?.Invoke(_currentStats);
        }

        public void ResetStats()
        {
            _currentStats = _baseStats.Clone();
        }

        #endregion

        #region Private Methods

        private void OnBombDetonated(BombController controller)
        {
            var handle = new BombHandle(controller.BombId);
            var tile = controller.Tile;

            EventBus.Raise(new BombDetonationEvent
            {
                Handle = handle,
                CenterTile = tile,
                ExplosionRadius = _currentStats.GetExplosionRadius(),
                Damage = _currentStats.Damage,
                SourceTeam = TeamType.Player
            });

            if (_activeBomb == controller)
                _activeBomb = null;

            _poolService.Despawn(controller);
        }

        #endregion
    }
}
