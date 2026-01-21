using System;
using System.Collections.Generic;
using System.Reflection;
using Core.Systems.Events;
using Core.Systems.Logging;
using Core.Systems.UI.Events;

namespace Core.Systems.UI.HUD
{
    public class HUDService : IHUDService
    {
        #region Fields

        private List<HUDItem> _hudItems = new();
        private Dictionary<Type, List<HUDItem>> _eventToItemsMap = new();
        private HashSet<string> _hudHideRequests = new();
        private bool _isHUDActive = true;

        private Dictionary<Type, Delegate> _eventDelegates = new();
        private MethodInfo _subscribeMethod;
        private MethodInfo _unsubscribeMethod;
        private MethodInfo _onGameEventMethod;
        
        #endregion
        
        #region Properties
        
        public bool IsHUDActive => _isHUDActive;
        
        #endregion

        public HUDService()
        {
            InitializeReflection();
        }
        
        private void InitializeReflection()
        {
            _subscribeMethod = typeof(EventBus).GetMethod("Subscribe");
            _unsubscribeMethod = typeof(EventBus).GetMethod("Unsubscribe");
            _onGameEventMethod = typeof(HUDService).GetMethod(nameof(OnGameEvent), BindingFlags.NonPublic | BindingFlags.Instance);
        }
        
        private void RegisterItemForEvents(HUDItem item)
        {
            var allEvents = new HashSet<string>();
            
            allEvents.UnionWith(item.Config.ShowOnEvents);
            allEvents.UnionWith(item.Config.HideOnEvents);
            allEvents.UnionWith(item.Config.UpdateOnEvents);

            foreach (var typeName in allEvents)
            {
                if (string.IsNullOrEmpty(typeName)) continue;
                
                var type = Type.GetType(typeName);
                if (type == null) continue;

                if (!_eventToItemsMap.ContainsKey(type))
                {
                    _eventToItemsMap[type] = new List<HUDItem>();
                }

                if (!_eventToItemsMap[type].Contains(item))
                {
                    _eventToItemsMap[type].Add(item);
                }
            }
        }

        private void UnregisterItemForEvents(HUDItem item)
        {
            foreach (var kvp in _eventToItemsMap)
            {
                kvp.Value.Remove(item);
            }
        }
        
        public void SubscribeToEvents()
        {
            foreach (var type in _eventToItemsMap.Keys)
            {
                SubscribeToType(type);
            }
            
            EventBus.Subscribe<MenuOpenEvent>(OnMenuOpened);
            EventBus.Subscribe<MenuCloseEvent>(OnMenuClosed);
        }

        public void UnsubscribeFromEvents()
        {
            foreach (var kvp in _eventDelegates)
            {
                var type = kvp.Key;
                var del = kvp.Value;
                
                if (_unsubscribeMethod != null)
                {
                    var genericUnsub = _unsubscribeMethod.MakeGenericMethod(type);
                    
                    genericUnsub.Invoke(null, new object[] { del });
                }
            }
            _eventDelegates.Clear();
            
            EventBus.Unsubscribe<MenuOpenEvent>(OnMenuOpened);
            EventBus.Unsubscribe<MenuCloseEvent>(OnMenuClosed);
        }

        private void SubscribeToType(Type type)
        {
            if (_eventDelegates.ContainsKey(type)) return;

            var genericHandler = _onGameEventMethod.MakeGenericMethod(type);
            var del = Delegate.CreateDelegate(typeof(Action<>).MakeGenericType(type), this, genericHandler);
            var genericSub = _subscribeMethod.MakeGenericMethod(type);
            
            genericSub.Invoke(null, new object[] { del });
            
            _eventDelegates[type] = del;
        }
        
        private void OnGameEvent<T>(T eventData) where T : IEvent
        {
            if (_eventToItemsMap.TryGetValue(typeof(T), out List<HUDItem> interestedItems))
            {
                foreach (var item in interestedItems)
                {
                    if (item != null)
                    {
                        item.OnEventReceived(eventData);
                    }
                }
            }
        }

        private void OnMenuOpened(MenuOpenEvent eventData)
        {
            if (eventData.Menu != null && eventData.Menu.HidesHUD)
            {
                RequestHUDHide(eventData.Menu.MenuName);
            }
        }

        private void OnMenuClosed(MenuCloseEvent eventData)
        {
             if (eventData.Menu != null)
             {
                 RemoveHUDHideRequest(eventData.Menu.MenuName);
             }
        }
        
        public void ShowItem(string itemName)
        {
            var item = _hudItems.Find(i => i.ItemName == itemName);
            
            if (item != null)
            {
                item.Show();
            }
            else
            {
                GameLogger.Log(LogLevel.Info, $"[HUDManager] HUD item not found: {itemName}");
            }
        }
        
        public void HideItem(string itemName)
        {
            var item = _hudItems.Find(i => i.ItemName == itemName);
            
            if (item != null)
            {
                item.Hide();
            }
            else
            {
                GameLogger.Log(LogLevel.Info, $"[HUDManager] HUD item not found: {itemName}");
            }
        }
        
        public void HideAll()
        {
            _isHUDActive = false;
            
            foreach (var item in _hudItems)
            {
                if (item != null && item.CurrentState == HUDElementState.Enabled)
                {
                    item.Hide();
                }
            }
        }
        
        public void ShowAll()
        {
            _isHUDActive = true;
            
            foreach (var item in _hudItems)
            {
                if (item != null && item.CurrentState == HUDElementState.Enabled)
                {
                    item.Show();
                }
            }
        }
        
        public T GetItem<T>(string itemName) where T : HUDItem
        {
            var item = _hudItems.Find(i => i.ItemName == itemName);
            return item as T;
        }
        
        public void RegisterItem(HUDItem item)
        {
            if (!_hudItems.Contains(item))
            {
                _hudItems.Add(item);
                
                item.Initialize(this);
                
                RegisterItemForEvents(item);
                
                var runtimeEvents = new HashSet<string>();
                
                runtimeEvents.UnionWith(item.Config.ShowOnEvents);
                runtimeEvents.UnionWith(item.Config.HideOnEvents);
                runtimeEvents.UnionWith(item.Config.UpdateOnEvents);

                foreach (var evt in runtimeEvents)
                {
                    if (string.IsNullOrEmpty(evt))
                        continue;
                    var t = Type.GetType(evt);
                    
                    if (t != null) 
                        SubscribeToType(t);
                }
            }
        }

        public void UnregisterItem(HUDItem item)
        {
            if (_hudItems.Contains(item))
            {
                UnregisterItemForEvents(item);
                
                _hudItems.Remove(item);
            }
        }

        private void RequestHUDHide(string reason)
        {
            if (_hudHideRequests.Contains(reason))
                return;
            
            _hudHideRequests.Add(reason);
            
            if (_hudHideRequests.Count > 0 && _isHUDActive)
                HideAll();
        }
        
        private void RemoveHUDHideRequest(string reason)
        {
            if (!_hudHideRequests.Contains(reason))
                return;
            
            _hudHideRequests.Remove(reason);
            
            if (_hudHideRequests.Count == 0)
                ShowAll();
        }
    }
}