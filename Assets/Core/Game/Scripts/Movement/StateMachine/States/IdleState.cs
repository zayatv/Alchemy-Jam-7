using Core.Game.Movement.Data;
using UnityEngine;

namespace Core.Game.Movement.StateMachine.States
{
    public class IdleState : MovementStateBase
    {
        public IdleState(IMovementState parentState) : base(parentState)
        {
        }
        
        public override void Update(MovementData data, float deltaTime)
        {
            base.Update(data, deltaTime);
            
            data.MoveVelocity = Vector3.MoveTowards(data.MoveVelocity, Vector3.zero, data.Config.Deceleration * deltaTime);
        }
        
        public override IMovementState CheckTransitions(MovementData data)
        {
            if (!data.IsGrounded)
                return data.States.Get<FallState>();
            
            if (data.HasMovementInput)
            {
                if (data.IsSprintRequested)
                    return data.States.Get<SprintState>();
                
                return data.States.Get<WalkState>();
            }
            
            return null;
        }
    }
}