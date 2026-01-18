using UnityEngine;

namespace Core.Systems.Grid
{
    [CreateAssetMenu(fileName = "GridConfig", menuName = "Core/Grid/Grid Config", order = 0)]
    public class GridConfig : ScriptableObject
    {
        #region Fields
        
        [Header("Grid Settings")]
        [Tooltip("The size of each tile in world units.")]
        [SerializeField] private float tileSize = 1.0f;

        [Tooltip("The origin point of the grid in tile coordinates.")]
        [SerializeField] private Vector2Int gridOrigin = Vector2Int.zero;
        
        #endregion

        #region Properties

        /// <summary>
        /// The size of each tile in world units.
        /// </summary>
        public float TileSize => tileSize;

        /// <summary>
        /// The origin point of the grid in tile coordinates.
        /// </summary>
        public Vector2Int GridOrigin => gridOrigin;
        
        #endregion

        private void OnValidate()
        {
            // Ensure tile size is positive
            if (tileSize <= 0f)
            {
                tileSize = 0.1f;
            }
        }
    }
}
