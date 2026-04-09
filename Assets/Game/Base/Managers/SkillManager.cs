using Base.Data;
using Base.Managers;
using Base.Save;
using Battle;
using Growth.Skill;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public struct SkillDatas
{
    public SkillSO so; //스킬의 정보
    public int level; //스킬의 현재레벨
    //public bool isUnlocked; 스킬 SO에 스킬 획득가능 레벨 생기면 추가
}

public class SkillManager : MonoBehaviour, IManager
{

    private SkillDictionarySO skillTable;
    private ProgressManager _progressManager;
    private RuntimeProgressData progress;
    private Dictionary<int, int> skillLevelDic => progress.skillProgress.skillProgressState;
    public int PlayerLevel => progress.playerInfo.level;
    public bool IsSkillLvUpPossible => 0 < progress.playerInfo.skillPoint;
    public bool TryGetSkillSO(int key, out SkillSO so)
    {
        so = skillTable?.GetSO(key);
        return so != null;
    }
    // 'key의 스킬을 레벨업/사용 가능한지 여부'를 체크하는 bool 함수
    public bool IsSkillUnlock(int key)
    {
        if (!TryGetSkillSO(key, out SkillSO so)) return false;
        return so.unlockPlayerLv <= PlayerLevel;
    }
    private EventHub eventHub;
    private PlayerEquipSkillController playerEquipSkillController;
    private SkillPool playerSkillPool;
    public bool TryGetSkill(int key, out Skill skill) => playerSkillPool.TryGetSkillByKey(key, out skill);
    public bool TryGetActiveSkill(int key, out ActiveSkill aSkill) => playerSkillPool.TryGetActiveSkillByKey(key, out aSkill);
    public bool TryGetPassiveSkill(int key, out PassiveSkill pSkill) => playerSkillPool.TryGetPassiveSkillByKey(key, out pSkill);
    public IReadOnlyList<Skill> AllSkillArr => playerSkillPool.AllSkillArr;
    public SkillSO[] GetAllSkills() => skillTable?.GetAll();

    public int GetSkillLevel(int key) =>
        (skillLevelDic.TryGetValue(key, out int value) ? value : 0); //값을 찾을 수 있으면 value, 없으면 스킬찍은적 없음

    /// <summary> 스킬 딕셔너리에 있는 모든 스킬이 얼만큼 레벨업 되어있는지 제공하는 메서드
    /// </summary>
    /// <returns></returns>
    public IReadOnlyList<SkillDatas> GetAllSkillInfo()
    {
        List<SkillDatas> datas = new();
        foreach (var skill in skillTable.GetAll())
        {
            datas.Add(new SkillDatas() { level = GetSkillLevel(skill.key), so = skill });
        }
        return datas;
    }
    /// <summary> 스킬 레벨업 가능한지 확인하는 메서드</summary>
    /// <param name="count">스킬 증가시키려는 횟수</param>
    // public bool CanEnhanceSkill(int count) => count < progress.playerInfo.skillPoint;
    public bool CanEnhanceSkill() => 0 < progress.playerInfo.skillPoint;

    public void MaxEnhanceSkill(SkillSO skill)
    {
        EnhanceSkill(skill, progress.playerInfo.skillPoint);
    }
    public void SkillLvMaxUp(int key)
    {
        if (!IsSkillLvUpPossible || !TryGetSkill(key, out Skill skill) || !IsSkillUnlock(key)) return;
        if(!skill.TryLevelMaxUp(progress.playerInfo.skillPoint, out int realLvUpCnt))return;
        skillLevelDic[key] += realLvUpCnt;
        progress.playerInfo.skillPoint -= realLvUpCnt;
    }
    public void SkillLvUp(int key, int count)
    {
        if (!IsSkillLvUpPossible || !TryGetSkill(key, out Skill skill) || !IsSkillUnlock(key)) return;
        if(!skill.TryLvUp(progress.playerInfo.skillPoint, count, out int realLvUpCnt))return;
        skillLevelDic[key] += realLvUpCnt;
        progress.playerInfo.skillPoint -= realLvUpCnt;
    }
    public void EnhanceSkill(SkillSO skill, int count)
    {
        if (skill == null || progress.playerInfo.skillPoint < count)
        {
            Debug.LogWarning($"{skill} == null 또는 스킬포인트가 부족해 스킬을 강화하지 못했습니다. (필요 {count} / 보유 {progress.playerInfo.skillPoint})");
            return;
        }

        if (!skillLevelDic.TryAdd(skill.key, count))
        {
            //이미 해당 키가 추가되어있다면(해당 스킬이 이미 찍혀있음) 수치추가만 한다
            skillLevelDic[skill.key] += count;
        }
        progress.playerInfo.skillPoint -= count;
        Debug.Log($"{skill.skillName}스킬을 {count}만큼 업그레이드 했습니다. 남은 스킬 포인트 : {progress.playerInfo.skillPoint}");
        eventHub.SkillEnhanced(skill);
    }

    public void SkillInit()
    {
        progress.playerInfo.skillPoint = progress.playerInfo.maxSkillPoint;
        progress.skillProgress.skillProgressState = new Dictionary<int, int>();
        eventHub.InitSkill();
    }
    public int GetOrder() => 20;

    public void Init()
    {
        skillTable = GameManager.Instance.GetGameSystem<GameDataProvider>().SkillTable;
        _progressManager = GameManager.Instance.GetGameSystem<ProgressManager>();
        eventHub = GameManager.Instance.GetGameSystem<EventHub>();
        progress = _progressManager.progress;

        playerEquipSkillController = GameManager.Instance.GetGameSystem<PlayerManager>().Player.ESController;
        playerSkillPool = playerEquipSkillController.Pool;
        playerEquipSkillController.SkillEquipInit();
    }
}
