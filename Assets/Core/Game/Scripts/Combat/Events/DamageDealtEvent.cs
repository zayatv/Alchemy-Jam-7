using Core.Systems.Combat;
using Core.Systems.Events;

namespace Core.Game.Combat.Events
{
    public struct DamageDealtEvent : IEvent
    {
        public IDamageable Target;
        public DamageInfo DamageInfo;
        public int RemainingHealth;
    }
}