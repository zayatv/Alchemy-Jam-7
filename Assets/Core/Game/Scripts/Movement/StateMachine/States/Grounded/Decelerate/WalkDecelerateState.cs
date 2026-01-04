using Core.Game.Movement.Data;
using Core.Game.Movement.StateMachine.States.Grounded.Accelerate;
using UnityEngine;

namespace Core.Game.Movement.StateMachine.States.Grounded.Decelerate
{
    public class WalkDecelerateState : DecelerateState
    {
        protected override Vector3 GetTargetVelocity(MovementData data) => Vector3.zero;
        
        public WalkDecelerateState(IMovementState parentState) : base(parentState)
        {
        }
        
        public override IMovementState CheckTransitions(MovementData data)
        {
            var baseTransition = base.CheckTransitions(data);
            
            if (baseTransition != null) 
                return baseTransition;
            
            if (data.HasMovementInput)
            {
                if (data.IsSprintRequested)
                    return data.States.Get<SprintAccelerateState>();
                
                return data.States.Get<WalkAccelerateState>();
            }
            
            if (HasStopped(data))
                return data.States.Get<IdleState>();
            
            return null;
        }
    }
}