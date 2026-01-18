using System;
using UnityEngine;

namespace Core.Systems.Grid
{
    [Serializable]
    public struct TileCoordinate : IEquatable<TileCoordinate>
    {
        #region Fields
        
        [SerializeField]
        private Vector2Int _value;
        
        #endregion
        
        #region Properties

        /// <summary>
        /// The underlying Vector2Int value for direct access and calculations.
        /// </summary>
        public Vector2Int Value => _value;

        /// <summary>
        /// The X coordinate of the tile.
        /// </summary>
        public int X => _value.x;

        /// <summary>
        /// The Y coordinate of the tile.
        /// </summary>
        public int Y => _value.y;
        
        #endregion
        
        #region Constructors

        /// <summary>
        /// Creates a new TileCoordinate with the specified X and Y values.
        /// </summary>
        public TileCoordinate(int x, int y)
        {
            _value = new Vector2Int(x, y);
        }

        /// <summary>
        /// Creates a new TileCoordinate from a Vector2Int.
        /// </summary>
        public TileCoordinate(Vector2Int value)
        {
            _value = value;
        }
        
        #endregion

        /// <summary>
        /// Converts a world position to a tile coordinate.
        /// </summary>
        /// <param name="worldPosition">The world position to convert.</param>
        /// <param name="tileSize">The size of each tile.</param>
        /// <returns>The tile coordinate containing the world position.</returns>
        public static TileCoordinate FromWorldPosition(Vector3 worldPosition, float tileSize)
        {
            int x = Mathf.FloorToInt(worldPosition.x / tileSize);
            int y = Mathf.FloorToInt(worldPosition.z / tileSize);
            
            return new TileCoordinate(x, y);
        }

        /// <summary>
        /// Converts this tile coordinate to a world position at the corner of the tile.
        /// </summary>
        /// <param name="tileSize">The size of each tile.</param>
        /// <returns>The world position at the corner of the tile.</returns>
        public Vector3 ToWorldPosition(float tileSize)
        {
            return new Vector3(_value.x * tileSize, 0f, _value.y * tileSize);
        }

        /// <summary>
        /// Converts this tile coordinate to a world position at the center of the tile.
        /// Use this for consistent object placement.
        /// </summary>
        /// <param name="tileSize">The size of each tile.</param>
        /// <returns>The world position at the center of the tile.</returns>
        public Vector3 ToCenteredWorldPosition(float tileSize)
        {
            float halfTile = tileSize * 0.5f;
            
            return new Vector3(_value.x * tileSize + halfTile, 0f, _value.y * tileSize + halfTile);
        }

        /// <summary>
        /// Calculates the Manhattan distance to another tile.
        /// Manhattan distance is the sum of the absolute differences of their coordinates.
        /// </summary>
        /// <param name="other">The other tile coordinate.</param>
        /// <returns>The Manhattan distance between this tile and the other tile.</returns>
        public int ManhattanDistance(TileCoordinate other)
        {
            return Mathf.Abs(_value.x - other._value.x) + Mathf.Abs(_value.y - other._value.y);
        }

        /// <summary>
        /// Calculates the Euclidean distance to another tile.
        /// </summary>
        /// <param name="other">The other tile coordinate.</param>
        /// <returns>The Euclidean distance between this tile and the other tile.</returns>
        public float EuclideanDistance(TileCoordinate other)
        {
            return Vector2Int.Distance(_value, other._value);
        }

        public TileCoordinate[] GetAdjacentTiles()
        {
            TileCoordinate[] tiles =
            {
                new TileCoordinate(_value + Vector2Int.right),
                new TileCoordinate(_value + Vector2Int.left),
                new TileCoordinate(_value + Vector2Int.up),
                new TileCoordinate(_value + Vector2Int.down)
            };
            
            return tiles;
        }

        #region Operators

        /// <summary>
        /// Adds two tile coordinates together.
        /// </summary>
        public static TileCoordinate operator +(TileCoordinate a, TileCoordinate b)
        {
            return new TileCoordinate(a._value + b._value);
        }

        /// <summary>
        /// Subtracts one tile coordinate from another.
        /// </summary>
        public static TileCoordinate operator -(TileCoordinate a, TileCoordinate b)
        {
            return new TileCoordinate(a._value - b._value);
        }

        /// <summary>
        /// Multiplies a tile coordinate by a scalar.
        /// </summary>
        public static TileCoordinate operator *(TileCoordinate a, int scalar)
        {
            return new TileCoordinate(a._value * scalar);
        }

        /// <summary>
        /// Adds a Vector2Int to a TileCoordinate.
        /// </summary>
        public static TileCoordinate operator +(TileCoordinate a, Vector2Int b)
        {
            return new TileCoordinate(a._value + b);
        }

        /// <summary>
        /// Subtracts a Vector2Int from a TileCoordinate.
        /// </summary>
        public static TileCoordinate operator -(TileCoordinate a, Vector2Int b)
        {
            return new TileCoordinate(a._value - b);
        }

        /// <summary>
        /// Checks if two tile coordinates are equal.
        /// </summary>
        public static bool operator ==(TileCoordinate a, TileCoordinate b)
        {
            return a._value == b._value;
        }

        /// <summary>
        /// Checks if two tile coordinates are not equal.
        /// </summary>
        public static bool operator !=(TileCoordinate a, TileCoordinate b)
        {
            return a._value != b._value;
        }
        
        /// <summary>
        /// Implicit conversion from Vector2Int to TileCoordinate.
        /// </summary>
        public static implicit operator TileCoordinate(Vector2Int v) => new TileCoordinate(v);

        /// <summary>
        /// Implicit conversion from TileCoordinate to Vector2Int.
        /// </summary>
        public static implicit operator Vector2Int(TileCoordinate t) => t._value;

        #endregion

        #region Equality and Hashing

        /// <summary>
        /// Returns a hash code for this tile coordinate.
        /// Uses Vector2Int's hash code for consistency.
        /// </summary>
        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        /// <summary>
        /// Checks if this tile coordinate equals another object.
        /// </summary>
        public override bool Equals(object obj)
        {
            return obj is TileCoordinate other && Equals(other);
        }

        /// <summary>
        /// Checks if this tile coordinate equals another tile coordinate.
        /// </summary>
        public bool Equals(TileCoordinate other)
        {
            return _value == other._value;
        }

        #endregion
        
        public override string ToString()
        {
            return $"TileCoordinate({_value.x}, {_value.y})";
        }

        #region Static Helpers

        /// <summary>
        /// The zero tile coordinate (0, 0).
        /// </summary>
        public static TileCoordinate Zero => new TileCoordinate(0, 0);

        /// <summary>
        /// The one tile coordinate (1, 1).
        /// </summary>
        public static TileCoordinate One => new TileCoordinate(1, 1);

        /// <summary>
        /// Direction up (0, 1).
        /// </summary>
        public static TileCoordinate Up => new TileCoordinate(0, 1);

        /// <summary>
        /// Direction down (0, -1).
        /// </summary>
        public static TileCoordinate Down => new TileCoordinate(0, -1);

        /// <summary>
        /// Direction left (-1, 0).
        /// </summary>
        public static TileCoordinate Left => new TileCoordinate(-1, 0);

        /// <summary>
        /// Direction right (1, 0).
        /// </summary>
        public static TileCoordinate Right => new TileCoordinate(1, 0);

        #endregion
    }
}
