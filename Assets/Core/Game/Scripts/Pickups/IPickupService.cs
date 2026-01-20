using Core.Game.Upgrades;
using Core.Systems.ServiceLocator;
using UnityEngine;

namespace Core.Game.Pickups
{
    public interface IPickupService : IService
    {
        /// <summary>
        /// Spawns a health pickup at the specified position.
        /// </summary>
        /// <param name="position">World position to spawn at.</param>
        /// <param name="rotation">Rotation of the spawned pickup.</param>
        /// <returns>The spawned health pickup, or null if spawning failed.</returns>
        Pickup SpawnHealthPickup(Vector3 position, Quaternion rotation);

        /// <summary>
        /// Spawns an upgrade pickup at the specified position with a random upgrade.
        /// </summary>
        /// <param name="position">World position to spawn at.</param>
        /// <param name="rotation">Rotation of the spawned pickup.</param>
        /// <returns>The spawned upgrade pickup, or null if spawning failed.</returns>
        Pickup SpawnUpgradePickup(Vector3 position, Quaternion rotation);

        /// <summary>
        /// Spawns an upgrade pickup at the specified position with a specific upgrade.
        /// </summary>
        /// <param name="position">World position to spawn at.</param>
        /// <param name="rotation">Rotation of the spawned pickup.</param>
        /// <param name="upgrade">The specific upgrade to assign.</param>
        /// <returns>The spawned upgrade pickup, or null if spawning failed.</returns>
        Pickup SpawnUpgradePickup(Vector3 position, Quaternion rotation, UpgradeDefinition upgrade);

        /// <summary>
        /// Spawns a bomb pickup at the specified position.
        /// </summary>
        /// <param name="position">World position to spawn at.</param>
        /// <param name="rotation">Rotation of the spawned pickup.</param>
        /// <returns>The spawned bomb pickup, or null if spawning failed.</returns>
        Pickup SpawnBombPickup(Vector3 position, Quaternion rotation);

        /// <summary>
        /// Despawns a pickup and returns it to the pool.
        /// </summary>
        /// <param name="pickup">The pickup to despawn.</param>
        void DespawnPickup(Pickup pickup);
    }
}
