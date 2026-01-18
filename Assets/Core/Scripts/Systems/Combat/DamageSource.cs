namespace Core.Systems.Combat
{
    /// <summary>
    /// Defines the source or type of damage dealt to an entity.
    /// Used for damage calculation, immunity checks, and visual/audio feedback.
    /// </summary>
    public enum DamageSource
    {
        /// <summary>
        /// Damage caused by a bomb explosion.
        /// Primary damage type in the bomb system.
        /// </summary>
        Bomb,

        /// <summary>
        /// Damage caused by environmental hazards (e.g., traps).
        /// </summary>
        Environment,

        /// <summary>
        /// Damage caused by direct physical contact with an enemy.
        /// </summary>
        Contact
    }
}
