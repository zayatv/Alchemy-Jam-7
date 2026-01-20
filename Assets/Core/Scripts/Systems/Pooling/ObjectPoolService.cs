using System.Collections.Generic;
using Core.Systems.Logging;
using UnityEngine;

namespace Core.Systems.Pooling
{
    public class ObjectPoolService : MonoBehaviour, IObjectPoolService
    {
        #region Fields

        private readonly Dictionary<int, object> _pools = new Dictionary<int, object>();
        private readonly Dictionary<int, int> _instanceToPrefabId = new Dictionary<int, int>();

        #endregion
        
        #region IObjectPoolService Implementation
        
        public T Spawn<T>(T prefab, Vector3 position, Quaternion rotation) where T : Component, IPoolable
        {
            if (prefab == null)
            {
                GameLogger.Log(LogLevel.Error, "Cannot spawn from null prefab");

                return null;
            }

            int prefabId = prefab.GetInstanceID();
            ObjectPool<T> pool = GetOrCreatePool(prefab);
            T instance = pool.Spawn(position, rotation);

            int instanceId = instance.GetInstanceID();
            _instanceToPrefabId[instanceId] = prefabId;
            
            GameLogger.Log(LogLevel.Debug, $"Spawned {prefab.name} (Instance ID: {instanceId}) in Pool {pool.AvailableCount}");

            return instance;
        }
        
        public void Despawn<T>(T instance) where T : Component, IPoolable
        {
            if (instance == null)
            {
                GameLogger.Log(LogLevel.Warning, "Cannot despawn null instance");

                return;
            }

            int instanceId = instance.GetInstanceID();
            int prefabId = GetPrefabIdFromInstance(instanceId);

            if (prefabId != 0 && _pools.TryGetValue(prefabId, out object poolObj) && poolObj is ObjectPool<T> pool)
            {
                pool.Despawn(instance);
            }
            else
            {
                GameLogger.Log(LogLevel.Error, $"No pool found for instance {instance.name}. Destroying instead.");

                _instanceToPrefabId.Remove(instanceId);
                Destroy(instance.GameObject);
            }
        }
        
        public void Prewarm<T>(T prefab, int count) where T : Component, IPoolable
        {
            if (prefab == null)
            {
                GameLogger.Log(LogLevel.Error, "Cannot prewarm null prefab");
                
                return;
            }

            if (count <= 0)
            {
                GameLogger.Log(LogLevel.Error, "Prewarm count must be greater than 0");
                
                return;
            }

            ObjectPool<T> pool = GetOrCreatePool(prefab);
            pool.Prewarm(count);
        }
        
        public void ClearPool<T>(T prefab) where T : Component, IPoolable
        {
            if (prefab == null)
            {
                GameLogger.Log(LogLevel.Error, "Cannot clear pool for null prefab");
                
                return;
            }

            int prefabId = prefab.GetInstanceID();

            if (_pools.TryGetValue(prefabId, out object poolObj) && poolObj is ObjectPool<T> pool)
            {
                pool.Clear();
                _pools.Remove(prefabId);
            }
            else
            {
                GameLogger.Log(LogLevel.Warning, $"No pool found for prefab {prefab.name}");
            }
        }
        
        public int GetPoolSize<T>(T prefab) where T : Component, IPoolable
        {
            if (prefab == null)
            {
                GameLogger.Log(LogLevel.Error, "Cannot get pool size for null prefab");
                
                return 0;
            }

            int prefabId = prefab.GetInstanceID();

            if (_pools.TryGetValue(prefabId, out object poolObj) && poolObj is ObjectPool<T> pool)
            {
                return pool.AvailableCount;
            }

            return 0;
        }
        
        #endregion

        /// <summary>
        /// Gets or creates a pool for the specified prefab.
        /// </summary>
        /// <param name="prefab">The prefab to get or create a pool for.</param>
        /// <typeparam name="T">The type of prefab.</typeparam>
        /// <returns>The pool for the specified prefab.</returns>
        private ObjectPool<T> GetOrCreatePool<T>(T prefab) where T : Component, IPoolable
        {
            int prefabId = prefab.GetInstanceID();

            if (_pools.TryGetValue(prefabId, out object poolObj) && poolObj is ObjectPool<T> existingPool)
            {
                return existingPool;
            }

            ObjectPool<T> newPool = new ObjectPool<T>(prefab, transform);
            _pools[prefabId] = newPool;

            GameLogger.Log(LogLevel.Debug, $"Created new pool for {prefab.name} (Instance ID: {prefabId})");

            return newPool;
        }

        /// <summary>
        /// Gets the prefab ID from an instance's ID using the tracked mappings.
        /// </summary>
        /// <param name="instanceId">The instance ID of the spawned object.</param>
        /// <returns>The prefab ID, or 0 if not found.</returns>
        private int GetPrefabIdFromInstance(int instanceId)
        {
            if (_instanceToPrefabId.TryGetValue(instanceId, out int prefabId))
            {
                return prefabId;
            }

            GameLogger.Log(LogLevel.Warning, $"No prefab mapping found for instance ID {instanceId}");
            return 0;
        }
        
        private void OnDestroy()
        {
            foreach (var kvp in _pools)
            {
                if (kvp.Value is IObjectPool pool)
                {
                    pool.Clear();
                }
            }

            _pools.Clear();
            _instanceToPrefabId.Clear();

            GameLogger.Log(LogLevel.Debug, "Service destroyed, all pools cleared");
        }
    }
}
