using Core.Game.Movement.Data;
using UnityEngine;

namespace Core.Game.Movement.StateMachine.States
{
    public class AirborneState : MovementStateBase
    {
        public AirborneState() : base(null)
        {
        }
        
        public override void Update(MovementData data, float deltaTime)
        {
            base.Update(data, deltaTime);
            
            if (data.IsJumpPressed)
                data.BufferJump();
            
            ApplyAirControl(data, deltaTime);
            
            if (data.MoveVelocity.sqrMagnitude > 0.01f)
                RotateInAir(data, deltaTime);
        }
        
        public override IMovementState CheckTransitions(MovementData data)
        {
            if (data.CanCoyoteJump && data.IsJumpPressed)
                return data.States.Get<JumpState>();
            
            return null;
        }

        /// <summary>
        /// Adjusts the character's velocity while airborne based on input and movement configuration.
        /// </summary>
        /// <param name="data">The movement data containing the character's current state and configuration.</param>
        /// <param name="deltaTime">The time elapsed since the last update, used to calculate velocity adjustments.</param>
        private void ApplyAirControl(MovementData data, float deltaTime)
        {
            Vector3 inputDirection = data.GetWorldMoveDirection();
            
            if (inputDirection.sqrMagnitude < 0.01f)
            {
                data.MoveVelocity = Vector3.Lerp(data.MoveVelocity, Vector3.zero, 0.02f * deltaTime);
                
                return;
            }
            
            float maxAirSpeed = Mathf.Max(data.HorizontalSpeed, data.Config.WalkSpeed);
            Vector3 targetVelocity = inputDirection * maxAirSpeed;
            
            data.MoveVelocity = Vector3.Lerp(data.MoveVelocity, targetVelocity, data.Config.AirControlMultiplier * data.Config.AirAcceleration * deltaTime);
        }

        /// <summary>
        /// Gradually rotates the character in the air toward the direction of its movement velocity.
        /// </summary>
        /// <param name="data">The movement data containing the character's current state, configuration, and velocity information.</param>
        /// <param name="deltaTime">The time elapsed since the last update, used to calculate the rotation speed.</param>
        private void RotateInAir(MovementData data, float deltaTime)
        {
            float airRotationSpeed = data.Config.RotationSpeed * 0.5f;
            Vector3 moveDirection = data.MoveVelocity.normalized;
            
            if (moveDirection.sqrMagnitude < 0.01f)
                return;
            
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection, data.UpDirection);
            
            data.Transform.rotation = Quaternion.Slerp(data.Transform.rotation, targetRotation, airRotationSpeed * deltaTime);
        }
    }
}