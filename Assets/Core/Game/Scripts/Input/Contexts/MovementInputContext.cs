using Core.Systems.Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Core.Game.Input.Contexts
{
    public class MovementInputContext : IInputContext
    {
        private readonly PlayerInputActions.GameplayActions _gameplayActions;
        
        private bool _isEnabled;
        
        public Vector2 Movement { get; private set; }
        public bool IsSprinting { get; private set; }
        public bool IsEnabled => _isEnabled;
        
        public MovementInputContext(PlayerInputActions.GameplayActions gameplayActions)
        {
            _gameplayActions = gameplayActions;
            
            _gameplayActions.Move.performed += OnMovePerformed;
            _gameplayActions.Move.canceled += OnMoveCanceled;
            _gameplayActions.Sprint.performed += ctx => { if (IsEnabled) IsSprinting = true; };
            _gameplayActions.Sprint.canceled += ctx => { if (IsEnabled) IsSprinting = false; };
        }

        public void Enable()
        {
            if (_isEnabled)
                return;
            
            _isEnabled = true;
        }

        public void Disable()
        {
            if (!_isEnabled)
                return;
            
            Movement = Vector2.zero;
            IsSprinting = false;
            _isEnabled = false;
        }
        
        private void OnMovePerformed(InputAction.CallbackContext context)
        {
            if (!IsEnabled) 
                return;
            
            Movement = context.ReadValue<Vector2>();
        }

        private void OnMoveCanceled(InputAction.CallbackContext context)
        {
            if (!IsEnabled) 
                return;
            
            Movement = Vector2.zero;
        }
    }
}