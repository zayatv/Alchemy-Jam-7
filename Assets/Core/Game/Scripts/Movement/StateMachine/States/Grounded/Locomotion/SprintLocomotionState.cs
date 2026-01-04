using Core.Game.Movement.Data;
using Core.Game.Movement.StateMachine.States.Grounded.Decelerate;

namespace Core.Game.Movement.StateMachine.States.Grounded.Locomotion
{
    public class SprintLocomotionState : LocomotionState
    {
        protected override float GetTargetSpeed(MovementData data) => data.Config.RunSpeed;
        
        public SprintLocomotionState(IMovementState parentState) : base(parentState)
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
                return data.States.Get<SprintDecelerateState>();
            
            return null;
        }
    }
}