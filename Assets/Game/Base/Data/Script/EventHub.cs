using Base.Managers;
using Base.Save;
using Battle;
using Growth.Equipment;
using Growth.Skill;
using Growth.StatUpgrade;
using Personal.HagYun;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Base.Data
{
    public class EventHub : MonoBehaviour, IGameSystem
    {
        #region UI파트
        public event Action OnButtonClicked; //버튼 클릭
        public void ButtonClicked() => OnButtonClicked?.Invoke();
        public event Action OnPopupOpened; //팝업창 열기
        public void PopupOpened() => OnPopupOpened?.Invoke();
        public event Action OnPopupClosed; //팝업창 닫기
        public void PopupClosed() => OnPopupClosed?.Invoke();       
        #endregion

        #region 스테이지 파트
        public event Action<StageSO> OnChangeStage; //스테이지 변경
        public void StageChanged(StageSO stage) => OnChangeStage?.Invoke(stage);
        public event Action<StageSO> OnClearStage; //스테이지 클리어
        public void StageCleared(StageSO stage) => OnClearStage?.Invoke(stage);
        public event Action<StageSO> OnFailStage; //스테이지 실패
        public void StageFailed(StageSO stage) => OnFailStage?.Invoke(stage);        
        public event Action<StageSO> OnStageChangeClear; //스테이지 변경 완료시 발행
        public void StageChangeClear(StageSO stageSo) => OnStageChangeClear?.Invoke(stageSo);
        #endregion

        #region 전투 파트
        public event Action<Character> OnDeadPlayer; //플레이어 사망
        public void PlayerDead(Character character) => OnDeadPlayer?.Invoke(character);
        public event Action<float, float> OnHpChange; //HP변경
        public void HpChanged(float hp, float maxHp) => OnHpChange?.Invoke(hp, maxHp);
        public event Action OnMonsterHit; //몬스터 피격
        public void MonsterHit() => OnMonsterHit?.Invoke();
        
        //일반 피격 UI / 사운드는 MonsterHit 사용하세요. BossHit은 보스 UI전용입니다
        public event Action OnBossHit; //보스몬스터 피격
        public void BossHit() => OnBossHit?.Invoke(); 
        public event Action OnPlayerHit; //플레이어 피격
        public event Action<MonsterSO> OnMonsterKill; //몬스터 사망
        public void MonsterKill(MonsterSO monsterSO) => OnMonsterKill?.Invoke(monsterSO);
        public event Action<Vector3, int, HitType, bool> OnRequestDamageText;
        public void RequestDamageText(Vector3 position, int damage, HitType type, bool isMonster = true) => OnRequestDamageText?.Invoke(position, damage, type, isMonster);
        
        
        #endregion

        #region 스킬 파트
        public event Action<int> OnSkillSet; 
        public void SkillSet(int order) => OnSkillSet?.Invoke(order);
        public event Action<int> OnSkillUnset;
        public void SkillUnset(int order) => OnSkillUnset?.Invoke(order);
        public void PlayerHit() => OnPlayerHit?.Invoke();
        public event Action<int> OnSkillUsed; //스킬 사용
        public void SkillUsed(int order) => OnSkillUsed?.Invoke(order);
        public event Action<ActiveSkill> OnSkillCanUse; //스킬 사용 가능
        public void SkillCanUse(ActiveSkill skill) => OnSkillCanUse?.Invoke(skill);
        public event Action<int> OnSkillCoolEnd; //스킬 쿨타임 돌았을 때
        public void SkillCoolEnd(int order) => OnSkillCoolEnd?.Invoke(order);
        public event Action OnCastingStart; //스킬 캐스팅 시작
        public void CastingStarted() => OnCastingStart?.Invoke();
        public event Action OnCastingEnd; //스킬 캐스팅 종료
        public void CastingEnd() => OnCastingEnd?.Invoke();
        #endregion

        #region 재화 파트
        public event Action<CurrencyType, int> OnCurrencyChange; //재화 변경
        public void CurrencyChange(CurrencyType type, int amount) => OnCurrencyChange?.Invoke(type, amount);
        public event Action<int> OnLevelChange; //레벨업
        public void LevelChanged(int level) => OnLevelChange?.Invoke(level);
        public event Action OnGetCurrency;
        public void GetCurrency() => OnGetCurrency?.Invoke(); //사운드 연동용 이벤트
        public event Action OnGetItems;
        public void GetItems() => OnGetItems?.Invoke(); // 사운드 연동용 이벤트
        public event Action OnGetEquipments;
        public void GetEquipments() => OnGetEquipments?.Invoke(); // 사운드 연동용 이벤트
        #endregion

        #region 성장 파트
        public event Action<StatusType> OnStatusEnhanced; //스탯 강화
        public void StatusEnhanced(StatusType type) => OnStatusEnhanced?.Invoke(type);
        public event Action<EquipmentSO> OnEquipEnhanced; //장비 강화
        public void EquipEnhanced(EquipmentSO equipmentSo) => OnEquipEnhanced?.Invoke(equipmentSo);
        public event Action<EquipmentSO> OnEquipChanged; //장착 장비 변경
        public void EquipChenged(EquipmentSO equipmentSo) => OnEquipChanged?.Invoke(equipmentSo);
        public event Action OnGetNewEquipment; //새 장비 획득
        public void GetNewEquipment() => OnGetNewEquipment?.Invoke();
        public event Action<SkillSO> OnSkillEnhanced; //스킬 강화
        public void SkillEnhanced(SkillSO skillSo) => OnSkillEnhanced?.Invoke(skillSo);
        public event Action OnSkillInit; //스킬 초기화
        public void InitSkill() => OnSkillInit?.Invoke();
        public event Action<int[]> OnSkillEquipped;
        public void SkillEquipped(int[] skillSlot) => OnSkillEquipped?.Invoke(skillSlot);
        #endregion

        #region 퀘스트
        public event Action OnAutoHuntActivate; //자동사냥 활성화
        public void ActivateAutoHunt() => OnAutoHuntActivate?.Invoke();
        public event Action OnBasicItemEquip;
        public void EquipBasicItem() => OnBasicItemEquip.Invoke();
        public event Action OnSkillEnhance;
        public void EnhanceSkill() => OnSkillEnhance?.Invoke();
        
        public event Action<QuestDataReader, bool> OnQuestCompleted;
        public void QuestCompleted(QuestDataReader data, bool isAllCleared) => OnQuestCompleted?.Invoke(data, isAllCleared);
        public static event Action<string> OnNewDayStarted; //자정이 됐음을 알림(일퀘용)
        public static void NewDayStarted(string dateStr) => OnNewDayStarted?.Invoke(dateStr);

        #endregion
        public int GetOrder() => 0;
    }
}