using UnityEngine;

namespace Core.Game.Movement.Data
{
    [CreateAssetMenu(fileName = "MovementConfig", menuName = "Core/Game/Movement/Movement Config")]
    public class MovementConfig : ScriptableObject
    {
        #region Movement Speeds
        
        [Header("Movement Speeds")]
        [Tooltip("Walking speed in units per second")]
        [SerializeField] private float walkSpeed = 3.0f;

        [Tooltip("Running/sprinting speed in units per second")]
        [SerializeField] private float runSpeed = 6.0f;

        #endregion

        #region Acceleration
        
        [Header("Acceleration")]
        [Tooltip("How quickly the character accelerates to target speed")]
        [SerializeField] private float acceleration = 10.0f;

        [Tooltip("How quickly the character decelerates to a stop")]
        [SerializeField] private float deceleration = 15.0f;
        
        [Tooltip("Air control multiplier (0 = no air control, 1 = full control)")]
        [SerializeField, Range(0f, 1f)] private float airControlMultiplier = 0.3f;
        
        [Tooltip("How quickly air control velocity changes")]
        [SerializeField] private float airAcceleration = 5.0f;

        #endregion

        #region Rotation
        
        [Header("Rotation")]
        [Tooltip("How quickly the character rotates to face movement direction")]
        [SerializeField] private float rotationSpeed = 10.0f;

        #endregion

        #region Gravity & Physics
        
        [Header("Gravity")]
        [Tooltip("Gravity strength (positive value, direction is separate)")]
        [SerializeField] private float gravityStrength = 20.0f;
        
        [Tooltip("Maximum fall speed")]
        [SerializeField] private float terminalVelocity = 50.0f;
        
        [Tooltip("Small downward force applied when grounded to maintain ground contact")]
        [SerializeField] private float groundingForce = 2.0f;
        
        [Tooltip("Gravity multiplier when falling (for snappier falls)")]
        [SerializeField] private float fallGravityMultiplier = 1.5f;

        #endregion

        #region Jump
        
        [Header("Jump")]
        [Tooltip("Initial jump velocity")]
        [SerializeField] private float jumpForce = 10.0f;
        
        [Tooltip("Time after leaving ground where jump is still allowed (coyote time)")]
        [SerializeField] private float coyoteTime = 0.15f;
        
        [Tooltip("Time before landing where jump input is buffered")]
        [SerializeField] private float jumpBufferTime = 0.1f;

        #endregion

        #region Ground Detection
        
        [Header("Ground Detection")]
        [Tooltip("Ground check distance")]
        [SerializeField] private float groundCheckDistance = 0.1f;

        #endregion

        #region Input
        
        [Header("Input")]
        [Tooltip("Input magnitude threshold to consider as movement")]
        [SerializeField] private float inputThreshold = 0.01f;

        #endregion

        #region Properties

        // Movement
        public float WalkSpeed => walkSpeed;
        public float RunSpeed => runSpeed;
        
        // Acceleration
        public float Acceleration => acceleration;
        public float Deceleration => deceleration;
        public float AirControlMultiplier => airControlMultiplier;
        public float AirAcceleration => airAcceleration;
        
        // Rotation
        public float RotationSpeed => rotationSpeed;
        
        // Gravity
        public float GravityStrength => gravityStrength;
        public float TerminalVelocity => terminalVelocity;
        public float GroundingForce => groundingForce;
        public float FallGravityMultiplier => fallGravityMultiplier;
        
        // Jump
        public float JumpForce => jumpForce;
        public float CoyoteTime => coyoteTime;
        public float JumpBufferTime => jumpBufferTime;
        
        // Ground Detection
        public float GroundCheckDistance => groundCheckDistance;
        
        // Input
        public float InputThreshold => inputThreshold;

        #endregion
    }
}