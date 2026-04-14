#if UNITY_EDITOR
using Growth.Equipment;
using UnityEditor;
using UnityEngine;

public static class EquipmentTestGenerator
{
    private const string RootFolder = "Assets/Game/Growth/Equipment/TestSO";

    private const int WeaponStartKey = 3100;
    private const int ArmorStartKey = 3200;
    private const int AccessoryStartKey = 3300;

    private static readonly EquipRarity[] rarityOrder =
    {
        EquipRarity.Common,
        EquipRarity.UnCommon,
        EquipRarity.Rare,
        EquipRarity.Unique
    };

    private static readonly EquipQuality[] qualityOrder =
    {
        EquipQuality.Low,
        EquipQuality.Middle,
        EquipQuality.High,
        EquipQuality.Best
    };

    [MenuItem("Tools/Generate/Test Equipment/16 Weapons")]
    public static void GenerateTestWeapons()
    {
        GenerateByType(EquipType.Weapon, WeaponStartKey);
    }

    [MenuItem("Tools/Generate/Test Equipment/16 Armors")]
    public static void GenerateTestArmors()
    {
        GenerateByType(EquipType.Armor, ArmorStartKey);
    }

    [MenuItem("Tools/Generate/Test Equipment/16 Accessories")]
    public static void GenerateTestAccessories()
    {
        GenerateByType(EquipType.Accessory, AccessoryStartKey);
    }

    [MenuItem("Tools/Generate/Test Equipment/48 All")]
    public static void GenerateAllTestEquipments()
    {
        GenerateByType(EquipType.Weapon, WeaponStartKey, false);
        GenerateByType(EquipType.Armor, ArmorStartKey, false);
        GenerateByType(EquipType.Accessory, AccessoryStartKey, false);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("테스트용 장비 48종(무기/방어구/악세서리) 생성 완료");
    }

    private static void GenerateByType(EquipType equipType, int startKey, bool saveAndRefresh = true)
    {
        EnsureRootFolder();
        EnsureTypeFolder(GetTypeFolderName(equipType));

        int currentKey = startKey;
        string typeFolder = $"{RootFolder}/{GetTypeFolderName(equipType)}";

        foreach (EquipRarity rarity in rarityOrder)
        {
            foreach (EquipQuality quality in qualityOrder)
            {
                EquipmentSO equipment = ScriptableObject.CreateInstance<EquipmentSO>();
                ApplyTestEquipmentStat(equipment, equipType, rarity, quality, currentKey);

                string assetName = $"{GetTypeFolderName(equipType)}_{currentKey}_{rarity}_{quality}.asset";
                string assetPath = $"{typeFolder}/{assetName}";

                AssetDatabase.DeleteAsset(assetPath);
                AssetDatabase.CreateAsset(equipment, assetPath);

                currentKey++;
            }
        }

        if (saveAndRefresh)
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"{equipType} 테스트 SO 16종 생성 완료");
        }
    }

    private static void EnsureRootFolder()
    {
        if (!AssetDatabase.IsValidFolder(RootFolder))
        {
            AssetDatabase.CreateFolder("Assets/Game/Growth/Equipment", "TestSO");
        }
    }

    private static void EnsureTypeFolder(string folderName)
    {
        string fullPath = $"{RootFolder}/{folderName}";
        if (!AssetDatabase.IsValidFolder(fullPath))
        {
            AssetDatabase.CreateFolder(RootFolder, folderName);
        }
    }

    private static string GetTypeFolderName(EquipType equipType)
    {
        return equipType switch
        {
            EquipType.Weapon => "Weapon",
            EquipType.Armor => "Armor",
            EquipType.Accessory => "Accessory",
            _ => "Unknown"
        };
    }

    private static void ApplyTestEquipmentStat(
        EquipmentSO equipment,
        EquipType equipType,
        EquipRarity rarity,
        EquipQuality quality,
        int key)
    {
        float rarityMultiplier = GetRarityMultiplier(rarity);
        float qualityMultiplier = GetQualityMultiplier(quality);
        float totalMultiplier = rarityMultiplier * qualityMultiplier;

        equipment.key = key;
        equipment.equipType = equipType;
        equipment.rarity = rarity;
        equipment.quality = quality;
        equipment.UpgradeNeedCost = Mathf.Max(1, Mathf.RoundToInt(totalMultiplier * 15));
        equipment.combineNeedAmount = rarity == EquipRarity.Unique ? 3 : 5;

        switch (equipType)
        {
            case EquipType.Weapon:
                ApplyWeaponStat(equipment, totalMultiplier, key);
                break;

            case EquipType.Armor:
                ApplyArmorStat(equipment, totalMultiplier, key);
                break;

            case EquipType.Accessory:
                ApplyAccessoryStat(equipment, totalMultiplier, key);
                break;
        }
    }

    private static void ApplyWeaponStat(EquipmentSO equipment, float totalMultiplier, int key)
    {
        equipment.itemName = $"스태프_{key - WeaponStartKey}";
        equipment.name = $"Weapon_{key}_{equipment.rarity}_{equipment.quality}";

        equipment.equipBaseIncrease = new StatIncrease
        {
            atk = Mathf.RoundToInt(10 * totalMultiplier),
            atkRate = RoundRate(0.03f * totalMultiplier),
            damageDealtRate = RoundRate(0.05f * totalMultiplier)
        };

        equipment.equipPerLevelIncrease = new StatIncrease
        {
            atk = Mathf.Max(1, Mathf.RoundToInt(2 * totalMultiplier)),
            atkRate = RoundRate(0.005f * totalMultiplier)
        };

        equipment.ownedBaseIncrease = new StatIncrease
        {
            atk = Mathf.Max(1, Mathf.RoundToInt(3 * totalMultiplier))
        };

        equipment.ownedPerLevelIncrease = new StatIncrease
        {
            atk = Mathf.Max(1, Mathf.RoundToInt(1 * totalMultiplier))
        };
    }

    private static void ApplyArmorStat(EquipmentSO equipment, float totalMultiplier, int key)
    {
        equipment.itemName = $"갑옷_{key - ArmorStartKey}";
        equipment.name = $"Armor_{key}_{equipment.rarity}_{equipment.quality}";

        equipment.equipBaseIncrease = new StatIncrease
        {
            maxHp = Mathf.RoundToInt(35 * totalMultiplier),
            def = Mathf.Max(1, Mathf.RoundToInt(4 * totalMultiplier)),
            maxHpRate = RoundRate(0.02f * totalMultiplier)
        };

        equipment.equipPerLevelIncrease = new StatIncrease
        {
            maxHp = Mathf.Max(1, Mathf.RoundToInt(8 * totalMultiplier)),
            def = Mathf.Max(1, Mathf.RoundToInt(1 * totalMultiplier)),
            maxHpRate = RoundRate(0.003f * totalMultiplier)
        };

        equipment.ownedBaseIncrease = new StatIncrease
        {
            maxHp = Mathf.Max(1, Mathf.RoundToInt(12 * totalMultiplier)),
            def = Mathf.Max(1, Mathf.RoundToInt(1 * totalMultiplier))
        };

        equipment.ownedPerLevelIncrease = new StatIncrease
        {
            maxHp = Mathf.Max(1, Mathf.RoundToInt(3 * totalMultiplier))
        };
    }

    private static void ApplyAccessoryStat(EquipmentSO equipment, float totalMultiplier, int key)
    {
        equipment.itemName = $"장신구_{key - AccessoryStartKey}";
        equipment.name = $"Accessory_{key}_{equipment.rarity}_{equipment.quality}";

        equipment.equipBaseIncrease = new StatIncrease
        {
            itemDropRate = RoundRate(0.04f * totalMultiplier),
            goldGain = RoundRate(0.05f * totalMultiplier),
            expGain = RoundRate(0.05f * totalMultiplier),
            statStoneGain = RoundRate(0.05f * totalMultiplier),
            moveSpeed = RoundRate(0.15f * totalMultiplier)
        };

        equipment.equipPerLevelIncrease = new StatIncrease
        {
            itemDropRate = RoundRate(0.006f * totalMultiplier),
            goldGain = RoundRate(0.007f * totalMultiplier),
            expGain = RoundRate(0.007f * totalMultiplier),
            statStoneGain = RoundRate(0.007f * totalMultiplier),
            moveSpeed = RoundRate(0.02f * totalMultiplier)
        };

        equipment.ownedBaseIncrease = new StatIncrease
        {
            goldGain = RoundRate(0.02f * totalMultiplier),
            expGain = RoundRate(0.02f * totalMultiplier)
        };

        equipment.ownedPerLevelIncrease = new StatIncrease
        {
            itemDropRate = RoundRate(0.003f * totalMultiplier),
            statStoneGain = RoundRate(0.003f * totalMultiplier)
        };
    }

    private static float GetRarityMultiplier(EquipRarity rarity)
    {
        return rarity switch
        {
            EquipRarity.Common => 1.0f,
            EquipRarity.UnCommon => 1.2f,
            EquipRarity.Rare => 1.5f,
            EquipRarity.Unique => 2.0f,
            _ => 1.0f
        };
    }

    private static float GetQualityMultiplier(EquipQuality quality)
    {
        return quality switch
        {
            EquipQuality.Low => 0.85f,
            EquipQuality.Middle => 1.0f,
            EquipQuality.High => 1.15f,
            EquipQuality.Best => 1.3f,
            _ => 1.0f
        };
    }

    private static float RoundRate(float value)
    {
        return Mathf.Round(value * 1000f) / 1000f;
    }
}
#endif