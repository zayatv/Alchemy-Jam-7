using System;
using System.Collections.Generic;
using Core.Game.Movement.StateMachine;

namespace Core.Game.Movement.Data
{
    public class MovementStates
    {
        private readonly HashSet<IMovementState> _states = new();
        private readonly Dictionary<Type, IMovementState> _stateCache = new();

        /// <summary>
        /// Registers a movement state into the collection of states.
        /// </summary>
        /// <param name="state">The movement state to register. Cannot be null.</param>
        /// <exception cref="ArgumentNullException">Thrown when the provided state is null.</exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when a state of the same type has already been registered.
        /// </exception>
        public void Register(IMovementState state)
        {
            if (state == null)
                throw new ArgumentNullException(nameof(state));

            Type stateType = state.GetType();

            if (_stateCache.ContainsKey(stateType))
                throw new InvalidOperationException($"State of type {stateType.Name} is already registered.");
            
            _states.Add(state);
            _stateCache[stateType] = state;
        }

        /// <summary>
        /// Retrieves a movement state of the specified type from the collection of registered states.
        /// </summary>
        /// <typeparam name="T">The type of the movement state to retrieve. Must inherit from <see cref="IMovementState"/>.</typeparam>
        /// <returns>The instance of the requested movement state if it is registered.</returns>
        /// <exception cref="KeyNotFoundException">
        /// Thrown when a state of the specified type is not found in the collection of registered states.
        /// </exception>
        public T Get<T>() where T : class, IMovementState
        {
            if (_stateCache.TryGetValue(typeof(T), out IMovementState state))
                return state as T;

            throw new KeyNotFoundException($"State of type {typeof(T).Name} is not registered.");
        }

        /// <summary>
        /// Attempts to retrieve a movement state of the specified type from the collection of states.
        /// </summary>
        /// <typeparam name="T">The type of the movement state to retrieve. Must implement <see cref="IMovementState"/>.</typeparam>
        /// <param name="state">
        /// When this method returns, contains the movement state of the specified type if found;
        /// otherwise, the default value for the type of the state parameter.
        /// </param>
        /// <returns>
        /// Returns <c>true</c> if a movement state of the specified type was found in the collection;
        /// otherwise, <c>false</c>.
        /// </returns>
        public bool TryGet<T>(out T state) where T : class, IMovementState
        {
            if (_stateCache.TryGetValue(typeof(T), out IMovementState foundState))
            {
                state = foundState as T;
                
                return true;
            }
            
            state = null;
            
            return false;
        }

        /// <summary>
        /// Determines whether a movement state of the specified type is registered in the collection of movement states.
        /// </summary>
        /// <typeparam name="T">The type of the movement state to check for existence. Must inherit from <see cref="IMovementState"/>.</typeparam>
        /// <returns>True if a movement state of the specified type is registered; otherwise, false.</returns>
        public bool Has<T>() where T : class, IMovementState
        {
            return _stateCache.ContainsKey(typeof(T));
        }

        /// <summary>
        /// Retrieves all registered movement states.
        /// </summary>
        /// <returns>An enumerable collection of all movement states currently registered.</returns>
        public IEnumerable<IMovementState> GetAll()
        {
            return _states;
        }
    }
}