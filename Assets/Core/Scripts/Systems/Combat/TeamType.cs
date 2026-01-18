namespace Core.Systems.Combat
{
    public enum TeamType
    {
        /// <summary>
        /// The player's team. Player-controlled characters and allies.
        /// </summary>
        Player,

        /// <summary>
        /// The enemy team. Hostile entities that oppose the player.
        /// </summary>
        Enemy,

        /// <summary>
        /// Neutral team. Can be damaged by both player and enemy teams.
        /// Typically used for destructible environment objects.
        /// </summary>
        Neutral
    }
}
