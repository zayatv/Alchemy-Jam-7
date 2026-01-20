using System;
using Core.Game.Combat.Bombs;
using Core.Game.Combat.Events;
using Core.Systems.Combat;
using Core.Systems.Events;
using Core.Systems.Grid;
using Core.Systems.Logging;
using Core.Systems.ServiceLocator;
using UnityEngine;

namespace Core.Game.Combat
{
    public class HealthComponent : MonoBehaviour, IDamageable, ITargetable
    {
        #region Serialized Fields

        [Header("Configuration")]
        [Tooltip("Health configuration defining max health")]
        [SerializeField] private HealthConfig config;

        [Header("Combat Settings")]
        [Tooltip("The team this entity belongs to (Player, Enemy, Neutral)")]
        [SerializeField] private TeamType team = TeamType.Neutral;

        [Tooltip("The type of target this entity represents (Player, Enemy, Destructible, Bomb)")]
        [SerializeField] private TargetType targetType = TargetType.Destructible;

        #endregion

        #region Private Fields

        private int _currentHealth;
        private Collider _collider;
        private IGridService _gridService;
        private float _immunityTimer;

        #endregion

        #region IDamageable Properties
        
        public int CurrentHealth => _currentHealth;
        public int MaxHealth => config != null ? config.MaxHealth : 1;
        public bool IsAlive => _currentHealth > 0;
        public TeamType Team => team;

        #endregion

        #region ITargetable Properties
        
        public TileCoordinate CurrentTile
        {
            get
            {
                if (_gridService == null)
                {
                    if (!ServiceLocator.TryGet(out _gridService))
                    {
                        GameLogger.Log(LogLevel.Warning, $"GridService not available for {gameObject.name}. Returning default tile.");

                        return TileCoordinate.Zero;
                    }
                }

                return _gridService.WorldToTile(transform.position);
            }
        }

        public Collider Collider => _collider;
        public bool IsTargetable => IsAlive;
        public TargetType TargetType => targetType;
        public Transform Transform => transform;

        public TileCoordinate[] GetOccupiedTiles()
        {
            if (_gridService == null && !ServiceLocator.TryGet(out _gridService))
            {
                return new[] { TileCoordinate.Zero };
            }

            if (_collider == null)
            {
                return new[] { _gridService.WorldToTile(transform.position) };
            }

            Bounds bounds = _collider.bounds;
            float tileSize = _gridService.TileSize;
            
            int minTileX = Mathf.FloorToInt(bounds.min.x / tileSize);
            int maxTileX = Mathf.FloorToInt(bounds.max.x / tileSize);
            int minTileZ = Mathf.FloorToInt(bounds.min.z / tileSize);
            int maxTileZ = Mathf.FloorToInt(bounds.max.z / tileSize);

            int countX = maxTileX - minTileX + 1;
            int countZ = maxTileZ - minTileZ + 1;

            var tiles = new TileCoordinate[countX * countZ];
            int index = 0;

            for (int x = minTileX; x <= maxTileX; x++)
            {
                for (int z = minTileZ; z <= maxTileZ; z++)
                {
                    tiles[index++] = new TileCoordinate(x, z);
                }
            }

            return tiles;
        }

        #endregion

        #region Events

        public event Action<DamageInfo> OnDamageReceived;
        public event Action OnDeath;
        public event Action<int, int> OnHealthChanged;
        public event Action<bool> OnImmunityChanged;

        #endregion

        #region Properties

        public bool IsImmune => _immunityTimer > 0f;

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            _collider = GetComponent<Collider>();

            if (_collider == null)
            {
                GameLogger.Log(LogLevel.Warning, $"No Collider found on {gameObject.name}. ITargetable.Collider will be null.");
            }

            _currentHealth = MaxHealth;

            ServiceLocator.TryGet(out _gridService);
        }

        private void Update()
        {
            if (_immunityTimer <= 0f)
                return;

            _immunityTimer -= Time.deltaTime;

            if (_immunityTimer <= 0f)
            {
                _immunityTimer = 0f;
                
                OnImmunityChanged?.Invoke(false);
            }
        }

        #endregion

        #region IDamageable Implementation
        
        public void TakeDamage(DamageInfo damage)
        {
            if (!IsAlive)
                return;

            if (IsImmune)
                return;

            if (ShouldIgnoreDamage(damage))
                return;

            _currentHealth -= damage.Amount;
            _currentHealth = Mathf.Max(0, _currentHealth);

            OnDamageReceived?.Invoke(damage);
            OnHealthChanged?.Invoke(_currentHealth, MaxHealth);

            EventBus.Raise(new DamageDealtEvent
            {
                Target = this,
                DamageInfo = damage,
                RemainingHealth = _currentHealth
            });

            if (_currentHealth <= 0)
            {
                HandleDeath(damage);
            }
            else
            {
                StartImmunity();
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Heals this entity by the specified amount.
        /// Clamps health to MaxHealth and raises OnHealthChanged event.
        /// </summary>
        /// <param name="amount">The amount of health to restore.</param>
        public void Heal(int amount)
        {
            if (!IsAlive)
                return;

            _currentHealth += amount;
            _currentHealth = Mathf.Min(_currentHealth, MaxHealth);

            OnHealthChanged?.Invoke(_currentHealth, MaxHealth);
        }

        /// <summary>
        /// Sets the health to a specific value.
        /// Clamps to valid range [0, MaxHealth] and raises OnHealthChanged event.
        /// If health is set to 0, triggers death handling.
        /// </summary>
        /// <param name="health">The new health value.</param>
        public void SetHealth(int health)
        {
            bool wasAlive = IsAlive;

            _currentHealth = Mathf.Clamp(health, 0, MaxHealth);

            OnHealthChanged?.Invoke(_currentHealth, MaxHealth);

            if (wasAlive && !IsAlive)
            {
                HandleDeath(new DamageInfo
                {
                    Amount = 0,
                    SourceTeam = TeamType.Neutral,
                    Source = DamageSource.Environment,
                    SourceTile = TileCoordinate.Zero,
                    WorldPosition = transform.position,
                    Instigator = null
                });
            }
        }

        /// <summary>
        /// Grants temporary immunity for the specified duration.
        /// </summary>
        /// <param name="duration">Duration of immunity in seconds.</param>
        public void GrantImmunity(float duration)
        {
            if (duration <= 0f)
                return;

            bool wasImmune = IsImmune;
            
            _immunityTimer = Mathf.Max(_immunityTimer, duration);

            if (!wasImmune)
            {
                OnImmunityChanged?.Invoke(true);
            }
        }

        /// <summary>
        /// Clears any active immunity immediately.
        /// </summary>
        public void ClearImmunity()
        {
            if (!IsImmune)
                return;

            _immunityTimer = 0f;
            
            OnImmunityChanged?.Invoke(false);
        }

        #endregion

        #region Private Methods

        private bool ShouldIgnoreDamage(DamageInfo damage)
        {
            if (damage.Source != DamageSource.Bomb)
                return false;

            if (targetType != TargetType.Player)
                return false;

            if (!ServiceLocator.TryGet(out IBombService bombService))
                return false;

            return bombService.CurrentStats.PlayerImmuneToBombs;
        }

        private void StartImmunity()
        {
            if (config == null || !config.EnableDamageImmunity)
                return;

            if (config.ImmunityDuration <= 0f)
                return;

            _immunityTimer = config.ImmunityDuration;
            
            OnImmunityChanged?.Invoke(true);
        }

        private void HandleDeath(DamageInfo finalBlow)
        {
            OnDeath?.Invoke();

            EventBus.Raise(new EntityDeathEvent
            {
                Entity = this,
                FinalBlow = finalBlow,
                TargetType = targetType
            });

            if (targetType == TargetType.Player)
            {
                EventBus.Raise(new PlayerDamagedEvent
                {
                    Damage = finalBlow.Amount,
                    RemainingHealth = 0,
                    Source = finalBlow.Source
                });
            }
        }

        #endregion
    }
}
