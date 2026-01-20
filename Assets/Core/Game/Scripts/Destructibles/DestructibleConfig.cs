using Core.Game.Destructibles.Events;
using Core.Systems.Audio;
using Core.Systems.VFX;
using UnityEngine;

namespace Core.Game.Destructibles
{
    [CreateAssetMenu(fileName = "DestructibleConfig", menuName = "Core/Game/Destructibles/Destructible Config")]
    public class DestructibleConfig : ScriptableObject
    {
        #region Serialized Fields

        [Header("Identity")]
        [Tooltip("Type of destructible for event tracking")]
        [SerializeField] private DestructibleType destructibleType = DestructibleType.Generic;

        [Tooltip("Display name for this destructible")]
        [SerializeField] private string displayName;

        [Header("Destruction Effects")]
        [Tooltip("VFX to spawn when destroyed")]
        [SerializeField] private VFXCue destroyVFX;

        [Tooltip("Sound to play when destroyed")]
        [SerializeField] private AudioCue destroySound;

        [Header("Hit Effects")]
        [Tooltip("VFX to spawn when hit but not destroyed")]
        [SerializeField] private VFXCue hitVFX;

        [Tooltip("Sound to play when hit but not destroyed")]
        [SerializeField] private AudioCue hitSound;

        #endregion

        #region Properties

        public DestructibleType DestructibleType => destructibleType;
        public string DisplayName => displayName;
        public VFXCue DestroyVFX => destroyVFX;
        public AudioCue DestroySound => destroySound;
        public VFXCue HitVFX => hitVFX;
        public AudioCue HitSound => hitSound;

        #endregion
    }
}
