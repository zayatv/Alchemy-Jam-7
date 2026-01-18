using System;
using UnityEngine;

namespace Core.Systems.Grid.Patterns
{
    public class CrossTilePattern : ITilePattern
    {
        private readonly int _armLength;
        
        public CrossTilePattern(int armLength)
        {
            _armLength = armLength;
        }
        
        public TileCoordinate[] GetTilesInPattern(TileCoordinate origin)
        {
            if (_armLength < 0)
            {
                throw new ArgumentException("Arm length must be non-negative", nameof(_armLength));
            }

            int totalTiles = 1 + (4 * _armLength);
            var tiles = new TileCoordinate[totalTiles];
            int index = 0;

            tiles[index++] = origin;

            for (int i = 1; i <= _armLength; i++)
            {
                tiles[index++] = new TileCoordinate(origin.Value + Vector2Int.right * i);
                tiles[index++] = new TileCoordinate(origin.Value + Vector2Int.left * i);
                tiles[index++] = new TileCoordinate(origin.Value + Vector2Int.up * i);
                tiles[index++] = new TileCoordinate(origin.Value + Vector2Int.down * i);
            }

            return tiles;
        }
    }
}