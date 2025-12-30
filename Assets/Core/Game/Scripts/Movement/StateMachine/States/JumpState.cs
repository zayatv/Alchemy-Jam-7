using Core.Game.Movement.Data;

namespace Core.Game.Movement.StateMachine.States
{
    public class JumpState : MovementStateBase
    {
        public JumpState(IMovementState parentState) : base(parentState)
        {
        }
        
        public override void Enter(MovementData data)
        {
            base.Enter(data);
            
            data.SetGravityVelocity(-data.GravityDirection * data.Config.JumpForce);
            data.ConsumeJumpBuffer();
        }

        public override IMovementState CheckTransitions(MovementData data)
        {
            if (!data.IsRising)
                return data.States.Get<FallState>();

            if (data.IsGrounded && data.TimeInAir > 0.1f)
                return data.States.Get<FallState>();

            return null;
        }
    }
}