using System;
using System.Collections.Generic;
using Core.Systems.Animations;
using Core.Systems.Events;
using Core.Systems.Logging;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Systems.UI.HUD
{
    public abstract class HUDItem : MonoBehaviour
    {
        #region Fields
        
        [Title("HUD Item Configuration")] 
        [SerializeField]
        protected HUDItemConfiguration config = new HUDItemConfiguration();

        [Title("References")] 
        [SerializeField]
        protected CanvasGroup canvasGroup;

        protected bool isVisible = false;
        protected bool isInitialized = false;
        protected HUDService HUDService;
        protected HUDElementState currentState;
        protected DOTweenAnimationService doTweenAnimationService;

        private HashSet<Type> _showEvents = new();
        private HashSet<Type> _hideEvents = new();
        private HashSet<Type> _updateEvents = new();
        
        #endregion
        
        #region Properties

        public HUDItemConfiguration Config => config;
        public bool IsVisible => isVisible;
        public string ItemName => gameObject.name;
        public HUDElementState CurrentState => currentState;
        
        #endregion
        
        #region Initialization
        
        public virtual void Initialize(HUDService service)
        {
            if (isInitialized) 
                return;
            
            HUDService = service;
            doTweenAnimationService = ServiceLocator.ServiceLocator.Get<DOTweenAnimationService>();

            // Cache event types
            CacheEventTypes(config.ShowOnEvents, _showEvents);
            CacheEventTypes(config.HideOnEvents, _hideEvents);
            CacheEventTypes(config.UpdateOnEvents, _updateEvents);

            if (canvasGroup == null)
            {
                canvasGroup = GetComponent<CanvasGroup>();
            }

            OnInitialize();
            
            isInitialized = true;

            if (config.VisibleOnStart)
            {
                ShowImmediate();
            }
            else
            {
                HideImmediate();
            }
        }
        
        protected virtual void OnInitialize()
        {
        }

        private void CacheEventTypes(System.Collections.Generic.List<string> classNames, HashSet<System.Type> hashSet)
        {
            foreach (var className in classNames)
            {
                if (string.IsNullOrEmpty(className)) continue;
                var type = System.Type.GetType(className);
                if (type != null)
                {
                    hashSet.Add(type);
                }
            }
        }
        
        #endregion
        
        public virtual void OnEventReceived(IEvent eventData)
        {
            if (!ValidateData(eventData)) 
                return;
            
            var type = eventData.GetType();

            if (_hideEvents.Contains(type))
            {
                currentState = HUDElementState.Disabled;
                
                if (HUDService.IsHUDActive)
                    Hide();
            }

            if (_updateEvents.Contains(type))
            {
                UpdateData(eventData);
            }
            
            if (_showEvents.Contains(type))
            {
                currentState = HUDElementState.Enabled;
                
                if (HUDService.IsHUDActive)
                    Show();
            }
        }
        
        protected virtual void UpdateData(IEvent eventData)
        {
        }
        
        public virtual void Show()
        {
            if (isVisible) return;

            isVisible = true;
            
            doTweenAnimationService.KillTweensForTarget(canvasGroup);
            
            if (!gameObject.activeSelf)
                gameObject.SetActive(true);

            PlayShowAnimation();

            OnShow();
        }
        
        public virtual void Hide()
        {
            if (!isVisible) return;

            isVisible = false;

            doTweenAnimationService.KillTweensForTarget(canvasGroup);
            
            Tween hideTween = PlayHideAnimation();

            if (hideTween != null)
            {
                hideTween.onComplete += () =>
                {
                    OnHide();
                    gameObject.SetActive(false);
                };
            }
            else
            {
                OnHide();
                gameObject.SetActive(false);
            }
        }
        
        public void ShowImmediate()
        {
            isVisible = true;
            
            gameObject.SetActive(true);
            
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f;
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
            }

            currentState = HUDElementState.Enabled;

            OnShow();
        }
        
        public void HideImmediate()
        {
            isVisible = false;
            currentState = HUDElementState.Disabled;
            
            OnHide();
            
            if (!config.DisableWhenHidden && canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
                GameLogger.Log(LogLevel.Debug, $"[HUDItem] Disabling {ItemName} canvas group");
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        protected virtual Tween PlayShowAnimation()
        {
            if (canvasGroup == null) 
                return null;
            
            canvasGroup.alpha = 0f;
            
            return doTweenAnimationService.SafeFade(canvasGroup, 1f, 0.2f);
        }
        
        protected virtual Tween PlayHideAnimation()
        {
            if (canvasGroup == null) 
                return null;
            
            return doTweenAnimationService.SafeFade(canvasGroup, 0f, 0.2f);
        }
        
        protected virtual void OnShow()
        {
        }
        
        protected virtual void OnHide()
        {
        }
        
        protected virtual bool ValidateData(IEvent eventData)
        {
            return true;
        }
    }
}