using System.IO;
using System.Reflection;
using Core.Game.Combat;
using Core.Game.Destructibles.Events;
using UnityEditor;
using UnityEngine;

namespace Core.Game.Destructibles.Editor
{
    public static class DestructibleConfigGenerator
    {
        private const string OutputPath = "Assets/Core/Game/Assets/Destructibles";

        [MenuItem("Tools/Game/Generate Destructible Configs (Normal Difficulty)")]
        public static void GenerateAllConfigs()
        {
            EnsureDirectoryExists();

            CreateDoorConfig();
            CreateItemCageConfig();
            CreateStoneConfig();
            CreateVaseConfig();
            CreateChestConfig();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("All destructible configurations generated successfully!");
        }

        private static void EnsureDirectoryExists()
        {
            if (!Directory.Exists(OutputPath))
            {
                Directory.CreateDirectory(OutputPath);
            }
        }

        #region Door

        [MenuItem("Tools/Game/Generate Destructible Configs/Door")]
        public static void CreateDoorConfig()
        {
            // Door HealthConfig (1x2 tiles, standard health)
            var doorHealth = CreateHealthConfig("Door_HealthConfig", 1, false, 0f);

            // Door DestructibleConfig
            var doorConfig = CreateDestructibleConfig("Door_DestructibleConfig", DestructibleType.Door, "Door");

            // Door has no drop table - just needs to be destroyed

            Debug.Log("Door config created");
        }

        #endregion

        #region Item Cage

        [MenuItem("Tools/Game/Generate Destructible Configs/Item Cage")]
        public static void CreateItemCageConfig()
        {
            // Item Cage HealthConfig (2x2 tiles)
            var cageHealth = CreateHealthConfig("ItemCage_HealthConfig", 1, false, 0f);

            // Item Cage DestructibleConfig
            var cageConfig = CreateDestructibleConfig("ItemCage_DestructibleConfig", DestructibleType.ItemCage, "Item Cage");

            // Item Cage uses UpgradeDropper component, not DropTable

            Debug.Log("Item Cage config created");
        }

        #endregion

        #region Stone

        [MenuItem("Tools/Game/Generate Destructible Configs/Stone")]
        public static void CreateStoneConfig()
        {
            // Stone HealthConfig (2x2 tiles)
            var stoneHealth = CreateHealthConfig("Stone_HealthConfig", 1, false, 0f);

            // Stone DestructibleConfig
            var stoneConfig = CreateDestructibleConfig("Stone_DestructibleConfig", DestructibleType.Stone, "Stone");

            // Stone DropTable:
            // 50% nothing, 30% 1 Bomb, 20% 1 Heart, 10% 2 Bombs
            // Total weight = 110, so actual percentages: ~45.45%, ~27.27%, ~18.18%, ~9.09%
            var stoneDropTable = CreateDropTable("Stone_DropTable");

            var entries = new DropEntry[4];
            entries[0] = CreateDropEntry(50f); // Nothing
            entries[1] = CreateDropEntry(30f, CreateDropItem(DropType.Bomb, 1));
            entries[2] = CreateDropEntry(20f, CreateDropItem(DropType.Health, 1));
            entries[3] = CreateDropEntry(10f, CreateDropItem(DropType.Bomb, 2));

            SetDropTableEntries(stoneDropTable, entries, null);

            Debug.Log("Stone config created");
        }

        #endregion

        #region Vase

        [MenuItem("Tools/Game/Generate Destructible Configs/Vase")]
        public static void CreateVaseConfig()
        {
            // Vase HealthConfig (1x1 tile)
            var vaseHealth = CreateHealthConfig("Vase_HealthConfig", 1, false, 0f);

            // Vase DestructibleConfig
            var vaseConfig = CreateDestructibleConfig("Vase_DestructibleConfig", DestructibleType.Vase, "Vase");

            // Vase DropTable:
            // 34% 1 Heart, 33% 1 Bomb, 33% 1 Heart + 1 Bomb
            var vaseDropTable = CreateDropTable("Vase_DropTable");

            var entries = new DropEntry[3];
            entries[0] = CreateDropEntry(34f, CreateDropItem(DropType.Health, 1));
            entries[1] = CreateDropEntry(33f, CreateDropItem(DropType.Bomb, 1));
            entries[2] = CreateDropEntry(33f, CreateDropItem(DropType.Health, 1), CreateDropItem(DropType.Bomb, 1));

            SetDropTableEntries(vaseDropTable, entries, null);

            Debug.Log("Vase config created");
        }

        #endregion

        #region Chest

        [MenuItem("Tools/Game/Generate Destructible Configs/Chest")]
        public static void CreateChestConfig()
        {
            // Chest HealthConfig (1x1 tile)
            var chestHealth = CreateHealthConfig("Chest_HealthConfig", 1, false, 0f);

            // Chest DestructibleConfig
            var chestConfig = CreateDestructibleConfig("Chest_DestructibleConfig", DestructibleType.Chest, "Chest");

            // Chest DropTable:
            // Guaranteed: 2 Bombs
            // 49% nothing extra, 20% +1 (total 3), 20% +2 (total 4), 10% +3 (total 5), 1% +8 (total 10)
            var chestDropTable = CreateDropTable("Chest_DropTable");

            // Guaranteed 2 bombs
            var guaranteedDrops = new DropItem[1];
            guaranteedDrops[0] = CreateDropItem(DropType.Bomb, 2);

            // Rolled entries (total weight = 100)
            var entries = new DropEntry[5];
            entries[0] = CreateDropEntry(49f); // Nothing extra
            entries[1] = CreateDropEntry(20f, CreateDropItem(DropType.Bomb, 1)); // +1 = 3 total
            entries[2] = CreateDropEntry(20f, CreateDropItem(DropType.Bomb, 2)); // +2 = 4 total
            entries[3] = CreateDropEntry(10f, CreateDropItem(DropType.Bomb, 3)); // +3 = 5 total
            entries[4] = CreateDropEntry(1f, CreateDropItem(DropType.Bomb, 8));  // +8 = 10 total

            SetDropTableEntries(chestDropTable, entries, guaranteedDrops);

            Debug.Log("Chest config created");
        }

        #endregion

        #region Helper Methods

        private static HealthConfig CreateHealthConfig(string name, int maxHealth, bool enableImmunity, float immunityDuration)
        {
            string path = $"{OutputPath}/{name}.asset";

            var config = AssetDatabase.LoadAssetAtPath<HealthConfig>(path);
            if (config == null)
            {
                config = ScriptableObject.CreateInstance<HealthConfig>();
                AssetDatabase.CreateAsset(config, path);
            }

            var so = new SerializedObject(config);
            so.FindProperty("maxHealth").intValue = maxHealth;
            so.FindProperty("enableDamageImmunity").boolValue = enableImmunity;
            so.FindProperty("immunityDuration").floatValue = immunityDuration;
            so.ApplyModifiedPropertiesWithoutUndo();

            EditorUtility.SetDirty(config);
            return config;
        }

        private static DestructibleConfig CreateDestructibleConfig(string name, DestructibleType type, string displayName)
        {
            string path = $"{OutputPath}/{name}.asset";

            var config = AssetDatabase.LoadAssetAtPath<DestructibleConfig>(path);
            if (config == null)
            {
                config = ScriptableObject.CreateInstance<DestructibleConfig>();
                AssetDatabase.CreateAsset(config, path);
            }

            var so = new SerializedObject(config);
            so.FindProperty("destructibleType").enumValueIndex = (int)type;
            so.FindProperty("displayName").stringValue = displayName;
            so.ApplyModifiedPropertiesWithoutUndo();

            EditorUtility.SetDirty(config);
            return config;
        }

        private static DropTable CreateDropTable(string name)
        {
            string path = $"{OutputPath}/{name}.asset";

            var dropTable = AssetDatabase.LoadAssetAtPath<DropTable>(path);
            if (dropTable == null)
            {
                dropTable = ScriptableObject.CreateInstance<DropTable>();
                AssetDatabase.CreateAsset(dropTable, path);
            }

            EditorUtility.SetDirty(dropTable);
            return dropTable;
        }

        private static void SetDropTableEntries(DropTable dropTable, DropEntry[] entries, DropItem[] guaranteedDrops)
        {
            var so = new SerializedObject(dropTable);

            // Set entries
            var entriesProp = so.FindProperty("entries");
            entriesProp.arraySize = entries.Length;

            for (int i = 0; i < entries.Length; i++)
            {
                var entryProp = entriesProp.GetArrayElementAtIndex(i);
                var weightProp = entryProp.FindPropertyRelative("weight");
                var itemsProp = entryProp.FindPropertyRelative("items");

                weightProp.floatValue = entries[i].Weight;

                var items = entries[i].Items;
                if (items != null)
                {
                    itemsProp.arraySize = items.Length;
                    for (int j = 0; j < items.Length; j++)
                    {
                        SetDropItemProperty(itemsProp.GetArrayElementAtIndex(j), items[j]);
                    }
                }
                else
                {
                    itemsProp.arraySize = 0;
                }
            }

            // Set guaranteed drops
            var guaranteedProp = so.FindProperty("guaranteedDrops");
            if (guaranteedDrops != null)
            {
                guaranteedProp.arraySize = guaranteedDrops.Length;
                for (int i = 0; i < guaranteedDrops.Length; i++)
                {
                    SetDropItemProperty(guaranteedProp.GetArrayElementAtIndex(i), guaranteedDrops[i]);
                }
            }
            else
            {
                guaranteedProp.arraySize = 0;
            }

            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetDropItemProperty(SerializedProperty prop, DropItem item)
        {
            prop.FindPropertyRelative("dropType").enumValueIndex = (int)item.DropType;
            prop.FindPropertyRelative("amount").intValue = item.Amount;
            prop.FindPropertyRelative("randomUpgrade").boolValue = item.RandomUpgrade;
            // specificUpgrade left as null for non-upgrade drops
        }

        private static DropEntry CreateDropEntry(float weight, params DropItem[] items)
        {
            var entry = new DropEntry();

            var weightField = typeof(DropEntry).GetField("weight", BindingFlags.NonPublic | BindingFlags.Instance);
            var itemsField = typeof(DropEntry).GetField("items", BindingFlags.NonPublic | BindingFlags.Instance);

            weightField?.SetValue(entry, weight);
            itemsField?.SetValue(entry, items.Length > 0 ? items : null);

            return entry;
        }

        private static DropItem CreateDropItem(DropType type, int amount = 1, bool randomUpgrade = false)
        {
            var item = new DropItem();

            var typeField = typeof(DropItem).GetField("dropType", BindingFlags.NonPublic | BindingFlags.Instance);
            var amountField = typeof(DropItem).GetField("amount", BindingFlags.NonPublic | BindingFlags.Instance);
            var randomField = typeof(DropItem).GetField("randomUpgrade", BindingFlags.NonPublic | BindingFlags.Instance);

            typeField?.SetValue(item, type);
            amountField?.SetValue(item, amount);
            randomField?.SetValue(item, randomUpgrade);

            return item;
        }

        #endregion
    }
}
