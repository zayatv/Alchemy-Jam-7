using Core.Systems.Animations;
using DG.Tweening;
using UnityEngine;

namespace Core.Systems.UI.Common
{
    public abstract class BaseTab : MonoBehaviour
    {
        #region Fields
        
        [SerializeField] protected CanvasGroup canvasGroup;
        
        private DOTweenAnimationService _animationService;
        
        #endregion

        protected virtual void Awake()
        {
            if (!canvasGroup && !TryGetComponent(out canvasGroup))
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
        }

        private void Start()
        {
            if (_animationService == null)
                ServiceLocator.ServiceLocator.TryGet(out _animationService);
        }

        public virtual Tween Open()
        {
            KillBaseTabAnimations();
            
            var tween = OpenTabAnimation();
            
            if (tween != null)
            {
                DisableInteractions();
                
                tween.onComplete += EnableInteractions;
            }

            return tween;
        }
        
        public virtual Tween Close()
        {
            KillBaseTabAnimations();
            
            var tween = CloseTabAnimation();
            
            if (tween != null)
            {
                DisableInteractions();
                
                CloseTabAnimation().onComplete += () =>
                {
                    EnableInteractions();
                    gameObject.SetActive(false);
                };
            }
            else
            {
                gameObject.SetActive(false);
            }
            
            return tween;
        }
        
        public virtual Tween OpenTabAnimation()
        {
            return null;
        }
        
        public virtual Tween CloseTabAnimation()
        {
            return null;
        }
        
        protected virtual void KillBaseTabAnimations()
        {
            _animationService.KillTweensForTarget(this);
            _animationService.KillTweensForTarget(transform);
            
            if (canvasGroup != null)
                _animationService.KillTweensForTarget(canvasGroup);
        }
        
        protected virtual void OnDisable()
        {
            KillBaseTabAnimations();
        }
        
        protected virtual void OnDestroy()
        {
            KillBaseTabAnimations();
        }
        
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
    }
}