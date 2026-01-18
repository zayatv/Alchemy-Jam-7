using Core.Systems.Combat;
using Core.Systems.Events;

namespace Core.Game.Combat.Events
{
    public struct PlayerDamagedEvent : IEvent
    {
        public int Damage;
        public int RemainingHealth;
        public DamageSource Source;
    }
}