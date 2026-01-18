namespace Core.Systems.Audio
{
    public struct AudioHandle
    {
        /// <summary>
        /// Unique identifier for this audio instance.
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// Whether this handle references a valid audio instance.
        /// </summary>
        public bool IsValid => Id != -1;

        /// <summary>
        /// Invalid handle representing no audio instance.
        /// </summary>
        public static AudioHandle Invalid => new AudioHandle(-1);

        public AudioHandle(int id)
        {
            Id = id;
        }
    }
}
