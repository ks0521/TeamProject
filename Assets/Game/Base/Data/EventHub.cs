using Base.Managers;
using Battle;
using Growth.StatUpgrade;
using Personal.HagYun;
using System;
using UnityEngine;

namespace Base.Data
{
    public class EventHub : MonoBehaviour, IGameSystem
    {
        public event Action<StageSO> OnChangeStage; //스테이지 변경
        public void StageChanged(StageSO stage) => OnChangeStage?.Invoke(stage);
        public event Action<StageSO> OnClearStage; //스테이지 클리어
        public void StageCleared(StageSO stage) => OnClearStage?.Invoke(stage);
        public event Action<StageSO> OnFailStage; //스테이지 실패
        public void StageFailed(StageSO stage) => OnFailStage?.Invoke(stage);
        public event Action<Character> OnDeadPlayer; //플레이어 사망
        public void PlayerDead(Character character) => OnDeadPlayer?.Invoke(character);
        public event Action<float, float> OnHpChange; //HP변경
        public void HpChanged(float hp, float maxHp) => OnHpChange?.Invoke(hp, maxHp);
        public event Action<Skill> OnSkillUsed; //스킬 사용
        public void SkillUsed(Skill skill) => OnSkillUsed?.Invoke(skill);
        public event Action<Skill> OnSkillCanUse; //스킬 사용 가능
        public void SkillCanUse(Skill skill) => OnSkillCanUse?.Invoke(skill); 
        public event Action OnCastingStart; //스킬 캐스팅 시작
        public void CastingStarted() => OnCastingStart?.Invoke();
        public event Action OnButtonClicked; //버튼 클릭
        public void ButtonClicked() => OnButtonClicked?.Invoke();
        public event Action OnMonsterHit; //몬스터 피격
        public void MonsterHit() => OnMonsterHit?.Invoke();
        public event Action<int> OnGoldChange; //골드 수치 변경
        public void GoldChanged(int gold) => OnGoldChange?.Invoke(gold);
        public event Action<int> OnStatStoneChange; //스탯 강화석 수치 변경
        public void StatStoneChanged(int statStone) => OnStatStoneChange?.Invoke(statStone);
        public event Action<int> OnExpChange; //경험치 변경
        public void ExpChanged(int exp) => OnExpChange?.Invoke(exp);

        
        public int GetOrder() => 0;
    }
}