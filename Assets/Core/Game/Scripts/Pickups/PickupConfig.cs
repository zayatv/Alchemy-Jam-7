using UnityEngine;

namespace Core.Game.Pickups
{
    [CreateAssetMenu(fileName = "PickupConfig", menuName = "Core/Game/Pickups/Pickup Config")]
    public class PickupConfig : ScriptableObject
    {
        #region Serialized Fields

        [Header("Health Pickup")]
        [Tooltip("Prefab for health pickup")]
        [SerializeField] private Pickup healthPickupPrefab;

        [Header("Upgrade Pickup")]
        [Tooltip("Prefab for upgrade pickup")]
        [SerializeField] private Pickup upgradePickupPrefab;

        [Header("Bomb Pickup")]
        [Tooltip("Prefab for bomb pickup")]
        [SerializeField] private Pickup bombPickupPrefab;

        [Header("Pooling")]
        [Tooltip("Number of each pickup type to prewarm in pool")]
        [SerializeField] private int prewarmCount = 5;

        #endregion

        #region Properties

        public Pickup HealthPickupPrefab => healthPickupPrefab;
        public Pickup UpgradePickupPrefab => upgradePickupPrefab;
        public Pickup BombPickupPrefab => bombPickupPrefab;
        public int PrewarmCount => prewarmCount;

        #endregion
    }
}
