using System;
using Base.Data;
using UnityEngine;

[CreateAssetMenu(menuName = "Test/Base/Player")]
public class PlayerBaseStatusSO : ScriptableObject
{
    [Header("Base Stat")] public BattleStat baseBattle;
    public RewardStat baseReward;

    [Header("Basic Attack Param")] public float baseAttackRange = 1.5f; //일반공격 범위
    public float baseAttackCooldown = 1.0f; //일반공격 주기
}