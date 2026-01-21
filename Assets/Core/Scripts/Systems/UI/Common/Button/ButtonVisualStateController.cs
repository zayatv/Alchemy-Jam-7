using System.Collections.Generic;
using Core.Systems.Animations;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Systems.UI.Common.Button
{
    public class ButtonVisualStateController
    {
        private struct TweenKey
        {
            public Component Component;
            public VisualAnimationType AnimationType;

            public TweenKey(Component component, VisualAnimationType animationType)
            {
                Component = component;
                AnimationType = animationType;
            }

            public override bool Equals(object obj)
            {
                if (obj is TweenKey other)
                {
                    return Component == other.Component && AnimationType == other.AnimationType;
                }
                return false;
            }

            public override int GetHashCode()
            {
                return (Component?.GetHashCode() ?? 0) * 397 ^ AnimationType.GetHashCode();
            }
        }

        private readonly List<ButtonVisualElement> _visualElements;
        private ButtonState _currentState = ButtonState.Normal;
        private readonly Dictionary<TweenKey, Tween> _activeTweens = new Dictionary<TweenKey, Tween>();
        private readonly Dictionary<Component, Vector2> _originalSizes = new Dictionary<Component, Vector2>();
        private readonly Dictionary<Component, Vector3> _originalScales = new Dictionary<Component, Vector3>();
        
        private DOTweenAnimationService _animationService;

        public ButtonVisualStateController(List<ButtonVisualElement> visualElements)
        {
            _visualElements = visualElements ?? new List<ButtonVisualElement>();
            CacheOriginalSizes();
            EnsureAnimationService();
        }

        private void EnsureAnimationService()
        {
            if (_animationService == null)
                ServiceLocator.ServiceLocator.TryGet(out _animationService);
        }
        
        private void CacheOriginalSizes()
        {
            foreach (var element in _visualElements)
            {
                if (element.target is RectTransform rectTransform)
                {
                    var size = rectTransform.rect.size;
                    
                    if (size.x > 0f && size.y > 0f)
                    {
                        _originalSizes[element.target] = size;
                        _originalScales[element.target] = element.target.transform.localScale;
                    }
                }
            }
        }
        
        public void TransitionToState(ButtonState newState, bool instant = false)
        {
            if (_currentState == newState && !instant)
                return;

            _currentState = newState;
            
            EnsureAnimationService();

            foreach (var element in _visualElements)
            {
                if (element.target == null)
                    continue;

                AnimateElement(element, newState, instant);
            }
        }
        
        private void AnimateElement(ButtonVisualElement element, ButtonState state, bool instant)
        {
            float duration = instant ? 0f : element.transitionDuration;

            switch (element.animationType)
            {
                case VisualAnimationType.Color:
                    KillTweenForType(element.target, VisualAnimationType.Color);
                    AnimateColor(element, state, duration);
                    
                    break;

                case VisualAnimationType.Scale:
                    KillTweenForType(element.target, VisualAnimationType.Scale);
                    AnimateScale(element, state, duration);
                    
                    break;

                case VisualAnimationType.Position:
                    KillTweenForType(element.target, VisualAnimationType.Position);
                    AnimatePosition(element, state, duration);
                    
                    break;

                case VisualAnimationType.Rotation:
                    KillTweenForType(element.target, VisualAnimationType.Rotation);
                    AnimateRotation(element, state, duration);
                    
                    break;

                case VisualAnimationType.Alpha:
                    KillTweenForType(element.target, VisualAnimationType.Alpha);
                    AnimateAlpha(element, state, duration);
                    
                    break;

                case VisualAnimationType.All:
                    KillTweenForType(element.target, VisualAnimationType.Color);
                    KillTweenForType(element.target, VisualAnimationType.Scale);
                    KillTweenForType(element.target, VisualAnimationType.Position);
                    KillTweenForType(element.target, VisualAnimationType.Rotation);
                    KillTweenForType(element.target, VisualAnimationType.Alpha);

                    AnimateColor(element, state, duration);
                    AnimateScale(element, state, duration);
                    AnimatePosition(element, state, duration);
                    AnimateRotation(element, state, duration);
                    AnimateAlpha(element, state, duration);
                    
                    break;
            }
        }

        private void AnimateColor(ButtonVisualElement element, ButtonState state, float duration)
        {
            var targetColor = element.colorConfig.GetColorForState(state);

            if (element.target is Graphic graphic)
            {
                if (duration <= 0f)
                {
                    graphic.color = targetColor;
                }
                else
                {
                    var tween = _animationService.SafeColor(graphic, targetColor, duration, element.transitionEase);
                    
                    RegisterTween(element.target, VisualAnimationType.Color, tween);
                }
            }
        }

        private void AnimateScale(ButtonVisualElement element, ButtonState state, float duration)
        {
            Vector3 targetScale;

            if (element.scaleConfig.scaleMode == ScaleMode.Pixels)
            {
                targetScale = CalculateScaleFromPixels(element, state);
            }
            else
            {
                targetScale = element.scaleConfig.GetScaleForState(state);
            }

            var transform = element.target.transform;

            if (duration <= 0f)
            {
                transform.localScale = targetScale;
            }
            else
            {
                var tween = _animationService.SafeScale(transform, targetScale, duration, element.transitionEase);
                
                RegisterTween(element.target, VisualAnimationType.Scale, tween);
            }
        }
        
        private Vector3 CalculateScaleFromPixels(ButtonVisualElement element, ButtonState state)
        {
            if (!(element.target is RectTransform rectTransform))
            {
                return Vector3.one;
            }

            if (!_originalSizes.TryGetValue(element.target, out var originalSize))
            {
                originalSize = rectTransform.rect.size;
                
                if (originalSize.x > 0f && originalSize.y > 0f)
                {
                    _originalSizes[element.target] = originalSize;
                }
            }

            if (!_originalScales.TryGetValue(element.target, out var originalScale))
            {
                originalScale = element.target.transform.localScale;
                
                if (_originalSizes.ContainsKey(element.target))
                {
                    _originalScales[element.target] = originalScale;
                }
            }
            
            if (originalSize.x <= 0f || originalSize.y <= 0f)
            {
                return originalScale;
            }

            var pixelExpansion = element.scaleConfig.GetPixelsForState(state);

            float scaleMultiplierX = (originalSize.x + pixelExpansion.x) / originalSize.x;
            float scaleMultiplierY = (originalSize.y + pixelExpansion.y) / originalSize.y;

            return new Vector3(originalScale.x * scaleMultiplierX, originalScale.y * scaleMultiplierY, originalScale.z);
        }

        private void AnimatePosition(ButtonVisualElement element, ButtonState state, float duration)
        {
            var targetPosition = element.positionConfig.GetPositionForState(state);

            if (element.target is RectTransform rectTransform)
            {
                if (duration <= 0f)
                {
                    rectTransform.anchoredPosition = targetPosition;
                }
                else
                {
                    var tween = _animationService.SafeMove(rectTransform, targetPosition, duration, element.transitionEase);
                    
                    RegisterTween(element.target, VisualAnimationType.Position, tween);
                }
            }
        }

        private void AnimateRotation(ButtonVisualElement element, ButtonState state, float duration)
        {
            var targetRotation = element.rotationConfig.GetRotationForState(state);

            if (element.target is RectTransform rectTransform)
            {
                if (duration <= 0f)
                {
                    rectTransform.localEulerAngles = targetRotation;
                }
                else
                {
                    var tween = rectTransform.DOLocalRotate(targetRotation, duration, RotateMode.FastBeyond360).SetEase(element.transitionEase).SetUpdate(true);
                    
                    _animationService.RegisterTween(tween);
                    
                    RegisterTween(element.target, VisualAnimationType.Rotation, tween);
                }
            }
        }

        private void AnimateAlpha(ButtonVisualElement element, ButtonState state, float duration)
        {
            var targetAlpha = element.alphaConfig.GetAlphaForState(state);

            if (element.target is CanvasGroup canvasGroup)
            {
                if (duration <= 0f)
                {
                    canvasGroup.alpha = targetAlpha;
                }
                else
                {
                    var tween = _animationService.SafeFade(canvasGroup, targetAlpha, duration, element.transitionEase);
                    
                    RegisterTween(element.target, VisualAnimationType.Alpha, tween);
                }
            }
            else if (element.target is Graphic graphic)
            {
                var currentColor = graphic.color;
                var targetColor = new Color(currentColor.r, currentColor.g, currentColor.b, targetAlpha);

                if (duration <= 0f)
                {
                    graphic.color = targetColor;
                }
                else
                {
                    var tween = _animationService.SafeFade(graphic, targetAlpha, duration, element.transitionEase);
                    
                    RegisterTween(element.target, VisualAnimationType.Alpha, tween);
                }
            }
        }

        private void RegisterTween(Component target, VisualAnimationType animationType, Tween tween)
        {
            if (tween != null)
            {
                var key = new TweenKey(target, animationType);
                
                _activeTweens[key] = tween;
            }
        }
        
        private void KillTweenForType(Component target, VisualAnimationType animationType)
        {
            var key = new TweenKey(target, animationType);

            if (_activeTweens.TryGetValue(key, out var tween))
            {
                tween?.Kill();
                
                _activeTweens.Remove(key);
            }
        }
        
        public void KillAllTweens()
        {
            foreach (var kvp in _activeTweens)
            {
                kvp.Value?.Kill();
            }

            _activeTweens.Clear();

            foreach (var element in _visualElements)
            {
                if (element.target != null)
                {
                    _animationService.KillTweensForTarget(element.target);
                    _animationService.KillTweensForTarget(element.target.transform);
                }
            }
        }
        
        public void UpdateOriginalSize(Component target)
        {
            if (target is RectTransform rectTransform)
            {
                var size = rectTransform.rect.size;

                if (size.x > 0f && size.y > 0f)
                {
                    _originalSizes[target] = size;
                    _originalScales[target] = target.transform.localScale;
                }
            }
        }
        
        public void RefreshOriginalSizes()
        {
            _originalSizes.Clear();
            _originalScales.Clear();
            CacheOriginalSizes();
        }
    }
}
