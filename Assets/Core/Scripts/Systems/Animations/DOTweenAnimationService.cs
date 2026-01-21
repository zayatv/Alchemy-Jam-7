using System;
using System.Collections.Generic;
using Core.Systems.Logging;
using Core.Systems.ServiceLocator;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Systems.Animations
{
    public class DOTweenAnimationService : MonoBehaviour, IService
    {
        #region Fields
        
        private List<Tween> _activeTweens = new List<Tween>();
        
        #endregion
        
        #region Unity Methods
        
        private void Awake()
        {
            if (DOTween.instance == null)
                DOTween.Init(null, true, LogBehaviour.ErrorsOnly);
        }
        
        private void OnDestroy()
        {
            KillAllTweens();
        }
        
        #endregion
        
        #region Animations

        /// <summary>
        /// Safely applies a punch scale animation to a target Transform with error handling.
        /// </summary>
        /// <param name="target">The target Transform to animate. If null, the animation will not proceed.</param>
        /// <param name="punch">The vector that determines the punch scale strength in each axis.</param>
        /// <param name="duration">The duration of the punch scale animation in seconds.</param>
        /// <param name="vibrato">The number of oscillations during the punch scale animation. Default is 10.</param>
        /// <param name="elasticity">The amount by which the punch overshoots. Value ranges from 0 to 1. Default is 1f.</param>
        /// <param name="ease">The easing function to apply to the animation. Default is Ease.OutQuad.</param>
        /// <param name="useUnscaledTime">Determines whether the animation should ignore time scaling. Default is true.</param>
        /// <returns>A Tween representing the punch scale animation. Returns null if the target is null or if DOTween is unavailable.</returns>
        public Tween SafePunchScale(Transform target, Vector3 punch, float duration, int vibrato = 10, float elasticity = 1f, Ease ease = Ease.OutQuad, bool useUnscaledTime = true)
        {
            if (target == null || !IsDOTweenAvailable())
                return null;
            
            try
            {
                var originalScale = target.localScale;
                var tween = target.DOPunchScale(punch, duration, vibrato, elasticity).SetEase(ease);
                
                if (useUnscaledTime)
                {
                    tween.SetUpdate(UpdateType.Normal, true);
                }

                tween.onKill += () =>
                {
                    if (target != null)
                        target.localScale = originalScale;
                };
                
                RegisterTween(tween);
                
                return tween;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Safely applies a color transition to a target Graphic with error handling.
        /// </summary>
        /// <param name="target">The target Graphic whose color will be animated. If null, the animation will not proceed.</param>
        /// <param name="endValue">The final color value to transition to.</param>
        /// <param name="duration">The duration of the color transition in seconds.</param>
        /// <param name="ease">The easing function to apply to the animation. Default is Ease.OutQuad.</param>
        /// <param name="useUnscaledTime">Determines whether the animation should ignore time scaling. Default is true.</param>
        /// <returns>A Tween representing the color transition animation. Returns null if the target is null or if DOTween is unavailable.</returns>
        public Tween SafeColor(Graphic target, Color endValue, float duration, Ease ease = Ease.OutQuad, bool useUnscaledTime = true)
        {
            if (target == null || !IsDOTweenAvailable())
                return null;

            try
            {
                var tween = target.DOColor(endValue, duration).SetEase(ease);
                
                if (useUnscaledTime)
                {
                    tween.SetUpdate(UpdateType.Normal, true);
                }

                tween.onKill += () =>
                {
                    if (target != null)
                        target.color = endValue;
                };
                
                RegisterTween(tween);
                
                return tween;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Safely applies a color flash animation to a target Graphic with error handling.
        /// </summary>
        /// <param name="target">The target Graphic to animate. If null, the animation will not proceed.</param>
        /// <param name="flashColor">The color to flash the target Graphic to.</param>
        /// <param name="flashDuration">The duration of the flash animation in seconds.</param>
        /// <param name="returnToOriginal">Determines whether the target Graphic should return to its original color after the flash.</param>
        /// <param name="ease">The easing function to apply to the animation. Default is Ease.OutQuad.</param>
        /// <param name="useUnscaledTime">Determines whether the animation should ignore time scaling. Default is true.</param>
        /// <returns>A Sequence representing the color flash animation. Returns null if the target is null or if DOTween is unavailable.</returns>
        public Sequence SafeColorFlash(Graphic target, Color flashColor, float flashDuration, bool returnToOriginal, Ease ease = Ease.OutQuad, bool useUnscaledTime = true)
        {
            if (target == null || !IsDOTweenAvailable())
                return null;

            try
            {
                var originalColor = target.color;
                var sequence = DOTween.Sequence();
                
                sequence.Append(target.DOColor(flashColor, flashDuration).SetEase(ease));
                
                if (returnToOriginal)
                {
                    sequence.Append(target.DOColor(originalColor, flashDuration).SetEase(ease));
                }
                
                if (useUnscaledTime)
                {
                    sequence.SetUpdate(UpdateType.Normal, true);
                }
                
                sequence.onKill = () => 
                {
                    if (returnToOriginal && target != null)
                        target.color = originalColor;
                };
                
                RegisterTween(sequence);
                
                return sequence;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Safely animates the fade effect on a target CanvasGroup with error handling.
        /// </summary>
        /// <param name="target">The CanvasGroup to fade. If null, the fade animation will not proceed.</param>
        /// <param name="endValue">The target alpha value to fade to.</param>
        /// <param name="duration">The duration of the fade animation in seconds.</param>
        /// <param name="ease">The easing function to apply to the fade animation. Default is Ease.OutQuad.</param>
        /// <param name="useUnscaledTime">Determines whether the animation should ignore time scaling. Default is true.</param>
        /// <returns>A Tween representing the fade animation. Returns null if the target is null or if DOTween is unavailable.</returns>
        public Tween SafeFade(CanvasGroup target, float endValue, float duration, Ease ease = Ease.OutQuad, bool useUnscaledTime = true)
        {
            if (target == null || !IsDOTweenAvailable())
                return null;

            try
            {
                var tween = target.DOFade(endValue, duration).SetEase(ease);
                
                if (useUnscaledTime)
                {
                    tween.SetUpdate(UpdateType.Normal, true);
                }

                tween.onKill += () =>
                {
                    if (target != null)
                        target.alpha = endValue;
                };
                
                RegisterTween(tween);
                
                return tween;
            }
            catch (Exception)
            {
                return null;
            }
        }
        
        /// <summary>
        /// Safely animates the fade effect on a target CanvasGroup with error handling.
        /// </summary>
        /// <param name="graphic">The Graphic to fade. If null, the fade animation will not proceed.</param>
        /// <param name="endValue">The target alpha value to fade to.</param>
        /// <param name="duration">The duration of the fade animation in seconds.</param>
        /// <param name="ease">The easing function to apply to the fade animation. Default is Ease.OutQuad.</param>
        /// <param name="useUnscaledTime">Determines whether the animation should ignore time scaling. Default is true.</param>
        /// <returns>A Tween representing the fade animation. Returns null if the target is null or if DOTween is unavailable.</returns>
        public Tween SafeFade(Graphic graphic, float endValue, float duration, Ease ease = Ease.OutQuad, bool useUnscaledTime = true)
        {
            if (graphic == null || !IsDOTweenAvailable())
                return null;

            try
            {
                var tween = graphic.DOFade(endValue, duration).SetEase(ease);
                
                if (useUnscaledTime)
                {
                    tween.SetUpdate(UpdateType.Normal, true);
                }

                tween.onKill += () =>
                {
                    if (graphic != null)
                    {
                        Color color = graphic.color;
                        color.a = endValue;
                        graphic.color = color;
                    }
                };
                
                RegisterTween(tween);
                
                return tween;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Safely animates the movement of a RectTransform to a specified position with error handling and optional unscaled time usage.
        /// </summary>
        /// <param name="target">The RectTransform to animate. If null, the animation will not proceed.</param>
        /// <param name="endValue">The target position for the RectTransform's anchored position.</param>
        /// <param name="duration">The duration of the movement animation in seconds.</param>
        /// <param name="ease">The easing function to apply to the animation. Default is Ease.OutCubic.</param>
        /// <param name="useUnscaledTime">Determines whether the animation should ignore time scaling. Default is true.</param>
        /// <returns>A Tween representing the movement animation. Returns null if the target is null or if DOTween is unavailable.</returns>
        public Tween SafeMove(RectTransform target, Vector2 endValue, float duration, Ease ease = Ease.OutCubic, bool useUnscaledTime = true)
        {
            if (target == null || !IsDOTweenAvailable())
                return null;

            try
            {
                var tween = target.DOAnchorPos(endValue, duration).SetEase(ease);
                
                if (useUnscaledTime)
                {
                    tween.SetUpdate(UpdateType.Normal, true);
                }

                tween.onKill += () =>
                {
                    if (target != null)
                        target.anchoredPosition = endValue;
                };
                
                RegisterTween(tween);
                
                return tween;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Safely animates a Slider's progress bar value with error handling.
        /// </summary>
        /// <param name="target">The target Slider to animate. If null, the animation will not proceed.</param>
        /// <param name="endValue">The final value for the Slider at the end of the animation.</param>
        /// <param name="duration">The duration of the animation in seconds.</param>
        /// <param name="ease">The easing function to apply to the animation. Default is Ease.OutQuart.</param>
        /// <param name="useUnscaledTime">Determines whether the animation should ignore time scaling. Default is true.</param>
        /// <returns>A Tween representing the progress bar animation. Returns null if the target is null or if DOTween is unavailable.</returns>
        public Tween SafeProgressBar(Slider target, float endValue, float duration, Ease ease = Ease.OutQuart, bool useUnscaledTime = true)
        {
            if (target == null || !IsDOTweenAvailable())
                return null;

            try
            {
                var tween = DOTween.To(() => target.value, x => target.value = x, endValue, duration).SetEase(ease);
                
                if (useUnscaledTime)
                {
                    tween.SetUpdate(UpdateType.Normal, true);
                }

                tween.onKill += () =>
                {
                    if (target != null)
                        target.value = endValue;
                };
                
                RegisterTween(tween);
                
                return tween;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Safely applies a continuous pulse scale animation to a target Transform with error handling.
        /// </summary>
        /// <param name="target">The target Transform to animate. If null, the animation will not proceed.</param>
        /// <param name="pulseScale">The multiplier for the target's scale during pulsing. The scale oscillates between the original and this scaled value.</param>
        /// <param name="pulseDuration">The duration of one complete pulse cycle in seconds.</param>
        /// <param name="ease">The easing function to apply during the pulse animation. Default is Ease.InOutSine.</param>
        /// <param name="useUnscaledTime">Determines whether the animation should ignore time scaling. Default is true.</param>
        /// <returns>A Sequence representing the pulse scale animation. Returns null if the target is null or if DOTween is unavailable.</returns>
        public Sequence SafePulseEffect(Transform target, float pulseScale, float pulseDuration, Ease ease = Ease.InOutSine, bool useUnscaledTime = true)
        {
            if (target == null || !IsDOTweenAvailable())
                return null;

            try
            {
                var originalScale = target.localScale;
                var targetScale = originalScale * pulseScale;
                var sequence = DOTween.Sequence();
                
                sequence.Append(target.DOScale(targetScale, pulseDuration * 0.5f).SetEase(ease))
                    .Append(target.DOScale(originalScale, pulseDuration * 0.5f).SetEase(ease))
                    .SetLoops(-1, LoopType.Restart);
                
                if (useUnscaledTime)
                {
                    sequence.SetUpdate(UpdateType.Normal, true);
                }
                
                sequence.onKill = () => 
                {
                    if (target != null)
                        target.localScale = originalScale;
                };
                       
                RegisterTween(sequence);
                
                return sequence;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Safely interpolates a number value over time and updates the text of a TextMeshProUGUI component.
        /// </summary>
        /// <param name="target">The TextMeshProUGUI component whose text will be updated. If null, the interpolation will not proceed.</param>
        /// <param name="fromValue">The starting integer value of the interpolation.</param>
        /// <param name="toValue">The target integer value to interpolate towards.</param>
        /// <param name="duration">The duration of the interpolation in seconds.</param>
        /// <param name="ease">The easing function to apply to the interpolation. Default is Ease.OutQuart.</param>
        /// <param name="useUnscaledTime">Determines whether the interpolation should ignore time scaling. Default is true.</param>
        /// <returns>A Tween representing the number interpolation. Returns null if the target is null or if DOTween is unavailable.</returns>
        public Tween SafeNumberInterpolation(TextMeshProUGUI target, int fromValue, int toValue, float duration, Ease ease = Ease.OutQuart, bool useUnscaledTime = true)
        {
            if (target == null || !IsDOTweenAvailable())
                return null;

            try
            {
                var tween = DOTween.To(
                    () => fromValue,
                    value => 
                    {
                        target.text = Mathf.RoundToInt(value).ToString();
                    },
                    toValue,
                    duration
                ).SetEase(ease);
                
                if (useUnscaledTime)
                {
                    tween.SetUpdate(UpdateType.Normal, true);
                }
                
                tween.onComplete = () => 
                {
                    target.text = toValue.ToString();
                };
                
                tween.onKill = () =>
                {
                    if (target != null)
                        target.text = toValue.ToString();
                };
                
                RegisterTween(tween);
                
                return tween;
            }
            catch (Exception)
            {
                return null;
            }
        }
        
        /// <summary>
        /// Safely animate transform scale with error handling
        /// </summary>
        /// <param name="target">Transform to scale</param>
        /// <param name="targetScale">Target scale</param>
        /// <param name="duration">Animation duration</param>
        /// <param name="ease">Easing curve</param>
        /// <param name="useUnscaledTime">Whether to use unscaled time</param>
        /// <returns>The created tween or null if failed</returns>
        public Tween SafeScale(Transform target, Vector3 targetScale, float duration, Ease ease = Ease.OutQuad, bool useUnscaledTime = true)
        {
            if (target == null || !IsDOTweenAvailable())
                return null;
    
            try
            {
                var tween = target.DOScale(targetScale, duration).SetEase(ease);
                
                if (useUnscaledTime)
                {
                    tween.SetUpdate(UpdateType.Normal, true);
                }
                
                tween.onKill += () =>
                {
                    if (target != null)
                        target.localScale = targetScale;
                };
            
                RegisterTween(tween);
                
                return tween;
            }
            catch (Exception)
            {
                return null;
            }
        }
        
        /// <summary>
        /// Check if DOTween is available and initialized
        /// </summary>
        /// <returns>True if DOTween is available, false otherwise</returns>
        private bool IsDOTweenAvailable()
        {
            try
            {
                return DOTween.instance != null;
            }
            catch (Exception ex)
            {
                GameLogger.Log(LogLevel.Critical, $"Error checking DOTween availability: {ex.Message}");
                return false;
            }
        }
        
        #endregion
        
        #region Tween Management
        
        /// <summary>
        /// Register a tween for tracking and cleanup
        /// </summary>
        /// <param name="tween">The tween to register</param>
        public void RegisterTween(Tween tween)
        {
            if (tween == null)
                return;
            
            try
            {
                _activeTweens.Add(tween);
                
                tween.onComplete += () => {
                    try
                    {
                        _activeTweens.Remove(tween);
                    }
                    catch (Exception ex)
                    {
                        GameLogger.Log(LogLevel.Error, $"Error removing completed tween: {ex.Message}");
                    }
                };
                
                tween.onKill += () => {
                    try
                    {
                        _activeTweens.Remove(tween);
                    }
                    catch (Exception ex)
                    {
                        GameLogger.Log(LogLevel.Error, $"Error removing killed tween: {ex.Message}");
                    }
                };
            }
            catch (Exception ex)
            {
                GameLogger.Log(LogLevel.Error, $"Error registering tween: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Kill all active tweens
        /// </summary>
        public void KillAllTweens()
        {
            try
            {
                var tweensToKill = new List<Tween>(_activeTweens);
                
                foreach (var tween in tweensToKill)
                {
                    try
                    {
                        if (tween != null && tween.IsActive())
                        {
                            tween.Kill();
                        }
                    }
                    catch (Exception ex)
                    {
                        GameLogger.Log(LogLevel.Error, $"Error killing individual tween: {ex.Message}");
                    }
                }
                
                _activeTweens.Clear();
                
                GC.Collect();
                
                GameLogger.Log(LogLevel.Debug, "All tweens killed successfully");
            }
            catch (Exception ex)
            {
                GameLogger.Log(LogLevel.Critical, $"Critical error in KillAllTweens: {ex.Message}");
                
                try
                {
                    DOTween.KillAll();
                    _activeTweens.Clear();
                }
                catch
                {
                    GameLogger.Log(LogLevel.Error, "Emergency cleanup also failed");
                }
            }
        }
        
        /// <summary>
        /// Kill all tweens associated with a specific target
        /// </summary>
        /// <param name="target">The target object</param>
        public void KillTweensForTarget(object target)
        {
            if (target == null) return;
            
            DOTween.Kill(target);
            
            _activeTweens.RemoveAll(tween => tween == null || !tween.IsActive());
        }

        /// <summary>
        /// Retrieves a list of active tweens associated with the specified target.
        /// </summary>
        /// <param name="target">The target object whose active tweens are to be retrieved.</param>
        /// <returns>A list of active tweens tied to the specified target.</returns>
        public List<Tween> GetActiveTweensForTarget(object target)
        {
            if (target == null) return new List<Tween>();
            
            return _activeTweens.FindAll(tween => tween.target == target);
        }
        
        /// <summary>
        /// Pause all active tweens
        /// </summary>
        public void PauseAllTweens()
        {
            foreach (var tween in _activeTweens)
            {
                if (tween != null && tween.IsActive() && tween.IsPlaying())
                {
                    tween.Pause();
                }
            }
        }

        /// <summary>
        /// Pauses all active and playing tweens associated with the specified target.
        /// </summary>
        /// <param name="target">The target object whose associated tweens will be paused. If no tweens are found for the specified target, no action will be taken.</param>
        public void PauseAllTweensForTarget(object target)
        {
            foreach (var tween in _activeTweens)
            {
                if (tween != null && tween.IsActive() && tween.IsPlaying() && tween.target == target)
                {
                    tween.Pause();
                }
            }
        }
        
        /// <summary>
        /// Resume all paused tweens
        /// </summary>
        public void ResumeAllTweens()
        {
            foreach (var tween in _activeTweens)
            {
                if (tween != null && tween.IsActive() && !tween.IsPlaying())
                {
                    tween.Play();
                }
            }
        }

        /// <summary>
        /// Resumes all paused tweens associated with the specified target.
        /// </summary>
        /// <param name="target">The target object whose associated tweens should be resumed. If null, no action is taken.</param>
        public void ResumeAllTweensForTarget(object target)
        {
            foreach (var tween in _activeTweens)
            {
                if (tween != null && tween.IsActive() && !tween.IsPlaying() && tween.target == target)
                {
                    tween.Play();
                }
            }
        }
        
        #endregion
    }
}