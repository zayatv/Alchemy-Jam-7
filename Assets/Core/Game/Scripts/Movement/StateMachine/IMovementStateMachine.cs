namespace Core.Game.Movement.StateMachine
{
    public interface IMovementStateMachine
    {
        public IMovementState CurrentState { get; }

        /// <summary>
        /// Updates the current state of the movement state machine.
        /// </summary>
        /// <param name="deltaTime">The time in seconds that has elapsed since the last frame.</param>
        void OnUpdate(float deltaTime);

        /// <summary>
        /// Performs a physics-related update for the current state of the movement state machine.
        /// </summary>
        /// <param name="fixedDeltaTime">The time in seconds that has elapsed since the last fixed update frame.</param>
        void OnFixedUpdate(float fixedDeltaTime);

        /// <summary>
        /// Executes any necessary updates after all normal update functions have been called for the current frame.
        /// </summary>
        /// <param name="deltaTime">The time in seconds that has elapsed since the last frame.</param>
        void OnLateUpdate(float deltaTime);

        /// <summary>
        /// Changes the current state of the movement state machine to a new state.
        /// </summary>
        /// <param name="newState">The new state to transition to.</param>
        void ChangeState(IMovementState newState);

        /// <summary>
        /// Retrieves a string representation of the current state hierarchy within the movement state machine.
        /// </summary>
        /// <returns>A string that shows the hierarchy of the current state and its parent states,
        /// separated by arrows, or "No State" if there is no active state.</returns>
        string GetStateHierarchyString();
    }
}