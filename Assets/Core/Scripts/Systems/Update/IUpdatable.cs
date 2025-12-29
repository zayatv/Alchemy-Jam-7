namespace Core.Systems.Update
{
    /// <summary>
    /// Interface for objects that need per-frame updates.
    /// Implement this interface and register with UpdateService to receive Update calls.
    /// </summary>
    public interface IUpdatable
    {
        /// <summary>
        /// Update priority. Lower values execute first.
        /// Common values: -100 (very early), 0 (normal), 100 (late)
        /// </summary>
        int UpdatePriority { get; }

        /// <summary>
        /// Called every frame (Update).
        /// </summary>
        void OnUpdate(float deltaTime);
    }

    /// <summary>
    /// Interface for objects that need fixed timestep updates.
    /// Implement this interface and register with UpdateService to receive FixedUpdate calls.
    /// </summary>
    public interface IFixedUpdatable
    {
        /// <summary>
        /// Update priority. Lower values execute first.
        /// Common values: -100 (very early), 0 (normal), 100 (late)
        /// </summary>
        int FixedUpdatePriority { get; }

        /// <summary>
        /// Called every fixed timestep (FixedUpdate).
        /// </summary>
        void OnFixedUpdate(float fixedDeltaTime);
    }

    /// <summary>
    /// Interface for objects that need late updates.
    /// Implement this interface and register with UpdateService to receive LateUpdate calls.
    /// </summary>
    public interface ILateUpdatable
    {
        /// <summary>
        /// Update priority. Lower values execute first.
        /// Common values: -100 (very early), 0 (normal), 100 (late)
        /// </summary>
        int LateUpdatePriority { get; }

        /// <summary>
        /// Called after all Update calls (LateUpdate).
        /// </summary>
        void OnLateUpdate(float deltaTime);
    }
}
