using Core.Systems.Events;
using Core.Systems.Input;
using Core.Systems.Update;
using UnityEngine.InputSystem;

namespace Core.Game.Input.Contexts
{
    /// <summary>
    /// Input context for detecting bomb placement input.
    /// Only handles input detection - raises events for other systems to handle the logic.
    /// </summary>
    public class BombInputContext : IInputContext, ILateUpdatable
    {
        private readonly PlayerInputActions.GameplayActions _gameplayActions;

        private bool _isEnabled;

        /// <summary>
        /// True if the bomb button was pressed this frame.
        /// </summary>
        public bool BombPressed { get; private set; }

        public bool IsEnabled => _isEnabled;
        public int LateUpdatePriority => 100;

        public BombInputContext(PlayerInputActions.GameplayActions gameplayActions)
        {
            _gameplayActions = gameplayActions;
            _gameplayActions.PlaceBomb.performed += OnPlaceBombPerformed;
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

            BombPressed = false;
            _isEnabled = false;
        }

        public void OnLateUpdate(float deltaTime)
        {
            if (!_isEnabled)
                return;

            // Reset at end of frame
            BombPressed = false;
        }

        private void OnPlaceBombPerformed(InputAction.CallbackContext ctx)
        {
            if (!_isEnabled)
                return;

            BombPressed = true;

            // Raise event for bomb systems to handle
            EventBus.Raise(new BombInputEvent { BombPressed = true });
        }
    }

    /// <summary>
    /// Event raised when bomb input is detected.
    /// </summary>
    public struct BombInputEvent : IEvent
    {
        public bool BombPressed;
    }
}
