using Core.Game.Movement.Input;
using UnityEngine;

namespace Core.Game.Movement.Data
{
    public class MovementData
    {
        #region References
        
        public CharacterController Controller;
        public Transform Transform;
        public IMovementInputProvider InputProvider;
        public MovementConfig Config;
        public MovementStates States;
        
        #endregion

        #region Velocity Components
        
        public Vector3 MoveVelocity;
        public Vector3 GravityVelocity;
        public Vector3 GravityDirection = Vector3.down;

        public Quaternion TargetRotation;
        public float RotationSpeedMultiplier = 1f;
        
        #endregion

        #region State Data

        public float CurrentSpeed;
        public bool IsGrounded;
        public bool WasGroundedLastFrame;
        public float TimeSinceGrounded;
        public float TimeInAir;
        public float DistanceToGround;
        public bool IsNearGround;

        #endregion

        #region Computed Properties
        
        public Vector3 TotalVelocity => MoveVelocity + GravityVelocity;
        public float HorizontalSpeed => MoveVelocity.magnitude;
        public float VerticalSpeed => Vector3.Dot(GravityVelocity, -GravityDirection);
        public bool IsFalling => Vector3.Dot(GravityVelocity, GravityDirection) > 0.1f;
        public bool IsRising => Vector3.Dot(GravityVelocity, GravityDirection) < -0.1f;
        public Vector3 UpDirection => -GravityDirection;
        public bool HasMovementInput => InputProvider != null && InputProvider.IsInputEnabled() && InputProvider.GetMovementInput().sqrMagnitude > Config.InputThreshold * Config.InputThreshold;
        public bool IsSprintRequested => InputProvider != null && InputProvider.IsInputEnabled() && InputProvider.IsSprintRequested();
        public bool IsJumpPressed => InputProvider != null && InputProvider.IsInputEnabled() && InputProvider.IsJumpPressed();
        public bool CanCoyoteJump => TimeSinceGrounded <= Config.CoyoteTime;
        
        #endregion
        
        #region Jump Buffer
        
        public float LastJumpPressTime { get; set; } = float.MinValue;
        public bool HasBufferedJump => (Time.time - LastJumpPressTime) <= Config.JumpBufferTime;

        /// <summary>
        /// Records the current time as the last jump press time to enable jump buffering.
        /// This allows the system to register a jump input even if the jump conditions are not immediately met,
        /// as long as they occur within the configured jump buffer time window.
        /// </summary>
        public void BufferJump()
        {
            LastJumpPressTime = Time.time;
        }

        /// <summary>
        /// Clears the jump buffer by resetting the last jump press time to its initial value.
        /// This effectively cancels any stored jump input, ensuring that the system no longer considers a buffered jump request.
        /// </summary>
        public void ConsumeJumpBuffer()
        {
            LastJumpPressTime = float.MinValue;
        }
        
        #endregion

        #region Velocity Methods

        /// <summary>
        /// Sets the gravity velocity for the character, affecting its vertical movement.
        /// This method directly assigns a vector value to the gravity velocity,
        /// determining the character’s downward or upward movement rate based on gravity.
        /// </summary>
        /// <param name="velocity">The vector to set as the gravity velocity.</param>
        public void SetGravityVelocity(Vector3 velocity)
        {
            GravityVelocity = velocity;
        }

        /// <summary>
        /// Adds a gravity impulse to the current gravity velocity, modifying the vertical movement of the entity.
        /// This can be used to simulate external forces such as explosive effects or sudden changes in gravity.
        /// </summary>
        /// <param name="impulse">The vector representing the gravity impulse to be added.</param>
        public void AddGravityImpulse(Vector3 impulse)
        {
            GravityVelocity += impulse;
        }

        /// <summary>
        /// Resets the current gravity-induced velocity to zero.
        /// This stops any ongoing effects of gravity on the character, which can be useful
        /// when transitioning to grounded states or resetting physics-related calculations.
        /// </summary>
        public void ResetGravityVelocity()
        {
            GravityVelocity = Vector3.zero;
        }

        /// <summary>
        /// Applies a small downward force to the gravity velocity to ensure the character remains in contact with the ground when grounded.
        /// This force helps to stabilize ground interactions and minimize scenarios where slight movements could unintentionally lift the character off the ground.
        /// </summary>
        public void ApplyGroundingForce()
        {
            GravityVelocity = GravityDirection * Config.GroundingForce;
        }

        /// <summary>
        /// Resets both the movement velocity and the gravity velocity to zero.
        /// This halts all current movement and gravitational effects applied to the character.
        /// </summary>
        public void ResetAllVelocity()
        {
            MoveVelocity = Vector3.zero;
            GravityVelocity = Vector3.zero;
        }
        
        #endregion

        #region Input Helpers

        /// <summary>
        /// Calculates and returns the world-space movement direction based on the input provided by the input system.
        /// This method considers the gravity direction for projecting the movement input onto a plane
        /// and ensures movement is normalized for consistent control.
        /// </summary>
        /// <returns>
        /// A normalized Vector3 representing the movement direction in world space, or zero if input is disabled
        /// or falls below the configured input threshold.
        /// </returns>
        public Vector3 GetWorldMoveDirection()
        {
            if (InputProvider == null || !InputProvider.IsInputEnabled())
                return Vector3.zero;
            
            Vector2 input = InputProvider.GetMovementInput();
            
            if (input.sqrMagnitude < Config.InputThreshold * Config.InputThreshold)
                return Vector3.zero;
            
            Vector3 forward = Vector3.ProjectOnPlane(Vector3.forward, GravityDirection).normalized;
            Vector3 right = Vector3.ProjectOnPlane(Vector3.right, GravityDirection).normalized;
            
            if (forward.sqrMagnitude < 0.01f)
                forward = Vector3.ProjectOnPlane(Vector3.up, GravityDirection).normalized;
            
            if (right.sqrMagnitude < 0.01f)
                right = Vector3.Cross(UpDirection, forward).normalized;
            
            return (forward * input.y + right * input.x).normalized;
        }

        #endregion

        #region Ground Detection

        /// <summary>
        /// Performs an enhanced ground check using raycasting to detect ground below the character.
        /// This enables ground snapping for stair descent and better ground detection.
        /// </summary>
        /// <param name="maxDistance">Maximum distance to check for ground below</param>
        /// <returns>True if ground was detected within the specified distance</returns>
        public bool CheckGroundBelow(float maxDistance)
        {
            Vector3 origin = Transform.position;
            float checkDistance = maxDistance + Controller.skinWidth;

            if (Physics.Raycast(origin, GravityDirection, out RaycastHit hit, checkDistance, Config.GroundLayers, QueryTriggerInteraction.Ignore))
            {
                DistanceToGround = hit.distance - Controller.skinWidth;
                IsNearGround = DistanceToGround <= maxDistance;
                return IsNearGround;
            }

            DistanceToGround = maxDistance;
            IsNearGround = false;
            return false;
        }

        #endregion
    }
}