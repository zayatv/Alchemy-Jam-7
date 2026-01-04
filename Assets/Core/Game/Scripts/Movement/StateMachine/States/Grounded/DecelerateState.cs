using Core.Game.Movement.Data;
using Core.Game.Movement.StateMachine.States.Airborne;
using UnityEngine;

namespace Core.Game.Movement.StateMachine.States.Grounded
{
    public class DecelerateState : MovementStateBase
    {
        protected virtual Vector3 GetTargetVelocity(MovementData data) => Vector3.zero;

        public DecelerateState(IMovementState parentState) : base(parentState)
        {
        }
        
        public override void Update(MovementData data, float deltaTime)
        {
            base.Update(data, deltaTime);
            
            Vector3 targetVelocity = GetTargetVelocity(data);
            
            data.MoveVelocity = Vector3.Lerp(data.MoveVelocity, targetVelocity, 1f - Mathf.Exp(-data.Config.Deceleration * deltaTime));
        }
        
        public override IMovementState CheckTransitions(MovementData data)
        {
            if (!data.IsGrounded)
                return data.States.Get<FallState>();
            
            return null;
        }
        
        protected bool HasStopped(MovementData data)
        {
            return data.HorizontalSpeed <= data.Config.MinMoveSpeed;
        }
        
        protected bool HasReachedWalkSpeed(MovementData data)
        {
            return data.HorizontalSpeed <= data.Config.WalkSpeed * 1.05f;
        }
    }
}