using Core.Game.Combat.Bombs;
using Core.Systems.ServiceLocator;
using UnityEngine;

namespace Core.Game.Upgrades.Definitions
{
    [CreateAssetMenu(fileName = "FasterPlacement", menuName = "Core/Game/Upgrades/Stackable/Faster Placement")]
    public class FasterPlacementUpgrade : StackableUpgradeDefinition
    {
        #region Serialized Fields

        [Header("Effect")]
        [SerializeField] [Range(0.1f, 0.9f)] private float cooldownMultiplier = 0.5f;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the multiplier applied to placement cooldown per stack.
        /// </summary>
        public float CooldownMultiplier => cooldownMultiplier;

        #endregion

        #region UpgradeDefinition Overrides

        public override void Apply(IUpgradeService upgradeService)
        {
            var bombService = ServiceLocator.Get<IBombService>();
            bombService.ModifyStats(stats => stats.PlacementCooldown *= cooldownMultiplier);
        }

        public override void Remove(IUpgradeService upgradeService)
        {
        }

        #endregion
    }
}
