using UnityEngine;
using UnityEngine.Audio;
using Sirenix.OdinInspector;

namespace Core.Systems.Audio
{
    [CreateAssetMenu(fileName = "NewAudioCue", menuName = "Core/Audio/Audio Cue")]
    public class AudioCue : ScriptableObject
    {
        #region Fields
        
        [TitleGroup("Audio Clips")]
        [SerializeField]
        [ListDrawerSettings(ShowFoldout = true, DraggableItems = true, ShowItemCount = true)]
        [AssetsOnly]
        [Tooltip("Audio clips to choose from randomly")]
        private AudioClip[] clips = new AudioClip[0];

        [TitleGroup("Audio Clips")]
        [Button("Preview Random Clip"), PropertyOrder(1)]
        [EnableIf("@clips != null && clips.Length > 0")]
        private void PreviewClip()
        {
#if UNITY_EDITOR
            var clip = GetRandomClip();
            
            if (clip != null)
            {
                PlayClipInEditor(clip);
            }
#endif
        }

        [TitleGroup("Audio Clips")]
        [Button("Stop Preview"), PropertyOrder(2)]
        private void StopPreview()
        {
#if UNITY_EDITOR
            StopAllClipsInEditor();
#endif
        }

        [BoxGroup("Volume")]
        [SerializeField]
        [MinMaxSlider(0f, 1f, true)]
        [LabelText("Volume Range")]
        [Tooltip("Random volume range (X = min, Y = max)")]
        private Vector2 volumeRange = new Vector2(1f, 1f);

        [BoxGroup("Pitch")]
        [SerializeField]
        [MinMaxSlider(0.1f, 3f, true)]
        [LabelText("Pitch Range")]
        [Tooltip("Random pitch range (X = min, Y = max)")]
        private Vector2 pitchRange = new Vector2(1f, 1f);

        [TitleGroup("Output")]
        [SerializeField]
        [Tooltip("Audio mixer group (optional)")]
        private AudioMixerGroup mixerGroup;

        [TitleGroup("Playback")]
        [SerializeField]
        [ToggleLeft]
        [Tooltip("Should the audio loop?")]
        private bool loop;

        [SerializeField]
        [PropertyRange(0, 256)]
        [LabelText("Priority")]
        [Tooltip("Priority of the audio source (0 = highest, 256 = lowest)")]
        private int priority = 128;

        [TitleGroup("Spatial Settings")]
        [SerializeField]
        [PropertyRange(0f, 1f)]
        [LabelText("Spatial Blend")]
        [Tooltip("0 = 2D sound, 1 = 3D sound")]
        [OnValueChanged("OnSpatialBlendChanged")]
        private float spatialBlend;

        [TitleGroup("Spatial Settings")]
        [ShowIf("@spatialBlend > 0")]
        [FoldoutGroup("Spatial Settings/3D Sound Settings")]
        [SerializeField]
        [MinValue(0.01f)]
        [Tooltip("Minimum distance before attenuation starts")]
        private float minDistance = 1f;

        [FoldoutGroup("Spatial Settings/3D Sound Settings")]
        [SerializeField]
        [MinValue(0.01f)]
        [Tooltip("Maximum distance for 3D sound attenuation")]
        private float maxDistance = 50f;

        [FoldoutGroup("Spatial Settings/3D Sound Settings")]
        [SerializeField]
        [EnumToggleButtons]
        [HideIf("useCustomRolloff")]
        [Tooltip("How the sound attenuates over distance")]
        private AudioRolloffMode rolloffMode = AudioRolloffMode.Logarithmic;

        [FoldoutGroup("Spatial Settings/3D Sound Settings")]
        [SerializeField]
        [ToggleLeft]
        [Tooltip("Use custom rolloff curve instead of built-in modes")]
        private bool useCustomRolloff;

        [FoldoutGroup("Spatial Settings/3D Sound Settings")]
        [SerializeField]
        [ShowIf("useCustomRolloff")]
        [InfoBox("X axis = normalized distance (0-1), Y axis = volume (0-1)")]
        [Tooltip("Custom volume rolloff curve")]
        private AnimationCurve customRolloffCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);

        [FoldoutGroup("Spatial Settings/3D Sound Settings")]
        [SerializeField]
        [PropertyRange(0f, 5f)]
        [Tooltip("Doppler effect intensity (0 = none, 1 = default)")]
        private float dopplerLevel = 1f;

        [FoldoutGroup("Spatial Settings/3D Sound Settings")]
        [SerializeField]
        [PropertyRange(0f, 360f)]
        [Tooltip("Spread angle in degrees for 3D stereo/surround")]
        private float spread;

        [TitleGroup("Advanced 3D Curves")]
        [ShowIf("@spatialBlend > 0")]
        [InfoBox("Optional curves to control audio properties over distance. Leave empty to use default behavior.", InfoMessageType.Info)]
        [SerializeField]
        [ToggleLeft]
        [LabelText("Enable Custom 3D Curves")]
        private bool useCustom3DCurves;

        [TitleGroup("Advanced 3D Curves")]
        [ShowIf("@spatialBlend > 0 && useCustom3DCurves")]
        [SerializeField]
        [LabelText("Spatial Blend Over Distance")]
        [Tooltip("Curve controlling spatial blend over distance")]
        private AnimationCurve spatialBlendCurve;

        [TitleGroup("Advanced 3D Curves")]
        [ShowIf("@spatialBlend > 0 && useCustom3DCurves")]
        [SerializeField]
        [LabelText("Spread Over Distance")]
        [Tooltip("Curve controlling spread over distance")]
        private AnimationCurve spreadCurve;

        [TitleGroup("Advanced 3D Curves")]
        [ShowIf("@spatialBlend > 0 && useCustom3DCurves")]
        [SerializeField]
        [LabelText("Reverb Mix Over Distance")]
        [Tooltip("Curve controlling reverb zone mix over distance")]
        private AnimationCurve reverbZoneMixCurve;

        [TitleGroup("Reverb & Effects")]
        [SerializeField]
        [PropertyRange(0f, 1.1f)]
        [Tooltip("Amount of signal routed to reverb zones")]
        private float reverbZoneMix = 1f;

        [TitleGroup("Reverb & Effects")]
        [HorizontalGroup("Reverb & Effects/Bypass")]
        [SerializeField]
        [ToggleLeft]
        [LabelText("Bypass Effects")]
        [Tooltip("Bypass any applied effects")]
        private bool bypassEffects;

        [HorizontalGroup("Reverb & Effects/Bypass")]
        [SerializeField]
        [ToggleLeft]
        [LabelText("Bypass Listener")]
        [Tooltip("Bypass listener effects")]
        private bool bypassListenerEffects;

        [HorizontalGroup("Reverb & Effects/Bypass")]
        [SerializeField]
        [ToggleLeft]
        [LabelText("Bypass Reverb")]
        [Tooltip("Bypass reverb zones")]
        private bool bypassReverbZones;
        
        #endregion
        
        #region Properties

        public AudioClip[] Clips => clips;
        public Vector2 VolumeRange => volumeRange;
        public Vector2 PitchRange => pitchRange;
        public AudioMixerGroup MixerGroup => mixerGroup;
        public bool Loop => loop;
        public int Priority => priority;

        // Spatial properties
        public float SpatialBlend => spatialBlend;
        public float MinDistance => minDistance;
        public float MaxDistance => maxDistance;
        public AudioRolloffMode RolloffMode => useCustomRolloff ? AudioRolloffMode.Custom : rolloffMode;

        // Advanced 3D properties
        public bool UseCustomRolloff => useCustomRolloff;
        public AnimationCurve CustomRolloffCurve => customRolloffCurve;
        public float DopplerLevel => dopplerLevel;
        public float Spread => spread;
        public AnimationCurve SpatialBlendCurve => spatialBlendCurve;
        public AnimationCurve SpreadCurve => spreadCurve;
        public AnimationCurve ReverbZoneMixCurve => reverbZoneMixCurve;

        // Reverb properties
        public float ReverbZoneMix => reverbZoneMix;
        public bool BypassEffects => bypassEffects;
        public bool BypassListenerEffects => bypassListenerEffects;
        public bool BypassReverbZones => bypassReverbZones;
        
        #endregion
        
        #region Utility

        /// <summary>
        /// Gets a random clip from the clips array.
        /// </summary>
        public AudioClip GetRandomClip()
        {
            if (clips == null || clips.Length == 0)
                return null;

            return clips[Random.Range(0, clips.Length)];
        }

        /// <summary>
        /// Gets a random volume within the volume range.
        /// </summary>
        public float GetRandomVolume()
        {
            return Random.Range(volumeRange.x, volumeRange.y);
        }

        /// <summary>
        /// Gets a random pitch within the pitch range.
        /// </summary>
        public float GetRandomPitch()
        {
            return Random.Range(pitchRange.x, pitchRange.y);
        }

        /// <summary>
        /// Checks if this cue has custom 3D curves configured.
        /// </summary>
        public bool HasCustom3DCurves =>
            useCustom3DCurves && (
                spatialBlendCurve != null && spatialBlendCurve.length > 0 ||
                spreadCurve != null && spreadCurve.length > 0 ||
                reverbZoneMixCurve != null && reverbZoneMixCurve.length > 0
            );
        
        #endregion
        
        #region Editor Methods

#if UNITY_EDITOR
        private static System.Type _audioUtilType;
        private static System.Reflection.MethodInfo _playClipMethod;
        private static System.Reflection.MethodInfo _stopClipsMethod;

        private static void EnsureAudioUtilReflection()
        {
            if (_audioUtilType == null)
            {
                var unityEditorAssembly = typeof(UnityEditor.Editor).Assembly;
                _audioUtilType = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
            }

            if (_audioUtilType != null && _playClipMethod == null)
            {
                _playClipMethod = _audioUtilType.GetMethod(
                    "PlayPreviewClip",
                    System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public,
                    null,
                    new System.Type[] { typeof(AudioClip), typeof(int), typeof(bool) },
                    null
                );
            }

            if (_audioUtilType != null && _stopClipsMethod == null)
            {
                _stopClipsMethod = _audioUtilType.GetMethod(
                    "StopAllPreviewClips",
                    System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public
                );
            }
        }

        private static void PlayClipInEditor(AudioClip clip)
        {
            EnsureAudioUtilReflection();
            _playClipMethod?.Invoke(null, new object[] { clip, 0, false });
        }

        private static void StopAllClipsInEditor()
        {
            EnsureAudioUtilReflection();
            _stopClipsMethod?.Invoke(null, null);
        }

        private void OnSpatialBlendChanged()
        {
            // Just for triggering inspector refresh
        }

        [TitleGroup("Debug Info")]
        [ShowInInspector, ReadOnly]
        [ShowIf("@clips != null && clips.Length > 0")]
        private string ClipInfo => clips != null && clips.Length > 0
            ? $"{clips.Length} clip(s), Total duration: {GetTotalDuration():F2}s"
            : "No clips assigned";

        private float GetTotalDuration()
        {
            float total = 0f;
            if (clips != null)
            {
                foreach (var clip in clips)
                {
                    if (clip != null)
                        total += clip.length;
                }
            }
            return total;
        }
#endif
        
        #endregion
    }
}
