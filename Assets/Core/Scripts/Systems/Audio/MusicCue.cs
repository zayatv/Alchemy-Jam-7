using UnityEngine;
using UnityEngine.Audio;
using Sirenix.OdinInspector;

namespace Core.Systems.Audio
{
    [CreateAssetMenu(fileName = "NewMusicCue", menuName = "Core/Audio/Music Cue")]
    public class MusicCue : ScriptableObject
    {
        #region Fields
        
        [TitleGroup("Music Clips")]
        [HorizontalGroup("Music Clips/Clips")]
        [VerticalGroup("Music Clips/Clips/Intro")]
        [SerializeField]
        [AssetsOnly]
        [LabelText("Intro Clip")]
        [Tooltip("Optional intro clip that plays once before the loop")]
        [PreviewField(60, ObjectFieldAlignment.Center)]
        private AudioClip introClip;

        [VerticalGroup("Music Clips/Clips/Loop")]
        [SerializeField]
        [AssetsOnly]
        [Required("A loop clip is required for music playback")]
        [LabelText("Loop Clip")]
        [Tooltip("Main music clip that loops")]
        [PreviewField(60, ObjectFieldAlignment.Center)]
        private AudioClip loopClip;

        [TitleGroup("Music Clips")]
        [ButtonGroup("Music Clips/Buttons")]
        [Button("Preview Intro", ButtonSizes.Medium)]
        [EnableIf("HasIntro")]
        private void PreviewIntro()
        {
#if UNITY_EDITOR
            if (introClip != null)
            {
                PlayClipInEditor(introClip);
            }
#endif
        }

        [ButtonGroup("Music Clips/Buttons")]
        [Button("Preview Loop", ButtonSizes.Medium)]
        [EnableIf("HasLoop")]
        private void PreviewLoop()
        {
#if UNITY_EDITOR
            if (loopClip != null)
            {
                PlayClipInEditor(loopClip);
            }
#endif
        }

        [ButtonGroup("Music Clips/Buttons")]
        [Button("Stop", ButtonSizes.Medium)]
        private void StopPreview()
        {
#if UNITY_EDITOR
            StopAllClipsInEditor();
#endif
        }

        [TitleGroup("Volume & Output")]
        [HorizontalGroup("Volume & Output/Main")]
        [SerializeField]
        [PropertyRange(0f, 1f)]
        [LabelText("Volume")]
        [Tooltip("Base volume of the music")]
        private float volume = 1f;

        [HorizontalGroup("Volume & Output/Main")]
        [SerializeField]
        [LabelText("Mixer Group")]
        [Tooltip("Audio mixer group for music routing")]
        private AudioMixerGroup mixerGroup;

        [TitleGroup("Playback Settings")]
        [HorizontalGroup("Playback Settings/Priority")]
        [SerializeField]
        [PropertyRange(0, 256)]
        [LabelText("Priority")]
        [Tooltip("Priority (0 = highest, typically music should be high priority)")]
        private int priority = 32;
        
        #endregion
        
        #region Properties

        public AudioClip IntroClip => introClip;
        public AudioClip LoopClip => loopClip;
        public float Volume => volume;
        public AudioMixerGroup MixerGroup => mixerGroup;
        public int Priority => priority;

        /// <summary>
        /// Whether this music cue has an intro section.
        /// </summary>
        public bool HasIntro => introClip != null;

        /// <summary>
        /// Whether this music cue has a valid loop section.
        /// </summary>
        public bool HasLoop => loopClip != null;

        /// <summary>
        /// Total duration of the intro clip, or 0 if no intro.
        /// </summary>
        public float IntroDuration => introClip != null ? introClip.length : 0f;

        /// <summary>
        /// Duration of one loop iteration.
        /// </summary>
        public float LoopDuration => loopClip != null ? loopClip.length : 0f;
        
        #endregion
        
        #region Editor Utility

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

        private void PlayClipInEditor(AudioClip clip)
        {
            EnsureAudioUtilReflection();
            _playClipMethod?.Invoke(null, new object[] { clip, 0, false });
        }

        private static void StopAllClipsInEditor()
        {
            EnsureAudioUtilReflection();
            _stopClipsMethod?.Invoke(null, null);
        }

        [TitleGroup("Track Info")]
        [ShowInInspector, ReadOnly]
        [PropertyOrder(100)]
        private string TrackInfo
        {
            get
            {
                string info = "";

                if (HasIntro)
                    info += $"Intro: {IntroDuration:F2}s";

                if (HasLoop)
                {
                    if (!string.IsNullOrEmpty(info)) info += " | ";
                    info += $"Loop: {LoopDuration:F2}s";
                }

                if (HasIntro || HasLoop)
                {
                    info += $" | Total: {IntroDuration + LoopDuration:F2}s";
                }

                return string.IsNullOrEmpty(info) ? "No clips assigned" : info;
            }
        }
#endif
        
        #endregion
    }
}
