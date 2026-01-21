using Core.Game.Combat.Bombs;
using Core.Systems.ServiceLocator;
using UnityEngine;

namespace Core.Game.Upgrades.Definitions
{
    [CreateAssetMenu(fileName = "ShortFuse", menuName = "Core/Game/Upgrades/Special/Short Fuse")]
    public class ShortFuseUpgrade : SpecialUpgradeDefinition
    {
        #region Serialized Fields

        [Header("Effect")]
        [SerializeField] [Range(0.1f, 0.9f)] private float fuseTimeMultiplier = 0.5f;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the multiplier applied to fuse time.
        /// </summary>
        public float FuseTimeMultiplier => fuseTimeMultiplier;

        #endregion

        #region UpgradeDefinition Overrides

        public override void Apply(IUpgradeService upgradeService)
        {
            var bombService = ServiceLocator.Get<IBombService>();
            bombService.ModifyStats(stats => stats.FuseTime *= fuseTimeMultiplier);
        }

        public override void Remove(IUpgradeService upgradeService)
        {
        }

        #endregion
    }
}
