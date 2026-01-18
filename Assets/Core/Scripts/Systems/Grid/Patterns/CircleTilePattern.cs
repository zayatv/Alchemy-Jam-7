using System;
using System.Collections.Generic;

namespace Core.Systems.Grid.Patterns
{
    public class CircleTilePattern : ITilePattern
    {
        private readonly int _radius;
        
        public CircleTilePattern(int radius)
        {
            _radius = radius;
        }
        
        public TileCoordinate[] GetTilesInPattern(TileCoordinate origin)
        {
            if (_radius < 0)
            {
                throw new ArgumentException("Radius must be non-negative", nameof(_radius));
            }

            var tiles = new List<TileCoordinate>();
            
            for (int x = -_radius; x <= _radius; x++)
            {
                for (int y = -_radius; y <= _radius; y++)
                {
                    var tile = new TileCoordinate(origin.X + x, origin.Y + y);

                    if (tile.ManhattanDistance(origin) <= _radius)
                    {
                        tiles.Add(tile);
                    }
                }
            }

            return tiles.ToArray();
        }
    }
}