using Base.Data;
using Base.Managers;
using Base.Save;
using Battle;
using Growth.Skill;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour, IManager
{
    private SkillDictionarySO skillTable;
    private ProgressManager _progressManager;
    private RuntimeProgressData progress;
    public int[] SaveEquippedSkills => progress.skillProgress.skillSlots;
    private Dictionary<int, int> skillCurLevelDic => progress.skillProgress.skillProgressState;
    public int PlayerLevel => progress.playerInfo.level;
    public int PlayerSkillPoint => progress.playerInfo.skillPoint;
    public bool IsSkillResetPossible { get; private set; }
    public bool IsSkillPointUsePossible => 0 < PlayerSkillPoint;
    public bool TryGetSkillSO(int key, out SkillSO so)
    {
        so = skillTable?.GetSO(key);
        return so != null;
    }
    public bool TryGetSaveEquippedSkill(int slotNum, out ActiveSkill aSkill)
    {
        if (slotNum < 0 || 6 <= slotNum)
        {
            aSkill = null;
            return false;
        }

        return TryGetActiveSkill(SaveEquippedSkills[slotNum], out aSkill);
    }
    public bool CheckSkillCurLevelIsMax(SkillSO so)
    {
        if (so == null || !TryGetSkillLevel(so.key, out int curLv)) return false;
        return so.maxLv == curLv;
    }
    public bool IsSkillUnlock(int key)
    {
        if (!TryGetSkillSO(key, out var so)) return false;
        return so.unlockPlayerLv <= PlayerLevel;
    }
    public bool IsSkillUnlock(SkillSO so)
    {
        if (so == null) return false;
        return so.unlockPlayerLv <= PlayerLevel;
    }
    public bool TryGetSkillLevel(int key, out int curLv)
    {
        if (!skillCurLevelDic.TryGetValue(key, out curLv)) return false;
        return true;
    }
    public bool TryGetSkillLevel(SkillSO so, out int curLv)
    {
        if (!skillCurLevelDic.TryGetValue(so.key, out curLv)) return false;
        return true;
    }
    public bool IsSkillLvUpPossible(int key)
    {
        if (!TryGetSkillSO(key, out var so) && !IsSkillLvUpPossibe(so)) return false;
        return true;
    }
    public bool IsSkillEquipPossible(int key)
    {
        if (!TryGetSkillLevel(key, out int curLv)) return false;
        return true;
    }
    public bool IsSkillLvUpPossibe(SkillSO so)
    {
        // SkillPoint 사용 가능할 때
        // Skill Unlock 되었을 때
        // Skill CurLv이 MaxLv이 아닐 때 true
        int key = so.key;
        string skillName = so.skillName;
        if (!IsSkillPointUsePossible)
        {
            // Debug.LogWarning($"IsSkillLvUpPossibe : 포인트 없음");
            return false;
        }
        else if (!IsSkillUnlock(so))
        {
            // Debug.LogWarning($"IsSkillLvUpPossibe : {key}번 {skillName} 스킬 잠김");
            return false;
        }
        else if (CheckSkillCurLevelIsMax(so))
        {
            // Debug.LogWarning($"IsSkillLvUpPossibe : {key}번 {skillName} 현재 스킬 레벨이 최대");
            return false;
        }
        return true;
    }
    private EventHub eventHub;
    private PlayerEquipSkillController playerEquipSkillController;
    private SkillPool playerSkillPool;
    public bool TryGetSkill(int key, out Skill skill) => playerSkillPool.TryGetSkillByKey(key, out skill);
    public bool TryGetActiveSkill(int key, out ActiveSkill aSkill) => playerSkillPool.TryGetActiveSkillByKey(key, out aSkill);
    public bool TryGetPassiveSkill(int key, out PassiveSkill pSkill) => playerSkillPool.TryGetPassiveSkillByKey(key, out pSkill);
    public IReadOnlyList<Skill> AllSkillList => playerSkillPool.AllSkillArr;
    public int ActiveSkillCnt => playerSkillPool.ActiveSkillCnt;
    public int PassiveSkillCnt => playerSkillPool.PassiveSkillCnt;
    public IReadOnlyList<SkillSO> AllSkillSOList { get; private set; }
    public IReadOnlyList<EquipSkill> PlayerEquipSkillList { get; private set; }

    public bool TrySkillLvUpPossible(int key, out Skill skill, out SkillSO so)
    {
        skill = null;
        so = null;
        if (TryGetSkill(key, out skill))
        {
            so = skill.SkillData;
            if (IsSkillLvUpPossibe(so))
                return true;
        }
        // Debug.LogWarning($"SkillLevelUp Check : {key}번 {skill.SkillData.skillName} 스킬 찾지 못함");
        return false;
    }
    void LevelUpFeat(int key, Skill skill, int lvUpCnt)
    {
        ref int skillPoint = ref progress.playerInfo.skillPoint;
        // lvUpCnt : 스킬 레벨업 시도 카운트
        // 스킬포인트보다 레벨업 시도 카운트가 높을 경우, 레벨업 시도 카운트를 스킬 포인트에 맞춤
        if (skillPoint < lvUpCnt) lvUpCnt = skillPoint;
        if (skillCurLevelDic == null) Debug.LogWarning("skillCurLevelDic 없음");
        else if (!skillCurLevelDic.TryGetValue(key, out int curLv))
        {
            Debug.LogWarning($"skillCurLevelDic에 {key}번 없음, {key}번 Dic 생성");
            skillCurLevelDic.Add(key, 0);
        }
        skillCurLevelDic[key] += lvUpCnt;
        skillPoint -= lvUpCnt;
        skill.StatUpdate();
        IsSkillResetPossible = true;
        eventHub.SkillLevelChange(skill);
    }
    public void SkillLvOneUp(int key)
    {
        if (!TrySkillLvUpPossible(key, out Skill skill, out SkillSO so))
        {
            Debug.LogWarning($"{key}번 스킬 {so.skillName} 1번 레벨업 불가");
            return;
        }
        LevelUpFeat(key, skill, 1);
        Debug.Log($"{key}번 스킬 {so.skillName} 1번 레벨업");
    }
    public void SkillLvMaxUp(int key)
    {
        if (!TrySkillLvUpPossible(key, out Skill skill, out SkillSO so))
        {
            Debug.LogWarning($"{key}번 스킬 {so.skillName} 최대 레벨업 불가");
            return;
        }
        int count = so.maxLv - (skillCurLevelDic.TryGetValue(key, out int curLv) ? curLv : 0);

        LevelUpFeat(key, skill, count);
        Debug.Log($"{key}번 스킬 {so.skillName} 최대 레벨업");
    }
    public void SkillLvUp(int key, int count)
    {

        if (TrySkillLvUpPossible(key, out Skill skill, out SkillSO so)) return;
        // 현재 레벨
        TryGetSkillLevel(key, out int curLv);
        // 현재 레벨 < 목표 카운트 : 목표 카운트를 현재 레벨에 맞춤
        if (curLv < count) count = curLv;

        int lvUpCnt = so.maxLv - count;

        LevelUpFeat(key, skill, lvUpCnt);
    }
    public void SkillLvReset(int key)
    {
        if (!TryGetSkill(key, out Skill skill))
        {
            Debug.LogWarning($"{key}번 스킬 {skill.SkillData.skillName} 레벨 초기화 불가");
            return;
        }
        if (!TryGetSkillLevel(key, out int curLv)) return;

        progress.playerInfo.skillPoint += curLv;
        skillCurLevelDic.Remove(key);
        skill.StatUpdate();
        eventHub.SkillLevelChange(skill);
        IsSkillResetPossible = false;
        Debug.Log($"{key}번 스킬 {skill.SkillData.skillName} 레벨 초기화");
    }
    void SkillAutoEquip(Skill skill)
    {
        if (!TryGetSkillLevel(skill.SkillData.key, out int curLv)) return;

        if (!(skill is ActiveSkill aSkill)) return;
        else if (playerEquipSkillController.IsThisSkillEquippedOtherSlot(aSkill)) return;
        for (int i = 0; i < 6; i++)
        {
            if (!playerEquipSkillController[i].isEquipped)
            {
                Debug.Log($"{i}번 슬롯에 {aSkill.SkillData.key}번 스킬 {aSkill.SkillData.skillName} 장착");
                playerEquipSkillController.SkillEquip(i, aSkill);
                break;
            }
        }
    }
    void SkillAutoUnequip(Skill skill)
    {
        if (TryGetSkillLevel(skill.SkillData.key, out int curLv)) return;

        if (!(skill is ActiveSkill aSkill)) return;
        else if (playerEquipSkillController.IsThisSkillEquippedOtherSlot(aSkill, out int equippedIndex))
        {
            // Debug.Log($"{equippedIndex}번 슬롯의 {aSkill.SkillData.key}번 스킬 {aSkill.SkillData.skillName} 해제");
            playerEquipSkillController.SkillUnequip(equippedIndex);
        }
    }
    public void SkillAllReset()
    {
        foreach (var skill in AllSkillList)
        {
            SkillLvReset(skill.SkillData.key);
        }
        eventHub.InitSkill();
    }
    public bool IsSkillEquippedByKey(int skillKey)
    {
        for (int i = 0; i < 6; i++)
        {
            if (PlayerEquipSkillList[i].EquippedSkillKey == skillKey)
                return true;
        }
        return false;
    }
    public bool TryGetEquipSkillBySlotNum(int slotNum, out EquipSkill eSkill)
    {
        eSkill = null;
        if(slotNum < 0 || 6 <= slotNum) return false;
        eSkill = PlayerEquipSkillList[slotNum];
        return true;
    }
    public bool TryGetEquipSkillByKey(int skillKey, out EquipSkill eSkill, out int skillSlotIndex)
    {
        eSkill = null;
        skillSlotIndex = -1;
        for(int i = 0; i < 6; i++)
        {
            var curESkill = PlayerEquipSkillList[i];
            if(!TryGetActiveSkill(skillKey, out var aSkill) && skillKey == curESkill.EquippedSkillKey)
            {
                eSkill = curESkill;
                skillSlotIndex = i;
                return true;
            }
        }
        return false;
    }
    public bool TryGetEquipSkillByKey(int skillKey, out EquipSkill eSkill) => 
    TryGetEquipSkillByKey(skillKey, out eSkill, out int num);
    public bool TryGetSkillPriority(int slotNum, out Priority pri)
    {
        pri = Priority.Low;
        if (slotNum < 0 || 6 <= slotNum) return false;
        var curESkill = PlayerEquipSkillList[slotNum];
        if (!curESkill.isEquipped) return false;
        pri = PlayerEquipSkillList[slotNum].priority;
        return true;
    }
    public bool TryChangeEquipSkillPriority(int slotNum, Priority pri)
    {
        if (slotNum < 0 || 6 <= slotNum) return false;
        var curESkill = PlayerEquipSkillList[slotNum];
        if (!curESkill.isEquipped) return false;
        curESkill.priority = pri;
        return true;
    }
    public void ChangeEquipSkillPriority(int slotNum, Priority pri) => TryChangeEquipSkillPriority(slotNum, pri);

    public bool TryGetSkillKeyByEquipSkill(EquipSkill eSkill, out int skillKey)
    {
        skillKey = -1;
        if(!eSkill.isEquipped)return false;
        skillKey = eSkill.EquippedSkillKey;
        return true;
    }
    public bool TryGetSkillKeyByEquipSkill(int slotNum, out int skillKey)
    {
        skillKey = -1;
        if(slotNum < 0 || 6 <= slotNum)return false;
        skillKey = PlayerEquipSkillList[slotNum].EquippedSkillKey;
        return true;
    }
    public void SkillInit()
    {
        progress.playerInfo.skillPoint = progress.playerInfo.maxSkillPoint;

        eventHub.InitSkill();
    }
    public int GetOrder() => 20;

    public void Init()
    {
        skillTable = GameManager.Instance.GetGameSystem<GameDataDictionaries>().SkillTable;
        _progressManager = GameManager.Instance.GetGameSystem<ProgressManager>();
        eventHub = GameManager.Instance.GetGameSystem<EventHub>();
        progress = _progressManager.progress;
        if (progress == null | progress.skillProgress == null) { Debug.LogWarning("skillManager : progress 없음"); }
        AllSkillSOList = skillTable?.GetAll();
        EventAddListner();
        SkillInit();
        PlayerEquipSkillControllerInit();
        InitResetPossibleCheck();
    }
    void PlayerEquipSkillControllerInit()
    {
        Player pl = GameManager.Instance.GetGameSystem<PlayerManager>().Player;
        playerEquipSkillController = pl.ESController;
        playerSkillPool = playerEquipSkillController.Pool;

        playerSkillPool.Init(this);

        playerEquipSkillController.Init(pl);
        playerEquipSkillController.SkillEquipInit();
        PlayerEquipSkillList = playerEquipSkillController.EquipSkillList;
    }
    void InitResetPossibleCheck()
    {
        foreach (var so in AllSkillSOList)
        {
            if (skillCurLevelDic.TryGetValue(so.key, out int curLv))
            {
                Debug.Log("Reset 가능");
                IsSkillResetPossible = true;
                return;
            }
        }
        IsSkillResetPossible = false;
    }
    // void SkillEquipSave(int slotIndex, int skillKey) => progress.skillProgress.skillSlots[slotIndex] = skillKey;
    void SkillEquipSave(int slotIndex, ActiveSkill aSkill)
        => progress.skillProgress.skillSlots[slotIndex] = aSkill.SkillData.key;
    void SkillUnequipSave(int slotIndex) => progress.skillProgress.skillSlots[slotIndex] = -1;
    void EventAddListner()
    {
        eventHub.OnSkillLevelOneUpInput += SkillLvOneUp;
        eventHub.OnSkillLevelMaxUpInput += SkillLvMaxUp;
        eventHub.OnSkillLevelResetInput += SkillAllReset;

        eventHub.OnSkillLevelChange += SkillAutoEquip;
        eventHub.OnSkillLevelChange += SkillAutoUnequip;

        eventHub.OnSkillEquipComplete += SkillEquipSave;
        eventHub.OnSkillUnset += SkillUnequipSave;

        eventHub.OnEquipSkillPriorityChange += ChangeEquipSkillPriority;
    }
    void OnDestroy()
    {
        EventRemoveListner();
    }
    void EventRemoveListner()
    {
        eventHub.OnSkillLevelOneUpInput -= SkillLvOneUp;
        eventHub.OnSkillLevelMaxUpInput -= SkillLvMaxUp;
        eventHub.OnSkillLevelResetInput -= SkillAllReset;

        eventHub.OnSkillLevelChange -= SkillAutoEquip;
        eventHub.OnSkillLevelChange -= SkillAutoUnequip;

        eventHub.OnSkillEquipComplete -= SkillEquipSave;
        eventHub.OnSkillUnset -= SkillUnequipSave;
        
        eventHub.OnEquipSkillPriorityChange -= ChangeEquipSkillPriority;
    }
}
