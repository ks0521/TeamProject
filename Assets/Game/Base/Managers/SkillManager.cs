using Base.Data;
using Base.Managers;
using Base.Save;
using Battle;
using Growth.Skill;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Unity.VisualScripting;
using UnityEngine;

public class SkillManager : MonoBehaviour, IManager
{

    private SkillDictionarySO skillTable;
    private ProgressManager _progressManager;
    private RuntimeProgressData progress;
    private Dictionary<int, int> skillCurLevelDic => progress.skillProgress.skillProgressState;
    public int PlayerLevel => progress.playerInfo.level;
    public int PlayerSkillPoint => progress.playerInfo.skillPoint;
    public bool IsSkillPointUsePossible => 0 < PlayerSkillPoint;
    public bool TryGetSkillSO(int key, out SkillSO so)
    {
        so = skillTable?.GetSO(key);
        return so != null;
    }
    public bool CheckSkillCurLevelIsMax(SkillSO so)
    {
        if (so == null || !skillCurLevelDic.TryGetValue(so.key, out int curLv)) return false;
        return so.maxLv == curLv;
    }
    public bool IsSkillUnlock(SkillSO so)
    {
        if (so == null) return false;
        return so.unlockPlayerLv <= PlayerLevel;
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
            Debug.LogWarning($"IsSkillLvUpPossibe : 포인트 없음");
            return false;
        }
        else if(!IsSkillUnlock(so))
        {
            Debug.LogWarning($"IsSkillLvUpPossibe : {key}번 {skillName} 스킬 잠김");
            return false;
        }
        else if(CheckSkillCurLevelIsMax(so))
        {
            Debug.LogWarning($"IsSkillLvUpPossibe : {key}번 {skillName} 현재 스킬 레벨이 최대");
            return false;
        }
        return true;
        // if (IsSkillPointUsePossible && IsSkillUnlock(so) && !CheckSkillCurLevelIsMax(so)) return true;
        // return false;
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

    public int GetSkillLevel(int key) =>
        skillCurLevelDic.TryGetValue(key, out int value) ? value : 0; //값을 찾을 수 있으면 value, 없으면 스킬찍은적 없음
    // public bool CanEnhanceSkill() => 0 < progress.playerInfo.skillPoint;
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
            Debug.LogWarning($"SkillLevelUp Check : {key}번 {skill.SkillData.skillName} 스킬 찾지 못함");
        return false;
    }
    void LevelUpFeat(int key, Skill skill, int lvUpCnt)
    {
        ref int skillPoint = ref progress.playerInfo.skillPoint;
        // lvUpCnt : 스킬 레벨업 시도 카운트
        // 스킬포인트보다 레벨업 시도 카운트가 높을 경우, 레벨업 시도 카운트를 스킬 포인트에 맞춤
        if (skillPoint < lvUpCnt) lvUpCnt = skillPoint;
        if(skillCurLevelDic == null)Debug.LogWarning("skillCurLevelDic 없음");
        else if(!skillCurLevelDic.TryGetValue(key, out int nar))Debug.LogWarning($"skillCurLevelDic에 {key}번 없음");
        skillCurLevelDic[key] += lvUpCnt;
        skillPoint -= lvUpCnt;
        skill.StatUpdate();
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
        int count = so.maxLv - skillCurLevelDic[key];

        LevelUpFeat(key, skill, count);
        Debug.Log($"{key}번 스킬 {so.skillName} 최대 레벨업");
    }
    // SkillEnhance -> SkillLvUp 으로 변경
    // key를 통해 스킬 탐색
    // public void EnhanceSkill(SkillSO skill, int count)
    public void SkillLvUp(int key, int count)
    {

        // if (skill == null || progress.playerInfo.skillPoint < count)
        // {
        //     Debug.LogWarning($"{skill} == null 또는 스킬포인트가 부족해 스킬을 강화하지 못했습니다. (필요 {count} / 보유 {progress.playerInfo.skillPoint})");
        //     return;
        // }

        // if (!skillLevelDic.TryAdd(skill.key, count))
        // {
        //     //이미 해당 키가 추가되어있다면(해당 스킬이 이미 찍혀있음) 수치추가만 한다
        //     skillLevelDic[skill.key] += count;
        // }
        // progress.playerInfo.skillPoint -= count;
        // Debug.Log($"{skill.skillName}스킬을 {count}만큼 업그레이드 했습니다. 남은 스킬 포인트 : {progress.playerInfo.skillPoint}");
        // eventHub.SkillEnhanced(skill);

        if (TrySkillLvUpPossible(key, out Skill skill, out SkillSO so)) return;
        // 현재 레벨
        int curLv = skillCurLevelDic[key];
        // 현재 레벨 < 목표 카운트 : 목표 카운트를 현재 레벨에 맞춤
        if (curLv < count) count = curLv;

        int lvUpCnt = so.maxLv - count;

        LevelUpFeat(key, skill, count);
    }
    public void SkillLvReset(int key)
    {
        if (!TryGetSkill(key, out Skill skill))
        {
            Debug.LogWarning($"{key}번 스킬 {skill.SkillData.skillName} 레벨 초기화 불가");
            return;
        }
        int curLv = skillCurLevelDic[key];
        if (curLv <= 0) return;

        ref int skillPoint = ref progress.playerInfo.skillPoint;

        skillCurLevelDic[key] = 0;
        skillPoint += curLv;
        skill.StatUpdate();
        eventHub.SkillLevelChange(skill);

        Debug.Log($"{key}번 스킬 {skill.SkillData.skillName} 레벨 초기화");
    }
    public void SkillAllReset()
    {
        foreach (var skill in AllSkillList)
        {
            SkillLvReset(skill.SkillData.key);
        }
        eventHub.InitSkill();
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
        playerEquipSkillController = GameManager.Instance.GetGameSystem<PlayerManager>().Player.ESController;
        playerEquipSkillController.SkillEquipInit(AllSkillSOList, skillCurLevelDic);
        PlayerEquipSkillList = playerEquipSkillController.EquipSkillList;
        Debug.Log($"{playerEquipSkillController.EquipSkillList.Count}, {PlayerEquipSkillList.Count}");
        playerSkillPool = playerEquipSkillController.Pool;
    }
    void EventAddListner()
    {
        eventHub.OnSkillLevelOneUpInput += SkillLvOneUp;
        eventHub.OnSkillLevelMaxUpInput += SkillLvMaxUp;
        eventHub.OnSkillLevelResetInput += SkillAllReset;
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
    }
}
