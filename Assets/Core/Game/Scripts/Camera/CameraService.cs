using UnityEngine;
using Core.Game.Camera.Data;
using Core.Game.Movement.Movement;
using Core.Systems.Update;
using Core.Systems.Logging;
using DG.Tweening;

namespace Core.Game.Camera
{
    public class CameraService : ICameraService, ILateUpdatable
    {
        #region Fields
        
        private readonly CameraConfig _config;
        private readonly CameraController _cameraController;

        private Transform _targetTransform;
        private MovementController _targetMovementController;

        // Current state
        private Vector3 _currentPosition;
        private Quaternion _currentRotation;
        private Vector3 _currentLookAheadOffset;
        private Vector3 _currentVelocity; // For SmoothDamp
        private float _currentZoomDistance;
        private float _currentZoomVelocity; // For zoom smoothing
        private Vector3 _lookAheadVelocity; // For look-ahead smoothing

        // Runtime overrides
        private float _lookAheadMultiplier = 1f;
        private float _smoothingSpeedMultiplier = 1f;
        private float? _zoomOverride = null;
        
        #endregion
        
        #region Properties

        public int LateUpdatePriority => 100; // Run after player movement (priority 0)

        public UnityEngine.Camera MainCamera => _cameraController != null ? _cameraController.Camera : null;
        public Transform CameraTransform => _cameraController != null ? _cameraController.transform : null;
        
        #endregion

        public CameraService(CameraConfig config, CameraController cameraController)
        {
            _config = config;
            _cameraController = cameraController;

            if (_cameraController != null)
            {
                _currentPosition = _cameraController.transform.position;
                _currentRotation = _cameraController.transform.rotation;
                _currentZoomDistance = _config.IdleZoomDistance;
            }

            GameLogger.Log(LogLevel.Debug, "[CameraService] Camera service created");
        }

        public void OnLateUpdate(float deltaTime)
        {
            if (_targetTransform == null || _targetMovementController == null || _cameraController == null)
            {
                return;
            }

            var data = _targetMovementController.Data;

            Vector3 horizontalLookAhead = CalculateHorizontalLookAhead(data);
            Vector3 verticalLookAhead = CalculateVerticalLookAhead(data);
            Vector3 targetLookAheadOffset = horizontalLookAhead + verticalLookAhead;

            // Smooth look-ahead offset
            _currentLookAheadOffset = Vector3.SmoothDamp(
                _currentLookAheadOffset,
                targetLookAheadOffset,
                ref _lookAheadVelocity,
                _config.LookAheadSmoothTime / _smoothingSpeedMultiplier,
                Mathf.Infinity,
                deltaTime
            );

            // Calculate zoom distance
            float targetZoomDistance = GetZoomDistanceForPlayerState();

            // Smooth zoom distance
            _currentZoomDistance = Mathf.SmoothDamp(
                _currentZoomDistance,
                targetZoomDistance,
                ref _currentZoomVelocity,
                _config.ZoomSmoothTime / _smoothingSpeedMultiplier,
                Mathf.Infinity,
                deltaTime
            );

            // Calculate final camera position
            Vector3 targetPosition = CalculateCameraPosition(
                _targetTransform.position,
                _currentLookAheadOffset,
                _currentZoomDistance
            );

            // Smooth camera position
            _currentPosition = Vector3.SmoothDamp(
                _currentPosition,
                targetPosition,
                ref _currentVelocity,
                _config.PositionSmoothTime / _smoothingSpeedMultiplier,
                Mathf.Infinity,
                deltaTime
            );

            // Calculate camera rotation
            Quaternion targetRotation = Quaternion.Euler(_config.CameraAngle);
            _currentRotation = Quaternion.Slerp(
                _currentRotation,
                targetRotation,
                _config.RotationSmoothSpeed * deltaTime
            );

            // Apply to camera controller
            _cameraController.SetPositionAndRotation(_currentPosition, _currentRotation);
        }

        /// <summary>
        /// Calculates the horizontal look-ahead vector based on the player's movement direction and configuration settings.
        /// </summary>
        /// <param name="data">An object containing the movement data of the player, including world movement direction.</param>
        /// <returns>A Vector3 representing the horizontal look-ahead offset.</returns>
        private Vector3 CalculateHorizontalLookAhead(Core.Game.Movement.Data.MovementData data)
        {
            Vector3 inputDirection = data.GetWorldMoveDirection();

            if (inputDirection.sqrMagnitude < 0.01f)
            {
                return Vector3.zero;
            }

            return inputDirection * _config.LookAheadDistance * _lookAheadMultiplier;
        }

        /// <summary>
        /// Calculates the vertical look-ahead vector based on the player's movement state and configuration settings.
        /// </summary>
        /// <param name="data">An object containing the movement data of the player, including information about grounded, rising, or falling states.</param>
        /// <returns>A Vector3 representing the vertical look-ahead offset.</returns>
        private Vector3 CalculateVerticalLookAhead(Core.Game.Movement.Data.MovementData data)
        {
            if (data.IsGrounded)
            {
                return Vector3.zero;
            }

            if (data.IsRising)
            {
                return Vector3.up * _config.VerticalLookAheadUp;
            }
            else if (data.IsFalling)
            {
                return Vector3.down * _config.VerticalLookAheadDown;
            }

            return Vector3.zero;
        }

        /// <summary>
        /// Determines the appropriate camera zoom distance based on the player's current state, such as movement speed or airborne status.
        /// </summary>
        /// <returns>A float representing the calculated camera zoom distance for the current player state.</returns>
        private float GetZoomDistanceForPlayerState()
        {
            // Manual override takes priority
            if (_zoomOverride.HasValue)
            {
                return _zoomOverride.Value;
            }

            // Airborne zoom
            if (!_targetMovementController.IsGrounded)
            {
                return _config.AirborneZoomDistance;
            }

            // Speed-based zoom (idle to sprint)
            float normalizedSpeed = _targetMovementController.GetNormalizedMovementSpeed();
            return Mathf.Lerp(_config.IdleZoomDistance, _config.SprintZoomDistance, normalizedSpeed);
        }

        /// <summary>
        /// Calculates the camera's position based on the target's position,
        /// a specified look-ahead offset, and the current zoom distance.
        /// </summary>
        /// <param name="targetPos">The world position of the target to focus on.</param>
        /// <param name="lookAheadOffset">The offset applied to the target position for look-ahead adjustment.</param>
        /// <param name="zoomDistance">The distance from the target at which the camera should be positioned.</param>
        /// <returns>A Vector3 representing the calculated position of the camera.</returns>
        private Vector3 CalculateCameraPosition(Vector3 targetPos, Vector3 lookAheadOffset, float zoomDistance)
        {
            Vector3 focusPoint = targetPos + lookAheadOffset;
            Quaternion angleRotation = Quaternion.Euler(_config.CameraAngle);
            Vector3 cameraOffset = angleRotation * (Vector3.back * zoomDistance);

            return focusPoint + cameraOffset;
        }

        /// <summary>
        /// Sets the target transform for the camera to follow and configures related components.
        /// </summary>
        /// <param name="target">The transform of the target object the camera should follow. Pass null to clear the current target.</param>
        public void SetTarget(Transform target)
        {
            _targetTransform = target;

            if (target != null)
            {
                _targetMovementController = target.GetComponent<MovementController>();

                if (_targetMovementController == null)
                {
                    GameLogger.Log(LogLevel.Warning, "[CameraService] Target does not have MovementController component");
                }
                else
                {
                    // Initialize camera position to avoid jump
                    if (_cameraController != null)
                    {
                        _currentPosition = CalculateCameraPosition(
                            target.position,
                            Vector3.zero,
                            _config.IdleZoomDistance
                        );
                        _cameraController.SetPosition(_currentPosition);
                    }

                    GameLogger.Log(LogLevel.Debug, "[CameraService] Camera target set successfully");
                }
            }
            else
            {
                _targetMovementController = null;
                GameLogger.Log(LogLevel.Debug, "[CameraService] Camera target cleared");
            }
        }

        /// <summary>
        /// Retrieves the current target that the camera is following.
        /// </summary>
        /// <returns>A Transform object representing the camera's target, or null if no target is set.</returns>
        public Transform GetTarget()
        {
            return _targetTransform;
        }

        /// <summary>
        /// Sets the multiplier used to adjust the intensity of the look-ahead behavior for the camera, ensuring the value stays non-negative.
        /// </summary>
        /// <param name="multiplier">The look-ahead multiplier to be applied. Values less than zero will be clamped to zero.</param>
        public void SetLookAheadMultiplier(float multiplier)
        {
            _lookAheadMultiplier = Mathf.Max(0f, multiplier);
            GameLogger.Log(LogLevel.Debug, $"[CameraService] Look-ahead multiplier set to {_lookAheadMultiplier}");
        }

        /// <summary>
        /// Adjusts the smoothing speed multiplier used in the camera service to control how smoothly the camera follows its target.
        /// </summary>
        /// <param name="multiplier">A float value representing the new smoothing speed multiplier. Must be greater than or equal to 0.01f.</param>
        public void SetSmoothingSpeed(float multiplier)
        {
            _smoothingSpeedMultiplier = Mathf.Max(0.01f, multiplier);
            GameLogger.Log(LogLevel.Debug, $"[CameraService] Smoothing speed multiplier set to {_smoothingSpeedMultiplier}");
        }

        /// <summary>
        /// Overrides the camera's zoom distance with the specified value, ensuring it remains above the minimum threshold.
        /// </summary>
        /// <param name="distance">The desired zoom distance. Values less than 1 are clamped to a minimum of 1.</param>
        public void OverrideZoomDistance(float distance)
        {
            _zoomOverride = Mathf.Max(1f, distance);
            GameLogger.Log(LogLevel.Debug, $"[CameraService] Zoom distance overridden to {_zoomOverride}");
        }

        /// <summary>
        /// Resets the zoom override setting, allowing the camera to return to its default zoom behavior.
        /// </summary>
        public void ResetZoomOverride()
        {
            _zoomOverride = null;
            GameLogger.Log(LogLevel.Debug, "[CameraService] Zoom override reset");
        }

        /// <summary>
        /// Retrieves the target position for the camera, including adjustments based on look-ahead offsets and zoom distance.
        /// </summary>
        /// <returns>A Vector3 representing the calculated target position for the camera.</returns>
        public Vector3 GetTargetPosition()
        {
            if (_targetTransform == null)
            {
                return _currentPosition;
            }

            return CalculateCameraPosition(
                _targetTransform.position,
                _currentLookAheadOffset,
                _currentZoomDistance
            );
        }

        /// <summary>
        /// Determines whether the camera is currently following a target.
        /// </summary>
        /// <returns>True if a target is being followed; otherwise, false.</returns>
        public bool IsFollowingTarget()
        {
            return _targetTransform != null && _targetMovementController != null;
        }

        /// <inheritdoc />
        public void ShakeCamera(float duration, float strength = 1f, int vibrato = 10, float randomness = 90f, bool fadeOut = true)
        {
            if (_cameraController == null) return;
            
            _cameraController.transform.DOShakePosition(duration, strength, vibrato, randomness, fadeOut);
        }
    }
}
