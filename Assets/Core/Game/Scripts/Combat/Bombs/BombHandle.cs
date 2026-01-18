namespace Core.Game.Combat.Bombs
{
    public readonly struct BombHandle
    {
        #region Properties
        
        public int Id { get; }
        public bool IsValid => Id != -1;
        
        public static BombHandle Invalid => new BombHandle(-1);

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="BombHandle"/> struct.
        /// </summary>
        /// <param name="id">The unique identifier for the bomb.</param>
        public BombHandle(int id)
        {
            Id = id;
        }

        #endregion

        #region Equality Members

        /// <summary>
        /// Determines whether the specified object is equal to this handle.
        /// </summary>
        /// <param name="obj">The object to compare with.</param>
        /// <returns>True if the objects are equal; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            return obj is BombHandle other && Id == other.Id;
        }

        /// <summary>
        /// Returns a hash code for this handle.
        /// </summary>
        /// <returns>A hash code based on the bomb ID.</returns>
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        /// <summary>
        /// Determines whether two bomb handles are equal.
        /// </summary>
        /// <param name="left">The first handle to compare.</param>
        /// <param name="right">The second handle to compare.</param>
        /// <returns>True if the handles are equal; otherwise, false.</returns>
        public static bool operator ==(BombHandle left, BombHandle right)
        {
            return left.Id == right.Id;
        }

        /// <summary>
        /// Determines whether two bomb handles are not equal.
        /// </summary>
        /// <param name="left">The first handle to compare.</param>
        /// <param name="right">The second handle to compare.</param>
        /// <returns>True if the handles are not equal; otherwise, false.</returns>
        public static bool operator !=(BombHandle left, BombHandle right)
        {
            return left.Id != right.Id;
        }

        #endregion
    }
}
