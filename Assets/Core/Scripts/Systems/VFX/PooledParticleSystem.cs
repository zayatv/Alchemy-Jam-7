using UnityEngine;
using Core.Systems.Pooling;

namespace Core.Systems.VFX
{
    [RequireComponent(typeof(ParticleSystem))]
    public class PooledParticleSystem : MonoBehaviour, IPoolable
    {
        private ParticleSystem _particleSystem;

        public GameObject GameObject => gameObject;

        private void Awake()
        {
            _particleSystem = GetComponent<ParticleSystem>();
        }
        
        public void OnSpawnFromPool()
        {
            if (_particleSystem != null)
            {
                _particleSystem.Clear();
                _particleSystem.Play(true);
            }
        }
        
        public void OnReturnToPool()
        {
            if (_particleSystem != null)
            {
                _particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                _particleSystem.Clear(true);
            }
        }
    }
}
