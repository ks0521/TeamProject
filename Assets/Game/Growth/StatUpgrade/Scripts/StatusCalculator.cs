using Base.Data;
using Base.Managers;
using Base.Save;
using Battle;
using Growth.Equipment;
using Growth.Skill;
using Growth.StatUpgrade;
using System;
using UnityEngine;

public class StatusCalculator : MonoBehaviour,IManager
{
    [SerializeField] private GameDataProvider dic;
    [SerializeField] private EventHub eventHub;
    [SerializeField] private RuntimeStatus runtimeStatus; //계산된 값을 저장하는 공간
    [SerializeField] private ProgressManager progressManager;
    [SerializeField] private RuntimeProgressData progress;
    [SerializeField] private StatusSO statusConfig; 
    [SerializeField] private Player player; //플레이어 체력 변동용
    [SerializeField] private TotalStat statIncreaseCache;
    [SerializeField] private TotalStat equipOwnIncreaseCache; //보유효과
    [SerializeField] private TotalStat equipUsingIncreaseCache; //장비 장착효과
    [SerializeField] private TotalStat skillIncreaseCache;

    public void Init()
    {
        runtimeStatus = GameManager.Instance.GetGameSystem<RuntimeStatus>();
        progressManager = GameManager.Instance.GetGameSystem<ProgressManager>();
        dic = GameManager.Instance.GetGameSystem<GameDataProvider>();
        eventHub = GameManager.Instance.GetGameSystem<EventHub>();
        
        BindAll();
        CalcAll();
    }

    void BindAll()
    {
        eventHub.OnGetNewEquipment -= CalcEquip;
        eventHub.OnEquipEnhanced -= CalcEquip;
        eventHub.OnInitSkill -= CalcInitSkill;
        eventHub.OnSkillEnhance -= CalcInitSkill;
        eventHub.OnStatusEnhanced -= CalcStatus;
        
        eventHub.OnGetNewEquipment += CalcEquip;
        eventHub.OnEquipEnhanced += CalcEquip;
        eventHub.OnEquipChanged += CalcEquip;
        eventHub.OnInitSkill += CalcInitSkill;
        eventHub.OnSkillEnhance += CalcInitSkill;
        eventHub.OnStatusEnhanced += CalcStatus;
    }
    public void InputResult()
    {
        runtimeStatus.finalStatus = statIncreaseCache + equipOwnIncreaseCache + equipUsingIncreaseCache + skillIncreaseCache;
        runtimeStatus.finalBattleStatStatus = runtimeStatus.finalStatus.battle; //코드 변경 소프트랜딩용
        runtimeStatus.finalRewardStatStatus = runtimeStatus.finalStatus.reward;
    }
    /// <summary> 모든 성장수단 계산 후 런타임 스탯에 적용 </summary>
    public void CalcAll()
    {
        progress = progressManager.progress;
        CalculateStatus();
        CalculateEquipment();
        CalculateSkill();
        InputResult();
    }
    /// <summary> 장비 성장만 변경사항 적용</summary>
    public void CalcEquip()
    {
        CalculateEquipment();
        InputResult();
    }
    public void CalcEquip(EquipmentSO equip)
    {
        CalculateEquipment();
        InputResult();
    }
    /// <summary> 스탯 성장만 변경사항 적용</summary>
    public void CalcStatus(StatusType type)
    {
        CalculateStatus();
        InputResult();
    }

    public void CalcInitSkill()
    {
        CalculateSkill();
        InputResult();
    }
    /// <summary> 런타임 데이터를 반영해 플레이어 최종스펙 수정</summary>
    /// <param name="runProgressState"></param>
    public void CalculateStatus()
    {
        if (statusConfig.TryGetStatEntry(StatusType.Atk, out StatEntry stat))
        {   //최종 공격력 = 기본 공격력 + 공격력 스탯 업그레이드 수 * 업그레이드 당 증가량
            statIncreaseCache.battle.atk =
                runtimeStatus.baseStat.total.battle.atk+
                progress.statUpgrades.upgradeLevelsByType[StatusType.Atk] * stat.increasePerEnhance;
        }
        else{ Debug.LogWarning($"공격력 SO 찾지 못함");}
        if (statusConfig.TryGetStatEntry(StatusType.MaxHp, out stat))
        {   //최종 최대체력 = 기본 최대체력 + 최대체력 스탯 업그레이드 수 * 업그레이드 당 증가량
            statIncreaseCache.battle.maxHp =
                runtimeStatus.baseStat.total.battle.maxHp +
                progress.statUpgrades.upgradeLevelsByType[StatusType.MaxHp] * stat.increasePerEnhance;
            //플레이어 hp도 동일한 양 증가시켜주기
        }
        else{ Debug.LogWarning($"최대체력 SO 찾지 못함");}

        if (statusConfig.TryGetStatEntry(StatusType.Def, out stat))
        {   //최종 방어력 = 기본 방어력 + 방어력 스탯 업그레이드 수 * 업그레이드 당 증가량
            statIncreaseCache.battle.def =
                runtimeStatus.baseStat.total.battle.def +
                progress.statUpgrades.upgradeLevelsByType[StatusType.Def] * stat.increasePerEnhance;
        }
        else{ Debug.LogWarning($"방어력 SO 찾지 못함");}

        if (statusConfig.TryGetStatEntry(StatusType.AtkSpeed, out stat))
        {   //최종 공격속도 = 기본 공격속도 - (공격스택 스탯 업그레이드 수 * 업그레이드 당 증가량)
            statIncreaseCache.battle.atkSpeed =
                runtimeStatus.baseStat.total.battle.atkSpeed -
                (progress.statUpgrades.upgradeLevelsByType[StatusType.AtkSpeed] * stat.increasePerEnhance);
            runtimeStatus.finalBattleStatStatus.atkSpeed = Mathf.Clamp(runtimeStatus.finalBattleStatStatus.atkSpeed, 0.1f, 3f); //공격속도 증가 디버프 있을수도 있어서
        }
        else{ Debug.LogWarning($"공격속도 SO 찾지 못함");}

        if (statusConfig.TryGetStatEntry(StatusType.CritChance, out stat))
        {   //최종 크리티컬 확률 = 기본 크리티컬 확률 + (크리티컬 확률 스탯 업그레이드 수 * 업그레이드 당 증가량)
            statIncreaseCache.battle.critChance =
                runtimeStatus.baseStat.total.battle.critChance +
                progress.statUpgrades.upgradeLevelsByType[StatusType.CritChance] * stat.increasePerEnhance;
            Mathf.Clamp(runtimeStatus.finalBattleStatStatus.critChance, 0f, 1f); //공격속도 증가 디버프 있을수도 있어서
        }
        else{ Debug.LogWarning($"치명타 확률 SO 찾지 못함");}

        if (statusConfig.TryGetStatEntry(StatusType.CritDmg, out stat))
        {   //최종 크리티컬 피해 = 기본 크리티컬 피해 + (크리티컬 피해 스탯 업그레이드 수 * 업그레이드 당 증가량)
            statIncreaseCache.battle.critDamage =
                runtimeStatus.baseStat.total.battle.critDamage +
                (progress.statUpgrades.upgradeLevelsByType[StatusType.CritDmg] * stat.increasePerEnhance);
        }
        else{ Debug.LogWarning($"치명타 피해 SO 찾지 못함");}

        if (statusConfig.TryGetStatEntry(StatusType.MoveSpeed, out stat))
        {   //최종 이동속도 = 기본 이동속도 + (이동속도 스탯 업그레이드 수 * 업그레이드 당 증가량)
            statIncreaseCache.battle.moveSpeed =
                runtimeStatus.baseStat.total.battle.moveSpeed +
                (progress.statUpgrades.upgradeLevelsByType[StatusType.MoveSpeed] * stat.increasePerEnhance);
        }
        else{ Debug.LogWarning($"이동속도 SO 찾지 못함");}

        if (statusConfig.TryGetStatEntry(StatusType.GoldRate, out stat))
        {   //최종 골드 획득량 = 기본 골드 획득량 + (골드 획득량 스탯 업그레이드 수 * 업그레이드 당 증가량)
            statIncreaseCache.reward.goldGain =
                runtimeStatus.baseStat.total.reward.goldGain +
                (progress.statUpgrades.upgradeLevelsByType[StatusType.GoldRate] * stat.increasePerEnhance);
        }
        else{ Debug.LogWarning($"골드 획득량 SO 찾지 못함");}

        if (statusConfig.TryGetStatEntry(StatusType.ExpRate, out stat))
        {   //최종 경험치 획득량 = 기본 경험치 획득량 + (경험치 획득량 스탯 업그레이드 수 * 업그레이드 당 증가량)
            statIncreaseCache.reward.expGain =
                runtimeStatus.baseStat.total.reward.expGain +
                (progress.statUpgrades.upgradeLevelsByType[StatusType.ExpRate] * stat.increasePerEnhance);
        }
        else{ Debug.LogWarning($"경험치 획득량 SO 찾지 못함");}

        if (statusConfig.TryGetStatEntry(StatusType.ItemDropRate, out stat))
        {   //최종 아이템 드랍 확률 = 기본 아이템 드랍 확률 - (아이템 드랍 확률 스탯 업그레이드 수 * 업그레이드 당 증가량)
            statIncreaseCache.reward.itemDropRate =
                runtimeStatus.baseStat.total.reward.itemDropRate +
                (progress.statUpgrades.upgradeLevelsByType[StatusType.ItemDropRate] * stat.increasePerEnhance);
        }
        else{ Debug.LogWarning($"아이템 드랍률 SO 찾지 못함");}

        statIncreaseCache.battle.atkRange = runtimeStatus.baseStat.total.battle.atkRange; //공격 범위는 고정
    }

    public void CalculateEquipment()
    {
        equipOwnIncreaseCache = new();
        equipUsingIncreaseCache = new();
        EquipmentSO equip;
        if (progress == null)
        {
            progress = GameManager.Instance.GetGameSystem<ProgressManager>().Progress;
        }
        //전체 아이템 보유효과 계산
        foreach (var equipment in progress.equipmentInventory.equipmentEntries)
        {
            equip = dic.equipmentTable.GetSO(equipment.Key);
            equipOwnIncreaseCache += equip.equipBaseIncrease; //보유 기본효과
            equipOwnIncreaseCache += (equip.ownedPerLevelIncrease * equipment.Value.enhancementLevel); //레벨당 상승하는 보유효과
        }
        Debug.Log("장비 보유효과 전부 계산완료");
        //무기
        
        Debug.Log(dic);
        equip = dic.equipmentTable.GetSO(progress.equipment.equippedWeponKey);
        if (equip == null)
        {
            Debug.Log("장착한 장비 없음");
            
        }
        else
        {
            equipUsingIncreaseCache += equip.equipBaseIncrease; //장착장비 기본효과 
            equipUsingIncreaseCache += (equip.equipPerLevelIncrease) * progress.equipmentInventory
                .equipmentEntries[progress.equipment.equippedWeponKey].enhancementLevel; //레벨당 상승하는 장착효과
        }
        /*
        //방어구
        equip = dic.equipmentTable.GetSO(progress.equipment.equippedArmorKey);
        equipUsingIncreaseCache += equip.equipBaseIncrease; //장착장비 기본효과 
        equipUsingIncreaseCache += (equip.equipPerLevelIncrease) * progress.equipmentInventory
            .equipmentEntries[progress.equipment.equippedArmorKey].enhancementLevel; //레벨당 상승하는 장착효과
        //장신구
        equip = dic.equipmentTable.GetSO(progress.equipment.equippedAccessoryKey);
        equipUsingIncreaseCache += equip.equipBaseIncrease; //장착장비 기본효과 
        equipUsingIncreaseCache += (equip.equipPerLevelIncrease) * progress.equipmentInventory
            .equipmentEntries[progress.equipment.equippedAccessoryKey].enhancementLevel; //레벨당 상승하는 장착효과
        */
        
        Debug.Log("장비 장착효과 전부 계산완료");
    }

    public void CalculateSkill()
    {
        skillIncreaseCache = new();
        foreach (var skillKey in progress.skillProgress.skillProgressState.Keys)
        {
            SkillSO skill = dic.SkillTable.GetSO(skillKey);
            if (skill is not PassiveSkillSO || skill == null) continue;
            skillIncreaseCache += ((PassiveSkillSO)skill).ResultAddStat(progress.skillProgress.skillProgressState[skillKey]);
        }
    }
    public int GetOrder() => 2;
}
