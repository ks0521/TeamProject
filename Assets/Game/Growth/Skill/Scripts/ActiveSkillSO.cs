using Personal.HagYun;
using UnityEngine;

namespace Growth.Skill
{
    [CreateAssetMenu(menuName = "Game/Growth/ActiveSkill")]
    public class ActiveSkillSO : SkillSO
    {
        public override SkillType Type => SkillType.Active; //스킬타입
        [Header("Active 전용")]
        public ScopeOfEffect SoE; //적용 범위
        public TargetingMode Targeting; //스킬 시전 위치 기준
        public float baseDamage; //기본 스킬 효과 배율
        public float incDamagePerLevel; //레벨당 스킬 효과 증가율
        public float ResultDamage(int curLv)
        {
            if (curLv < 0 || maxLv < curLv || curLv == 0)
            {
                return baseDamage;
            }
            float resultDamage = baseDamage + (incDamagePerLevel * curLv);
            // Debug.Log($"{baseDamage}, {incDamagePerLevel * curLv}, {resultDamage}");
            return resultDamage;
        }
        public float castingTime; //스킬 시전 시간
        public int range; //스킬 사거리
        public float coolDown; //스킬 쿨다운
        [Header("투사체 전용")] 
        public float speed; //투사체 속도
        [Header("범위 전용")] 
        public float effectArea; //효과 범위
        [Header("사운드")]
        public AudioClip skillSound;
        [Header("Skill Object")]
        public ActiveSkillObject skillObj;
    }
}