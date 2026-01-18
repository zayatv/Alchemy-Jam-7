using Core.Systems.ServiceLocator;
using UnityEngine;

namespace Core.Systems.Grid
{
    public interface IGridService : IService
    {
        /// <summary>
        /// The size of each tile in world units.
        /// </summary>
        float TileSize { get; }

        /// <summary>
        /// Converts a world position to a tile coordinate.
        /// </summary>
        /// <param name="worldPosition">The world position to convert.</param>
        /// <returns>The tile coordinate containing the world position.</returns>
        TileCoordinate WorldToTile(Vector3 worldPosition);

        /// <summary>
        /// Converts a tile coordinate to a world position at the corner of the tile.
        /// </summary>
        /// <param name="tile">The tile coordinate to convert.</param>
        /// <returns>The world position at the corner of the tile.</returns>
        Vector3 TileToWorld(TileCoordinate tile);

        /// <summary>
        /// Converts a tile coordinate to a world position at the center of the tile.
        /// Use this for consistent object placement.
        /// </summary>
        /// <param name="tile">The tile coordinate to convert.</param>
        /// <returns>The world position at the center of the tile.</returns>
        Vector3 TileToCenteredWorld(TileCoordinate tile);

        /// <summary>
        /// Retrieves the tiles arranged in a specific pattern based on a given origin tile.
        /// </summary>
        /// <param name="pattern">The pattern to generate.</param>
        /// <param name="origin">The center tile from which the pattern is generated.</param>
        /// <returns>An array of tiles forming the pattern, including the origin tile.</returns>
        TileCoordinate[] GetTilesInPattern(ITilePattern pattern, TileCoordinate origin);
    }
}
