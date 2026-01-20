using Core.Game.Combat;
using Core.Game.Pickups.Events;
using Core.Systems.Events;
using UnityEngine;

namespace Core.Game.Pickups
{
    public class HealthPickup : Pickup
    {
        #region Serialized Fields

        [Header("Health Pickup Settings")]
        [Tooltip("Amount of health to restore")]
        [SerializeField] private int healAmount = 1;

        #endregion

        #region Private Fields

        private HealthComponent _lastCollectorHealth;

        #endregion

        #region Protected Methods

        protected override bool TryApplyPickup(GameObject collector)
        {
            var healthComponent = collector.GetComponent<HealthComponent>();

            if (healthComponent == null)
                return false;

            if (!healthComponent.IsAlive)
                return false;

            _lastCollectorHealth = healthComponent;
            
            healthComponent.Heal(healAmount);
            
            return true;
        }

        protected override void OnPickupCollected(GameObject collector)
        {
            EventBus.Raise(new PickupCollectedEvent
            {
                Type = PickupType.Health,
                Position = transform.position
            });

            if (_lastCollectorHealth != null)
            {
                EventBus.Raise(new HealthPickupCollectedEvent
                {
                    HealAmount = healAmount,
                    NewHealth = _lastCollectorHealth.CurrentHealth,
                    MaxHealth = _lastCollectorHealth.MaxHealth
                });
            }

            _lastCollectorHealth = null;
        }

        #endregion
    }
}
