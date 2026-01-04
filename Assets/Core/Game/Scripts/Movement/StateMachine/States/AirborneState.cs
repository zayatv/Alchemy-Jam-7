using Core.Game.Movement.Data;
using Core.Game.Movement.StateMachine.States.Airborne;
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
                UpdateRotationTarget(data);
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
        /// Updates the target rotation based on the movement velocity when airborne.
        /// </summary>
        /// <param name="data">The movement data.</param>
        private void UpdateRotationTarget(MovementData data)
        {
            if (!data.HasMovementInput)
                return;
            
            Vector3 moveDirection = data.MoveVelocity.normalized;
            
            if (moveDirection.sqrMagnitude < 0.01f)
                return;
            
            data.TargetRotation = Quaternion.LookRotation(moveDirection, data.UpDirection);
            data.RotationSpeedMultiplier = 0.5f;
        }
    }
}