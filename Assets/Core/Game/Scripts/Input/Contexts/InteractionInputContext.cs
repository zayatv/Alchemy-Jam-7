using Core.Systems.Events;
using Core.Systems.Input;
using Core.Systems.Update;
using UnityEngine.InputSystem;

namespace Core.Game.Input.Contexts
{
    public class InteractionInputContext : IInputContext, ILateUpdatable
    {
        private readonly PlayerInputActions.GameplayActions _gameplayActions;
        
        private bool _isEnabled;
        
        public bool InteractPressed { get; private set; }
        public bool IsEnabled => _isEnabled;
        public int LateUpdatePriority => 100;
        
        public InteractionInputContext(PlayerInputActions.GameplayActions gameplayActions)
        {
            _gameplayActions = gameplayActions;

            _gameplayActions.Interact.performed += OnInteractPerformed;
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
            
            InteractPressed = false;
            _isEnabled = false;
        }
        
        public void OnLateUpdate(float deltaTime)
        {
            if (!_isEnabled)
                return;

            InteractPressed = false;
        }

        private void OnInteractPerformed(InputAction.CallbackContext obj)
        {
            if (!_isEnabled)
                return;
            
            InteractPressed = true;
            
            EventBus.Raise(new InteractionInputEvent {InteractPressed = true});
        }
    }

    public struct InteractionInputEvent : IEvent
    {
        public bool InteractPressed;
    }
}