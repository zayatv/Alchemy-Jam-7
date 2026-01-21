using UnityEngine;
using Core.Systems.ServiceLocator;

namespace Core.Game.Camera
{
    /// <summary>
    /// Service interface for camera control and management.
    /// Provides access to camera state and control methods.
    /// </summary>
    public interface ICameraService : IService
    {
        /// <summary>
        /// Access to the main Unity Camera component.
        /// </summary>
        UnityEngine.Camera MainCamera { get; }

        /// <summary>
        /// Access to the camera's Transform component.
        /// </summary>
        Transform CameraTransform { get; }

        /// <summary>
        /// Set the target for the camera to follow.
        /// </summary>
        /// <param name="target">Transform of the target (e.g., player)</param>
        void SetTarget(Transform target);

        /// <summary>
        /// Get the current follow target.
        /// </summary>
        /// <returns>Current target Transform, or null if no target is set</returns>
        Transform GetTarget();

        /// <summary>
        /// Set a multiplier for the look-ahead distance (runtime adjustment).
        /// </summary>
        /// <param name="multiplier">Multiplier value (1.0 = normal, 0.5 = half distance, 2.0 = double distance)</param>
        void SetLookAheadMultiplier(float multiplier);

        /// <summary>
        /// Set a multiplier for the smoothing speed (runtime adjustment).
        /// </summary>
        /// <param name="multiplier">Multiplier value (1.0 = normal, higher = faster smoothing)</param>
        void SetSmoothingSpeed(float multiplier);

        /// <summary>
        /// Override the automatic zoom distance with a manual value.
        /// </summary>
        /// <param name="distance">Desired camera distance</param>
        void OverrideZoomDistance(float distance);

        /// <summary>
        /// Reset zoom distance override and return to automatic state-based zoom.
        /// </summary>
        void ResetZoomOverride();

        /// <summary>
        /// Get the calculated target position the camera is moving towards.
        /// </summary>
        /// <returns>Target position in world space</returns>
        Vector3 GetTargetPosition();

        /// <summary>
        /// Check if the camera is currently following a target.
        /// </summary>
        /// <returns>True if following a target, false otherwise</returns>
        bool IsFollowingTarget();

        /// <summary>
        /// Apply a camera shake effect.
        /// </summary>
        /// <param name="duration">Duration of the shake in seconds.</param>
        /// <param name="strength">Strength of the shake (default 1).</param>
        /// <param name="vibrato">How much the camera shakes (default 10).</param>
        /// <param name="randomness">Randomness of the shake (default 90).</param>
        /// <param name="fadeOut">If true, the shake will fade out.</param>
        void ShakeCamera(float duration, float strength = 1f, int vibrato = 10, float randomness = 90f, bool fadeOut = true);
    }
}
