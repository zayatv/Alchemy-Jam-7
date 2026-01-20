using Core.Systems.Events;
using UnityEngine;

namespace Core.Game.Destructibles.Events
{
    public struct DestructibleDestroyedEvent : IEvent
    {
        public DestructibleType Type;
        public Vector3 Position;
        public int DropsSpawned;
    }

    public enum DestructibleType
    {
        Generic,
        Door,
        ItemCage,
        Stone,
        Vase,
        Chest,
        Enemy
    }
}
