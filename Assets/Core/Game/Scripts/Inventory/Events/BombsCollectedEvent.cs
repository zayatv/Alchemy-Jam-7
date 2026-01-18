using Core.Systems.Events;

namespace Core.Game.Inventory.Events
{
    public struct BombsCollectedEvent : IEvent
    {
        public int Amount;
        public int NewTotal;
    }
}