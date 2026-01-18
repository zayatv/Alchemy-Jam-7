using UnityEngine;
using Core.Systems.ServiceLocator;

namespace Core.Systems.VFX
{
    public interface IVFXService : IService
    {
        /// <summary>
        /// Spawns a VFX at the specified position with default rotation.
        /// </summary>
        /// <param name="cue">The VFX cue to spawn</param>
        /// <param name="position">World position to spawn at</param>
        /// <returns>Handle to the spawned VFX instance</returns>
        VFXHandle Spawn(VFXCue cue, Vector3 position);

        /// <summary>
        /// Spawns a VFX at the specified position and rotation.
        /// </summary>
        /// <param name="cue">The VFX cue to spawn</param>
        /// <param name="position">World position to spawn at</param>
        /// <param name="rotation">World rotation to spawn with</param>
        /// <returns>Handle to the spawned VFX instance</returns>
        VFXHandle Spawn(VFXCue cue, Vector3 position, Quaternion rotation);

        /// <summary>
        /// Spawns a VFX attached to a transform (follows the transform).
        /// </summary>
        /// <param name="cue">The VFX cue to spawn</param>
        /// <param name="parent">Transform to attach the VFX to</param>
        /// <returns>Handle to the spawned VFX instance</returns>
        VFXHandle SpawnAttached(VFXCue cue, Transform parent);

        /// <summary>
        /// Stops a playing VFX instance and returns it to the pool.
        /// </summary>
        /// <param name="handle">Handle to the VFX instance to stop</param>
        void Stop(VFXHandle handle);

        /// <summary>
        /// Stops all currently playing VFX.
        /// </summary>
        void StopAll();

        /// <summary>
        /// Stops all VFX in a specific category.
        /// </summary>
        /// <param name="category">Category name to stop</param>
        void StopCategory(string category);

        /// <summary>
        /// Pre-warms a VFX pool by creating instances.
        /// </summary>
        /// <param name="cue">The VFX cue to prewarm</param>
        /// <param name="count">Number of instances to create</param>
        void Prewarm(VFXCue cue, int count);

        /// <summary>
        /// Sets the scale of a VFX instance.
        /// </summary>
        /// <param name="handle">Handle to the VFX instance</param>
        /// <param name="scale">New scale to apply</param>
        void SetScale(VFXHandle handle, Vector3 scale);
    }
}
