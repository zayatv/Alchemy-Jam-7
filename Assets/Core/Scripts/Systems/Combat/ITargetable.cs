using Core.Systems.Grid;
using UnityEngine;

namespace Core.Systems.Combat
{
    public interface ITargetable
    {
        /// <summary>
        /// The collider component used for physics-based targeting.
        /// Used for overlap checks, raycasts, and collision detection.
        /// </summary>
        Collider Collider { get; }

        /// <summary>
        /// Whether this entity can currently be targeted.
        /// False during invulnerability frames, death states, or when disabled.
        /// Targeting systems should check this before applying effects.
        /// </summary>
        bool IsTargetable { get; }

        /// <summary>
        /// The type of target this entity represents.
        /// Used to filter targets and apply type-specific logic.
        /// </summary>
        TargetType TargetType { get; }

        /// <summary>
        /// The transform component of this entity.
        /// Provides position, rotation, and hierarchy information.
        /// Used for distance calculations, positioning, and spatial queries.
        /// </summary>
        Transform Transform { get; }

        /// <summary>
        /// Gets all tiles this target occupies based on its collider bounds.
        /// Uses the collider's AABB projected onto the XZ plane, so flying enemies
        /// at any height will still be registered on the tiles they overlap.
        /// </summary>
        /// <returns>Array of all tile coordinates this target occupies.</returns>
        TileCoordinate[] GetOccupiedTiles();
    }
}
