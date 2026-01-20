using Core.Game.Inventory;
using Core.Game.Pickups.Events;
using Core.Systems.Events;
using Core.Systems.ServiceLocator;
using UnityEngine;

namespace Core.Game.Pickups
{
    public class BombPickup : Pickup
    {
        #region Serialized Fields

        [Header("Bomb Pickup Settings")]
        [Tooltip("Number of bombs to add to inventory")]
        [SerializeField] private int bombCount = 1;

        #endregion

        #region Private Fields

        private IInventoryService _inventoryService;

        #endregion

        #region Protected Methods

        protected override bool TryApplyPickup(GameObject collector)
        {
            if (_inventoryService == null)
            {
                if (!ServiceLocator.TryGet(out _inventoryService))
                    return false;
            }

            if (_inventoryService.HasInfiniteBombs)
                return false;

            if (_inventoryService.BombCount >= _inventoryService.MaxBombCount)
                return false;

            _inventoryService.AddBombs(bombCount);
            return true;
        }

        protected override void OnPickupCollected(GameObject collector)
        {
            EventBus.Raise(new PickupCollectedEvent
            {
                Type = PickupType.Bomb,
                Position = transform.position
            });
        }

        #endregion
    }
}
