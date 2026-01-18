namespace Core.Systems.Grid
{
    public interface ITilePattern
    {
        /// <summary>
        /// Retrieves an array of tile coordinates that belong to a specific pattern,
        /// starting from the provided origin tile.
        /// </summary>
        /// <param name="origin">The starting point represented as a tile coordinate.</param>
        /// <returns>An array of tile coordinates that form the defined pattern.</returns>
        TileCoordinate[] GetTilesInPattern(TileCoordinate origin);
    }
}