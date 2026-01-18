using Core.Game.Movement.Data;
using Core.Game.Movement.StateMachine.States.Airborne;
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
            
            if (data.MoveVelocity.sqrMagnitude > 0.01f)
                UpdateRotationTarget(data);
        }
        
        public override IMovementState CheckTransitions(MovementData data)
        {
            return null;
        }

        /// <summary>
        /// Updates the target rotation based on the movement direction.
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
            data.RotationSpeedMultiplier = 1f;
        }
    }
}