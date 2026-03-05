using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Personal.GyuSeong{
    [Serializable]
    public struct RewardStat
    {
        [Range(-1f, 10f)] public float goldRate; //골드획득, 배수로 작용(0.2 = 20%)
        [Range(-1f, 10f)] public float expRate; //경험치획득
        [Range(-1, 10f)] public float itemRate; //아이템 드랍 확률

        public static RewardStat operator +(RewardStat a, RewardStat b)
        {
            return new RewardStat
            {
                goldRate = a.goldRate + b.goldRate,
                expRate = a.expRate + b.expRate,
                itemRate = a.itemRate + b.itemRate
            };
        }
    }
    [Serializable]
    public struct BattleStat
    {
        
        [Header("Core")] 
        public float maxHp; //최대 체력
        public float atk; //공격력
        public float def; //방어력

        [Header("Speed")] 
        public float atkSpeed; //공격속도, 1이 기본값으로 해당 수치는 배율처럼 작동
        //1.2 = 20% 공격속도 증가
        public float moveSpeed; //이동속도

        [Header("Chance")] 
        [Range(0, 1)] public float critChance;

        public static BattleStat operator +(BattleStat a, BattleStat b)
        {
            return new BattleStat
            {
                maxHp =  a.maxHp + b.maxHp,
                atk = a.atk + b.atk,
                def = a.def + b.def,
                atkSpeed = a.atkSpeed+b.atkSpeed,
                moveSpeed = a.moveSpeed + b.moveSpeed,
                critChance = a.critChance+b.critChance,
            };
        }
    }
    [CreateAssetMenu(menuName = "Test/Base/Player")]
    public class PlayerTester : ScriptableObject
    {
        [Header("Base Stat")] 
        public BattleStat baseBattle;
        public RewardStat baseReward;

        [Header("Basic Attack Param")] 
        public float baseAttackRange = 1.5f;
        public float baseAttackCooldown = 1.0f;
    }
}
