using DG.Tweening;
using UnityEngine;

namespace Core.Systems.UI
{
    public class BasicMenu : Menu<BasicMenu>
    {
        protected override Tween PlayOpenAnimation()
        {
            canvasGroup.alpha = 0f;
            transform.localScale = Vector3.zero;
            
            var sequence = DOTween.Sequence();
            
            sequence.Append(animationService.SafeFade(canvasGroup, 1f, 0.2f));
            sequence.Join(animationService.SafeScale(transform, Vector3.one, 0.2f));
            
            return sequence.SetUpdate(true);
        }

        protected override Tween PlayCloseAnimation()
        {
            canvasGroup.alpha = 0f;
            transform.localScale = Vector3.zero;
            
            var sequence = DOTween.Sequence();
            
            sequence.Append(animationService.SafeFade(canvasGroup, 0f, 0.2f));
            sequence.Join(animationService.SafeScale(transform, Vector3.zero, 0.2f));
            
            return sequence.SetUpdate(true);
        }
    }
}