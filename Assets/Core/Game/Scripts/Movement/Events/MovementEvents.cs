using Core.Game.Movement.StateMachine;
using Core.Systems.Events;

namespace Core.Game.Movement.Events
{
    public struct OnMovementStateChanged : IEvent
    {
        public IMovementState PreviousState;
        public IMovementState NewState;
    }
}