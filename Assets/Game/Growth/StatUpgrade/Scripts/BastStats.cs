using Base.Utils;
using Growth.Equipment;
using System;
using UnityEngine;

namespace Base.Data
{
    [Serializable]
    public struct TotalStat
    {
        public BattleStat battle;
        public RewardStat reward;
        public ExtraStat extra;
        public static TotalStat operator +(TotalStat stat, StatIncrease modifier)
        {
            return new TotalStat()
            {
                battle = stat.battle + modifier, reward = stat.reward + modifier, extra = stat.extra + modifier
            };
        }

        public static TotalStat operator +(TotalStat a, TotalStat b)
            //=> StructMemberCalculator<TotalStat>.Add(a, b);
        {
            return new TotalStat()
            {
                battle = a.battle + b.battle, extra = a.extra + b.extra, reward = a.reward + b.reward
            };
        }
    }
    /// <summary> 플레이어 적 공통, 전투관련 스탯 </summary>
    [Serializable]
    public struct BattleStat
    {
        [Header("Attack")] 
        public float atk; //공격력
        public float atkRate; //공격력 %
        [Range(0, 1)] public float critChance; //크확(0 = 0%, 1 = 100%)
        public float critDamage; //치피
        
        [Header("Defence")] 
        public float maxHp; //최대 체력
        public float def; //방어력

        [Header("Utility")] 
        //1.2 = 20% 공격속도 증가
        public float atkSpeed; //공격속도, 1이 기본값으로 해당 수치는 배율처럼 작동
        [Range(0.1f,3f)]
        public float moveSpeed; //이동속도
        
        public float atkRange; //일반공격 사거리

        public static BattleStat operator +(BattleStat a, BattleStat b)
           => StructMemberCalculator<BattleStat>.Add(a, b);
        // {
        //     return new BattleStat
        //     {
        //         maxHp = a.maxHp + b.maxHp,
        //         atk = a.atk + b.atk,
        //         def = a.def + b.def,
        //         atkSpeed = a.atkSpeed + b.atkSpeed,
        //         moveSpeed = a.moveSpeed + b.moveSpeed,
        //         critChance = a.critChance + b.critChance,
        //         critDamage = a.critDamage + b.critDamage,
        //         atkRange = a.atkRange + b.atkRange
        //     };
        // }

        public static BattleStat operator +(BattleStat stat, StatIncrease modifier)
        {
            return new BattleStat
            {
                atk = stat.atk + modifier.atk,
                atkRate = stat.atkRate + modifier.atkRate,
                critChance = stat.critChance + modifier.critChance,
                critDamage = stat.critDamage + modifier.critDamage,
                maxHp = (stat.maxHp + modifier.maxHp) * (1+modifier.maxHpRate),
                def = stat.def + modifier.def,
                atkSpeed = stat.atkSpeed + modifier.atkSpeed,
                moveSpeed = stat.moveSpeed + modifier.moveSpeed,
                atkRange = stat.atkRange,
            };
        }
    }
    /// <summary> 플레이어용, 보상 획득에 관한 스탯 </summary>
    [Serializable]
    public struct RewardStat
    {
        [Range(-1f, 100f)] public float goldGain; //골드획득률(%)
        [Range(-1f, 100f)] public float statStoneGain; //골드획득률(%)
        [Range(-1f, 100f)] public float expGain; //경험치획득률(%)
        [Range(-1, 10f)] public float itemDropRate; //아이템 드랍 확률(%)

        public static RewardStat operator +(RewardStat a, RewardStat b)
           => StructMemberCalculator<RewardStat>.Add(a, b); 
        // {
        //     return new RewardStat
        //     {
        //         goldGain = a.goldGain + b.goldGain,
        //         statStoneGain = a.statStoneGain + b.statStoneGain,
        //         expGain = a.expGain + b.expGain,
        //         itemDropRate = a.itemDropRate + b.itemDropRate
        //     };
        // }
        public static RewardStat operator +(RewardStat stat, StatIncrease modifier)
        {
            return new RewardStat
            {
                goldGain = stat.goldGain + modifier.goldGain,
                statStoneGain = stat.statStoneGain + modifier.statStoneGain,
                expGain = stat.expGain + modifier.expGain,
                itemDropRate = stat.itemDropRate + modifier.itemDropRate
            };
        }
    }
    /// <summary> 실제 계산에 이용되는 추가 배율
    /// ex. 피해량 증가, 데미지 감소 등..</summary>
    [Serializable]
    public struct ExtraStat
    {
        public float damageDealtRate; // 피해량 증가율(%)
        public float damageReduceRate; // 피해량 감소(%)
        public static ExtraStat operator +(ExtraStat a, ExtraStat b) =>
            StructMemberCalculator<ExtraStat>.Add(a, b); 
        public static ExtraStat operator +(ExtraStat stat, StatIncrease modifier)
        {
            return new ExtraStat
            {
                damageDealtRate = stat.damageDealtRate + modifier.damageDealtRate,
                damageReduceRate = stat.damageReduceRate + modifier.damageReduction
            };
        }
    }

}