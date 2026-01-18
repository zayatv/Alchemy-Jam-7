namespace Core.Game.Upgrades
{
    public enum UpgradeRarity
    {
        /// <summary>
        /// Upgrades that can be acquired multiple times and stack their effects.
        /// </summary>
        Stackable,

        /// <summary>
        /// Unique upgrades that can only be acquired once and may exclude other upgrades.
        /// </summary>
        Special
    }
}
