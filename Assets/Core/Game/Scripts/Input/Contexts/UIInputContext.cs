using System;
using System.Collections.Generic;
using Core.Systems.Events;
using Core.Systems.Logging;
using Core.Systems.Input;
using Core.Systems.UI.Events;
using Core.Systems.Update;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Core.Game.Input.Contexts
{
    public class UIInputContext : IInputContext, ILateUpdatable
    {
        private readonly PlayerInputActions.UIActions _uiActions;
        private readonly Stack<Action> _escapeHandlers = new Stack<Action>();
        private readonly HashSet<string> _cursorRequests = new HashSet<string>();
        
        private bool _isEnabled;
        
        public bool CancelPressed { get; private set; }
        public bool HasEscapeHandlers => _escapeHandlers.Count > 0;
        public bool IsEnabled => _isEnabled;
        public int LateUpdatePriority => 100;
        
        public UIInputContext(PlayerInputActions.UIActions uiActions)
        {
            _uiActions = uiActions;
            
            _uiActions.Cancel.performed += OnCancelPerformed;
            
            RegisterUIMenuEvents();
        }

        public void Enable()
        {
            if (IsEnabled) 
                return;
            
            _isEnabled = true;
        }

        public void Disable()
        {
            if (!IsEnabled) 
                return;
            
            CancelPressed = false;
            _isEnabled = false;
        }
        
        #region Escape Handlers

        /// <summary>
        /// Adds a new escape handler to the stack of escape handlers.
        /// Escape handlers are executed to handle user input for cancellation or back actions.
        /// </summary>
        /// <param name="handler">The action to be invoked when an escape event is triggered. Must not be null.</param>
        public void PushEscapeHandler(Action handler)
        {
            if (handler == null)
            {
                GameLogger.Log(LogLevel.Warning, "Attempted to push null escape handler");

                return;
            }

            _escapeHandlers.Push(handler);

            GameLogger.Log(LogLevel.Debug, $"Pushed escape handler. Stack depth: {_escapeHandlers.Count}");
        }

        /// <summary>
        /// Removes and returns the top escape handler from the stack of escape handlers.
        /// If the stack is empty, a warning is logged, and null is returned.
        /// </summary>
        /// <returns>The action that was removed from the stack, or null if the stack is empty.</returns>
        public Action PopEscapeHandler()
        {
            if (_escapeHandlers.Count == 0)
            {
                GameLogger.Log(LogLevel.Warning, "Attempted to pop escape handler from empty stack");
                
                return null;
            }

            var handler = _escapeHandlers.Pop();
            
            GameLogger.Log(LogLevel.Debug, $"Popped escape handler. Stack depth: {_escapeHandlers.Count}");
            
            return handler;
        }

        /// <summary>
        /// Clears all escape handlers from the stack of escape handlers.
        /// Any currently registered escape handlers will be removed and no longer invoked when an escape event is triggered.
        /// </summary>
        public void ClearEscapeHandlers()
        {
            int count = _escapeHandlers.Count;
            
            _escapeHandlers.Clear();
            
            GameLogger.Log(LogLevel.Debug, $"Cleared {count} escape handlers from stack");
        }
        
        #endregion
        
        #region Cursor Requests

        public void RequestCursor(string reason)
        {
            _cursorRequests.Add(reason);
            
            UpdateCursorState();
        }

        public void ReleaseCursor(string reason)
        {
            _cursorRequests.Remove(reason);
            
            UpdateCursorState();
        }
        
        private void UpdateCursorState()
        {
            bool shouldShow = _cursorRequests.Count > 0;
            Cursor.visible = shouldShow;
            Cursor.lockState = shouldShow ? CursorLockMode.None : CursorLockMode.Locked;
        }
        
        #endregion
        
        public void OnLateUpdate(float deltaTime)
        {
            if (!IsEnabled)
                return;

            CancelPressed = false;
            
            if (Mouse.current.leftButton.wasPressedThisFrame && _cursorRequests.Count == 0 && Cursor.lockState != CursorLockMode.Locked)
                UpdateCursorState();
        }

        /// <summary>
        /// Executes the cancellation logic when a cancel action is performed.
        /// This includes setting the cancellation flag and invoking the most recent escape handler, if available.
        /// </summary>
        private void OnCancelPerformed(InputAction.CallbackContext context)
        {
            if (!IsEnabled) 
                return;

            CancelPressed = true;

            if (_escapeHandlers.Count > 0)
            {
                var handler = _escapeHandlers.Pop();
                
                GameLogger.Log(LogLevel.Debug, $"Invoking escape handler. Remaining stack depth: {_escapeHandlers.Count}");
                
                handler?.Invoke();
            }
        }

        private void RegisterUIMenuEvents()
        {
            EventBus.Subscribe<MenuOpenEvent>(eventData =>
            {
                PushEscapeHandler(eventData.Menu.OnBackPressed);
                RequestCursor(eventData.Menu.GetType().Name);
            });
            
            EventBus.Subscribe<MenuCloseEvent>(eventData =>
            {
                if (_escapeHandlers.TryPeek(out var handler) && handler == eventData.Menu.OnBackPressed)
                {
                    _escapeHandlers.Pop();
                }
                
                ReleaseCursor(eventData.Menu.GetType().Name);
            });
        }
    }
}