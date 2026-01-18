using UnityEngine;
using Core.Systems.ServiceLocator;

namespace Core.Systems.Pooling
{
    public interface IObjectPoolService : IService
    {
        /// <summary>
        /// Spawns an instance of the prefab from the pool at the specified position and rotation.
        /// If the pool is empty, a new instance will be created.
        /// </summary>
        /// <typeparam name="T">Type of component that implements IPoolable</typeparam>
        /// <param name="prefab">The prefab to spawn</param>
        /// <param name="position">Position to spawn at</param>
        /// <param name="rotation">Rotation to spawn with</param>
        /// <returns>The spawned instance</returns>
        T Spawn<T>(T prefab, Vector3 position, Quaternion rotation) where T : Component, IPoolable;

        /// <summary>
        /// Returns an instance to the pool for reuse.
        /// The instance will be deactivated and returned to the pool.
        /// </summary>
        /// <typeparam name="T">Type of component that implements IPoolable</typeparam>
        /// <param name="instance">The instance to return to the pool</param>
        void Despawn<T>(T instance) where T : Component, IPoolable;

        /// <summary>
        /// Pre-instantiates a specified number of instances and adds them to the pool.
        /// This is useful for warming up pools before they are needed to avoid runtime allocation spikes.
        /// </summary>
        /// <typeparam name="T">Type of component that implements IPoolable</typeparam>
        /// <param name="prefab">The prefab to prewarm</param>
        /// <param name="count">Number of instances to pre-instantiate</param>
        void Prewarm<T>(T prefab, int count) where T : Component, IPoolable;

        /// <summary>
        /// Clears a specific pool, destroying all instances.
        /// </summary>
        /// <typeparam name="T">Type of component that implements IPoolable</typeparam>
        /// <param name="prefab">The prefab whose pool should be cleared</param>
        void ClearPool<T>(T prefab) where T : Component, IPoolable;

        /// <summary>
        /// Gets the current size of a pool (number of available instances).
        /// </summary>
        /// <typeparam name="T">Type of component that implements IPoolable</typeparam>
        /// <param name="prefab">The prefab to check</param>
        /// <returns>Number of available instances in the pool</returns>
        int GetPoolSize<T>(T prefab) where T : Component, IPoolable;
    }
}
