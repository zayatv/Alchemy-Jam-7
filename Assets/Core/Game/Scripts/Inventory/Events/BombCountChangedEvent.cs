using Core.Systems.Events;

namespace Core.Game.Inventory.Events
{
    public struct BombCountChangedEvent : IEvent
    {
        public int OldCount;
        public int NewCount;
        public bool IsInfinite;
    }
}