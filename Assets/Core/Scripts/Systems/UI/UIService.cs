using System;
using System.Collections.Generic;
using System.Linq;
using Core.Systems.Events;
using Core.Systems.Logging;
using Core.Systems.UI.Events;
using UnityEngine;

namespace Core.Systems.UI
{
    public class UIService : IUIService
    {
        #region Fields

        private readonly Stack<Menu> _menuStack = new();
        private readonly List<Menu> _allMenus = new();
        private bool _isTransitioning = false;
        private bool _isHudVisibleBeforeOpeningMenu;

        #endregion

        #region Properties
        
        public Stack<Menu> MenuStack => _menuStack;
        public int MenuCount => _menuStack.Count;

        #endregion

        public void AddMenu(Menu menu)
        {
            _allMenus.Add(menu);
            
            if (!menu.OpenByDefault)
            {
                menu.gameObject.SetActive(false);
                    
                return;
            }
                
            OpenMenu(menu.GetType());
        }

        public void RemoveMenu(Menu menu)
        {
            CloseAllMenus(false);
            
            _allMenus.Remove(menu);
        }

        public void OpenMenu(Type menuType, bool closePrevious = false, bool animatePreviousOut = true)
        {
            if (_isTransitioning) return;

            Menu menuToOpen = _allMenus.FirstOrDefault(m => m.GetType() == menuType);
            
            if (menuToOpen == null)
            {
                GameLogger.Log(LogLevel.Error, $"Menu of type {menuType.Name} not found in UIService.");
                
                
                return;
            }
            
            if (_menuStack.Count > 0 && _menuStack.Peek().GetType() == menuType)
            {
                return;
            }

            _isTransitioning = true;

            Menu previousMenu = _menuStack.Count > 0 ? _menuStack.Peek() : null;

            Action openAction = () =>
            {
                menuToOpen.transform.SetAsLastSibling();
                menuToOpen.gameObject.SetActive(true);
                menuToOpen.Open();
                _menuStack.Push(menuToOpen);
                _isTransitioning = false;
                UpdateSystemStates();
                
                EventBus.Raise(new MenuOpenEvent
                {
                    Menu = menuToOpen,
                    OpenMenus = _menuStack.Count,
                });
            };

            if (previousMenu != null && closePrevious)
            {
                _menuStack.Pop();
                
                if (animatePreviousOut)
                {
                    previousMenu.Close().onComplete += () =>
                    {
                        openAction();
                    };
                }
                else
                {
                    previousMenu.gameObject.SetActive(false);
                    openAction();
                }
            }
            else
            {
                if (previousMenu != null)
                {
                    // Don't close the previous menu, just hide it
                    // Maybe animate out or do nothing and disable it
                }

                openAction();
            }
        }
        
        public void OpenMenu<T>(bool closePrevious = false, bool animatePreviousOut = true) where T : Menu
        {
            Type menuType = typeof(T);
            OpenMenu(menuType, closePrevious, animatePreviousOut);
        }
        
        /// <param name="menuName">The exact class name of the menu to open (e.g., "PauseMenu").</param>
        public void OpenMenuByName(string menuName)
        {
            var menuType = _allMenus.FirstOrDefault(m => m.GetType().Name == menuName)?.GetType();
            
            Debug.Log($"Looking for menu with name: {menuName}, found type: {menuType?.Name}");
            
            if (menuType != null)
            {
                OpenMenu(menuType);
            }
            else
            {
                Debug.LogError($"[UIManager] No menu with the name '{menuName}' was found in the allMenus list.");
            }
        }
        
        public void CloseMenu()
        {
            if (_menuStack.Count == 0 || _isTransitioning)
            {
                return;
            }

            _isTransitioning = true;
            
            Menu menuToClose = _menuStack.Pop();

            menuToClose.Close().onComplete += () =>
            {
                _isTransitioning = false;
                
                UpdateSystemStates();
            
                EventBus.Raise(new MenuCloseEvent
                {
                    Menu = menuToClose,
                    OpenMenus = _menuStack.Count,
                    WasLastOpenMenu = _menuStack.Count == 0
                });
            };
        }
        
        public void CloseAllMenus(bool animateTop = true)
        {
            if (_menuStack.Count == 0 || _isTransitioning)
                return;

            _isTransitioning = true;
            
            Menu topMenu = _menuStack.Pop();
            List<Menu> menusToClose = new List<Menu>();
            
            while (_menuStack.Count > 0)
            {
                Menu hiddenMenu = _menuStack.Pop();
                
                menusToClose.Add(hiddenMenu);
                hiddenMenu.Close(false); 
            }

            Action onAnimationComplete = () =>
            {
                EventBus.Raise(new MenuCloseEvent
                {
                    Menu = topMenu,
                    OpenMenus = menusToClose.Count,
                    WasLastOpenMenu = menusToClose.Count == 0
                });
                
                for (int i = 0; i < menusToClose.Count; i++)
                {
                    var menu = menusToClose[i];
                    int remaining = menusToClose.Count - 1 - i;
                    
                    EventBus.Raise(new MenuCloseEvent
                    {
                        Menu = menu,
                        OpenMenus = remaining,
                        WasLastOpenMenu = remaining == 0
                    });
                }
                
                _isTransitioning = false;
                
                UpdateSystemStates();
            };
            
            if (animateTop)
            {
                topMenu.Close().onComplete += () => onAnimationComplete();
            }
            else
            {
                topMenu.Close(false);
                onAnimationComplete();
            }
        }
        
        public T GetMenu<T>() where T : Menu
        {
            return _allMenus.OfType<T>().FirstOrDefault();
        }
        
        private void UpdateSystemStates()
        {
            if (_menuStack.Count > 0)
            {
                Menu topMenu = _menuStack.Peek();
                Time.timeScale = topMenu.PausesTime ? 0f : 1f;
            }
            else
            {
                Time.timeScale = 1f;
            }
        }
    }
}