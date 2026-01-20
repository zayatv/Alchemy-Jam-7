using System;
using Core.Game.Upgrades;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Game.Destructibles
{
    [Serializable]
    public class DropEntry
    {
        [HorizontalGroup("Main", Width = 80)]
        [LabelText("Weight")]
        [LabelWidth(45)]
        [MinValue(0f)]
        [SerializeField] private float weight = 1f;

        [HorizontalGroup("Main")]
        [LabelText("Items")]
        [LabelWidth(40)]
        [ListDrawerSettings(ShowFoldout = false, DraggableItems = true)]
        [SerializeField] private DropItem[] items;

        public float Weight => weight;
        public DropItem[] Items => items;
    }

    [Serializable]
    [InlineProperty]
    public class DropItem
    {
        [HideLabel]
        [EnumToggleButtons]
        [SerializeField] private DropType dropType;

        [MinValue(1)]
        [ShowIf("@IsHealthDrop || IsBombDrop")]
        [SerializeField] private int amount = 1;

        [HideLabel]
        [ShowIf("@IsUpgradeDrop && !randomUpgrade")]
        [SerializeField] private UpgradeDefinition specificUpgrade;

        [LabelText("Random")]
        [LabelWidth(50)]
        [ShowIf("@IsUpgradeDrop")]
        [SerializeField] private bool randomUpgrade;

        public DropType DropType => dropType;
        public int Amount => amount;
        public UpgradeDefinition SpecificUpgrade => specificUpgrade;
        public bool RandomUpgrade => randomUpgrade;
        
        private bool IsUpgradeDrop => dropType == DropType.Upgrade;
        private bool IsHealthDrop => dropType == DropType.Health;
        private bool IsBombDrop => dropType == DropType.Bomb;
    }

    public enum DropType
    {
        None,
        Health,
        Bomb,
        Upgrade
    }
}
