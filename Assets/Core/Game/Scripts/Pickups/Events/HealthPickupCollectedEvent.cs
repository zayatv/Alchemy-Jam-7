using Core.Systems.Events;

namespace Core.Game.Pickups.Events
{
    public struct HealthPickupCollectedEvent : IEvent
    {
        public int HealAmount;
        public int NewHealth;
        public int MaxHealth;
    }
}
