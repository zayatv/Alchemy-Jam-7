using System;
using Core.Systems.ServiceLocator;

namespace Core.Game.Inventory
{
    public interface IInventoryService : IService
    {
        /// <summary>
        /// Current number of bombs in inventory.
        /// </summary>
        int BombCount { get; }

        /// <summary>
        /// Maximum number of bombs that can be held.
        /// </summary>
        int MaxBombCount { get; }

        /// <summary>
        /// Whether infinite bombs mode is enabled.
        /// When true, ConsumeBomb always returns true and bomb count stays the same.
        /// </summary>
        bool HasInfiniteBombs { get; }

        /// <summary>
        /// Attempts to consume a bomb from inventory.
        /// </summary>
        /// <returns>False if no bombs available, otherwise decrements count and returns true.</returns>
        bool ConsumeBomb();

        /// <summary>
        /// Adds bombs to inventory, clamped to MaxBombCount unless infinite bombs is enabled.
        /// </summary>
        /// <param name="count">Number of bombs to add.</param>
        void AddBombs(int count);

        /// <summary>
        /// Enable or disable infinite bombs mode.
        /// </summary>
        /// <param name="infinite">True to enable infinite bombs.</param>
        void SetInfiniteBombs(bool infinite);

        /// <summary>
        /// Changes the maximum bomb capacity.
        /// </summary>
        /// <param name="max">New maximum bomb count.</param>
        void SetMaxBombCount(int max);

        /// <summary>
        /// Event raised when bomb count changes.
        /// Provides the new bomb count.
        /// </summary>
        event Action<int> OnBombCountChanged;

        /// <summary>
        /// Event raised when infinite bombs mode is toggled.
        /// Provides the new infinite bombs state.
        /// </summary>
        event Action<bool> OnInfiniteBombsChanged;
    }
}
