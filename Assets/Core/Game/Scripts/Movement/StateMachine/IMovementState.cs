using Core.Game.Movement.Data;

namespace Core.Game.Movement.StateMachine
{
    public interface IMovementState
    {
        IMovementState ParentState { get; }
        string StateName { get; }

        /// <summary>
        /// Prepares the movement state by initializing any necessary variables or configurations.
        /// </summary>
        /// <param name="data">The movement data containing the current state and context of the character.</param>
        void Enter(MovementData data);

        /// <summary>
        /// Cleans up or finalizes the movement state's resources or transitions before exiting.
        /// </summary>
        /// <param name="data">The movement data containing the current state and context of the character.</param>
        void Exit(MovementData data);

        /// <summary>
        /// Updates the internal state of the movement logic based on the provided data and elapsed time.
        /// </summary>
        /// <param name="data">The movement data containing state context and current character details.</param>
        /// <param name="deltaTime">The time elapsed since the last frame update.</param>
        void Update(MovementData data, float deltaTime);

        /// <summary>
        /// Updates the movement state at fixed intervals, typically used for applying physics-related updates.
        /// </summary>
        /// <param name="data">The movement data containing the current state and context of the character.</param>
        /// <param name="fixedDeltaTime">The time interval since the last fixed update. Used to apply consistent physics calculations.</param>
        void FixedUpdate(MovementData data, float fixedDeltaTime);

        /// <summary>
        /// Evaluates the current movement state to determine if a transition to a new state is required.
        /// </summary>
        /// <param name="data">The movement data containing the context and parameters of the character's motion.</param>
        /// <returns>The next movement state to transition to, or null if no transition is necessary.</returns>
        IMovementState CheckTransitions(MovementData data);
    }
}