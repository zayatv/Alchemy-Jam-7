using System;

namespace Core.Game.Combat.Bombs
{
    [Serializable]
    public class BombStats
    {
        #region Fields
        
        public int Damage;
        public float ExplosionRadius;
        public float ExplosionRadiusModifier;
        public float FuseTime;
        public float PlacementCooldown;
        public bool RequiresManualDetonation;
        public bool PlayerImmuneToBombs;
        public bool InfiniteBombs;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BombStats"/> class with default values.
        /// </summary>
        public BombStats()
        {
            Damage = 1;
            ExplosionRadius = 1f;
            ExplosionRadiusModifier = 1f;
            FuseTime = 3f;
            PlacementCooldown = 1f;
            RequiresManualDetonation = false;
            PlayerImmuneToBombs = false;
            InfiniteBombs = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BombStats"/> class with specified values.
        /// </summary>
        /// <param name="damage">The damage dealt by the bomb.</param>
        /// <param name="radius">The radius of the explosion range.</param>
        /// <param name="radiusModifier">The radius modifier for explosions.</param>
        /// <param name="fuseTime">The time before detonation.</param>
        /// <param name="placementCooldown">The cooldown between placements.</param>
        public BombStats(int damage, float radius, float radiusModifier, float fuseTime, float placementCooldown)
        {
            Damage = damage;
            ExplosionRadius = radius;
            ExplosionRadiusModifier = radiusModifier;
            FuseTime = fuseTime;
            PlacementCooldown = placementCooldown;
            RequiresManualDetonation = false;
            PlayerImmuneToBombs = false;
            InfiniteBombs = false;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates a deep copy of this bomb stats instance.
        /// </summary>
        /// <returns>A new <see cref="BombStats"/> instance with copied values.</returns>
        public BombStats Clone()
        {
            return new BombStats
            {
                Damage = Damage,
                ExplosionRadius = ExplosionRadius,
                ExplosionRadiusModifier = ExplosionRadiusModifier,
                FuseTime = FuseTime,
                PlacementCooldown = PlacementCooldown,
                RequiresManualDetonation = RequiresManualDetonation,
                PlayerImmuneToBombs = PlayerImmuneToBombs,
                InfiniteBombs = InfiniteBombs
            };
        }

        public float GetExplosionRadius()
        {
            return ExplosionRadius * ExplosionRadiusModifier;
        }

        #endregion
    }
}
