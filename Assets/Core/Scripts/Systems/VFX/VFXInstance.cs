using UnityEngine;

namespace Core.Systems.VFX
{
    public struct VFXInstance
    {
        public PooledParticleSystem PooledPS;
        public ParticleSystem ParticleSystem;
        public VFXCue Cue;
        public float SpawnTime;

        public VFXInstance(PooledParticleSystem pooledPS, ParticleSystem ps, VFXCue cue, float spawnTime)
        {
            PooledPS = pooledPS;
            ParticleSystem = ps;
            Cue = cue;
            SpawnTime = spawnTime;
        }
    }
}