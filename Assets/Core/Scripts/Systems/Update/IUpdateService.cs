using Core.Systems.ServiceLocator;

namespace Core.Systems.Update
{
    /// <summary>
    /// Service interface for managing update callbacks.
    /// Supports IUpdatable, IFixedUpdatable, and ILateUpdatable interfaces.
    /// Objects can implement one or more of these interfaces.
    /// </summary>
    public interface IUpdateService : IService
    {
        /// <summary>
        /// Register an updatable object.
        /// The object can implement IUpdatable, IFixedUpdatable, and/or ILateUpdatable.
        /// </summary>
        void Register(object updatable);

        /// <summary>
        /// Unregister an updatable object.
        /// </summary>
        void Unregister(object updatable);

        /// <summary>
        /// Check if an updatable is registered.
        /// </summary>
        bool IsRegistered(object updatable);
    }
}
