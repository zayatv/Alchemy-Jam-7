using System.Collections.Generic;
using Core.Game.Movement.Data;
using Core.Game.Movement.Events;
using Core.Systems.Logging;
using Core.Systems.Events;

namespace Core.Game.Movement.StateMachine
{
    public class MovementStateMachine : IMovementStateMachine
    {
        #region Fields
        
        private IMovementState _currentState;
        private readonly MovementData _data;
        
        #endregion

        #region Properties
        
        public IMovementState CurrentState => _currentState;
        
        #endregion
        
        #region Constructor

        public MovementStateMachine(MovementData data, IMovementState initialState)
        {
            _data = data;
            
            ChangeState(initialState);
        }
        
        #endregion

        #region Interface Implementations
        
        public void OnUpdate(float deltaTime)
        {
            if (_currentState == null)
                return;
            
            UpdateStateHierarchy(_currentState, deltaTime);
            
            IMovementState nextState = CheckTransitionsHierarchy(_currentState);
            
            if (nextState != null && nextState != _currentState)
                ChangeState(nextState);
        }

        public void OnFixedUpdate(float fixedDeltaTime)
        {
            if (_currentState == null)
                return;
            
            FixedUpdateStateHierarchy(_currentState, fixedDeltaTime);
        }
        
        public void OnLateUpdate(float deltaTime)
        {
            if (_currentState == null)
                return;
            
            LateUpdateStateHierarchy(_currentState, deltaTime);
        }

        public void ChangeState(IMovementState newState)
        {
            if (newState == null)
            {
                GameLogger.Log(LogLevel.Warning, "Attempted to change state to null");

                return;
            }
            
            IMovementState previousState = _currentState;
            
            if (_currentState != null)
                ExitStateHierarchy(_currentState, newState);
            
            _currentState = newState;
            
            EnterStateHierarchy(_currentState, previousState);
            
            EventBus.Raise(new OnMovementStateChanged { PreviousState = previousState, NewState = newState });
        }
        
        public string GetStateHierarchyString()
        {
            if (_currentState == null) 
                return "No State";

            List<string> hierarchy = new List<string>();
            IMovementState current = _currentState;

            while (current != null)
            {
                hierarchy.Insert(0, current.StateName);
                
                current = current.ParentState;
            }

            return string.Join(" → ", hierarchy);
        }

        #endregion
        
        #region State Hierarchy Methods

        /// <summary>
        /// Updates the state hierarchy recursively, ensuring parent states are updated before child states,
        /// allowing for a proper hierarchical update order in the state machine.
        /// </summary>
        /// <param name="state">The current state to be updated. This state and its parents (if any) will be updated in sequence.</param>
        /// <param name="deltaTime">The time elapsed since the last frame update, used to update state behavior.</param>
        private void UpdateStateHierarchy(IMovementState state, float deltaTime)
        {
            if (state == null)
                return;
            
            if (state.ParentState != null)
                UpdateStateHierarchy(state.ParentState, deltaTime);
            
            state.Update(_data, deltaTime);
        }

        /// <summary>
        /// Executes a fixed update on the state hierarchy, ensuring that parent states are updated before child states,
        /// following the hierarchy of states and applying fixed-interval logic for physics-related updates.
        /// </summary>
        /// <param name="state">The current state to be updated. The method will ensure that all ancestor states in the hierarchy
        /// are updated recursively before the given state.</param>
        /// <param name="fixedDeltaTime">The time interval since the last fixed update, used for consistent and accurate
        /// physics-related calculations.</param>
        private void FixedUpdateStateHierarchy(IMovementState state, float fixedDeltaTime)
        {
            if (state == null)
                return;
            
            if (state.ParentState != null)
                FixedUpdateStateHierarchy(state.ParentState, fixedDeltaTime);
            
            state.FixedUpdate(_data, fixedDeltaTime);
        }

        /// <summary>
        /// Performs a recursive late-update operation on the state hierarchy, ensuring parent states are
        /// late-updated before their child states, to maintain proper late-update order in the state machine.
        /// </summary>
        /// <param name="state">The current state to be late-updated. Parent states (if any) will be processed first.</param>
        /// <param name="deltaTime">The time elapsed since the last frame update, utilized for time-sensitive calculations.</param>
        private void LateUpdateStateHierarchy(IMovementState state, float deltaTime)
        {
            if (state == null)
                return;
            
            if (state.ParentState != null)
                LateUpdateStateHierarchy(state.ParentState, deltaTime);
            
            state.LateUpdate(_data, deltaTime);
        }

        /// <summary>
        /// Recursively checks the state hierarchy for transitions, starting from the specified state and ascending towards parent states,
        /// to determine the next state to transition to, if any.
        /// </summary>
        /// <param name="state">The current movement state being evaluated for valid transitions. This state and its parent states will be recursively checked.</param>
        /// <returns>The next movement state to transition to, or null if no valid transitions are detected in the hierarchy.</returns>
        private IMovementState CheckTransitionsHierarchy(IMovementState state)
        {
            if (state == null)
                return null;
            
            IMovementState nextState = state.CheckTransitions(_data);

            if (nextState != null)
                return nextState;
            
            if (state.ParentState != null)
                return CheckTransitionsHierarchy(state.ParentState);
            
            return null;
        }

        /// <summary>
        /// Enters the state hierarchy recursively, ensuring parent states are entered before the current state,
        /// maintaining a proper hierarchical entry order in the state machine.
        /// </summary>
        /// <param name="state">The state to be entered. This state and its parent states (if any) will be processed in sequence.</param>
        /// <param name="previousState">The previously active state before transitioning to the new state. Used to determine the common ancestor state, if any.</param>
        private void EnterStateHierarchy(IMovementState state, IMovementState previousState)
        {
            if (state == null)
                return;

            IMovementState commonAncestor = FindCommonAncestor(previousState, state);

            if (state.ParentState != null && state.ParentState != commonAncestor)
                EnterStateHierarchy(state.ParentState, previousState);

            if (state != commonAncestor)
                state.Enter(_data);
        }

        /// <summary>
        /// Exits the current state hierarchy recursively, ensuring cleanup or finalization
        /// for the current state and its parent states until a common ancestor with the new
        /// state is found. This allows for a proper transition between hierarchical states.
        /// </summary>
        /// <param name="state">The current state being exited. This state and its parent states will be exited as needed.</param>
        /// <param name="newState">The new state being transitioned to, which is used to determine the common ancestor and limit the recursive exit.</param>
        private void ExitStateHierarchy(IMovementState state, IMovementState newState)
        {
            if (state == null)
                return;

            IMovementState commonAncestor = FindCommonAncestor(state, newState);

            if (state != commonAncestor)
                state.Exit(_data);

            if (state.ParentState != null && state.ParentState != commonAncestor)
                ExitStateHierarchy(state.ParentState, newState);
        }

        /// <summary>
        /// Finds the closest common ancestor between two movement states within the state hierarchy.
        /// This is used to determine the shared point in the hierarchy between transitioning states
        /// to ensure proper exit and enter sequences for parent and child states.
        /// </summary>
        /// <param name="stateA">The first state involved in the comparison.</param>
        /// <param name="stateB">The second state involved in the comparison.</param>
        /// <returns>The closest common ancestor state shared by both <paramref name="stateA"/> and <paramref name="stateB"/>, or null if no common ancestor exists.</returns>
        private IMovementState FindCommonAncestor(IMovementState stateA, IMovementState stateB)
        {
            if (stateA == null || stateB == null)
                return null;

            HashSet<IMovementState> ancestorsA = new HashSet<IMovementState>();
            IMovementState current = stateA;
            
            while (current != null)
            {
                ancestorsA.Add(current);
                
                current = current.ParentState;
            }

            current = stateB;
            
            while (current != null)
            {
                if (ancestorsA.Contains(current))
                    return current;
                
                current = current.ParentState;
            }

            return null;
        }
        
        #endregion
    }
}