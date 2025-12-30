using Core.Game.Movement.Data;
using UnityEngine;

namespace Core.Game.Movement.StateMachine.States
{
    public class GroundedState : MovementStateBase
    {
        public GroundedState() : base(null)
        {
        }
        
        public override void Enter(MovementData data)
        {
            base.Enter(data);
            
            if (!data.WasGroundedLastFrame)
                data.ResetGravityVelocity();
        }
        
        public override void Update(MovementData data, float deltaTime)
        {
            base.Update(data, deltaTime);
            
            if (data.IsJumpPressed)
                data.BufferJump();
            
            if (data.MoveVelocity.sqrMagnitude > 0.01f)
                RotateTowardsMovement(data, deltaTime);
        }
        
        public override IMovementState CheckTransitions(MovementData data)
        {
            if ((data.IsJumpPressed || data.HasBufferedJump) && data.States.TryGet(out JumpState jumpState))
                return jumpState;
            
            return null;
        }

        /// Rotates the character towards the direction of movement based on the velocity provided in the movement data.
        /// Smoothly interpolates the rotation to align with the movement direction over time.
        /// <param name="data">
        /// The movement data containing information such as movement velocity, transform, and configuration settings.
        /// </param>
        /// <param name="deltaTime">
        /// The time elapsed since the last frame, used to control the interpolation based on the rotation speed.
        /// </param>
        private void RotateTowardsMovement(MovementData data, float deltaTime)
        {
            Vector3 moveDirection = data.MoveVelocity.normalized;
            
            if (moveDirection.sqrMagnitude < 0.01f)
                return;
            
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection, data.UpDirection);
            
            data.Transform.rotation = Quaternion.Slerp(data.Transform.rotation, targetRotation, data.Config.RotationSpeed * deltaTime);
        }
    }
}