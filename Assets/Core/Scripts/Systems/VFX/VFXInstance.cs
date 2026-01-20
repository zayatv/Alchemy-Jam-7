using UnityEngine;

namespace Core.Systems.VFX
{
    public struct VFXInstance
    {
        public PooledParticleSystem PooledPS;
        public ParticleSystem ParticleSystem;
        public ParticleSystem[] ChildParticleSystems;
        public ParticleSystemRenderer[] Renderers;
        public VFXPropertyBinder PropertyBinder;
        public VFXCue Cue;
        public float SpawnTime;

        public VFXInstance(PooledParticleSystem pooledPS, ParticleSystem ps, VFXCue cue, float spawnTime)
        {
            PooledPS = pooledPS;
            ParticleSystem = ps;
            ChildParticleSystems = pooledPS.GetComponentsInChildren<ParticleSystem>();
            Renderers = pooledPS.GetComponentsInChildren<ParticleSystemRenderer>();
            PropertyBinder = pooledPS.GetComponent<VFXPropertyBinder>();
            Cue = cue;
            SpawnTime = spawnTime;
        }
    }
}