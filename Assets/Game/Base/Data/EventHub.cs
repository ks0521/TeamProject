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
        public event Action<StageSO> OnClearStage; //스테이지 클리어
        public event Action<StageSO> OnFailStage; //스테이지 실패
        public event Action<Character> OnDeadPlayer; //플레이어 사망
        public event Action<float, float> OnHpChange; //HP변경
        public event Action<Skill> OnSkillUsed; //스킬 사용
        public event Action<Skill> OnSkillCanUse; //스킬 사용 가능
        public event Action OnCastingStart; //스킬 캐스팅 시작
        public event Action OnCastingEnd; //스킬 캐스팅 완료
        public event Action OnStatusUpgrade; //스탯 강화 완료
        public event Action OnButtonClicked; //버튼 클릭
        public event Action OnMonsterHit; //몬스터 피격
        public event Action<int> OnGoldChange;
        public event Action<int> OnStatStoneChange;
        public event Action<int> OnExpChange;
        public int GetOrder() => 0;
    }
}