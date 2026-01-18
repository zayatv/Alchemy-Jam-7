using System.Collections.Generic;
using Core.Systems.ServiceLocator;
using Core.Systems.Combat;
using UnityEngine;

namespace Core.Game.Combat
{
    public interface ITargetRegistry : IService
    {
        /// <summary>
        /// Registers a targetable entity with the registry.
        /// Call this when an entity becomes active and can be targeted.
        /// </summary>
        /// <param name="target">The targetable entity to register.</param>
        void Register(ITargetable target);

        /// <summary>
        /// Unregisters a targetable entity from the registry.
        /// Call this when an entity is destroyed or should no longer be targetable.
        /// </summary>
        /// <param name="target">The targetable entity to unregister.</param>
        void Unregister(ITargetable target);

        /// <summary>
        /// Retrieves a list of targetable entities within a specified radius from a given point in space.
        /// Useful for detecting all valid targets in an area-based attack or ability.
        /// </summary>
        /// <param name="center">The central point in world space to search around.</param>
        /// <param name="radius">The radius of the search area, in world units.</param>
        /// <returns>A read-only list of targetable entities within the specified radius, or an empty list if no targets are found.</returns>
        IReadOnlyList<ITargetable> GetTargetsInRadius(Vector3 center, float radius);

        /// <summary>
        /// Gets all registered targets of a specific type.
        /// Filters by TargetType (Player, Enemy, Destructible, Bomb).
        /// </summary>
        /// <param name="type">The target type to filter by.</param>
        /// <returns>Read-only list of targets matching the specified type.</returns>
        IReadOnlyList<ITargetable> GetTargetsOfType(TargetType type);

        /// <summary>
        /// Gets all registered targets that can be damaged by an attacker of the specified team.
        /// Applies damage filtering rules:
        /// - Player attacker: Returns Enemies + Destructibles (+ Player if allowed)
        /// - Enemy attacker: Returns Player only
        /// - Neutral attacker: Returns all targets
        /// Note: BombproofVest logic is handled separately by BombService.
        /// </summary>
        /// <param name="attackerTeam">The team of the attacking entity.</param>
        /// <returns>Read-only list of valid targets for the attacker's team.</returns>
        IReadOnlyList<ITargetable> GetTargetsForTeam(TeamType attackerTeam);
    }
}
