using UnityEngine;

namespace Core.Systems.Grid
{
    public class GridService : IGridService
    {
        #region Fields
        
        private readonly GridConfig _config;
        
        #endregion
        
        #region Properties
        
        public float TileSize => _config.TileSize;
        
        #endregion

        /// <summary>
        /// Creates a new GridService with the specified configuration.
        /// </summary>
        /// <param name="config">The grid configuration to use.</param>
        public GridService(GridConfig config)
        {
            _config = config;
        }

        public TileCoordinate WorldToTile(Vector3 worldPosition)
        {
            return TileCoordinate.FromWorldPosition(worldPosition, _config.TileSize);
        }
        
        public Vector3 TileToWorld(TileCoordinate tile)
        {
            return tile.ToWorldPosition(_config.TileSize);
        }
        
        public Vector3 TileToCenteredWorld(TileCoordinate tile)
        {
            return tile.ToCenteredWorldPosition(_config.TileSize);
        }

        public TileCoordinate[] GetTilesInPattern(ITilePattern pattern, TileCoordinate origin)
        {
            return pattern.GetTilesInPattern(origin);
        }
    }
}
