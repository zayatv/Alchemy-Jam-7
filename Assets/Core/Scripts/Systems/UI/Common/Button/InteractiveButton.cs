using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Core.Systems.UI.Common.Button
{
    [RequireComponent(typeof(CanvasGroup))]
    public class InteractiveButton : MonoBehaviour,
        IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler,
        ISelectHandler, IDeselectHandler, ISubmitHandler,
        IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        #region Fields

        [Title("Core Components")]
        [SerializeField, Required, PropertyOrder(-10)]
        [Tooltip("Container for button content")]
        private RectTransform contentContainer;

        [SerializeField, Required, PropertyOrder(-10)]
        [Tooltip("Canvas group for controlling interactivity")]
        private CanvasGroup canvasGroup;

        [Title("Visual Feedback")]
        [SerializeField, PropertyOrder(-9)]
        [Tooltip("Visual elements that will animate based on button state")]
        [ListDrawerSettings(
            ShowIndexLabels = true,
            ListElementLabelName = "DisplaySummary",
            ShowFoldout = true,
            DraggableItems = true,
            ShowPaging = true,
            NumberOfItemsPerPage = 5
        )]
        private List<ButtonVisualElement> visualElements = new List<ButtonVisualElement>();

        [Title("State Settings")]
        [SerializeField, PropertyOrder(-8)]
        [Tooltip("Should this button be enabled by default?")]
        private bool enabledByDefault = true;
        
        [SerializeField, PropertyOrder(-8)]
        [Tooltip("Can this button be in a selected state (e.g., for tabs)?")]
        private bool supportsSelection = false;

        [SerializeField, PropertyOrder(-8), ShowIf(nameof(supportsSelection))]
        [Tooltip("Start in selected state")]
        private bool startSelected = false;

        [Title("Interaction Settings")]
        [SerializeField, PropertyOrder(-7)]
        [Tooltip("Enable dragging functionality")]
        private bool enableDragging = false;

        [SerializeField, PropertyOrder(-7), ShowIf(nameof(enableDragging))]
        [Tooltip("Distance threshold before drag starts")]
        private float dragThreshold = 10f;

        [Title("Audio (Optional)")]
        [SerializeField, PropertyOrder(-6)]
        private bool playAudioOnHover = true;

        [SerializeField, PropertyOrder(-6)]
        private bool playAudioOnClick = true;

        [Title("Advanced Settings")]
        [SerializeField, PropertyOrder(-5)]
        [Tooltip("Transition to highlighted state immediately when enabled and selected")]
        private bool highlightWhenSelected = false;

        [SerializeField, PropertyOrder(-5)]
        [Tooltip("Prevent state changes while button is transitioning")]
        private bool blockDuringTransition = false;
        
        [Title("Event Callbacks")]
        [SerializeField, PropertyOrder(-4)]
        [Tooltip("Event invoked when button is clicked")]
        private UnityEvent onUnityEventClick;
        
        private ButtonVisualStateController _visualController;
        private ButtonState _currentState = ButtonState.Normal;
        private ButtonState _targetState = ButtonState.Normal;

        private bool _initialized = false;
        private bool _isHovered = false;
        private bool _isSelected = false;
        private bool _isPressed = false;
        private bool _isNavigationSelected = false;
        private bool _isEnabled = true;
        private bool _disableInteraction = false;

        private bool _isDragging = false;
        private Vector2 _dragStartPosition;

        private readonly Dictionary<string, object> _data = new Dictionary<string, object>();

        #endregion

        #region Properties

        public CanvasGroup CanvasGroup => canvasGroup;
        public bool IsSelected => _isSelected;
        public Dictionary<string, object> Data => _data;

        #endregion

        #region Events

        public event Action<InteractiveButton, PointerEventData> OnHoverEnter;
        public event Action<InteractiveButton, PointerEventData> OnHoverExit;

        public event Action<InteractiveButton, PointerEventData> OnClick;
        public event Action<InteractiveButton, PointerEventData> OnPointerDownEvent;
        public event Action<InteractiveButton, PointerEventData> OnPointerUpEvent;

        public event Action<InteractiveButton, BaseEventData> OnNavigationSelect;
        public event Action<InteractiveButton, BaseEventData> OnNavigationDeselect;
        public event Action<InteractiveButton, BaseEventData> OnSubmitEvent;

        public event Action<InteractiveButton, ButtonState, ButtonState> OnStateChanged;

        public event Action<InteractiveButton, bool> OnSelectionChanged;

        public event Action<InteractiveButton, PointerEventData> OnDragBegin;
        public event Action<InteractiveButton, PointerEventData> OnDragging;
        public event Action<InteractiveButton, PointerEventData> OnDragEnd;

        #endregion

        #region Initialization
        
        private void InitializeButton()
        {
            if (_initialized)
                return;

            if (canvasGroup == null)
                canvasGroup = GetComponent<CanvasGroup>();

            if (canvasGroup == null)
                canvasGroup = gameObject.AddComponent<CanvasGroup>();

            _visualController = new ButtonVisualStateController(visualElements);

            _isEnabled = enabledByDefault;
            _disableInteraction = !enabledByDefault;

            if (startSelected && supportsSelection)
            {
                _isSelected = true;
                _currentState = ButtonState.Selected;
            }

            _visualController.TransitionToState(_currentState, instant: true);

            _initialized = true;
        }
        
        private void EnsureInitialized()
        {
            if (!_initialized)
            {
                InitializeButton();
            }
        }

        #endregion

        #region Unity Lifecycle

        protected virtual void Awake()
        {
            InitializeButton();
        }

        protected virtual void OnEnable()
        {
            EnsureInitialized();
            UpdateButtonState();
        }

        protected virtual void OnDisable()
        {
            _isHovered = false;
            _isPressed = false;
            _isNavigationSelected = false;
            _isDragging = false;

            _visualController?.KillAllTweens();
        }

        protected virtual void OnDestroy()
        {
            _visualController?.KillAllTweens();
        }

        #endregion

        #region Public API
        
        public GameObject SetContent(GameObject item, bool instantiatePrefab = false)
        {
            foreach (Transform child in contentContainer)
            {
                Destroy(child.gameObject);
            }

            if (item == null)
                return null;

            GameObject instance;

            if (instantiatePrefab)
            {
                instance = Instantiate(item, contentContainer);
            }
            else
            {
                instance = item;
                instance.transform.SetParent(contentContainer, false);
            }

            var rectTransform = instance.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.anchoredPosition = Vector2.zero;
                rectTransform.localScale = Vector3.one;
            }

            return instance;
        }
        
        public void SetEnabled(bool enabled)
        {
            EnsureInitialized();

            if (_isEnabled == enabled)
                return;

            _isEnabled = enabled;
            
            DisableInteraction(!enabled);
            UpdateButtonState();
        }
        
        public void DisableInteraction(bool disable = true)
        {
            EnsureInitialized();
            
            _disableInteraction = disable;
        }
        
        public void SetSelected(bool selected, bool notifyListeners = true)
        {
            EnsureInitialized();

            if (!supportsSelection)
            {
                Debug.LogWarning($"Button {gameObject.name} does not support selection. Enable 'supportsSelection' in inspector.");
                return;
            }

            if (_isSelected == selected)
                return;

            _isSelected = selected;

            UpdateButtonState();

            if (notifyListeners)
            {
                OnSelectionChanged?.Invoke(this, _isSelected);
            }
        }
        
        public void SimulateClick()
        {
            if (_disableInteraction)
                return;

            OnClick?.Invoke(this, null);
            onUnityEventClick?.Invoke();
        }
        
        public void ForceState(ButtonState state, bool instant = false)
        {
            EnsureInitialized();
            TransitionToState(state, instant);
        }
        
        public void SetData(string key, object value)
        {
            _data[key] = value;
        }
        
        public T GetData<T>(string key, T defaultValue = default)
        {
            if (_data.TryGetValue(key, out var value) && value is T typedValue)
            {
                return typedValue;
            }
            return defaultValue;
        }
        
        public void AddVisualElement(ButtonVisualElement element)
        {
            EnsureInitialized();

            if (element != null && !visualElements.Contains(element))
            {
                visualElements.Add(element);
                
                _visualController = new ButtonVisualStateController(visualElements);
                
                _visualController.TransitionToState(_currentState, instant: true);
            }
        }
        
        public void RemoveVisualElement(ButtonVisualElement element)
        {
            EnsureInitialized();

            if (visualElements.Remove(element))
            {
                _visualController = new ButtonVisualStateController(visualElements);
                
                _visualController.TransitionToState(_currentState, instant: true);
            }
        }
        
        public void UpdateOriginalSize(Component target)
        {
            EnsureInitialized();
            
            _visualController?.UpdateOriginalSize(target);
        }
        
        public void RefreshOriginalSizes()
        {
            EnsureInitialized();
            
            _visualController?.RefreshOriginalSizes();
        }

        #endregion

        #region Event System Implementations

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_disableInteraction)
                return;

            _isHovered = true;

            if (playAudioOnHover)
            {
                // TODO: Play hover sound
            }

            UpdateButtonState();
            
            OnHoverEnter?.Invoke(this, eventData);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_disableInteraction && !_isHovered)
                return;

            _isHovered = false;

            UpdateButtonState();
            
            OnHoverExit?.Invoke(this, eventData);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (_disableInteraction)
                return;

            _isPressed = true;

            UpdateButtonState();
            
            OnPointerDownEvent?.Invoke(this, eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (_disableInteraction)
                return;

            _isPressed = false;

            OnPointerUpEvent?.Invoke(this, eventData);
            
            StartCoroutine(DelayedStateUpdate());
        }

        private IEnumerator DelayedStateUpdate()
        {
            yield return null;
            
            UpdateButtonState();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_disableInteraction)
                return;

            if (playAudioOnClick)
            {
                // TODO: Play click sound
            }

            if (supportsSelection && !_isSelected)
            {
                SetSelected(true);
            }

            OnClick?.Invoke(this, eventData);
            onUnityEventClick?.Invoke();
        }

        public void OnSelect(BaseEventData eventData)
        {
            if (_disableInteraction)
                return;
            
            _isNavigationSelected = true;

            UpdateButtonState();
            
            OnNavigationSelect?.Invoke(this, eventData);
        }

        public void OnDeselect(BaseEventData eventData)
        {
            if (_disableInteraction)
                return;
            
            _isNavigationSelected = false;

            UpdateButtonState();
            
            OnNavigationDeselect?.Invoke(this, eventData);
        }

        public void OnSubmit(BaseEventData eventData)
        {
            if (_disableInteraction)
                return;

            if (playAudioOnClick)
            {
                // TODO: Play click sound
            }

            if (supportsSelection && !_isSelected)
            {
                SetSelected(true);
            }

            OnSubmitEvent?.Invoke(this, eventData);
            
            OnClick?.Invoke(this, null);
            onUnityEventClick?.Invoke();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!enableDragging)
            {
                ExecuteEvents.ExecuteHierarchy(transform.parent.gameObject, eventData, ExecuteEvents.beginDragHandler);
                
                return;
            }
            
            if (_disableInteraction)
                return;

            _dragStartPosition = eventData.position;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!enableDragging)
            {
                ExecuteEvents.ExecuteHierarchy(transform.parent.gameObject, eventData, ExecuteEvents.dragHandler);
                
                return;
            }

            if (!_isDragging)
            {
                if (Vector2.Distance(_dragStartPosition, eventData.position) >= dragThreshold)
                {
                    _isDragging = true;
                    OnDragBegin?.Invoke(this, eventData);
                }
                
                return;
            }

            OnDragging?.Invoke(this, eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!enableDragging)
            {
                ExecuteEvents.ExecuteHierarchy(transform.parent.gameObject, eventData, ExecuteEvents.endDragHandler);
                
                return;
            }

            if (_isDragging)
            {
                _isDragging = false;
                OnDragEnd?.Invoke(this, eventData);
            }

            _dragStartPosition = Vector2.zero;
        }

        #endregion

        #region State Management
        
        private void UpdateButtonState()
        {
            ButtonState newState = DetermineButtonState();
            
            if (newState != _currentState)
            {
                TransitionToState(newState);
            }
        }
        
        private ButtonState DetermineButtonState()
        {
            if (!_isEnabled)
                return ButtonState.Disabled;

            if (_isPressed)
                return ButtonState.Pressed;

            if (_isSelected && supportsSelection)
            {
                if (highlightWhenSelected && (_isHovered || _isNavigationSelected))
                    return ButtonState.Highlighted;

                return ButtonState.Selected;
            }

            if (_isHovered || _isNavigationSelected)
                return ButtonState.Highlighted;

            return ButtonState.Normal;
        }
        
        private void TransitionToState(ButtonState newState, bool instant = false)
        {
            if (_currentState == newState && !instant)
                return;

            ButtonState oldState = _currentState;
            
            _currentState = newState;

            OnStateChanged?.Invoke(this, oldState, newState);

            _visualController.TransitionToState(newState, instant);
        }

        #endregion
    }
}
