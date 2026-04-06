using Base.Data;
using Base.Managers;
using Base.Save;
using Growth.Skill;
using System.Collections.Generic;
using UnityEngine;

public struct SkillDatas
{
    public SkillSO so; //스킬의 정보
    public int level; //스킬의 현재레벨
    //public bool isUnlocked; 스킬 SO에 스킬 획득가능 레벨 생기면 추가
}
public class SkillManager : MonoBehaviour,IManager
{
    private SkillDictionarySO skillTable;
    private PlayerProgressManager playerProgressManager;
    private RuntimeProgressState progress;
    private Dictionary<int, int> skillProgress => progress.skillProgress.skillProgressState;
    private EventHub eventHub;
    
    public SkillSO GetSkill(int key) => skillTable?.GetSO(key);
    public SkillSO[] GetAllSkills() => skillTable?.GetAll();
    public int GetSkillLevel(int key) => 
        (skillProgress.TryGetValue(key, out int value) ? value : 0); //값을 찾을 수 있으면 value, 없으면 스킬찍은적 없음
    
    /// <summary> 스킬 딕셔너리에 있는 모든 스킬이 얼만큼 레벨업 되어있는지 제공하는 메서드
    /// </summary>
    /// <returns></returns>
    public IReadOnlyList<SkillDatas> GetAllSkillInfo()
    {
        List<SkillDatas> datas = new();
        foreach (var skill in skillTable.GetAll())
        {
            datas.Add(new SkillDatas(){level = GetSkillLevel(skill.key), so = skill});
        }
        return datas;
    } 
    public bool CanEnhanceSkill(int count) => count < progress.currency.skillPoint;

    public void MaxEnhanceSkill(SkillSO skill)
    {
        EnhanceSkill(skill,progress.currency.skillPoint);
    }
    public void EnhanceSkill(SkillSO skill, int count)
    {
        if (skill == null || progress.currency.skillPoint < count)
        {
            Debug.LogWarning($"{skill} == null 또는 스킬포인트가 부족해 스킬을 강화하지 못했습니다. (필요 {count} / 보유 {progress.currency.skillPoint})");
            return;
        }

        if (!skillProgress.TryAdd(skill.key, count))
        {
            //이미 해당 키가 추가되어있다면(해당 스킬이 이미 찍혀있음) 수치추가만 한다
            skillProgress[skill.key] += count;
        }
        progress.currency.skillPoint -= count;
        Debug.Log($"{skill.skillName}스킬을 {count}만큼 업그레이드 했습니다. 남은 스킬 포인트 : {progress.currency.skillPoint}");
        eventHub.SkillEnhanced(skill);
    }
    public int GetOrder() => 20;

    public void Init()
    {
        skillTable = GameManager.Instance.GetGameSystem<GameDataProvider>().SkillTable;
        playerProgressManager = GameManager.Instance.GetGameSystem<PlayerProgressManager>();
        eventHub = GameManager.Instance.GetGameSystem<EventHub>();
        progress = playerProgressManager.progress;
    }
}
