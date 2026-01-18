using UnityEngine;

namespace Core.Systems.Pooling
{
    public interface IPoolable
    {
        /// <summary>
        /// The GameObject this poolable component is attached to.
        /// Used by the pooling system to manage activation state.
        /// </summary>
        GameObject GameObject { get; }

        /// <summary>
        /// Called when the object is spawned from the pool.
        /// Use this to reset state, enable components, or perform initialization.
        /// </summary>
        void OnSpawnFromPool();

        /// <summary>
        /// Called when the object is returned to the pool.
        /// Use this to clean up state, disable components, or prepare for reuse.
        /// </summary>
        void OnReturnToPool();
    }
}
