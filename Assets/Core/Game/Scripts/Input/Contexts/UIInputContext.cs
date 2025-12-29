using System;
using System.Collections.Generic;
using Core.Scripts.Systems.Logging;
using Core.Systems.Input;
using Core.Systems.Update;
using UnityEngine.InputSystem;

namespace Core.Game.Input.Contexts
{
    public class UIInputContext : IInputContext, ILateUpdatable
    {
        private readonly PlayerInputActions.UIActions _uiActions;
        private readonly Stack<Action> _escapeHandlers = new Stack<Action>();
        
        private bool _isEnabled;
        
        public bool CancelPressed { get; private set; }
        public bool HasEscapeHandlers => _escapeHandlers.Count > 0;
        public bool IsEnabled => _isEnabled;
        public int LateUpdatePriority => 100;
        
        public UIInputContext(PlayerInputActions.UIActions uiActions)
        {
            _uiActions = uiActions;
            
            _uiActions.Cancel.performed += OnCancelPerformed;
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
        
        public void OnLateUpdate(float deltaTime)
        {
            if (!IsEnabled)
                return;

            CancelPressed = false;
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
    }
}