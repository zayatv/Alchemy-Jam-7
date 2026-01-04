using Core.Game.Movement.Data;
using Core.Game.Movement.StateMachine.States.Grounded;

namespace Core.Game.Movement.StateMachine.States.Airborne
{
    public class FallState : MovementStateBase
    {
        public FallState(IMovementState parentState) : base(parentState)
        {
        }
        
        public override IMovementState CheckTransitions(MovementData data)
        {
            if (data.IsGrounded)
                return data.States.Get<IdleState>();
            
            return null;
        }
    }
}