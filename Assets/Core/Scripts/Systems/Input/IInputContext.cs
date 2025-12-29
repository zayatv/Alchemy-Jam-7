namespace Core.Systems.Input
{
    /// <summary>
    /// Base interface for input contexts.
    /// Input contexts group related inputs and can be enabled/disabled together.
    /// Contexts that need updates can additionally implement IUpdatable, IFixedUpdatable, or ILateUpdatable.
    /// </summary>
    public interface IInputContext
    {
        /// <summary>
        /// Whether this context is currently enabled.
        /// </summary>
        bool IsEnabled { get; }

        /// <summary>
        /// Enable this input context.
        /// </summary>
        void Enable();

        /// <summary>
        /// Disable this input context.
        /// </summary>
        void Disable();
    }
}
