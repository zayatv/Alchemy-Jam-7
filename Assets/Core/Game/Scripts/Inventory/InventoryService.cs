using System;
using Core.Game.Inventory.Events;
using Core.Systems.Events;

namespace Core.Game.Inventory
{
    public class InventoryService : IInventoryService
    {
        #region Fields
        
        private int _currentBombs;
        private int _maxBombs;
        private bool _hasInfiniteBombs;
        
        #endregion

        #region Properties
        
        public int BombCount => _currentBombs;
        public int MaxBombCount => _maxBombs;
        public bool HasInfiniteBombs => _hasInfiniteBombs;
        
        #endregion

        #region Events
        
        public event Action<int> OnBombCountChanged;
        public event Action<bool> OnInfiniteBombsChanged;
        
        #endregion

        /// <summary>
        /// Creates a new inventory service with specified starting and maximum bomb counts.
        /// </summary>
        /// <param name="startingBombs">Initial number of bombs in inventory.</param>
        /// <param name="maxBombs">Maximum number of bombs that can be held.</param>
        public InventoryService(int startingBombs, int maxBombs)
        {
            if (startingBombs < 0)
                throw new ArgumentException("Starting bombs cannot be negative.", nameof(startingBombs));

            if (maxBombs <= 0)
                throw new ArgumentException("Max bombs must be greater than zero.", nameof(maxBombs));

            _currentBombs = Math.Min(startingBombs, maxBombs);
            _maxBombs = maxBombs;
            _hasInfiniteBombs = false;
        }

        public bool ConsumeBomb()
        {
            if (_hasInfiniteBombs)
                return true;

            if (_currentBombs <= 0)
                return false;

            int oldCount = _currentBombs;
            
            _currentBombs--;
            
            RaiseBombCountChanged(oldCount, _currentBombs);

            return true;
        }

        public void AddBombs(int count)
        {
            if (count <= 0)
                return;

            int oldCount = _currentBombs;

            _currentBombs += count;

            if (!_hasInfiniteBombs)
                _currentBombs = Math.Min(_currentBombs, _maxBombs);

            RaiseBombCountChanged(oldCount, _currentBombs);
            RaiseBombsCollected(count, _currentBombs);
        }

        public void SetInfiniteBombs(bool infinite)
        {
            if (_hasInfiniteBombs == infinite)
                return;

            _hasInfiniteBombs = infinite;
            
            RaiseInfiniteBombsChanged(infinite);
        }

        public void SetMaxBombCount(int max)
        {
            if (max <= 0)
                throw new ArgumentException("Max bomb count must be greater than zero.", nameof(max));

            int oldCount = _currentBombs;
            
            _maxBombs = max;

            if (!_hasInfiniteBombs && _currentBombs > _maxBombs)
            {
                _currentBombs = _maxBombs;
                
                RaiseBombCountChanged(oldCount, _currentBombs);
            }
        }

        /// <summary>
        /// Raises bomb count changed events through both C# events and EventBus.
        /// </summary>
        /// <param name="oldCount">Previous bomb count.</param>
        /// <param name="newCount">New bomb count.</param>
        private void RaiseBombCountChanged(int oldCount, int newCount)
        {
            OnBombCountChanged?.Invoke(newCount);

            EventBus.Raise(new BombCountChangedEvent
            {
                OldCount = oldCount,
                NewCount = newCount,
                IsInfinite = _hasInfiniteBombs
            });
        }

        /// <summary>
        /// Raises bombs collected event through EventBus.
        /// </summary>
        /// <param name="amount">Number of bombs collected.</param>
        /// <param name="newTotal">New total bomb count.</param>
        private void RaiseBombsCollected(int amount, int newTotal)
        {
            EventBus.Raise(new BombsCollectedEvent
            {
                Amount = amount,
                NewTotal = newTotal
            });
        }

        /// <summary>
        /// Raises infinite bombs changed events through both C# events and EventBus.
        /// </summary>
        /// <param name="infinite">New infinite bombs state.</param>
        private void RaiseInfiniteBombsChanged(bool infinite)
        {
            OnInfiniteBombsChanged?.Invoke(infinite);
            
            EventBus.Raise(new BombCountChangedEvent
            {
                OldCount = _currentBombs,
                NewCount = _currentBombs,
                IsInfinite = infinite
            });
        }
    }
}
