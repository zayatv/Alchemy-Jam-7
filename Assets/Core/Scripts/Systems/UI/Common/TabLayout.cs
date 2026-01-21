using System;
using System.Collections.Generic;
using System.Linq;
using Core.Systems.UI.Common.Button;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Systems.UI.Common
{
    public class TabLayout : MonoBehaviour
    {
        #region Fields

        [Title("Tab Configuration")]
        [SerializeField, ListDrawerSettings(DraggableItems = true, ShowIndexLabels = true)]
        [Tooltip("List of tabs managed by this layout")]
        private List<TabData> tabs = new List<TabData>();

        [Title("Behavior")]
        [SerializeField]
        [Tooltip("Allow deselecting all tabs by clicking the active tab")]
        private bool allowDeselection = false;

        [SerializeField]
        [Tooltip("Select the first tab automatically when enabled")]
        private bool selectFirstTabOnEnable = true;

        [SerializeField]
        [Tooltip("Index of tab to select on enable (if selectFirstTabOnEnable is true)")]
        private int defaultTabIndex = 0;

        [Title("Keyboard Navigation")]
        [SerializeField]
        [Tooltip("Enable keyboard navigation between tabs (Left/Right arrows)")]
        private bool enableKeyboardNavigation = true;
        
        private TabData _currentTab;
        private int _currentTabIndex = -1;

        #endregion

        #region Events

        public event Action<TabData, TabData> OnTabChanged;

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            ValidateTabs();
            RegisterListeners();
        }

        private void OnEnable()
        {
            if (selectFirstTabOnEnable && tabs.Count > 0)
            {
                int indexToSelect = Mathf.Clamp(defaultTabIndex, 0, tabs.Count - 1);
                SelectTab(indexToSelect);
            }
        }

        private void OnDisable()
        {
            CloseAllTabs();
        }

        private void OnDestroy()
        {
            UnregisterListeners();
        }

        #endregion

        #region Public API
        
        public void SelectTab(int index)
        {
            if (index < 0 || index >= tabs.Count)
            {
                Debug.LogWarning($"Invalid tab index: {index}. Valid range: 0-{tabs.Count - 1}");
                return;
            }

            SelectTab(tabs[index]);
        }
        
        public void SelectTab(TabData tab)
        {
            if (tab == null || !tabs.Contains(tab))
            {
                Debug.LogWarning("Cannot select null or unregistered tab");
                return;
            }

            if (tab == _currentTab)
            {
                if (allowDeselection)
                {
                    DeselectCurrentTab();
                }
                return;
            }

            var previousTab = _currentTab;
            var tween = CloseCurrentTab();

            if (tween != null && tween.IsActive())
            {
                tween.onComplete += () => OpenTab(tab);
            }
            else
            {
                OpenTab(tab);
            }

            _currentTab = tab;
            _currentTabIndex = tabs.IndexOf(tab);

            OnTabChanged?.Invoke(previousTab, tab);
        }
        
        public void SelectNextTab(bool wrap = true)
        {
            if (tabs.Count == 0)
                return;

            int nextIndex = _currentTabIndex + 1;

            if (nextIndex >= tabs.Count)
            {
                if (wrap)
                    nextIndex = 0;
                else
                    return;
            }

            SelectTab(nextIndex);
        }
        
        public void SelectPreviousTab(bool wrap = true)
        {
            if (tabs.Count == 0)
                return;

            int prevIndex = _currentTabIndex - 1;

            if (prevIndex < 0)
            {
                if (wrap)
                    prevIndex = tabs.Count - 1;
                else
                    return;
            }

            SelectTab(prevIndex);
        }
        
        public void DeselectCurrentTab()
        {
            var previousTab = _currentTab;
            CloseCurrentTab();
            _currentTab = null;
            _currentTabIndex = -1;

            OnTabChanged?.Invoke(previousTab, null);
        }
        
        public void AddTab(TabData tab)
        {
            if (tab == null || tabs.Contains(tab))
                return;

            tabs.Add(tab);
            RegisterTabListeners(tab);

            // Close the tab initially
            if (tab.tabContent != null)
            {
                tab.tabContent.gameObject.SetActive(false);
            }

            if (tab.tabButton != null)
            {
                tab.tabButton.SetSelected(false, false);
            }
        }
        
        public void RemoveTab(TabData tab)
        {
            if (tab == null || !tabs.Contains(tab))
                return;

            if (tab == _currentTab)
            {
                DeselectCurrentTab();
            }

            UnregisterTabListeners(tab);
            
            tabs.Remove(tab);
        }

        #endregion

        #region Private Methods

        private void OpenTab(TabData tab)
        {
            if (tab == null)
                return;

            if (tab.tabContent != null)
            {
                tab.tabContent.gameObject.SetActive(true);
                tab.tabContent.Open();
            }

            if (tab.tabButton != null)
            {
                tab.tabButton.SetSelected(true, false);
            }
        }

        private Tween CloseCurrentTab()
        {
            if (_currentTab == null)
                return null;

            Tween closeTween = null;

            if (_currentTab.tabContent != null)
            {
                closeTween = _currentTab.tabContent.Close();
            }

            if (_currentTab.tabButton != null)
            {
                _currentTab.tabButton.SetSelected(false, false);
            }

            return closeTween;
        }

        private void CloseAllTabs()
        {
            foreach (var tab in tabs)
            {
                if (tab.tabButton != null)
                {
                    tab.tabButton.SetSelected(false, false);
                }

                if (tab.tabContent != null)
                {
                    tab.tabContent.gameObject.SetActive(false);
                }
            }

            _currentTab = null;
            _currentTabIndex = -1;
        }

        private void ValidateTabs()
        {
            tabs.RemoveAll(t => t == null || t.tabButton == null);

            foreach (var tab in tabs)
            {
                if (tab.tabContent == null)
                {
                    Debug.LogWarning($"Tab {tab.tabButton.name} is missing content reference");
                }
            }
        }

        private void RegisterListeners()
        {
            foreach (var tab in tabs)
            {
                RegisterTabListeners(tab);
            }
        }

        private void RegisterTabListeners(TabData tab)
        {
            if (tab?.tabButton != null)
            {
                tab.tabButton.OnTabSelected += OnTabButtonSelected;
            }
        }

        private void UnregisterListeners()
        {
            foreach (var tab in tabs)
            {
                UnregisterTabListeners(tab);
            }
        }

        private void UnregisterTabListeners(TabData tab)
        {
            if (tab?.tabButton != null)
            {
                tab.tabButton.OnTabSelected -= OnTabButtonSelected;
            }
        }

        private void OnTabButtonSelected(TabButton button)
        {
            var tab = tabs.FirstOrDefault(t => t.tabButton == button);

            if (tab != null)
            {
                SelectTab(tab);
            }
        }

        #endregion
    }
    
    [Serializable]
    public class TabData
    {
        [Required]
        public TabButton tabButton;

        [Required]
        public BaseTab tabContent;

        public bool IsValid => tabButton != null && tabContent != null;
    }
}
