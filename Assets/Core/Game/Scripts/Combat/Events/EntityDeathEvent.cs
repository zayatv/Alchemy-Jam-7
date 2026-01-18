using Core.Systems.Combat;
using Core.Systems.Events;

namespace Core.Game.Combat.Events
{
    public struct EntityDeathEvent : IEvent
    {
        public IDamageable Entity;
        public DamageInfo FinalBlow;
        public TargetType TargetType;
    }
}