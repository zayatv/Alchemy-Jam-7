using Core.Game.Movement.Data;
using Core.Game.Movement.StateMachine.States.Grounded.Decelerate;
using Core.Game.Movement.StateMachine.States.Grounded.Locomotion;

namespace Core.Game.Movement.StateMachine.States.Grounded.Accelerate
{
    public class SprintAccelerateState : AccelerateState
    {
        protected override float GetTargetSpeed(MovementData data) => data.Config.RunSpeed;
        
        public SprintAccelerateState(IMovementState parentState) : base(parentState)
        {
        }
        
        public override IMovementState CheckTransitions(MovementData data)
        {
            var baseTransition = base.CheckTransitions(data);
            
            if (baseTransition != null) 
                return baseTransition;
            
            if (!data.HasMovementInput)
                return data.States.Get<SprintDecelerateState>();
            
            if (!data.IsSprintRequested)
            {
                if (data.HorizontalSpeed > data.Config.WalkSpeed)
                    return data.States.Get<SprintDecelerateState>();
                
                return data.States.Get<WalkAccelerateState>();
            }
            
            if (HasReachedTargetSpeed(data))
                return data.States.Get<SprintLocomotionState>();
            
            return null;
        }
    }
}