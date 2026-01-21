using Core.Game.Combat.Bombs;
using Core.Systems.ServiceLocator;
using UnityEngine;

namespace Core.Game.Upgrades.Definitions
{
    [CreateAssetMenu(fileName = "BiggerBombs", menuName = "Core/Game/Upgrades/Stackable/Bigger Bombs")]
    public class BiggerBombsUpgrade : StackableUpgradeDefinition
    {
        #region Serialized Fields

        [Header("Effect")]
        [SerializeField] private int radiusIncreaseFlat = 1;
        [SerializeField] private float radiusIncreasePercentage = 1f;

        #endregion

        #region Properties
        
        public int RadiusIncreaseFlat => radiusIncreaseFlat;
        public float RadiusIncreasePercentage => radiusIncreasePercentage;

        #endregion

        #region UpgradeDefinition Overrides

        public override void Apply(IUpgradeService upgradeService)
        {
            var bombService = ServiceLocator.Get<IBombService>();
            bombService.ModifyStats(stats =>
            {
                stats.ExplosionRadius += radiusIncreaseFlat;
                stats.ExplosionRadiusModifier += radiusIncreasePercentage;
            });
        }

        public override void Remove(IUpgradeService upgradeService)
        {
            var bombService = ServiceLocator.Get<IBombService>();
            float totalIncreaseFlat = radiusIncreaseFlat * GetStackCount(upgradeService);
            float totalIncreasePercentage = radiusIncreasePercentage * GetStackCount(upgradeService);
            bombService.ModifyStats(stats =>
            {
                stats.ExplosionRadius -= totalIncreaseFlat;
                stats.ExplosionRadiusModifier -= totalIncreasePercentage;
            });
        }

        #endregion
    }
}
