using Core.Game.Movement.Data;
using Core.Game.Movement.StateMachine.States.Grounded.Accelerate;
using Core.Game.Movement.StateMachine.States.Grounded.Decelerate;

namespace Core.Game.Movement.StateMachine.States.Grounded.Locomotion
{
    public class WalkLocomotionState : LocomotionState
    {
        protected override float GetTargetSpeed(MovementData data) => data.Config.WalkSpeed;
        
        public WalkLocomotionState(IMovementState parentState) : base(parentState)
        {
        }

        public override IMovementState CheckTransitions(MovementData data)
        {
            var baseTransition = base.CheckTransitions(data);
            
            if (baseTransition != null) 
                return baseTransition;
            
            if (!data.HasMovementInput)
                return data.States.Get<WalkDecelerateState>();
            
            if (data.IsSprintRequested)
                return data.States.Get<SprintAccelerateState>();
            
            return null;
        }
    }
}