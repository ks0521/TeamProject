using System;
using System.Collections;
using System.Collections.Generic;
using Base.Save;
using Growth.StatUpgrade;
using UnityEngine;

public class StatusCalculator : MonoBehaviour
{
    [SerializeField] private PlayerRuntimeStatus runtimeStatus;
    [SerializeField] private StatusSO statusConfig;

    private void Awake()
    {
        if(runtimeStatus == null)
            runtimeStatus = GetComponent<PlayerRuntimeStatus>();
    }

    /// <summary> 런타임 데이터를 반영해 플레이어 최종스펙 수정</summary>
    /// <param name="runData"></param>
    public void Calculate(RuntimeData runData)
    {
        Debug.Log("계산 시작");
        StatEntry stat;
        if (statusConfig.TryGetStatEntry(StatusType.Atk, out stat))
        {   //최종 공격력 = 기본 공격력 + 공격력 스탯 업그레이드 수 * 업그레이드 당 증가량
            runtimeStatus.finalBattleStatus.atk =
                runtimeStatus.baseStat.baseBattle.atk +
                runData.stat.upgrade[StatusType.Atk] * stat.increasePerEnhance;
        }
        else{ Debug.LogWarning($"공격력 SO 찾지 못함");}
        if (statusConfig.TryGetStatEntry(StatusType.MaxHp, out stat))
        {   //최종 최대체력 = 기본 최대체력 + 최대체력 스탯 업그레이드 수 * 업그레이드 당 증가량
            runtimeStatus.finalBattleStatus.maxHp =
                runtimeStatus.baseStat.baseBattle.maxHp +
                runData.stat.upgrade[StatusType.MaxHp] * stat.increasePerEnhance;
            //플레이어 hp도 동일한 양 증가시켜주기
        }
        else{ Debug.LogWarning($"최대체력 SO 찾지 못함");}

        if (statusConfig.TryGetStatEntry(StatusType.Def, out stat))
        {   //최종 방어력 = 기본 방어력 + 방어력 스탯 업그레이드 수 * 업그레이드 당 증가량
            runtimeStatus.finalBattleStatus.def =
                runtimeStatus.baseStat.baseBattle.def +
                runData.stat.upgrade[StatusType.Def] * stat.increasePerEnhance;
        }
        else{ Debug.LogWarning($"방어력 SO 찾지 못함");}

        if (statusConfig.TryGetStatEntry(StatusType.AtkSpeed, out stat))
        {   //최종 공격속도 = 기본 공격속도 - (공격스택 스탯 업그레이드 수 * 업그레이드 당 증가량)
            runtimeStatus.finalBattleStatus.atkSpeed =
                runtimeStatus.baseStat.baseBattle.atkSpeed -
                (runData.stat.upgrade[StatusType.AtkSpeed] * stat.increasePerEnhance);
            runtimeStatus.finalBattleStatus.atkSpeed = Mathf.Clamp(runtimeStatus.finalBattleStatus.atkSpeed, 0.1f, 3f); //공격속도 증가 디버프 있을수도 있어서
        }
        else{ Debug.LogWarning($"공격속도 SO 찾지 못함");}

        if (statusConfig.TryGetStatEntry(StatusType.CritChance, out stat))
        {   //최종 크리티컬 확률 = 기본 크리티컬 확률 + (크리티컬 확률 스탯 업그레이드 수 * 업그레이드 당 증가량)
            runtimeStatus.finalBattleStatus.critChance =
                runtimeStatus.baseStat.baseBattle.critChance +
                runData.stat.upgrade[StatusType.CritChance] * stat.increasePerEnhance;
            Mathf.Clamp(runtimeStatus.finalBattleStatus.critChance, 0f, 1f); //공격속도 증가 디버프 있을수도 있어서
        }
        else{ Debug.LogWarning($"치명타 확률 SO 찾지 못함");}

        if (statusConfig.TryGetStatEntry(StatusType.CritDmg, out stat))
        {   //최종 크리티컬 피해 = 기본 크리티컬 피해 + (크리티컬 피해 스탯 업그레이드 수 * 업그레이드 당 증가량)
            runtimeStatus.finalBattleStatus.critDamage =
                runtimeStatus.baseStat.baseBattle.critDamage +
                (runData.stat.upgrade[StatusType.CritDmg] * stat.increasePerEnhance);
        }
        else{ Debug.LogWarning($"치명타 피해 SO 찾지 못함");}

        if (statusConfig.TryGetStatEntry(StatusType.MoveSpeed, out stat))
        {   //최종 이동속도 = 기본 이동속도 + (이동속도 스탯 업그레이드 수 * 업그레이드 당 증가량)
            runtimeStatus.finalBattleStatus.moveSpeed =
                runtimeStatus.baseStat.baseBattle.moveSpeed +
                (runData.stat.upgrade[StatusType.MoveSpeed] * stat.increasePerEnhance);
        }
        else{ Debug.LogWarning($"이동속도 SO 찾지 못함");}

        if (statusConfig.TryGetStatEntry(StatusType.GoldRate, out stat))
        {   //최종 골드 획득량 = 기본 골드 획득량 + (골드 획득량 스탯 업그레이드 수 * 업그레이드 당 증가량)
            runtimeStatus.finalRewardStatus.goldRate =
                runtimeStatus.baseStat.baseReward.goldRate +
                (runData.stat.upgrade[StatusType.GoldRate] * stat.increasePerEnhance);
        }
        else{ Debug.LogWarning($"골드 획득량 SO 찾지 못함");}

        if (statusConfig.TryGetStatEntry(StatusType.ExpRate, out stat))
        {   //최종 경험치 획득량 = 기본 경험치 획득량 + (경험치 획득량 스탯 업그레이드 수 * 업그레이드 당 증가량)
            runtimeStatus.finalRewardStatus.expRate =
                runtimeStatus.baseStat.baseReward.expRate +
                (runData.stat.upgrade[StatusType.ExpRate] * stat.increasePerEnhance);
        }
        else{ Debug.LogWarning($"경험치 획득량 SO 찾지 못함");}

        if (statusConfig.TryGetStatEntry(StatusType.ItemDropRate, out stat))
        {   //최종 아이템 드랍 확률 = 기본 아이템 드랍 확률 - (아이템 드랍 확률 스탯 업그레이드 수 * 업그레이드 당 증가량)
            runtimeStatus.finalRewardStatus.itemDropRateBonus =
                runtimeStatus.baseStat.baseReward.itemDropRateBonus +
                (runData.stat.upgrade[StatusType.ItemDropRate] * stat.increasePerEnhance);
        }
        else{ Debug.LogWarning($"아이템 드랍률 SO 찾지 못함");}

    }
}
