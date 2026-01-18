using System.Collections.Generic;
using Core.Systems.Logging;
using UnityEngine;

namespace Core.Systems.Pooling
{
    public class ObjectPool<T> : IObjectPool where T : Component, IPoolable
    {
        #region Fields
        
        private readonly T _prefab;
        private readonly Transform _parent;
        private readonly Queue<T> _availableInstances;
        
        #endregion
        
        #region Properties
        
        public int AvailableCount => _availableInstances.Count;
        
        #endregion

        /// <summary>
        /// Creates a new object pool.
        /// </summary>
        /// <param name="prefab">The prefab to pool</param>
        /// <param name="parent">Parent transform for pooled objects (for organization)</param>
        public ObjectPool(T prefab, Transform parent)
        {
            _prefab = prefab;
            _parent = parent;
            _availableInstances = new Queue<T>();
        }

        /// <summary>
        /// Spawns an instance from the pool at the specified position and rotation.
        /// If the pool is empty, a new instance will be instantiated.
        /// </summary>
        /// <param name="position">Position to spawn at</param>
        /// <param name="rotation">Rotation to spawn with</param>
        /// <returns>The spawned instance</returns>
        public T Spawn(Vector3 position, Quaternion rotation)
        {
            T instance;

            if (_availableInstances.Count > 0)
            {
                instance = _availableInstances.Dequeue();
            }
            else
            {
                instance = Object.Instantiate(_prefab, _parent);
            }

            Transform instanceTransform = instance.transform;
            instanceTransform.position = position;
            instanceTransform.rotation = rotation;

            instance.GameObject.SetActive(true);
            instance.OnSpawnFromPool();

            return instance;
        }

        /// <summary>
        /// Returns an instance to the pool for reuse.
        /// The instance will be deactivated and added back to the available queue.
        /// </summary>
        /// <param name="instance">The instance to return to the pool</param>
        public void Despawn(T instance)
        {
            if (instance == null)
            {
                GameLogger.Log(LogLevel.Warning, $"Attempted to despawn null instance");
                
                return;
            }

            instance.OnReturnToPool();
            instance.GameObject.SetActive(false);

            _availableInstances.Enqueue(instance);
        }

        /// <summary>
        /// Pre-instantiates a specified number of instances and adds them to the pool.
        /// All pre-instantiated instances start inactive.
        /// </summary>
        /// <param name="count">Number of instances to pre-instantiate</param>
        public void Prewarm(int count)
        {
            for (int i = 0; i < count; i++)
            {
                T instance = Object.Instantiate(_prefab, _parent);
                instance.GameObject.SetActive(false);
                _availableInstances.Enqueue(instance);
            }

            GameLogger.Log(LogLevel.Info, $"Prewarmed {count} instances of {_prefab.name}. Total available: {_availableInstances.Count}");
        }
        
        public void Clear()
        {
            int count = _availableInstances.Count;

            while (_availableInstances.Count > 0)
            {
                T instance = _availableInstances.Dequeue();
                
                if (instance != null)
                {
                    Object.Destroy(instance.GameObject);
                }
            }

            GameLogger.Log(LogLevel.Info, $"Cleared pool for {_prefab.name}. Destroyed {count} instances.");
        }
    }
}
