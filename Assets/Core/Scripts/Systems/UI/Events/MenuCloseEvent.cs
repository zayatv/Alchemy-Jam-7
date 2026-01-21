using Core.Systems.Events;

namespace Core.Systems.UI.Events
{
    public struct MenuCloseEvent : IEvent
    {
        public Menu Menu;
        public int OpenMenus;
        public bool WasLastOpenMenu;
    }
}