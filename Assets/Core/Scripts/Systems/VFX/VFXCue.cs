using UnityEngine;

namespace Core.Systems.VFX
{
    [CreateAssetMenu(fileName = "NewVFXCue", menuName = "Core/VFX/VFX Cue")]
    public class VFXCue : ScriptableObject
    {
        #region Fields
        
        [Header("Prefab")]
        [SerializeField]
        [Tooltip("Particle system prefab to spawn")]
        private ParticleSystem prefab;

        [Header("Timing")]
        [SerializeField]
        [Tooltip("Duration in seconds (0 = auto-detect from particle system)")]
        private float duration;

        [SerializeField]
        [Tooltip("Automatically return to pool when particle system completes")]
        private bool autoReturnOnComplete = true;

        [Header("Pooling")]
        [SerializeField]
        [Tooltip("Number of instances to pre-warm in the pool")]
        private int prewarmCount = 5;

        [Header("Organization")]
        [SerializeField]
        [Tooltip("Category for bulk operations (e.g., 'Explosions', 'Impacts')")]
        private string category = "";
        
        #endregion
        
        #region Properties

        /// <summary>
        /// Particle system prefab to spawn.
        /// </summary>
        public ParticleSystem Prefab => prefab;

        /// <summary>
        /// Duration in seconds (0 = auto-detect from particle system).
        /// </summary>
        public float Duration => duration;

        /// <summary>
        /// Whether to automatically return to pool when complete.
        /// </summary>
        public bool AutoReturnOnComplete => autoReturnOnComplete;

        /// <summary>
        /// Number of instances to pre-warm in the pool.
        /// </summary>
        public int PrewarmCount => prewarmCount;

        /// <summary>
        /// Category for bulk operations.
        /// </summary>
        public string Category => category;
        
        #endregion

        /// <summary>
        /// Gets the actual duration of this VFX.
        /// If Duration is 0, attempts to detect from the particle system's main module.
        /// </summary>
        public float GetEffectiveDuration()
        {
            if (duration > 0)
                return duration;

            if (prefab != null)
            {
                var main = prefab.main;
                
                return main.duration + main.startLifetime.constantMax;
            }

            return 0f;
        }
    }
}
