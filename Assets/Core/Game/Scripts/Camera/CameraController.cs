using UnityEngine;

namespace Core.Game.Camera
{
    [RequireComponent(typeof(UnityEngine.Camera))]
    public class CameraController : MonoBehaviour
    {
        private UnityEngine.Camera _camera;

        /// <summary>
        /// Access to the Unity Camera component.
        /// </summary>
        public UnityEngine.Camera Camera => _camera;

        private void Awake()
        {
            _camera = GetComponent<UnityEngine.Camera>();
        }

        /// <summary>
        /// Set camera position directly (called by CameraService).
        /// </summary>
        /// <param name="position">World space position</param>
        public void SetPosition(Vector3 position)
        {
            transform.position = position;
        }

        /// <summary>
        /// Set camera rotation directly (called by CameraService).
        /// </summary>
        /// <param name="rotation">World space rotation</param>
        public void SetRotation(Quaternion rotation)
        {
            transform.rotation = rotation;
        }

        /// <summary>
        /// Set both position and rotation (convenience method).
        /// </summary>
        /// <param name="position">World space position</param>
        /// <param name="rotation">World space rotation</param>
        public void SetPositionAndRotation(Vector3 position, Quaternion rotation)
        {
            transform.SetPositionAndRotation(position, rotation);
        }
    }
}
