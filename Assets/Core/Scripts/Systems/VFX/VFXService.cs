using System.Collections.Generic;
using Core.Systems.Logging;
using UnityEngine;
using Core.Systems.Pooling;
using Core.Systems.Update;

namespace Core.Systems.VFX
{
    public class VFXService : IVFXService, IUpdatable
    {
        #region Fields
        
        private readonly IObjectPoolService _poolService;
        private int _nextHandleId = 1;

        private Dictionary<int, VFXInstance> _activeVFX = new Dictionary<int, VFXInstance>();
        private Dictionary<ParticleSystem, int> _particleToHandle = new Dictionary<ParticleSystem, int>();

        private Dictionary<string, List<int>> _categoryHandles = new Dictionary<string, List<int>>();
        
        private readonly MaterialPropertyBlock _propertyBlock = new MaterialPropertyBlock();
        
        #endregion
        
        #region Properties

        public int UpdatePriority => 50;
        
        #endregion

        public VFXService(IObjectPoolService poolService)
        {
            _poolService = poolService;
        }
        
        #region IVFXService Implementation
        
        public VFXHandle Spawn(VFXCue cue, Vector3 position)
        {
            return Spawn(cue, position, Quaternion.identity);
        }
        
        public VFXHandle Spawn(VFXCue cue, Vector3 position, Quaternion rotation)
        {
            if (cue == null || cue.Prefab == null)
            {
                GameLogger.Log(LogLevel.Error, "Attempted to spawn null VFXCue or prefab");
                
                return VFXHandle.Invalid;
            }

            PooledParticleSystem pooledPrefab = cue.Prefab.GetComponent<PooledParticleSystem>();
            
            if (pooledPrefab == null)
            {
                GameLogger.Log(LogLevel.Error, $"VFXService: Prefab '{cue.Prefab.name}' must have PooledParticleSystem component");
                
                return VFXHandle.Invalid;
            }

            PooledParticleSystem pooledInstance = _poolService.Spawn(pooledPrefab, position, rotation);
            ParticleSystem ps = pooledInstance.GetComponent<ParticleSystem>();
            int handleId = _nextHandleId++;
            VFXInstance instance = new VFXInstance(pooledInstance, ps, cue, Time.time);
            
            _activeVFX[handleId] = instance;
            _particleToHandle[ps] = handleId;

            if (!string.IsNullOrEmpty(cue.Category))
            {
                if (!_categoryHandles.ContainsKey(cue.Category))
                {
                    _categoryHandles[cue.Category] = new List<int>();
                }
                
                _categoryHandles[cue.Category].Add(handleId);
            }

            return new VFXHandle(handleId);
        }
        
        public VFXHandle SpawnAttached(VFXCue cue, Transform parent)
        {
            if (parent == null)
            {
                GameLogger.Log(LogLevel.Warning, "Attempted to spawn attached VFX with null parent");
                
                return Spawn(cue, Vector3.zero);
            }

            VFXHandle handle = Spawn(cue, parent.position, parent.rotation);

            if (handle.IsValid && _activeVFX.TryGetValue(handle.Id, out VFXInstance instance))
            {
                instance.PooledPS.transform.SetParent(parent);
                instance.PooledPS.transform.localPosition = Vector3.zero;
                instance.PooledPS.transform.localRotation = Quaternion.identity;
            }

            return handle;
        }
        
        public void Stop(VFXHandle handle)
        {
            if (!handle.IsValid)
                return;

            if (_activeVFX.TryGetValue(handle.Id, out VFXInstance instance))
            {
                DespawnInstance(handle.Id, instance);
            }
        }
        
        public void StopAll()
        {
            List<int> handleIds = new List<int>(_activeVFX.Keys);

            foreach (int handleId in handleIds)
            {
                Stop(new VFXHandle(handleId));
            }
        }
        
        public void StopCategory(string category)
        {
            if (string.IsNullOrEmpty(category))
                return;

            if (_categoryHandles.TryGetValue(category, out List<int> handles))
            {
                List<int> handlesCopy = new List<int>(handles);

                foreach (int handleId in handlesCopy)
                {
                    Stop(new VFXHandle(handleId));
                }
            }
        }
        
        public void Prewarm(VFXCue cue, int count)
        {
            if (cue == null || cue.Prefab == null)
            {
                GameLogger.Log(LogLevel.Warning, "Attempted to prewarm null VFXCue or prefab");
                
                return;
            }

            PooledParticleSystem pooledPrefab = cue.Prefab.GetComponent<PooledParticleSystem>();
            
            if (pooledPrefab == null)
            {
                GameLogger.Log(LogLevel.Error, $"Cannot prewarm prefab '{cue.Prefab.name}' - missing PooledParticleSystem component");
                
                return;
            }

            _poolService.Prewarm(pooledPrefab, count);
        }

        public void SetFloat(VFXHandle handle, string name, float value)
        {
            if (!handle.IsValid) 
                return;
            
            if (_activeVFX.TryGetValue(handle.Id, out VFXInstance instance))
            {
                if (instance.PropertyBinder == null)
                    return;

                instance.PropertyBinder.ApplyFloat(name, value);
            }
        }

        public void SetVector(VFXHandle handle, string name, Vector4 value)
        {
            if (!handle.IsValid) 
                return;
            
            if (_activeVFX.TryGetValue(handle.Id, out VFXInstance instance))
            {
                if (instance.PropertyBinder == null)
                    return;

                instance.PropertyBinder.ApplyVector(name, value);
            }
        }

        public void SetColor(VFXHandle handle, string name, Color value)
        {
            if (!handle.IsValid) 
                return;
            
            if (_activeVFX.TryGetValue(handle.Id, out VFXInstance instance))
            {
                if (instance.PropertyBinder == null)
                    return;

                instance.PropertyBinder.ApplyColor(name, value);
            }
        }
        
        #endregion
        
        #region Update Interface Implementation
        
        public void OnUpdate(float deltaTime)
        {
            List<int> toRemove = new List<int>();

            foreach (var kvp in _activeVFX)
            {
                VFXInstance instance = kvp.Value;

                if (!instance.Cue.AutoReturnOnComplete)
                    continue;

                if (instance.ParticleSystem != null && !instance.ParticleSystem.IsAlive(true))
                {
                    toRemove.Add(kvp.Key);
                }
            }

            foreach (int handleId in toRemove)
            {
                if (_activeVFX.TryGetValue(handleId, out VFXInstance instance))
                {
                    DespawnInstance(handleId, instance);
                }
            }
        }
        
        #endregion
        
        #region Helper Methods

        private void DespawnInstance(int handleId, VFXInstance instance)
        {
            _activeVFX.Remove(handleId);
            _particleToHandle.Remove(instance.ParticleSystem);

            if (!string.IsNullOrEmpty(instance.Cue.Category))
            {
                if (_categoryHandles.TryGetValue(instance.Cue.Category, out List<int> handles))
                {
                    handles.Remove(handleId);
                }
            }

            if (instance.PooledPS != null)
            {
                _poolService.Despawn(instance.PooledPS);
            }
        }
        
        #endregion
    }
}
