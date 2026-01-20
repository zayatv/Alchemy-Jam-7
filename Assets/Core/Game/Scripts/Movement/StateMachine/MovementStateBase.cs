using Core.Game.Movement.Data;
using Core.Systems.Logging;

namespace Core.Game.Movement.StateMachine
{
    public class MovementStateBase : IMovementState
    {
        public virtual IMovementState ParentState { get; }
        public virtual string StateName => GetType().Name;

        public MovementStateBase(IMovementState parentState = null)
        {
            ParentState = parentState;
        }

        public virtual void Enter(MovementData data)
        {
            //LogStateChange($"Entering {StateName}", data);
        }

        public virtual void Exit(MovementData data)
        {
            //LogStateChange($"Exiting {StateName}", data);
        }

        public virtual void Update(MovementData data, float deltaTime)
        {
        }

        public virtual void FixedUpdate(MovementData data, float fixedDeltaTime)
        {
        }
        
        public virtual void LateUpdate(MovementData data, float deltaTime)
        {
        }

        public virtual IMovementState CheckTransitions(MovementData data)
        {
            return null;
        }

        protected void LogStateChange(string message, MovementData data)
        {
            GameLogger.Log(LogLevel.Debug, $"[MovementState] {message} - Entity: {data.Transform.name}");
        }
    }
}