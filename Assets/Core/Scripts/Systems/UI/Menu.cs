using Core.Systems.Animations;
using DG.Tweening;
using UnityEngine;

namespace Core.Systems.UI
{
    public abstract class Menu<T> : Menu where T : Menu<T>
    {
        public static T Instance { get; private set; }

        protected virtual void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError($"There is more than one instance of {typeof(T).Name} in the scene!");
                Destroy(gameObject);
                return;
            }
            
            Instance = (T) this;
            
            if (uiService == null)
                ServiceLocator.ServiceLocator.TryGet(out uiService);
            
            if (animationService == null)
                ServiceLocator.ServiceLocator.TryGet(out animationService);
        }

        protected virtual void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }
    }
    
    public abstract class Menu : MonoBehaviour
    {
        #region Fields
        
        [Header("Menu Properties")] 
        [Tooltip("What is the name of this menu? This is used for identification in the UI manager.")]
        [SerializeField]
        protected string menuName;
        
        [Tooltip("Does this menu pause the game's time scale when opened?")]
        [SerializeField]
        protected bool pausesTime = true;

        [Tooltip("Should the main HUD (e.g., minimap, health bar) be hidden when this menu is active?")]
        [SerializeField]
        protected bool hidesHUD = true;
        
        [Tooltip("Is this menu opened by default?")]
        [SerializeField]
        protected bool openByDefault = false;
        
        [Tooltip("Should this Menu be able to be closed by pressing the back button?")]
        [SerializeField]
        protected bool canBeClosedByBackButton = true;

        [Header("Animation Settings")] 
        [Tooltip("The main container for all visual elements of the menu.")]
        [SerializeField]
        protected RectTransform mainContainer;

        [Tooltip("The canvas group for fading the menu in and out.")]
        [SerializeField]
        protected CanvasGroup canvasGroup;
        
        protected IUIService uiService;
        protected DOTweenAnimationService animationService;
        
        #endregion
        
        #region Properties
        
        public string MenuName => menuName;
        public bool PausesTime => pausesTime;
        public bool HidesHUD => hidesHUD;
        public bool OpenByDefault => openByDefault;
        public bool CanBeClosedByBackButton => canBeClosedByBackButton;
        public RectTransform MainContainer => mainContainer;
        public CanvasGroup CanvasGroup => canvasGroup;
        
        #endregion

        public virtual void Open(bool useAnimation = true)
        {
            gameObject.SetActive(true);
            
            OnOpen();
            
            if (useAnimation && canvasGroup != null && mainContainer != null)
            {
                var tween = PlayOpenAnimation();

                if (tween != null)
                {
                    DisableInteractions();
                
                    tween.SetUpdate(true);
                    tween.onComplete += EnableInteractions;
                }
                
                return;
            }
            
            EnableInteractions();
        }

        public virtual Tween Close(bool useAnimation = true)
        {
            OnClose();
            
            if (useAnimation && canvasGroup != null && mainContainer != null)
            {
                var tween = PlayCloseAnimation() ?? DOTween.Sequence().SetUpdate(true);
                
                if (tween != null)
                {
                    DisableInteractions();
                
                    tween.SetUpdate(true);
                    tween.onComplete += () =>
                    {
                        KillAllAnimations();
                        EnableInteractions();
                        
                        gameObject.SetActive(false);
                    };
                }
                else
                {
                    CloseMenuInstantly();
                }
                
                return tween;
            }

            CloseMenuInstantly();
            
            return DOTween.Sequence().SetUpdate(true);
        }
        
        public virtual void OnBackPressed()
        {
            if (canBeClosedByBackButton && uiService != null)
                uiService.CloseMenu();
        }

        protected virtual void KillAllAnimations()
        {
        }

        protected virtual void OnOpen()
        {
        }
        
        protected virtual void OnClose()
        {
        }
        
        protected abstract Tween PlayOpenAnimation();
        protected abstract Tween PlayCloseAnimation();
        
        private void DisableInteractions()
        {
            if (canvasGroup != null)
            {
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            }
        }
        
        private void EnableInteractions()
        {
            if (canvasGroup != null)
            {
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
            }
        }

        private void CloseMenuInstantly()
        {
            KillAllAnimations();
            EnableInteractions();
            
            gameObject.SetActive(false);
        }
    }
}