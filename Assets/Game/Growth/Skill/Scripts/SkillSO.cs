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
        [Header("Passive/Active 공용")]
        public int key; //고유 키
        public string skillName; //스킬 이름
        public virtual Type SkillType => Type.Passive; //스킬타입
        public float baseValue; //기본 스킬 효과 배율
        public float incValuePerLevel; //레벨당 스킬 효과 증가율
        public string description;
        [Header("스킬 아이콘")] 
        public Sprite skillIcon;
        [Header("사운드")] 
        public AudioClip skillSound;
    }
}