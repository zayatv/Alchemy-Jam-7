using UnityEngine;
using UnityEngine.Audio;
using Core.Systems.ServiceLocator;

namespace Core.Systems.Audio
{
    public interface IAudioService : IService
    {
        #region Sound Effects

        /// <summary>
        /// Plays an audio cue as a 2D sound.
        /// </summary>
        /// <param name="cue">The audio cue to play</param>
        /// <returns>Handle to the playing audio instance</returns>
        AudioHandle Play(AudioCue cue);

        /// <summary>
        /// Plays an audio cue at a specific world position (3D sound).
        /// </summary>
        /// <param name="cue">The audio cue to play</param>
        /// <param name="position">World position to play the sound at</param>
        /// <returns>Handle to the playing audio instance</returns>
        AudioHandle PlayAtPosition(AudioCue cue, Vector3 position);

        /// <summary>
        /// Plays an audio cue attached to a transform (follows the transform).
        /// </summary>
        /// <param name="cue">The audio cue to play</param>
        /// <param name="parent">Transform to attach the audio to</param>
        /// <returns>Handle to the playing audio instance</returns>
        AudioHandle PlayAttached(AudioCue cue, Transform parent);

        /// <summary>
        /// Stops a playing audio instance.
        /// </summary>
        /// <param name="handle">Handle to the audio instance to stop</param>
        void Stop(AudioHandle handle);

        /// <summary>
        /// Stops all currently playing sound effects (not music).
        /// </summary>
        void StopAllSFX();

        /// <summary>
        /// Stops all currently playing audio (sounds and music).
        /// </summary>
        void StopAll();

        #endregion

        #region Volume Control

        /// <summary>
        /// Sets the volume of a playing audio instance.
        /// </summary>
        /// <param name="handle">Handle to the audio instance</param>
        /// <param name="volume">Volume (0-1)</param>
        void SetVolume(AudioHandle handle, float volume);

        /// <summary>
        /// Sets the master volume for all audio.
        /// </summary>
        /// <param name="volume">Master volume (0-1)</param>
        void SetMasterVolume(float volume);

        /// <summary>
        /// Gets the current master volume.
        /// </summary>
        float MasterVolume { get; }

        /// <summary>
        /// Sets the volume of a specific mixer group via exposed parameter.
        /// </summary>
        /// <param name="mixer">The audio mixer containing the parameter</param>
        /// <param name="exposedParameter">Name of the exposed parameter (typically in dB)</param>
        /// <param name="normalizedVolume">Volume (0-1) which will be converted to dB</param>
        void SetMixerVolume(AudioMixer mixer, string exposedParameter, float normalizedVolume);

        /// <summary>
        /// Gets the volume of a specific mixer group via exposed parameter.
        /// </summary>
        /// <param name="mixer">The audio mixer containing the parameter</param>
        /// <param name="exposedParameter">Name of the exposed parameter</param>
        /// <returns>Normalized volume (0-1)</returns>
        float GetMixerVolume(AudioMixer mixer, string exposedParameter);

        /// <summary>
        /// Fades a mixer parameter volume over time.
        /// </summary>
        /// <param name="mixer">The audio mixer containing the parameter</param>
        /// <param name="exposedParameter">Name of the exposed parameter</param>
        /// <param name="targetVolume">Target normalized volume (0-1)</param>
        /// <param name="duration">Fade duration in seconds</param>
        void FadeMixerVolume(AudioMixer mixer, string exposedParameter, float targetVolume, float duration);

        #endregion

        #region Fading

        /// <summary>
        /// Fades in a playing audio instance.
        /// </summary>
        /// <param name="handle">Handle to the audio instance</param>
        /// <param name="targetVolume">Target volume (0-1)</param>
        /// <param name="duration">Fade duration in seconds</param>
        void FadeIn(AudioHandle handle, float targetVolume, float duration);

        /// <summary>
        /// Fades out a playing audio instance.
        /// </summary>
        /// <param name="handle">Handle to the audio instance</param>
        /// <param name="duration">Fade duration in seconds</param>
        /// <param name="stopAfterFade">Whether to stop the audio after fade completes</param>
        void FadeOut(AudioHandle handle, float duration, bool stopAfterFade = true);

        /// <summary>
        /// Fades the volume of a playing audio instance to a target.
        /// </summary>
        /// <param name="handle">Handle to the audio instance</param>
        /// <param name="targetVolume">Target volume (0-1)</param>
        /// <param name="duration">Fade duration in seconds</param>
        void FadeTo(AudioHandle handle, float targetVolume, float duration);

        #endregion

        #region Music

        /// <summary>
        /// Plays a music cue immediately without fade.
        /// </summary>
        /// <param name="cue">The music cue to play</param>
        /// <returns>Handle to the playing music instance</returns>
        AudioHandle PlayMusic(MusicCue cue);

        /// <summary>
        /// Plays a music cue with fade in.
        /// </summary>
        /// <param name="cue">The music cue to play</param>
        /// <param name="fadeInDuration">Fade in duration in seconds</param>
        /// <returns>Handle to the playing music instance</returns>
        AudioHandle PlayMusic(MusicCue cue, float fadeInDuration);

        /// <summary>
        /// Stops the currently playing music immediately without fade.
        /// </summary>
        void StopMusic();

        /// <summary>
        /// Stops the currently playing music with fade out.
        /// </summary>
        /// <param name="fadeOutDuration">Fade out duration in seconds</param>
        void StopMusic(float fadeOutDuration);

        /// <summary>
        /// Crossfades from the current music to a new music cue.
        /// </summary>
        /// <param name="newCue">The new music cue to crossfade to</param>
        /// <param name="crossfadeDuration">Crossfade duration in seconds</param>
        /// <returns>Handle to the new music instance</returns>
        AudioHandle CrossfadeMusic(MusicCue newCue, float crossfadeDuration);

        /// <summary>
        /// Pauses the currently playing music.
        /// </summary>
        void PauseMusic();

        /// <summary>
        /// Resumes the currently paused music.
        /// </summary>
        void ResumeMusic();

        /// <summary>
        /// Sets the music volume (separate from master volume).
        /// </summary>
        /// <param name="volume">Music volume (0-1)</param>
        void SetMusicVolume(float volume);

        /// <summary>
        /// Gets the current music volume.
        /// </summary>
        float MusicVolume { get; }

        /// <summary>
        /// Gets whether music is currently playing.
        /// </summary>
        bool IsMusicPlaying { get; }

        /// <summary>
        /// Gets whether music is currently paused.
        /// </summary>
        bool IsMusicPaused { get; }

        /// <summary>
        /// Gets the currently playing music cue, or null if none.
        /// </summary>
        MusicCue CurrentMusicCue { get; }

        /// <summary>
        /// Gets the current playback time of the music.
        /// </summary>
        float MusicPlaybackTime { get; }

        #endregion

        #region Utility

        /// <summary>
        /// Checks if a specific audio handle is still playing.
        /// </summary>
        /// <param name="handle">Handle to check</param>
        /// <returns>True if the audio is still playing</returns>
        bool IsPlaying(AudioHandle handle);

        /// <summary>
        /// Gets the current playback time of an audio instance.
        /// </summary>
        /// <param name="handle">Handle to the audio instance</param>
        /// <returns>Current time in seconds, or -1 if invalid</returns>
        float GetPlaybackTime(AudioHandle handle);

        /// <summary>
        /// Sets the playback time of an audio instance.
        /// </summary>
        /// <param name="handle">Handle to the audio instance</param>
        /// <param name="time">Time in seconds</param>
        void SetPlaybackTime(AudioHandle handle, float time);

        #endregion
    }
}
