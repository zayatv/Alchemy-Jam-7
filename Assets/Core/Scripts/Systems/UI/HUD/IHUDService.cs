using Core.Systems.ServiceLocator;

namespace Core.Systems.UI.HUD
{
    public interface IHUDService : IService
    {
        bool IsHUDActive { get; }
        
        void ShowItem(string itemName);
        void HideItem(string itemName);
        
        void HideAll();
        void ShowAll();
        
        T GetItem<T>(string itemName) where T : HUDItem;
        
        void RegisterItem(HUDItem item);
        void UnregisterItem(HUDItem item);
        void SubscribeToEvents();
        void UnsubscribeFromEvents();
    }
}
