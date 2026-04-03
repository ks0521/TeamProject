using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Growth.Equipment
{
    public enum EquipType
    {
        Weapon, Armor, Accessory
    }

    public enum EquipRarity
    {
        Common, UnCommon, Rare, Unique
    }

    public enum EquipQuality
    {//하, 중, 상, 최상
        Low, Middle, High, Best
    }
    /// <summary> 장비의 증가효과(공통)</summary>
    [Serializable]
    public struct StatIncrease
    {
        [Header("Attack")]
        public int flatAttack;              // 공격력 증가(상수)
        public float attackRate;            // 공격력 % 증가
        [Header("Defence")]
        public int flatMaxHp;               // HP 증가(상수)
        public float maxHpRate;             // HP % 증가
        public float damageReductionRate;   // 받는 피해 비율 감소
        [Header("Reward")]
        public float itemDropRateBonus;     // 아이템 드랍률 증가
        public float goldGainRate;          // 골드 획득량 증가
        public float expGainRate;           // 경험치 획득량 증가
        public float statStoneGainRate;     // 스탯 강화석 획득량 증가
        [Header("Utility")]
        public float moveSpeedRate;         // 이동속도 증가
        public float attackSpeedRate;       // 공격속도 증가
    }
    [CreateAssetMenu(menuName = "Game/Reward/Equipment")]
    public class EquipmentSO : ItemSO
    {
        public EquipType equipType;
        [Header("Quality")]
        public EquipRarity rarity;
        public EquipQuality quality;

        public int combineNeedAmount;
        public int UpgradeNeedCost;
        public StatIncrease equipBaseIncrease;       // 장착 시 증가 스탯
        public StatIncrease equipPerLevelIncrease; // 레벨당 장착 시 증가 스탯
        public StatIncrease ownedBaseIncrease;     // 보유 시 증가 스탯
        public StatIncrease ownedPerLevelIncrease;    // 레벨당 보유 시 증가 스탯
    }
}
