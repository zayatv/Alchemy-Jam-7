using System.Collections.Generic;
using UnityEngine;

namespace Core.Systems.Update
{
    /// <summary>
    /// Service for managing update callbacks with priority support.
    /// Supports IUpdatable, IFixedUpdatable, and ILateUpdatable interfaces.
    /// </summary>
    public class UpdateService : MonoBehaviour, IUpdateService
    {
        private readonly List<IUpdatable> _updatables = new List<IUpdatable>();
        private readonly List<IFixedUpdatable> _fixedUpdatables = new List<IFixedUpdatable>();
        private readonly List<ILateUpdatable> _lateUpdatables = new List<ILateUpdatable>();

        private bool _needsUpdateSort = false;
        private bool _needsFixedUpdateSort = false;
        private bool _needsLateUpdateSort = false;

        public void Register(object updatable)
        {
            if (updatable == null)
            {
                Debug.LogWarning("[UpdateService] Attempted to register null updatable");
                
                return;
            }

            bool registered = false;

            // Check and register for IUpdatable
            if (updatable is IUpdatable update)
            {
                if (!_updatables.Contains(update))
                {
                    _updatables.Add(update);
                    
                    _needsUpdateSort = true;
                    
                    Debug.Log($"[UpdateService] Registered {updatable.GetType().Name} for Update with priority {update.UpdatePriority}");
                    
                    registered = true;
                }
            }

            // Check and register for IFixedUpdatable
            if (updatable is IFixedUpdatable fixedUpdate)
            {
                if (!_fixedUpdatables.Contains(fixedUpdate))
                {
                    _fixedUpdatables.Add(fixedUpdate);
                    
                    _needsFixedUpdateSort = true;
                    
                    Debug.Log($"[UpdateService] Registered {updatable.GetType().Name} for FixedUpdate with priority {fixedUpdate.FixedUpdatePriority}");
                    
                    registered = true;
                }
            }

            // Check and register for ILateUpdatable
            if (updatable is ILateUpdatable lateUpdate)
            {
                if (!_lateUpdatables.Contains(lateUpdate))
                {
                    _lateUpdatables.Add(lateUpdate);
                    
                    _needsLateUpdateSort = true;
                    
                    Debug.Log($"[UpdateService] Registered {updatable.GetType().Name} for LateUpdate with priority {lateUpdate.LateUpdatePriority}");
                    
                    registered = true;
                }
            }

            if (!registered)
                Debug.LogWarning($"[UpdateService] {updatable.GetType().Name} does not implement IUpdatable, IFixedUpdatable, or ILateUpdatable");
        }

        public void Unregister(object updatable)
        {
            if (updatable == null)
            {
                Debug.LogWarning("[UpdateService] Attempted to unregister null updatable");
                
                return;
            }

            bool unregistered = false;

            // Unregister from IUpdatable
            if (updatable is IUpdatable update && _updatables.Remove(update))
            {
                Debug.Log($"[UpdateService] Unregistered {updatable.GetType().Name} from Update");
                
                unregistered = true;
            }

            // Unregister from IFixedUpdatable
            if (updatable is IFixedUpdatable fixedUpdate && _fixedUpdatables.Remove(fixedUpdate))
            {
                Debug.Log($"[UpdateService] Unregistered {updatable.GetType().Name} from FixedUpdate");
                
                unregistered = true;
            }

            // Unregister from ILateUpdatable
            if (updatable is ILateUpdatable lateUpdate && _lateUpdatables.Remove(lateUpdate))
            {
                Debug.Log($"[UpdateService] Unregistered {updatable.GetType().Name} from LateUpdate");
                
                unregistered = true;
            }

            if (!unregistered)
                Debug.LogWarning($"[UpdateService] {updatable.GetType().Name} was not registered");
        }

        public bool IsRegistered(object updatable)
        {
            if (updatable == null) 
                return false;

            if (updatable is IUpdatable update && _updatables.Contains(update))
                return true;

            if (updatable is IFixedUpdatable fixedUpdate && _fixedUpdatables.Contains(fixedUpdate))
                return true;

            if (updatable is ILateUpdatable lateUpdate && _lateUpdatables.Contains(lateUpdate))
                return true;

            return false;
        }

        private void Update()
        {
            if (_needsUpdateSort)
                SortUpdatables();

            float deltaTime = Time.deltaTime;
            
            for (int i = 0; i < _updatables.Count; i++)
            {
                _updatables[i].OnUpdate(deltaTime);
            }
        }

        private void FixedUpdate()
        {
            if (_needsFixedUpdateSort)
                SortFixedUpdatables();

            float fixedDeltaTime = Time.fixedDeltaTime;
            
            for (int i = 0; i < _fixedUpdatables.Count; i++)
            {
                _fixedUpdatables[i].OnFixedUpdate(fixedDeltaTime);
            }
        }

        private void LateUpdate()
        {
            if (_needsLateUpdateSort)
                SortLateUpdatables();

            float deltaTime = Time.deltaTime;
            
            for (int i = 0; i < _lateUpdatables.Count; i++)
            {
                _lateUpdatables[i].OnLateUpdate(deltaTime);
            }
        }

        private void SortUpdatables()
        {
            _updatables.Sort((a, b) => a.UpdatePriority.CompareTo(b.UpdatePriority));
            
            _needsUpdateSort = false;
            
            Debug.Log($"[UpdateService] Sorted {_updatables.Count} updatables by priority");
        }

        private void SortFixedUpdatables()
        {
            _fixedUpdatables.Sort((a, b) => a.FixedUpdatePriority.CompareTo(b.FixedUpdatePriority));
            
            _needsFixedUpdateSort = false;
            
            Debug.Log($"[UpdateService] Sorted {_fixedUpdatables.Count} fixed updatables by priority");
        }

        private void SortLateUpdatables()
        {
            _lateUpdatables.Sort((a, b) => a.LateUpdatePriority.CompareTo(b.LateUpdatePriority));
            
            _needsLateUpdateSort = false;
            
            Debug.Log($"[UpdateService] Sorted {_lateUpdatables.Count} late updatables by priority");
        }
    }
}
