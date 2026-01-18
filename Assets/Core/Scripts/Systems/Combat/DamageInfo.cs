using Core.Systems.Grid;
using UnityEngine;

namespace Core.Systems.Combat
{
    public struct DamageInfo
    {
        /// <summary>
        /// The amount of damage to be dealt.
        /// </summary>
        public int Amount;

        /// <summary>
        /// The team that caused this damage.
        /// Used to determine friendly fire and targeting rules.
        /// </summary>
        public TeamType SourceTeam;

        /// <summary>
        /// The type/source of the damage.
        /// Used for damage calculation, immunity, and feedback.
        /// </summary>
        public DamageSource Source;

        /// <summary>
        /// The tile coordinate where the damage originated from.
        /// Used for knockback direction, distance calculations, and spatial queries.
        /// </summary>
        public TileCoordinate SourceTile;

        /// <summary>
        /// The exact world position where the damage originated.
        /// Used for precise VFX/SFX positioning and directional effects.
        /// </summary>
        public Vector3 WorldPosition;

        /// <summary>
        /// The entity that caused this damage.
        /// Can be a player, enemy, bomb controller, environment object, etc.
        /// Typed as object to allow flexibility - cast to specific type as needed.
        /// </summary>
        public object Instigator;

        /// <summary>
        /// Creates a new DamageInfo with all required parameters.
        /// </summary>
        /// <param name="amount">The amount of damage to deal.</param>
        /// <param name="sourceTeam">The team that caused the damage.</param>
        /// <param name="source">The type/source of the damage.</param>
        /// <param name="sourceTile">The tile where damage originated.</param>
        /// <param name="worldPosition">The world position where damage originated.</param>
        /// <param name="instigator">The entity that caused the damage.</param>
        public DamageInfo(int amount, TeamType sourceTeam, DamageSource source, TileCoordinate sourceTile, Vector3 worldPosition, object instigator)
        {
            Amount = amount;
            SourceTeam = sourceTeam;
            Source = source;
            SourceTile = sourceTile;
            WorldPosition = worldPosition;
            Instigator = instigator;
        }
    }
}
