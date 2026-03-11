using UnityEngine;

namespace Growth.Skill
{
    /// <summary> 스킬 우선순위 </summary>
    public enum Priority
    {
        Low,
        Mid,
        High
    }

    /// <summary> 효과 적용 범위 </summary>
    public enum ScopeOfEffect
    {
        Single, //단일 공격
        Area //범위 공격
    }

    /// <summary> 공격이 어디서 시작할지 결정</summary>
    public enum TargetingMode
    {
        Self, //플레이어 위치
        Homing, //특정 대상 추적
        GroundTarget //특정 지점에서 공격
    }

    /// <summary> 스킬의 타입(MVP에서는 액티브만)</summary>
    public enum Type
    {
        Passive,
        Active
    }

    [CreateAssetMenu(menuName = "Game/Growth/Skill")]
    public class SkillSO : ScriptableObject
    {
        public int key; //고유 키
        public string skillName; //스킬 이름
        public Type type; //스킬타입
        public Priority priority; //스킬 우선순위
        public ScopeOfEffect SoE; //적용 범위
        public TargetingMode Targeting; //스킬 시전 위치 기준
        public float castingTime; //스킬 시전 시간
        public int range; //스킬 사거리
        public float baseDamage; //기본 데미지 배율
        public float incDamagePerLevel; //레벨당 데미지 증가율
        public float coolDown; //스킬 쿨다운
        [Header("투사체 전용")] 
        public float speed; //투사체 속도
        [Header("범위 전용")] 
        public float effectArea; //효과 범위
    }
}