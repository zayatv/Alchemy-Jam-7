using UnityEngine;

namespace Core.Game.Camera.Data
{
    [CreateAssetMenu(fileName = "CameraConfig", menuName = "Core/Game/Camera/Camera Config")]
    public class CameraConfig : ScriptableObject
    {
        [Header("Camera Positioning")]
        [Tooltip("Top-down camera angle (pitch, yaw, roll)")]
        [SerializeField] private Vector3 cameraAngle = new Vector3(60f, 0f, 0f);

        [Header("Look-Ahead (Hollow Knight Style)")]
        [Tooltip("Horizontal look-ahead distance in movement direction (3-5 recommended)")]
        [SerializeField, Range(0f, 10f)] private float lookAheadDistance = 4f;

        [Tooltip("How fast look-ahead responds to direction changes")]
        [SerializeField] private float lookAheadSmoothTime = 0.3f;

        [Header("Vertical Look-Ahead")]
        [Tooltip("Upward camera shift when jumping")]
        [SerializeField] private float verticalLookAheadUp = 2f;

        [Tooltip("Downward camera shift when falling")]
        [SerializeField] private float verticalLookAheadDown = 1.5f;

        [Header("Dynamic Zoom")]
        [Tooltip("Camera distance when player is idle")]
        [SerializeField] private float idleZoomDistance = 8f;

        [Tooltip("Camera distance when player is sprinting")]
        [SerializeField] private float sprintZoomDistance = 12f;

        [Tooltip("Camera distance when player is airborne")]
        [SerializeField] private float airborneZoomDistance = 10f;

        [Tooltip("How fast zoom adjusts to speed changes")]
        [SerializeField] private float zoomSmoothTime = 0.5f;

        [Header("Smoothing")]
        [Tooltip("Camera position smoothing time (lower = faster, more responsive)")]
        [SerializeField] private float positionSmoothTime = 0.15f;

        [Tooltip("Camera rotation smoothing speed")]
        [SerializeField] private float rotationSmoothSpeed = 5f;

        // Properties
        public Vector3 CameraAngle => cameraAngle;
        public float LookAheadDistance => lookAheadDistance;
        public float LookAheadSmoothTime => lookAheadSmoothTime;
        public float VerticalLookAheadUp => verticalLookAheadUp;
        public float VerticalLookAheadDown => verticalLookAheadDown;
        public float IdleZoomDistance => idleZoomDistance;
        public float SprintZoomDistance => sprintZoomDistance;
        public float AirborneZoomDistance => airborneZoomDistance;
        public float ZoomSmoothTime => zoomSmoothTime;
        public float PositionSmoothTime => positionSmoothTime;
        public float RotationSmoothSpeed => rotationSmoothSpeed;
    }
}
