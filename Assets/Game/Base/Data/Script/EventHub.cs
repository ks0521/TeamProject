using Base.Managers;
using Base.Save;
using Battle;
using Growth.StatUpgrade;
using JetBrains.Annotations;
using Personal.HagYun;
using System;
using UnityEngine;

namespace Base.Data
{
    public class EventHub : MonoBehaviour, IGameSystem
    {
        public event Action OnPopupOpened; //팝업창 열기
        public void PopupOpened() => OnPopupOpened?.Invoke();
        public event Action OnPopupClosed; //팝업창 닫기
        public void PopupClosed() => OnPopupClosed?.Invoke();

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
        public event Action<int> OnSkillSet;
        public void SkillSet(int order) => OnSkillSet?.Invoke(order);
        public event Action<int> OnSkillUnset;
        public void SkillUnset(int order) => OnSkillUnset?.Invoke(order);
        public event Action<int> OnSkillUsed; //스킬 사용
        public void SkillUsed(int order) => OnSkillUsed?.Invoke(order);
        public event Action<Skill> OnSkillCanUse; //스킬 사용 가능
        public void SkillCanUse(Skill skill) => OnSkillCanUse?.Invoke(skill);
        public event Action<int> OnSkillCoolEnd; //스킬 쿨타임 돌았을 때
        public void SkillCoolEnd(int order) => OnSkillCoolEnd?.Invoke(order);
        public event Action OnCastingStart; //스킬 캐스팅 시작
        public void CastingStarted() => OnCastingStart?.Invoke();
        public event Action OnCastingEnd; //스킬 캐스팅 종료
        public void CastingEnd() => OnCastingEnd?.Invoke();
        public event Action OnButtonClicked; //버튼 클릭
        public void ButtonClicked() => OnButtonClicked?.Invoke();
        public event Action OnMonsterHit; //몬스터 피격
        public void MonsterHit() => OnMonsterHit?.Invoke();
        public event Action OnPlayerHit; //플레이어 피격
        public void PlayerHit() => OnPlayerHit?.Invoke();
        public event Action<CurrencyType, int> OnCurrencyChange; //재화 변경
        public void CurrencyChange(CurrencyType type, int amount) => OnCurrencyChange?.Invoke(type, amount);
        public event Action<int> OnLevelChange; //레벨업
        public void LevelChanged(int level) => OnLevelChange?.Invoke(level);
        public event Action OnGetCurrency;
        public void GetCurrency() => OnGetCurrency?.Invoke(); //사운드 연동용 이벤트
        public event Action OnGetItems;
        public void GetItems() => OnGetItems?.Invoke(); // 사운드 연동용 이벤트
        public event Action<StageSO> OnStageChangeClear; //스테이지 변경 완료시 발행
        public void StageChangeClear(StageSO stageSo) => OnStageChangeClear?.Invoke(stageSo);
        public int GetOrder() => 0;
    }
}