using System.Collections.Generic;
using Core.Systems.ServiceLocator;

namespace Core.Systems.Input
{
    /// <summary>
    /// Core input service interface.
    /// Provides access to input contexts that can be enabled/disabled.
    /// </summary>
    public interface IInputService : IService
    {
        /// <summary>
        /// Enable an input context.
        /// </summary>
        void EnableContext<T>() where T : IInputContext;

        /// <summary>
        /// Disable an input context.
        /// </summary>
        void DisableContext<T>() where T : IInputContext;

        /// <summary>
        /// Get an input context.
        /// </summary>
        T GetContext<T>() where T : IInputContext;

        /// <summary>
        /// Check if a context is enabled.
        /// </summary>
        bool IsContextEnabled<T>() where T : IInputContext;

        /// <summary>
        /// Enable all contexts.
        /// </summary>
        void EnableAll();

        /// <summary>
        /// Disable all contexts.
        /// </summary>
        void DisableAll();

        /// <summary>
        /// Get all registered contexts.
        /// </summary>
        IEnumerable<IInputContext> GetAllContexts();
    }
}
