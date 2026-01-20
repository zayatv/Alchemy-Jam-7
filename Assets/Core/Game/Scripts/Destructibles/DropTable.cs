using System.Collections.Generic;
using UnityEngine;

namespace Core.Game.Destructibles
{
    [CreateAssetMenu(fileName = "DropTable", menuName = "Core/Game/Destructibles/Drop Table")]
    public class DropTable : ScriptableObject
    {
        #region Serialized Fields

        [Tooltip("List of possible drops with their weights")]
        [SerializeField] private DropEntry[] entries;

        [Tooltip("Guaranteed drops that always occur regardless of roll")]
        [SerializeField] private DropItem[] guaranteedDrops;

        [Tooltip("Offset from center for spawning drops (randomized within range)")]
        [SerializeField] private float dropSpreadRadius = 0.5f;

        #endregion

        #region Properties

        public DropEntry[] Entries => entries;
        public DropItem[] GuaranteedDrops => guaranteedDrops;
        public float DropSpreadRadius => dropSpreadRadius;

        #endregion

        #region Public Methods

        /// <summary>
        /// Rolls the drop table and returns the selected entry.
        /// Returns null if no entries exist or if a "nothing" roll occurs.
        /// </summary>
        public DropEntry Roll()
        {
            if (entries == null || entries.Length == 0)
                return null;

            float totalWeight = 0f;
            foreach (var entry in entries)
            {
                totalWeight += entry.Weight;
            }

            if (totalWeight <= 0f)
                return null;

            float roll = Random.Range(0f, totalWeight);
            float cumulative = 0f;

            foreach (var entry in entries)
            {
                cumulative += entry.Weight;
                if (roll < cumulative)
                {
                    return entry;
                }
            }

            return entries[entries.Length - 1];
        }

        /// <summary>
        /// Gets all items to drop, including guaranteed drops and rolled drops.
        /// </summary>
        public List<DropItem> GetAllDrops()
        {
            var result = new List<DropItem>();

            if (guaranteedDrops != null)
            {
                result.AddRange(guaranteedDrops);
            }

            var rolledEntry = Roll();
            if (rolledEntry?.Items != null)
            {
                result.AddRange(rolledEntry.Items);
            }

            return result;
        }

        #endregion
    }
}
