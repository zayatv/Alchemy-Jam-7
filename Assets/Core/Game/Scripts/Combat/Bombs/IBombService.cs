using System;
using Core.Systems.Grid;
using Core.Systems.ServiceLocator;

namespace Core.Game.Combat.Bombs
{
    public interface IBombService : IService
    {
        #region Properties

        /// <summary>
        /// Gets the current bomb statistics.
        /// </summary>
        BombStats CurrentStats { get; }

        /// <summary>
        /// Gets a value indicating whether a bomb can currently be placed.
        /// </summary>
        bool CanPlaceBomb { get; }

        /// <summary>
        /// Gets a value indicating whether there is an active bomb in the world.
        /// </summary>
        bool HasActiveBomb { get; }

        /// <summary>
        /// Gets the remaining cooldown time before another bomb can be placed.
        /// </summary>
        float CooldownRemaining { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Places a bomb at the specified tile coordinate.
        /// </summary>
        /// <param name="tile">The tile coordinate to place the bomb at.</param>
        /// <returns>A handle to the placed bomb, or an invalid handle if placement failed.</returns>
        BombHandle PlaceBomb(TileCoordinate tile);

        /// <summary>
        /// Detonates the currently active bomb if manual detonation is required.
        /// </summary>
        void DetonateActiveBomb();

        /// <summary>
        /// Modifies the current bomb statistics using the provided modifier action.
        /// </summary>
        /// <param name="modifier">An action that modifies the bomb stats.</param>
        void ModifyStats(Action<BombStats> modifier);

        /// <summary>
        /// Resets the bomb statistics to their base values.
        /// </summary>
        void ResetStats();

        #endregion
    }
}
