using Core.Game.Upgrades;
using Core.Systems.Events;

namespace Core.Game.Pickups.Events
{
    public struct UpgradePickupCollectedEvent : IEvent
    {
        public UpgradeDefinition Upgrade;
    }
}
