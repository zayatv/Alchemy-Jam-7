using Core.Game.Movement.Data;
using Core.Game.Movement.StateMachine.States.Grounded.Decelerate;
using Core.Game.Movement.StateMachine.States.Grounded.Locomotion;

namespace Core.Game.Movement.StateMachine.States.Grounded.Accelerate
{
    public class WalkAccelerateState : AccelerateState
    {
        protected override float GetTargetSpeed(MovementData data) => data.Config.WalkSpeed;
        
        public WalkAccelerateState(IMovementState parentState) : base(parentState)
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
            
            if (HasReachedTargetSpeed(data))
                return data.States.Get<WalkLocomotionState>();
            
            return null;
        }
    }
}