namespace Core.Systems.Combat
{
    public enum TargetType
    {
        /// <summary>
        /// A player-controlled character or ally.
        /// </summary>
        Player,

        /// <summary>
        /// An enemy character or hostile entity.
        /// </summary>
        Enemy,

        /// <summary>
        /// A destructible environment object (e.g., crates, barrels).
        /// Can be destroyed by bombs or attacks.
        /// </summary>
        Destructible
    }
}
