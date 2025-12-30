using Core.Game.Input.Contexts;
using Core.Game.Movement.Movement;
using Core.Systems.Input;
using Core.Systems.ServiceLocator;
using UnityEngine;

namespace Core.Game.Movement.Input
{
    public class PlayerMovementInputProvider : MonoBehaviour, IMovementInputProvider
    {
        #region Fields
        
        [Header("References")]
        [SerializeField] private MovementController movementController;
        
        private IInputService _inputService;
        private MovementInputContext _movementInputContext;
        
        #endregion
        
        #region Unity Lifecycle

        private void Start()
        {
            if (ServiceLocator.TryGet(out _inputService))
                _movementInputContext = _inputService.GetContext<MovementInputContext>();
            
            movementController.SetInputProvider(this);
        }

        #endregion

        #region Movement Input Provder Interface Implementation
        
        public Vector2 GetMovementInput()
        {
            return _movementInputContext?.Movement ?? Vector2.zero;
        }

        public bool IsSprintRequested()
        {
            return _movementInputContext?.IsSprinting ?? false;
        }

        public bool IsJumpPressed()
        {
            return _movementInputContext?.IsJumpPressed ?? false;
        }

        public bool IsInputEnabled()
        {
            return _movementInputContext?.IsEnabled ?? false;
        }

        #endregion
    }
}