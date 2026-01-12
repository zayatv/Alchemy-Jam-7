using Core.Game.Movement.Data;
using Core.Game.Movement.StateMachine.States.Airborne;
using Core.Game.Movement.StateMachine.States.Grounded.Accelerate;
using UnityEngine;

namespace Core.Game.Movement.StateMachine.States.Grounded
{
    public class IdleState : MovementStateBase
    {
        public IdleState(IMovementState parentState) : base(parentState)
        {
        }
        
        public override void Update(MovementData data, float deltaTime)
        {
            base.Update(data, deltaTime);
            
            if (data.MoveVelocity.sqrMagnitude > 0.001f)
                data.MoveVelocity = Vector3.MoveTowards(data.MoveVelocity, Vector3.zero, data.Config.Deceleration * deltaTime);
        }
        
        public override IMovementState CheckTransitions(MovementData data)
        {
            if (data.HasMovementInput)
            {
                if (data.IsSprintRequested)
                    return data.States.Get<SprintAccelerateState>();
                
                return data.States.Get<WalkAccelerateState>();
            }
            
            return null;
        }
    }
}