using Core.Systems.Audio;
using Core.Systems.VFX;
using UnityEngine;

namespace Core.Game.Combat.Bombs
{
    [CreateAssetMenu(fileName = "NewBombDefinition", menuName = "Core/Game/Combat/Bomb Definition")]
    public class BombDefinition : ScriptableObject
    {
        #region Serialized Fields

        [Header("Base Stats")]
        [SerializeField] private int baseDamage = 1;
        [SerializeField] private int baseRadius = 1;
        [SerializeField] private float baseFuseTime = 3f;
        [SerializeField] private float basePlacementCooldown = 0.2f;

        [Header("Prefab")]
        [SerializeField] private BombController bombPrefab;

        [Header("VFX")]
        [SerializeField] private VFXCue fuseVFX;
        [SerializeField] private VFXCue areaIndicatorVFX;
        [SerializeField] private VFXCue areaExplosionVFX;

        [Header("Audio")]
        [SerializeField] private AudioCue placementSound;
        [SerializeField] private AudioCue fuseSound;
        [SerializeField] private AudioCue explosionSound;

        [Header("Explosion Blocking")]
        [Tooltip("Layers that block explosion damage (e.g., walls)")]
        [SerializeField] private LayerMask explosionBlockingLayers;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the base damage value for this bomb type.
        /// </summary>
        public int BaseDamage => baseDamage;

        /// <summary>
        /// Gets the base explosion radius for this bomb type.
        /// </summary>
        public int BaseRadius => baseRadius;

        /// <summary>
        /// Gets the base fuse time in seconds for this bomb type.
        /// </summary>
        public float BaseFuseTime => baseFuseTime;

        /// <summary>
        /// Gets the base placement cooldown in seconds for this bomb type.
        /// </summary>
        public float BasePlacementCooldown => basePlacementCooldown;

        /// <summary>
        /// Gets the bomb controller prefab to instantiate.
        /// </summary>
        public BombController BombPrefab => bombPrefab;

        /// <summary>
        /// Gets the visual effect cue for the fuse.
        /// </summary>
        public VFXCue FuseVFX => fuseVFX;

        /// <summary>
        /// Gets the visual effect cue for the area indicator.
        /// </summary>
        public VFXCue AreaIndicatorVFX => areaIndicatorVFX;

        /// <summary>
        /// Gets the visual effect cue for the explosion.
        /// </summary>
        public VFXCue AreaExplosionVFX => areaExplosionVFX;
        
        /// <summary>
        /// Gets the audio cue for bomb placement.
        /// </summary>
        public AudioCue PlacementSound => placementSound;

        /// <summary>
        /// Gets the audio cue for the fuse burning.
        /// </summary>
        public AudioCue FuseSound => fuseSound;

        /// <summary>
        /// Gets the audio cue for the explosion.
        /// </summary>
        public AudioCue ExplosionSound => explosionSound;

        /// <summary>
        /// Gets the layer mask for objects that block explosion damage.
        /// </summary>
        public LayerMask ExplosionBlockingLayers => explosionBlockingLayers;

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates a new <see cref="BombStats"/> instance with the base values from this definition.
        /// </summary>
        /// <returns>A new bomb stats instance with base values.</returns>
        public BombStats CreateBaseStats()
        {
            return new BombStats(baseDamage, baseRadius, 1f, baseFuseTime, basePlacementCooldown);
        }

        #endregion
    }
}
