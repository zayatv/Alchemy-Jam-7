namespace Core.Systems.VFX
{
    public struct VFXHandle
    {
        /// <summary>
        /// Unique identifier for this VFX instance.
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// Whether this handle references a valid VFX instance.
        /// </summary>
        public bool IsValid => Id != -1;

        /// <summary>
        /// Invalid handle representing no VFX instance.
        /// </summary>
        public static VFXHandle Invalid => new VFXHandle(-1);

        public VFXHandle(int id)
        {
            Id = id;
        }
    }
}
