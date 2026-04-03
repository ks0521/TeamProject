using System;
using UnityEngine;

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
    /// <summary> 스탯 증가효과(성장수단 공통)</summary>
    [Serializable]
    public struct StatIncrease
    {
        [Header("Attack")]
        [Tooltip("공격력 증가")]public float atk;               // 공격력 증가
        [Tooltip("공격력% 증가")]public float atkRate;           // 공격력 % 증가
        [Tooltip("피해량 증가")]public float damageDealtRate;    //피해량 증가   
        [Tooltip("치명타 확률 증가")]public float critChance;       //치명타 확률
        [Tooltip("치명타 피해량 증가")]public float critDamage;       //치명타 피해량
        
        [Header("Defence")]
        [Tooltip("최대 HP 증가")]public float maxHp;             // HP 증가
        [Tooltip("최대 HP% 증가")]public float maxHpRate;         // HP % 증가
        [Tooltip("방어력 증가")]public float def;               //방어력 증가
        [Tooltip("피해 감소량 증가")]public float damageReduction;   // 받는 피해 비율 감소
        
        [Header("Utility")]
        [Tooltip("이동속도 증가")]public float moveSpeed;         // 이동속도 증가(배율)
        [Tooltip("공격속도 증가")]public float atkSpeed;       // 공격속도 증가(배율)
        
        [Header("Reward")]
        [Tooltip("아이템 드랍률 증가")]public float itemDropRate;      // 아이템 드랍률 증가
        [Tooltip("골드 획득량 증가")]public float goldGain;          // 골드 획득량 증가
        [Tooltip("경험치 획득량 증가")]public float expGain;           // 경험치 획득량 증가
        [Tooltip("스탯 성장석 획득량 증가")]public float statStoneGain;     // 스탯 강화석 획득량 증가
        public static StatIncrease operator *(StatIncrease stat, int mul)
        {
            return new StatIncrease()
            {
                atk = stat.atk * mul,
                atkRate = stat.atkRate * mul,
                damageDealtRate = stat.damageDealtRate * mul,
                atkSpeed = stat.atkSpeed * mul,
                critChance = stat.critChance * mul,
                critDamage = stat.critDamage * mul,
                maxHp = stat.maxHp * mul,
                maxHpRate = stat.maxHpRate * mul,
                damageReduction = stat.damageReduction * mul,
                def = stat.def * mul,
                moveSpeed = stat.moveSpeed * mul,
                itemDropRate = stat.itemDropRate * mul,
                expGain = stat.expGain * mul,
                goldGain = stat.goldGain * mul,
                statStoneGain = stat.statStoneGain * mul
            };
        }
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
