using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Core.Systems.Audio
{
    public class AudioService : MonoBehaviour, IAudioService
    {
        #region Fields
        
        private int _nextHandleId = 1;
        private float _masterVolume = 1f;
        private float _musicVolume = 1f;

        // Audio source pools
        private List<AudioSource> _audioSourcePool = new List<AudioSource>();
        private Dictionary<int, AudioSource> _activeAudio = new Dictionary<int, AudioSource>();
        private Dictionary<AudioSource, int> _sourceToHandle = new Dictionary<AudioSource, int>();
        private Dictionary<AudioSource, float> _sourceBaseVolume = new Dictionary<AudioSource, float>();

        // Fade tracking
        private Dictionary<int, Coroutine> _activeFades = new Dictionary<int, Coroutine>();
        private Dictionary<string, Coroutine> _activeMixerFades = new Dictionary<string, Coroutine>();

        // Music state
        private AudioSource _musicSourceA;
        private AudioSource _musicSourceB;
        private AudioSource _currentMusicSource;
        private MusicCue _currentMusicCue;
        private bool _isMusicPaused;
        private Coroutine _musicIntroCoroutine;
        private Coroutine _musicCrossfadeCoroutine;
        
        #endregion

        #region Constants

        private const float MIN_DB = -80f;
        private const float MAX_DB = 0f;

        #endregion
        
        #region Properties

        public float MasterVolume => _masterVolume;
        public float MusicVolume => _musicVolume;
        public bool IsMusicPlaying => _currentMusicSource != null && _currentMusicSource.isPlaying;
        public bool IsMusicPaused => _isMusicPaused;
        public MusicCue CurrentMusicCue => _currentMusicCue;
        public float MusicPlaybackTime => _currentMusicSource != null ? _currentMusicSource.time : 0f;
        
        #endregion

        #region Initialization

        /// <summary>
        /// Initializes the audio source pool and music sources.
        /// </summary>
        /// <param name="poolSize">The number of audio sources to preallocate.</param>
        public void Initialize(int poolSize)
        {
            for (int i = 0; i < poolSize; i++)
            {
                CreateAudioSource();
            }

            _musicSourceA = CreateMusicSource("MusicSourceA");
            _musicSourceB = CreateMusicSource("MusicSourceB");
        }

        private AudioSource CreateMusicSource(string name)
        {
            GameObject sourceObj = new GameObject(name);
            sourceObj.transform.SetParent(transform);
            AudioSource source = sourceObj.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.spatialBlend = 0f;
            source.priority = 0;
            return source;
        }

        #endregion

        #region Sound Effects
        
        public AudioHandle Play(AudioCue cue)
        {
            if (cue == null)
            {
                Debug.LogWarning("AudioService: Attempted to play null AudioCue");
                return AudioHandle.Invalid;
            }

            AudioClip clip = cue.GetRandomClip();
            if (clip == null)
            {
                Debug.LogWarning($"AudioService: No clips assigned to AudioCue '{cue.name}'");
                return AudioHandle.Invalid;
            }

            AudioSource source = GetAudioSource();
            ConfigureAudioSource(source, cue, clip);
            source.transform.SetParent(transform);
            source.transform.localPosition = Vector3.zero;
            source.Play();

            return RegisterAudioSource(source, cue, clip);
        }

        public AudioHandle PlayAtPosition(AudioCue cue, Vector3 position)
        {
            if (cue == null)
            {
                Debug.LogWarning("AudioService: Attempted to play null AudioCue");
                return AudioHandle.Invalid;
            }

            AudioClip clip = cue.GetRandomClip();
            if (clip == null)
            {
                Debug.LogWarning($"AudioService: No clips assigned to AudioCue '{cue.name}'");
                return AudioHandle.Invalid;
            }

            AudioSource source = GetAudioSource();
            ConfigureAudioSource(source, cue, clip);
            source.transform.SetParent(transform);
            source.transform.position = position;
            source.Play();

            return RegisterAudioSource(source, cue, clip);
        }

        public AudioHandle PlayAttached(AudioCue cue, Transform parent)
        {
            if (cue == null)
            {
                Debug.LogWarning("AudioService: Attempted to play null AudioCue");
                return AudioHandle.Invalid;
            }

            if (parent == null)
            {
                Debug.LogWarning("AudioService: Attempted to play attached audio with null parent");
                return Play(cue);
            }

            AudioClip clip = cue.GetRandomClip();
            if (clip == null)
            {
                Debug.LogWarning($"AudioService: No clips assigned to AudioCue '{cue.name}'");
                return AudioHandle.Invalid;
            }

            AudioSource source = GetAudioSource();
            ConfigureAudioSource(source, cue, clip);
            source.transform.SetParent(parent);
            source.transform.localPosition = Vector3.zero;
            source.Play();

            return RegisterAudioSource(source, cue, clip);
        }

        private AudioHandle RegisterAudioSource(AudioSource source, AudioCue cue, AudioClip clip)
        {
            int handleId = _nextHandleId++;
            _activeAudio[handleId] = source;
            _sourceToHandle[source] = handleId;
            _sourceBaseVolume[source] = source.volume / _masterVolume;

            if (!cue.Loop)
            {
                StartCoroutine(ReturnSourceAfterDelay(source, clip.length));
            }

            return new AudioHandle(handleId);
        }

        public void Stop(AudioHandle handle)
        {
            if (!handle.IsValid)
                return;

            // Cancel any active fade
            if (_activeFades.TryGetValue(handle.Id, out Coroutine fadeCoroutine))
            {
                StopCoroutine(fadeCoroutine);
                _activeFades.Remove(handle.Id);
            }

            if (_activeAudio.TryGetValue(handle.Id, out AudioSource source))
            {
                source.Stop();
                ReturnAudioSource(source);
            }
        }

        public void StopAllSFX()
        {
            List<int> handleIds = new List<int>(_activeAudio.Keys);

            foreach (int handleId in handleIds)
            {
                Stop(new AudioHandle(handleId));
            }
        }

        public void StopAll()
        {
            StopAllSFX();
            StopMusic(0f);
        }

        #endregion

        #region Volume Control

        public void SetVolume(AudioHandle handle, float volume)
        {
            if (!handle.IsValid)
                return;

            if (_activeAudio.TryGetValue(handle.Id, out AudioSource source))
            {
                float clampedVolume = Mathf.Clamp01(volume);
                _sourceBaseVolume[source] = clampedVolume;
                source.volume = clampedVolume * _masterVolume;
            }
        }

        public void SetMasterVolume(float volume)
        {
            _masterVolume = Mathf.Clamp01(volume);

            // Update all active audio sources
            foreach (var kvp in _activeAudio)
            {
                if (_sourceBaseVolume.TryGetValue(kvp.Value, out float baseVolume))
                {
                    kvp.Value.volume = baseVolume * _masterVolume;
                }
            }
        }

        public void SetMixerVolume(AudioMixer mixer, string exposedParameter, float normalizedVolume)
        {
            if (mixer == null)
            {
                Debug.LogWarning("AudioService: Attempted to set volume on null mixer");
                return;
            }

            float dB = NormalizedToDecibels(normalizedVolume);
            mixer.SetFloat(exposedParameter, dB);
        }

        public float GetMixerVolume(AudioMixer mixer, string exposedParameter)
        {
            if (mixer == null)
                return 0f;

            if (mixer.GetFloat(exposedParameter, out float dB))
            {
                return DecibelsToNormalized(dB);
            }

            return 1f;
        }

        public void FadeMixerVolume(AudioMixer mixer, string exposedParameter, float targetVolume, float duration)
        {
            if (mixer == null)
            {
                Debug.LogWarning("AudioService: Attempted to fade volume on null mixer");
                return;
            }

            string fadeKey = $"{mixer.name}_{exposedParameter}";

            // Cancel any existing fade on this parameter
            if (_activeMixerFades.TryGetValue(fadeKey, out Coroutine existingFade))
            {
                StopCoroutine(existingFade);
            }

            Coroutine fadeCoroutine = StartCoroutine(FadeMixerVolumeCoroutine(mixer, exposedParameter, targetVolume, duration, fadeKey));
            _activeMixerFades[fadeKey] = fadeCoroutine;
        }

        private IEnumerator FadeMixerVolumeCoroutine(AudioMixer mixer, string exposedParameter, float targetVolume, float duration, string fadeKey)
        {
            float startVolume = GetMixerVolume(mixer, exposedParameter);
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = elapsed / duration;
                float currentVolume = Mathf.Lerp(startVolume, targetVolume, t);
                SetMixerVolume(mixer, exposedParameter, currentVolume);
                yield return null;
            }

            SetMixerVolume(mixer, exposedParameter, targetVolume);
            _activeMixerFades.Remove(fadeKey);
        }

        private float NormalizedToDecibels(float normalized)
        {
            if (normalized <= 0f)
                return MIN_DB;

            return Mathf.Clamp(20f * Mathf.Log10(normalized), MIN_DB, MAX_DB);
        }

        private float DecibelsToNormalized(float dB)
        {
            if (dB <= MIN_DB)
                return 0f;

            return Mathf.Clamp01(Mathf.Pow(10f, dB / 20f));
        }

        #endregion

        #region Fading

        public void FadeIn(AudioHandle handle, float targetVolume, float duration)
        {
            if (!handle.IsValid)
                return;

            if (_activeAudio.TryGetValue(handle.Id, out AudioSource source))
            {
                // Start from zero
                source.volume = 0f;
                StartFade(handle, source, targetVolume, duration, false);
            }
        }

        public void FadeOut(AudioHandle handle, float duration, bool stopAfterFade = true)
        {
            if (!handle.IsValid)
                return;

            if (_activeAudio.TryGetValue(handle.Id, out AudioSource source))
            {
                StartFade(handle, source, 0f, duration, stopAfterFade);
            }
        }

        public void FadeTo(AudioHandle handle, float targetVolume, float duration)
        {
            if (!handle.IsValid)
                return;

            if (_activeAudio.TryGetValue(handle.Id, out AudioSource source))
            {
                StartFade(handle, source, targetVolume, duration, false);
            }
        }

        private void StartFade(AudioHandle handle, AudioSource source, float targetVolume, float duration, bool stopAfterFade)
        {
            // Cancel any existing fade
            if (_activeFades.TryGetValue(handle.Id, out Coroutine existingFade))
            {
                StopCoroutine(existingFade);
            }

            Coroutine fadeCoroutine = StartCoroutine(FadeCoroutine(handle, source, targetVolume, duration, stopAfterFade));
            _activeFades[handle.Id] = fadeCoroutine;
        }

        private IEnumerator FadeCoroutine(AudioHandle handle, AudioSource source, float targetVolume, float duration, bool stopAfterFade)
        {
            float startVolume = _sourceBaseVolume.ContainsKey(source) ? _sourceBaseVolume[source] : source.volume / _masterVolume;
            float elapsed = 0f;

            while (elapsed < duration && source != null)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = elapsed / duration;
                float currentVolume = Mathf.Lerp(startVolume, targetVolume, t);
                _sourceBaseVolume[source] = currentVolume;
                source.volume = currentVolume * _masterVolume;
                yield return null;
            }

            if (source != null)
            {
                _sourceBaseVolume[source] = targetVolume;
                source.volume = targetVolume * _masterVolume;
            }

            _activeFades.Remove(handle.Id);

            if (stopAfterFade && targetVolume <= 0f)
            {
                Stop(handle);
            }
        }

        #endregion

        #region Music

        public AudioHandle PlayMusic(MusicCue cue)
        {
            return PlayMusicInternal(cue, 0f);
        }

        public AudioHandle PlayMusic(MusicCue cue, float fadeInDuration)
        {
            return PlayMusicInternal(cue, fadeInDuration);
        }

        private AudioHandle PlayMusicInternal(MusicCue cue, float fadeInDuration)
        {
            if (cue == null)
            {
                Debug.LogWarning("AudioService: Attempted to play null MusicCue");
                return AudioHandle.Invalid;
            }

            // Stop any current music immediately
            if (_currentMusicSource != null && _currentMusicSource.isPlaying)
            {
                StopMusicImmediate();
            }

            // Cancel any pending intro/crossfade coroutines
            if (_musicIntroCoroutine != null)
            {
                StopCoroutine(_musicIntroCoroutine);
                _musicIntroCoroutine = null;
            }

            _currentMusicCue = cue;
            _currentMusicSource = _musicSourceA;
            _isMusicPaused = false;

            ConfigureMusicSource(_currentMusicSource, cue);

            if (cue.HasIntro)
            {
                // Play intro first, then start loop
                _currentMusicSource.clip = cue.IntroClip;
                _currentMusicSource.loop = false;
                _currentMusicSource.Play();

                if (fadeInDuration > 0f)
                {
                    _currentMusicSource.volume = 0f;
                    StartCoroutine(FadeMusicSourceCoroutine(_currentMusicSource, cue.Volume * _musicVolume * _masterVolume, fadeInDuration));
                }

                _musicIntroCoroutine = StartCoroutine(PlayMusicLoopAfterIntro(cue));
            }
            else if (cue.HasLoop)
            {
                // Play loop directly
                _currentMusicSource.clip = cue.LoopClip;
                _currentMusicSource.loop = true;
                _currentMusicSource.Play();

                if (fadeInDuration > 0f)
                {
                    _currentMusicSource.volume = 0f;
                    StartCoroutine(FadeMusicSourceCoroutine(_currentMusicSource, cue.Volume * _musicVolume * _masterVolume, fadeInDuration));
                }
            }
            else
            {
                Debug.LogWarning($"AudioService: MusicCue '{cue.name}' has no clips assigned");
                return AudioHandle.Invalid;
            }

            int handleId = _nextHandleId++;
            return new AudioHandle(handleId);
        }

        private IEnumerator PlayMusicLoopAfterIntro(MusicCue cue)
        {
            yield return new WaitForSeconds(cue.IntroDuration);

            if (_currentMusicSource != null && _currentMusicCue == cue)
            {
                _currentMusicSource.clip = cue.LoopClip;
                _currentMusicSource.loop = true;
                _currentMusicSource.Play();
            }

            _musicIntroCoroutine = null;
        }

        public void StopMusic()
        {
            StopMusicImmediate();
        }

        public void StopMusic(float fadeOutDuration)
        {
            if (_currentMusicSource == null || !_currentMusicSource.isPlaying)
                return;

            if (fadeOutDuration > 0f)
            {
                StartCoroutine(StopMusicWithFade(fadeOutDuration));
            }
            else
            {
                StopMusicImmediate();
            }
        }

        private void StopMusicImmediate()
        {
            if (_currentMusicSource == null)
                return;

            _currentMusicSource.Stop();
            _currentMusicSource = null;
            _currentMusicCue = null;
            _isMusicPaused = false;
        }

        private IEnumerator StopMusicWithFade(float duration)
        {
            AudioSource sourceToStop = _currentMusicSource;

            yield return FadeMusicSourceCoroutine(sourceToStop, 0f, duration);

            sourceToStop.Stop();

            if (_currentMusicSource == sourceToStop)
            {
                _currentMusicSource = null;
                _currentMusicCue = null;
                _isMusicPaused = false;
            }
        }

        public AudioHandle CrossfadeMusic(MusicCue newCue, float crossfadeDuration)
        {
            if (newCue == null)
            {
                Debug.LogWarning("AudioService: Attempted to crossfade to null MusicCue");
                return AudioHandle.Invalid;
            }

            // Cancel any existing crossfade
            if (_musicCrossfadeCoroutine != null)
            {
                StopCoroutine(_musicCrossfadeCoroutine);
            }

            // Swap music sources
            AudioSource oldSource = _currentMusicSource;
            AudioSource newSource = (_currentMusicSource == _musicSourceA) ? _musicSourceB : _musicSourceA;

            _currentMusicCue = newCue;
            _currentMusicSource = newSource;

            ConfigureMusicSource(newSource, newCue);

            // Start new music at volume 0
            if (newCue.HasIntro)
            {
                newSource.clip = newCue.IntroClip;
                newSource.loop = false;
                _musicIntroCoroutine = StartCoroutine(PlayMusicLoopAfterIntro(newCue));
            }
            else if (newCue.HasLoop)
            {
                newSource.clip = newCue.LoopClip;
                newSource.loop = true;
            }

            newSource.volume = 0f;
            newSource.Play();

            _musicCrossfadeCoroutine = StartCoroutine(CrossfadeCoroutine(oldSource, newSource, newCue, crossfadeDuration));

            int handleId = _nextHandleId++;
            return new AudioHandle(handleId);
        }

        private IEnumerator CrossfadeCoroutine(AudioSource oldSource, AudioSource newSource, MusicCue newCue, float duration)
        {
            float elapsed = 0f;
            float oldStartVolume = oldSource != null ? oldSource.volume : 0f;
            float newTargetVolume = newCue.Volume * _musicVolume * _masterVolume;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = elapsed / duration;

                if (oldSource != null && oldSource.isPlaying)
                {
                    oldSource.volume = Mathf.Lerp(oldStartVolume, 0f, t);
                }

                if (newSource != null)
                {
                    newSource.volume = Mathf.Lerp(0f, newTargetVolume, t);
                }

                yield return null;
            }

            if (oldSource != null)
            {
                oldSource.Stop();
                oldSource.volume = 0f;
            }

            if (newSource != null)
            {
                newSource.volume = newTargetVolume;
            }

            _musicCrossfadeCoroutine = null;
        }

        public void PauseMusic()
        {
            if (_currentMusicSource != null && _currentMusicSource.isPlaying)
            {
                _currentMusicSource.Pause();
                _isMusicPaused = true;
            }
        }

        public void ResumeMusic()
        {
            if (_currentMusicSource != null && _isMusicPaused)
            {
                _currentMusicSource.UnPause();
                _isMusicPaused = false;
            }
        }

        public void SetMusicVolume(float volume)
        {
            _musicVolume = Mathf.Clamp01(volume);

            // Update current music source if playing
            if (_currentMusicSource != null && _currentMusicCue != null)
            {
                _currentMusicSource.volume = _currentMusicCue.Volume * _musicVolume * _masterVolume;
            }
        }

        private void ConfigureMusicSource(AudioSource source, MusicCue cue)
        {
            source.volume = cue.Volume * _musicVolume * _masterVolume;
            source.priority = cue.Priority;
            source.outputAudioMixerGroup = cue.MixerGroup;
            source.spatialBlend = 0f; // Music is always 2D
        }

        private IEnumerator FadeMusicSourceCoroutine(AudioSource source, float targetVolume, float duration)
        {
            float startVolume = source.volume;
            float elapsed = 0f;

            while (elapsed < duration && source != null)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = elapsed / duration;
                source.volume = Mathf.Lerp(startVolume, targetVolume, t);
                yield return null;
            }

            if (source != null)
            {
                source.volume = targetVolume;
            }
        }

        #endregion

        #region Utility

        public bool IsPlaying(AudioHandle handle)
        {
            if (!handle.IsValid)
                return false;

            if (_activeAudio.TryGetValue(handle.Id, out AudioSource source))
            {
                return source != null && source.isPlaying;
            }

            return false;
        }

        public float GetPlaybackTime(AudioHandle handle)
        {
            if (!handle.IsValid)
                return -1f;

            if (_activeAudio.TryGetValue(handle.Id, out AudioSource source))
            {
                return source != null ? source.time : -1f;
            }

            return -1f;
        }

        public void SetPlaybackTime(AudioHandle handle, float time)
        {
            if (!handle.IsValid)
                return;

            if (_activeAudio.TryGetValue(handle.Id, out AudioSource source))
            {
                if (source != null && source.clip != null)
                {
                    source.time = Mathf.Clamp(time, 0f, source.clip.length);
                }
            }
        }

        #endregion

        #region Private Methods

        private AudioSource GetAudioSource()
        {
            if (_audioSourcePool.Count > 0)
            {
                AudioSource source = _audioSourcePool[_audioSourcePool.Count - 1];
                _audioSourcePool.RemoveAt(_audioSourcePool.Count - 1);
                source.gameObject.SetActive(true);
                return source;
            }

            return CreateAudioSource();
        }

        private AudioSource CreateAudioSource()
        {
            GameObject sourceObj = new GameObject("AudioSource");
            sourceObj.transform.SetParent(transform);
            sourceObj.SetActive(false);

            AudioSource source = sourceObj.AddComponent<AudioSource>();
            source.playOnAwake = false;

            return source;
        }

        private void ConfigureAudioSource(AudioSource source, AudioCue cue, AudioClip clip)
        {
            source.clip = clip;
            float baseVolume = cue.GetRandomVolume();
            source.volume = baseVolume * _masterVolume;
            source.pitch = cue.GetRandomPitch();
            source.loop = cue.Loop;
            source.priority = cue.Priority;
            source.outputAudioMixerGroup = cue.MixerGroup;

            // Basic spatial settings
            source.spatialBlend = cue.SpatialBlend;
            source.minDistance = cue.MinDistance;
            source.maxDistance = cue.MaxDistance;
            source.rolloffMode = cue.RolloffMode;

            // Advanced 3D settings
            source.dopplerLevel = cue.DopplerLevel;
            source.spread = cue.Spread;
            source.reverbZoneMix = cue.ReverbZoneMix;

            // Bypass settings
            source.bypassEffects = cue.BypassEffects;
            source.bypassListenerEffects = cue.BypassListenerEffects;
            source.bypassReverbZones = cue.BypassReverbZones;

            // Apply custom rolloff curve if specified
            if (cue.UseCustomRolloff && cue.CustomRolloffCurve != null && cue.CustomRolloffCurve.length > 0)
            {
                source.rolloffMode = AudioRolloffMode.Custom;
                source.SetCustomCurve(AudioSourceCurveType.CustomRolloff, ScaleCurveToDistance(cue.CustomRolloffCurve, cue.MaxDistance));
            }

            // Apply custom 3D curves if configured
            if (cue.HasCustom3DCurves)
            {
                if (cue.SpatialBlendCurve != null && cue.SpatialBlendCurve.length > 0)
                {
                    source.SetCustomCurve(AudioSourceCurveType.SpatialBlend, ScaleCurveToDistance(cue.SpatialBlendCurve, cue.MaxDistance));
                }

                if (cue.SpreadCurve != null && cue.SpreadCurve.length > 0)
                {
                    source.SetCustomCurve(AudioSourceCurveType.Spread, ScaleCurveToDistance(cue.SpreadCurve, cue.MaxDistance));
                }

                if (cue.ReverbZoneMixCurve != null && cue.ReverbZoneMixCurve.length > 0)
                {
                    source.SetCustomCurve(AudioSourceCurveType.ReverbZoneMix, ScaleCurveToDistance(cue.ReverbZoneMixCurve, cue.MaxDistance));
                }
            }
        }

        private AnimationCurve ScaleCurveToDistance(AnimationCurve normalizedCurve, float maxDistance)
        {
            // Convert normalized (0-1) curve to actual distance values
            Keyframe[] keys = new Keyframe[normalizedCurve.length];
            for (int i = 0; i < normalizedCurve.length; i++)
            {
                Keyframe key = normalizedCurve[i];
                keys[i] = new Keyframe(key.time * maxDistance, key.value, key.inTangent / maxDistance, key.outTangent / maxDistance);
            }
            return new AnimationCurve(keys);
        }

        private void ReturnAudioSource(AudioSource source)
        {
            if (source == null)
                return;

            // Remove from tracking dictionaries
            if (_sourceToHandle.TryGetValue(source, out int handleId))
            {
                _activeAudio.Remove(handleId);
                _sourceToHandle.Remove(source);
                _sourceBaseVolume.Remove(source);
                _activeFades.Remove(handleId);
            }

            // Reset and return to pool
            source.Stop();
            source.clip = null;
            source.transform.SetParent(transform);
            source.transform.localPosition = Vector3.zero;
            source.gameObject.SetActive(false);

            // Reset custom curves
            source.rolloffMode = AudioRolloffMode.Logarithmic;

            _audioSourcePool.Add(source);
        }

        private IEnumerator ReturnSourceAfterDelay(AudioSource source, float delay)
        {
            yield return new WaitForSeconds(delay);

            // Check if the source is still active (hasn't been stopped manually)
            if (source != null && source.gameObject.activeSelf && _sourceToHandle.ContainsKey(source))
            {
                ReturnAudioSource(source);
            }
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
            _activeAudio.Clear();
            _sourceToHandle.Clear();
            _sourceBaseVolume.Clear();
            _audioSourcePool.Clear();
            _activeFades.Clear();
            _activeMixerFades.Clear();
        }

        #endregion
    }
}
