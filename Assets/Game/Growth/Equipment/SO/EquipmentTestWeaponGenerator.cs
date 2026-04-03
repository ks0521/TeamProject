using System;
using Growth.Equipment;
using UnityEditor;
using UnityEngine;

public static class EquipmentTestWeaponGenerator
{
    private const string RootFolder = "Assets/Game/Growth/Equipment/TestSO";
    private const int StartKey = 3100;

    [MenuItem("Tools/Generate/16 Test Weapon SO")]
    public static void GenerateTestWeapons()
    {
        EnsureRootFolder();

        int currentKey = StartKey;

        EquipRarity[] rarityOrder =
        {
            EquipRarity.Common,
            EquipRarity.UnCommon,
            EquipRarity.Rare,
            EquipRarity.Unique
        };

        EquipQuality[] qualityOrder =
        {
            EquipQuality.Low,
            EquipQuality.Middle,
            EquipQuality.High,
            EquipQuality.Best
        };

        foreach (EquipRarity rarity in rarityOrder)
        {
            foreach (EquipQuality quality in qualityOrder)
            {
                EquipmentSO weapon = ScriptableObject.CreateInstance<EquipmentSO>();
                ApplyTestWeaponStat(weapon, rarity, quality, currentKey);

                // 이름 앞에 key를 붙여두면 프로젝트 창에서도 원하는 순서대로 보기 쉬움
                string assetName = $"Weapon_{currentKey}_{rarity}_{quality}.asset";
                string assetPath = $"{RootFolder}/{assetName}";

                AssetDatabase.DeleteAsset(assetPath);
                AssetDatabase.CreateAsset(weapon, assetPath);

                currentKey++;
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("3100부터 시작하는 테스트용 무기 SO 16종 생성 완료");
    }

    private static void EnsureRootFolder()
    {
        if (!AssetDatabase.IsValidFolder(RootFolder))
        {
            AssetDatabase.CreateFolder("Assets/Game/Growth/Equipment", "TestSO");
        }
    }

    private static void ApplyTestWeaponStat(EquipmentSO weapon, EquipRarity rarity, EquipQuality quality, int key)
    {
        float rarityMultiplier = GetRarityMultiplier(rarity);
        float qualityMultiplier = GetQualityMultiplier(quality);
        float totalMultiplier = rarityMultiplier * qualityMultiplier;

        weapon.key = key;
        weapon.itemName = $"스태프_{key - 3100}";
        weapon.name = $"Weapon_{key}_{rarity}_{quality}";
        weapon.equipType = EquipType.Weapon;
        weapon.rarity = rarity;
        weapon.quality = quality;
        weapon.UpgradeNeedCost = (int)(totalMultiplier * 15);
        // 장착 기본 효과 : 공격력 + 공격력%
        weapon.equipBaseIncrease = new StatIncrease
        {
            atk = Mathf.RoundToInt(10 * totalMultiplier),
            atkRate = RoundRate(0.03f * totalMultiplier),
            damageDealtRate = RoundRate(0.05f * totalMultiplier)
        };

        // 장착 레벨당 효과 : 공격력 + 공격력%
        weapon.equipPerLevelIncrease = new StatIncrease
        {
            atk = Mathf.Max(1, Mathf.RoundToInt(2 * totalMultiplier)),
            atkRate = RoundRate(0.005f * totalMultiplier)
        };

        // 보유 기본 효과 : 공격력만
        weapon.ownedBaseIncrease = new StatIncrease
        {
            atk = Mathf.Max(1, Mathf.RoundToInt(3 * totalMultiplier))
        };

        // 보유 레벨당 효과 : 공격력만
        weapon.ownedPerLevelIncrease = new StatIncrease
        {
            atk = Mathf.Max(1, Mathf.RoundToInt(1 * totalMultiplier))
        };
        if (weapon.rarity == EquipRarity.Unique)
        {
            weapon.combineNeedAmount = 3;
        }
        else
        {
            weapon.combineNeedAmount = 5;
        }
    }

    private static float GetRarityMultiplier(EquipRarity rarity)
    {
        return rarity switch
        {
            EquipRarity.Common   => 1.0f,
            EquipRarity.UnCommon => 1.2f,
            EquipRarity.Rare     => 1.5f,
            EquipRarity.Unique   => 2.0f,
            _ => 1.0f
        };
    }

    private static float GetQualityMultiplier(EquipQuality quality)
    {
        return quality switch
        {
            EquipQuality.Low    => 0.85f,
            EquipQuality.Middle => 1.0f,
            EquipQuality.High   => 1.15f,
            EquipQuality.Best   => 1.3f,
            _ => 1.0f
        };
    }

    private static float RoundRate(float value)
    {
        return Mathf.Round(value * 1000f) / 1000f;
    }
}