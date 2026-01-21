using System;
using Core.Systems.Animations;
using Core.Systems.UI.Common.Button;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Core.Systems.UI
{
    public class UIModal : Menu<UIModal>
    {
        #region Fields

        [Header("Universal Modal Items")]
        [SerializeField] private RectTransform content;
        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private InteractiveButton confirmButton;
        [SerializeField] private TextMeshProUGUI confirmButtonText;
        [SerializeField] private InteractiveButton cancelButton;
        [SerializeField] private TextMeshProUGUI cancelButtonText;
        
        [Header("Default Confirmation Modal Items")]
        [SerializeField] private RectTransform defaultConfirmationPanel;
        [SerializeField] private TextMeshProUGUI defaultConfirmationText;

        private Action _onConfirm;
        private Action _onCancel;
        
        #endregion
        
        #region Setup Methods
        
        private void UniversalSetup(string titleText, bool showConfirmButton, string confirmText, Action onConfirm, bool showCancelButton, string cancelText, Action onCancel)
        {
            _onConfirm = onConfirm;
            _onCancel = onCancel;

            this.title.text = titleText;

            confirmButton.gameObject.SetActive(showConfirmButton);
            confirmButtonText.text = confirmText;
            confirmButton.OnClick += OnConfirmPressed;
            
            cancelButton.gameObject.SetActive(showCancelButton);
            cancelButtonText.text = cancelText;
            cancelButton.OnClick += OnCancelPressed;
        }
        
        public void SetupDefaultConfirmationModal(string titleText, string message, Action onConfirm, bool cancelAction, Action onCancel = null)
        {
            UniversalSetup(titleText, true, "Confirm", onConfirm, cancelAction, "Cancel", onCancel);
            
            defaultConfirmationPanel.gameObject.SetActive(true);
            defaultConfirmationText.text = message;
        }
        
        #endregion
        
        #region Button Callbacks
        
        private void OnConfirmPressed(InteractiveButton button, PointerEventData pointerData)
        {
            OnConfirm();
        }

        private void OnCancelPressed(InteractiveButton button, PointerEventData pointerData)
        {
            OnCancel();
        }

        private void OnConfirm()
        {
            uiService.CloseMenu();
            _onConfirm?.Invoke();
        }
        
        private void OnCancel()
        {
            uiService.CloseMenu();
            _onCancel?.Invoke();
        }
        
        #endregion
        
        #region Menu Overrides
        
        public override void Open(bool useAnimation = true)
        {
            base.Open(useAnimation);
            
            _onConfirm = null;
            _onCancel = null;
        }

        public override Tween Close(bool useAnimation = true)
        {
            confirmButton.OnClick -= OnConfirmPressed;
            cancelButton.OnClick -= OnCancelPressed;
            
            return base.Close(useAnimation);
        }
        
        public override void OnBackPressed()
        {
            OnCancel();
        }
        
        protected override Tween PlayOpenAnimation()
        {
            KillAllAnimations();
            
            canvasGroup.alpha = 0f;
            content.localScale = Vector3.zero;
            
            var sequence = DOTween.Sequence();

            sequence.Append(animationService.SafeFade(canvasGroup, 1f, 0.3f, Ease.OutExpo));
            sequence.Join(animationService.SafeScale(content, Vector3.one, 0.3f, Ease.OutExpo));
            
            sequence.SetUpdate(true);
            
            animationService.RegisterTween(sequence);
            
            return sequence;
        }
        
        protected override Tween PlayCloseAnimation()
        {
            KillAllAnimations();
            
            canvasGroup.alpha = 1f;
            
            var sequence = DOTween.Sequence();

            sequence.Append(animationService.SafeFade(canvasGroup, 0f, 0.3f, Ease.OutExpo));
            sequence.Join(animationService.SafeScale(content, Vector3.zero, 0.3f, Ease.OutExpo));
            
            sequence.SetUpdate(true);
            
            animationService.RegisterTween(sequence);

            return sequence;
        }
        
        protected override void KillAllAnimations()
        {
            base.KillAllAnimations();
            
            if (canvasGroup != null)
            {
                animationService.KillTweensForTarget(canvasGroup);
            }
            
            animationService.KillTweensForTarget(content);
        }
        
        #endregion
    }
}