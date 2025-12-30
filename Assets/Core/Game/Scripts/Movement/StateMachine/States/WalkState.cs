using Core.Game.Movement.Data;
using UnityEngine;

namespace Core.Game.Movement.StateMachine.States
{
    public class WalkState : MovementStateBase
    {
        public WalkState(IMovementState parentState) : base(parentState)
        {
        }
        
        public override void Update(MovementData data, float deltaTime)
        {
            base.Update(data, deltaTime);
            
            Vector3 inputDirection = data.GetWorldMoveDirection();
            
            if (inputDirection.sqrMagnitude < 0.01f)
            {
                data.MoveVelocity = Vector3.MoveTowards(data.MoveVelocity, Vector3.zero, data.Config.Deceleration * deltaTime);
            }
            else
            {
                Vector3 targetVelocity = inputDirection * data.Config.WalkSpeed;
                
                data.MoveVelocity = Vector3.MoveTowards(data.MoveVelocity, targetVelocity, data.Config.Acceleration * deltaTime);
            }
        }
        
        public override IMovementState CheckTransitions(MovementData data)
        {
            if (!data.IsGrounded)
                return data.States.Get<FallState>();
            
            if (!data.HasMovementInput)
                return data.States.Get<IdleState>();
            
            if (data.IsSprintRequested)
                return data.States.Get<SprintState>();
            
            return null;
        }
    }
}