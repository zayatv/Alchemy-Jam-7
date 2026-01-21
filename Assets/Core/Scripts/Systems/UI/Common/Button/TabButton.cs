using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Core.Systems.UI.Common.Button
{
    [RequireComponent(typeof(InteractiveButton))]
    public class TabButton : MonoBehaviour
    {
        #region Fields

        [Title("Tab Configuration")]
        [SerializeField, Required]
        [Tooltip("The interactive button component")]
        private InteractiveButton interactiveButton;

        [Title("Auto-Select Behavior")]
        [SerializeField]
        [Tooltip("Automatically select this tab when clicked (disable for custom behavior)")]
        private bool autoSelectOnClick = true;

        [Title("Interaction Settings")]
        [SerializeField]
        [Tooltip("Disable interaction (clicks, hovers) when this tab is selected")]
        private bool disableInteractionWhenSelected = true;

        #endregion

        #region Properties

        public InteractiveButton Button => interactiveButton;
        public bool IsSelected => interactiveButton != null && interactiveButton.IsSelected;
        public bool DisableInteractionWhenSelected
        {
            get => disableInteractionWhenSelected;
            set
            {
                if (disableInteractionWhenSelected != value)
                {
                    disableInteractionWhenSelected = value;
                    
                    UpdateInteractability(IsSelected);
                }
            }
        }

        #endregion

        #region Events

        public event Action<TabButton> OnTabSelected;
        public event Action<TabButton> OnTabDeselected;

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            if (interactiveButton == null)
                interactiveButton = GetComponent<InteractiveButton>();

            RegisterListeners();
        }

        private void Start()
        {
            UpdateInteractability(IsSelected);
        }

        private void OnDestroy()
        {
            UnregisterListeners();
        }

        #endregion

        #region Public API
        
        public void SetSelected(bool selected, bool notifyListeners = true)
        {
            if (interactiveButton == null)
                return;
            
            bool wasSelected = IsSelected;
            
            interactiveButton.SetSelected(selected, false);

            UpdateInteractability(selected);

            if (notifyListeners && wasSelected != selected)
            {
                if (selected)
                    OnTabSelected?.Invoke(this);
                else
                    OnTabDeselected?.Invoke(this);
            }
        }

        #endregion

        #region Internal Methods
        
        private void UpdateInteractability(bool isSelected)
        {
            if (interactiveButton == null || interactiveButton.CanvasGroup == null)
                return;
            
            bool shouldBlockInteraction = disableInteractionWhenSelected && isSelected;

            if (shouldBlockInteraction)
            {
                interactiveButton.DisableInteraction();
            }
            else
            {
                interactiveButton.DisableInteraction(false);
            }
        }

        #endregion

        #region Event Handling

        private void RegisterListeners()
        {
            if (interactiveButton != null)
            {
                interactiveButton.OnClick += OnButtonClicked;
                interactiveButton.OnSubmitEvent += OnButtonSubmitted;
                interactiveButton.OnSelectionChanged += OnButtonSelectionChanged;
            }
        }

        private void UnregisterListeners()
        {
            if (interactiveButton != null)
            {
                interactiveButton.OnClick -= OnButtonClicked;
                interactiveButton.OnSubmitEvent -= OnButtonSubmitted;
                interactiveButton.OnSelectionChanged -= OnButtonSelectionChanged;
            }
        }

        private void OnButtonClicked(InteractiveButton button, PointerEventData eventData)
        {
            if (autoSelectOnClick && !IsSelected)
            {
                SetSelected(true);
            }
        }

        private void OnButtonSubmitted(InteractiveButton button, BaseEventData eventData)
        {
            if (autoSelectOnClick && !IsSelected)
            {
                SetSelected(true);
            }
        }

        private void OnButtonSelectionChanged(InteractiveButton button, bool isSelected)
        {
            UpdateInteractability(isSelected);

            if (isSelected)
                OnTabSelected?.Invoke(this);
            else
                OnTabDeselected?.Invoke(this);
        }

        #endregion

        #region Editor Utilities

#if UNITY_EDITOR
        [Button("Setup Tab Button", ButtonSizes.Large), PropertyOrder(-1)]
        [InfoBox("Automatically configures this GameObject as a proper tab button")]
        private void SetupTabButton()
        {
            if (interactiveButton == null)
                interactiveButton = GetComponent<InteractiveButton>();

            if (interactiveButton == null)
                interactiveButton = gameObject.AddComponent<InteractiveButton>();

            var so = new UnityEditor.SerializedObject(interactiveButton);
            
            so.FindProperty("supportsSelection").boolValue = true;
            so.FindProperty("highlightWhenSelected").boolValue = true;
            so.ApplyModifiedProperties();

            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.EditorUtility.SetDirty(interactiveButton);

            Debug.Log($"Tab button configured: {gameObject.name}");
        }
#endif

        #endregion
    }
}
