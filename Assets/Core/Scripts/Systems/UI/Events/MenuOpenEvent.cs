using Core.Systems.Events;

namespace Core.Systems.UI.Events
{
    public struct MenuOpenEvent : IEvent
    {
        public Menu Menu;
        public int OpenMenus;
    }
}