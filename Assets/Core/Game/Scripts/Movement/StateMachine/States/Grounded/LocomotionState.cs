using Core.Game.Movement.Data;
using Core.Game.Movement.StateMachine.States.Airborne;
using UnityEngine;

namespace Core.Game.Movement.StateMachine.States.Grounded
{
    public class LocomotionState : MovementStateBase
    {
        protected virtual float GetTargetSpeed(MovementData data) => 0f;
        
        public LocomotionState(IMovementState parentState) : base(parentState)
        {
        }

        public override void Update(MovementData data, float deltaTime)
        {
            base.Update(data, deltaTime);

            Vector3 inputDirection = data.GetWorldMoveDirection();

            if (inputDirection.sqrMagnitude < 0.01f)
                return;

            Vector3 targetVelocity = inputDirection * GetTargetSpeed(data);

            data.MoveVelocity = Vector3.Lerp(data.MoveVelocity, targetVelocity, 1f - Mathf.Exp(-data.Config.Acceleration * deltaTime));
        }

        public override IMovementState CheckTransitions(MovementData data)
        {
            if (!data.IsGrounded)
                return data.States.Get<FallState>();
            
            return null;
        }
    }
}