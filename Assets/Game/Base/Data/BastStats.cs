using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Base.Data
{
    /// <summary> 플레이어용, 보상 획득에 관한 스탯 </summary>
    [Serializable]
    public struct RewardStat
    {
        [Range(-1f, 100f)] public float goldRate; //골드획득률(배율)
        [Range(-1f, 100f)] public float expRate; //경험치획득률(배율)
        [Range(-1, 10f)] public float itemDropRateBonus; //아이템 드랍 확률(배율)

        public static RewardStat operator +(RewardStat a, RewardStat b)
        {
            return new RewardStat
            {
                goldRate = a.goldRate + b.goldRate,
                expRate = a.expRate + b.expRate,
                itemDropRateBonus = a.itemDropRateBonus + b.itemDropRateBonus
            };
        }
    }
    /// <summary> 플레이어 적 공통, 전투관련 스탯 </summary>
    [Serializable]
    public struct BattleStat
    {
        [Header("Core")] public float maxHp; //최대 체력
        public float atk; //공격력
        public float def; //방어력

        [Header("Speed")] public float atkSpeed; //공격속도, 1이 기본값으로 해당 수치는 배율처럼 작동

        //1.2 = 20% 공격속도 증가
        public float moveSpeed; //이동속도

        [Header("Critical")] [Range(0, 1)] public float critChance; //크확(0 = 0%, 1 = 100%)
        public float critDamage; //치피

        public static BattleStat operator +(BattleStat a, BattleStat b)
        {
            return new BattleStat
            {
                maxHp = a.maxHp + b.maxHp,
                atk = a.atk + b.atk,
                def = a.def + b.def,
                atkSpeed = a.atkSpeed + b.atkSpeed,
                moveSpeed = a.moveSpeed + b.moveSpeed,
                critChance = a.critChance + b.critChance,
                critDamage = a.critDamage + b.critDamage
            };
        }
    }

}