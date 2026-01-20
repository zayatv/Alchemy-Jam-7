using Core.Systems.Events;
using UnityEngine;

namespace Core.Game.Pickups.Events
{
    public struct PickupCollectedEvent : IEvent
    {
        public PickupType Type;
        public Vector3 Position;
    }

    public enum PickupType
    {
        Health,
        Upgrade,
        Bomb
    }
}
