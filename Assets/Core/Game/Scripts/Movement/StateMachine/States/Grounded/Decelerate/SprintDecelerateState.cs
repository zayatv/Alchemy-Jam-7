using Core.Game.Movement.Data;
using Core.Game.Movement.StateMachine.States.Grounded.Accelerate;
using Core.Game.Movement.StateMachine.States.Grounded.Locomotion;
using UnityEngine;

namespace Core.Game.Movement.StateMachine.States.Grounded.Decelerate
{
    public class SprintDecelerateState : DecelerateState
    {
        protected override Vector3 GetTargetVelocity(MovementData data)
        {
            if (data.HasMovementInput && !data.IsSprintRequested)
            {
                Vector3 inputDirection = data.GetWorldMoveDirection();
                
                return inputDirection * data.Config.WalkSpeed;
            }
            
            return Vector3.zero;
        }
        
        public SprintDecelerateState(IMovementState parentState) : base(parentState)
        {
        }
        
        public override IMovementState CheckTransitions(MovementData data)
        {
            var baseTransition = base.CheckTransitions(data);
            
            if (baseTransition != null) 
                return baseTransition;
            
            if (data.HasMovementInput && data.IsSprintRequested)
                return data.States.Get<SprintAccelerateState>();
            
            if (data.HasMovementInput && !data.IsSprintRequested && HasReachedWalkSpeed(data))
                return data.States.Get<WalkLocomotionState>();
            
            if (!data.HasMovementInput && HasReachedWalkSpeed(data))
                return data.States.Get<WalkDecelerateState>();
            
            return null;
        }
    }
}