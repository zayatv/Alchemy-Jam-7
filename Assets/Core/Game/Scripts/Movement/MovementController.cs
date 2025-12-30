using Core.Game.Movement.Input;
using Core.Game.Movement.Data;
using Core.Game.Movement.StateMachine;
using Core.Game.Movement.StateMachine.States;
using Core.Scripts.Systems.Logging;
using Core.Systems.ServiceLocator;
using Core.Systems.Update;
using UnityEngine;

namespace Core.Game.Movement.Movement
{
    [RequireComponent(typeof(CharacterController))]
    public class MovementController : MonoBehaviour, IFixedUpdatable, IUpdatable
    {
        #region Serialized Fields
        
        [Header("Configuration")]
        [SerializeField] private MovementConfig movementConfig;
        
        [Header("Debug")]
        [SerializeField] private bool showDebugInfo;
        
        #endregion
        
        #region Fields
        
        private CharacterController _characterController;
        private IUpdateService _updateService;
        private IMovementInputProvider _movementInputProvider;
        
        private IMovementStateMachine _stateMachine;
        private MovementData _movementData;
        
        #endregion
        
        #region Properties
        
        public int FixedUpdatePriority => 0;
        public int UpdatePriority => 0;
        
        public MovementData Data => _movementData;
        public IMovementStateMachine StateMachine => _stateMachine;
        
        #endregion
        
        #region Unity Lifecycle

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
            
            InitializeMovementData();
            InitializeStates();
            InitializeStateMachine();
        }

        private void Start()
        {
            RegisterWithUpdateService();
        }

        private void OnDestroy()
        {
            UnregisterFromUpdateService();
        }

        #endregion
        
        #region Initialization

        /// <summary>
        /// Initializes the movement data for the movement controller.
        /// Sets up the <see cref="MovementData"/> with references to the character controller,
        /// transform, input provider, configuration, and establishes the default gravity direction.
        /// This method ensures that the movement system operates with the necessary
        /// dependencies and initial settings.
        /// </summary>
        private void InitializeMovementData()
        {
            _movementData = new MovementData
            {
                Controller = _characterController,
                Transform = transform,
                InputProvider = _movementInputProvider,
                Config = movementConfig,
                GravityDirection = Vector3.down
            };
        }

        /// <summary>
        /// Initializes the movement states and registers them within the <see cref="MovementData.States"/> collection.
        /// This method ensures that all movement states, including grounded states (e.g., idle, walking, sprinting)
        /// and airborne states (e.g., falling, jumping), are correctly set up and linked to their respective parent states.
        /// </summary>
        private void InitializeStates()
        {
            var groundedState = new GroundedState();
            var airborneState = new AirborneState();
            
            _movementData.States.Register(groundedState);
            _movementData.States.Register(airborneState);
            
            _movementData.States.Register(new IdleState(groundedState));
            _movementData.States.Register(new WalkState(groundedState));
            _movementData.States.Register(new SprintState(groundedState));
            _movementData.States.Register(new FallState(airborneState));
            _movementData.States.Register(new JumpState(airborneState));
        }

        /// <summary>
        /// Initializes the movement state machine for the movement controller.
        /// Creates a new state machine instance and sets its initial state using the provided
        /// <see cref="MovementData"/> and the default <see cref="IdleState"/>.
        /// Logs the successfully initialized state hierarchy for debugging purposes.
        /// </summary>
        private void InitializeStateMachine()
        {
            _stateMachine = new MovementStateMachine(_movementData, _movementData.States.Get<IdleState>());
            
            GameLogger.Log(LogLevel.Debug, $"MovementController initialized with state: {_stateMachine.GetStateHierarchyString()}");
        }
        
        #endregion
        
        #region Interface Implementations
        
        public void OnUpdate(float deltaTime)
        {
            UpdateGroundedState();
            UpdateTimers(deltaTime);
            
            _stateMachine.OnUpdate(deltaTime);
            
            ApplyGravity(deltaTime);
            ApplyMovement(deltaTime);
            
            _movementData.WasGroundedLastFrame = _movementData.IsGrounded;
            
            if (showDebugInfo)
                LogDebugInfo();
        }
        
        public void OnFixedUpdate(float fixedDeltaTime)
        {
            _stateMachine.OnFixedUpdate(fixedDeltaTime);
        }
        
        #endregion
        
        #region Physics

        /// <summary>
        /// Updates the grounded state of the character.
        /// Determines whether the character is currently grounded by
        /// querying the <see cref="CharacterController"/> and updates the
        /// <see cref="MovementData.IsGrounded"/> property accordingly.
        /// This method ensures that the movement system has accurate information
        /// about the character's contact with the ground, which can affect
        /// movement behavior and physics interactions.
        /// </summary>
        private void UpdateGroundedState()
        {
            _movementData.IsGrounded = _characterController.isGrounded;
        }

        /// <summary>
        /// Updates the timers related to the grounded and airborne states of the character.
        /// Resets or increments the time counters for grounded and air states based on
        /// the current grounded status provided by the <see cref="MovementData"/>.
        /// </summary>
        /// <param name="deltaTime">The time elapsed since the last update, used to increment timers.</param>
        private void UpdateTimers(float deltaTime)
        {
            if (_movementData.IsGrounded)
            {
                _movementData.TimeSinceGrounded = 0f;
                _movementData.TimeInAir = 0f;
            }
            else
            {
                _movementData.TimeSinceGrounded += deltaTime;
                _movementData.TimeInAir += deltaTime;
            }
        }

        /// <summary>
        /// Applies gravitational force to the character based on their current state and direction.
        /// Adjusts the gravity velocity of the character while respecting configured gravity settings
        /// such as strength, direction, and terminal velocity.
        /// This method ensures that gravity is correctly applied during falling, rising, or grounded states.
        /// </summary>
        /// <param name="deltaTime">The time elapsed since the last frame, used for calculating gravity adjustments.</param>
        private void ApplyGravity(float deltaTime)
        {
            if (_movementData.IsGrounded && !_movementData.IsRising)
            {
                _movementData.ApplyGroundingForce();
            }
            else
            {
                float gravityMultiplier = 1f;
                
                if (_movementData.IsFalling)
                    gravityMultiplier = movementConfig.FallGravityMultiplier;
                
                Vector3 gravityDelta = _movementData.GravityDirection * movementConfig.GravityStrength * gravityMultiplier * deltaTime;
                
                _movementData.GravityVelocity += gravityDelta;
                
                float currentGravitySpeed = _movementData.GravityVelocity.magnitude;
                
                if (currentGravitySpeed > movementConfig.TerminalVelocity)
                    _movementData.GravityVelocity = _movementData.GravityVelocity.normalized * movementConfig.TerminalVelocity;
            }
        }

        /// <summary>
        /// Applies the calculated movement to the character controller.
        /// Calculates the displacement based on the total velocity and delta time,
        /// moves the character accordingly, and updates the current speed in the movement data.
        /// </summary>
        /// <param name="deltaTime">The time elapsed since the last update, typically used
        /// to ensure frame rate-independent movement calculations.</param>
        private void ApplyMovement(float deltaTime)
        {
            Vector3 displacement = _movementData.TotalVelocity * deltaTime;
            
            _characterController.Move(displacement);
            
            _movementData.CurrentSpeed = _movementData.HorizontalSpeed;
        }
        
        #endregion
        
        #region Public Methods

        /// <summary>
        /// Sets the movement input provider for the movement controller.
        /// Assigns the specified <paramref name="inputProvider"/> to handle input-related functionality.
        /// Updates the current <see cref="MovementData"/> instance, if available, to reference the new input provider.
        /// </summary>
        /// <param name="inputProvider">
        /// The input provider to be used for processing movement input.
        /// </param>
        public void SetInputProvider(IMovementInputProvider inputProvider)
        {
            _movementInputProvider = inputProvider;
            
            if (_movementData != null)
                _movementData.InputProvider = inputProvider;
        }

        /// <summary>
        /// Sets the gravity direction for the movement controller.
        /// Updates the <see cref="MovementData"/> to apply a new gravity vector, normalized
        /// to ensure consistent behavior. This determines the overall direction in which
        /// gravity force is applied during movement.
        /// </summary>
        /// <param name="direction">
        /// The vector representing the new gravity direction.
        /// This parameter should specify a valid direction (non-zero vector) to be normalized.
        /// </param>
        public void SetGravityDirection(Vector3 direction)
        {
            _movementData.GravityDirection = direction.normalized;
        }

        /// <summary>
        /// Instantly moves the character to the specified world position.
        /// Disables the character controller during the teleportation to avoid conflicts,
        /// updates the transform's position to the new location, re-enables the character controller,
        /// and resets all velocity data in <see cref="MovementData"/>.
        /// </summary>
        /// <param name="position">
        /// The target position to teleport the character to, specified in world coordinates.
        /// </param>
        public void Teleport(Vector3 position)
        {
            _characterController.enabled = false;
            transform.position = position;
            _characterController.enabled = true;
            
            _movementData.ResetAllVelocity();
        }

        /// <summary>
        /// Forces a transition to the specified movement state.
        /// This method bypasses the state machine's normal transition logic
        /// and immediately switches to the given state, ensuring the current
        /// movement behavior aligns with the provided state.
        /// </summary>
        /// <param name="state">The target movement state to transition to. This state
        /// will immediately replace the current state in the state machine.</param>
        public void ForceState(IMovementState state)
        {
            _stateMachine.ChangeState(state);
        }
        
        #endregion
        
        #region Service Registration
        
        private void RegisterWithUpdateService()
        {
            if (ServiceLocator.TryGet(out _updateService))
            {
                _updateService.Register(this);
                
                GameLogger.Log(LogLevel.Debug, $"Registered {GetType().Name} with UpdateService");
            }
            else
            {
                GameLogger.Log(LogLevel.Error, $"Failed to register {GetType().Name} with UpdateService");
            }
        }
        
        private void UnregisterFromUpdateService()
        {
            _updateService?.Unregister(this);
        }
        
        #endregion
        
        #region Debug
        
        private void LogDebugInfo()
        {
            GameLogger.Log(LogLevel.Debug, 
                $"[Movement] State: {_stateMachine.GetStateHierarchyString()} | " + 
                $"Grounded: {_movementData.IsGrounded} | " +
                $"Speed: {_movementData.CurrentSpeed:F2} | " +
                $"GravVel: {_movementData.GravityVelocity.magnitude:F2}");
        }
        
        private void OnDrawGizmosSelected()
        {
            if (_movementData == null) return;
            
            // Draw gravity direction
            Gizmos.color = Color.magenta;
            Gizmos.DrawRay(transform.position, _movementData.GravityDirection * 2f);
            
            // Draw move velocity
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, _movementData.MoveVelocity);
            
            // Draw total velocity
            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(transform.position, _movementData.TotalVelocity * 0.5f);
        }
        
        #endregion
    }
}