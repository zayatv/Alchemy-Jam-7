namespace Core.Systems.Pooling
{
    public interface IObjectPool
    {
        /// <summary>
        /// Gets the current number of available instances in the pool.
        /// </summary>
        int AvailableCount { get; }

        /// <summary>
        /// Clears the pool, destroying all available instances.
        /// </summary>
        void Clear();
    }
}
