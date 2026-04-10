using UnityEngine;

namespace Growth.Skill
{
    public class AreaSkillByProjectile : ProjectileSkill
    {
        // area check
        [SerializeField] CapsuleCollider2D effectAreaCollider;
        public override void Init(ActiveSkill launcher)
        {
            base.Init(launcher);
            ProjectileAndAreaInit();
        }
        protected void ProjectileAndAreaInit()
        {
            float areaSize = launcher.ActiveSkillData.effectArea;
            Transform effectTransform = effectAnim.transform;
            effectTransform.localScale *= areaSize;
            effectTransform.position *= areaSize;
        }
        public override void SkillEffect()
        {
            DisableProjectile();
            EnableEffect();
            SkillAtk(target);
            if (launcher.ActiveSkillData.effectArea == 0)
            {
                Debug.LogWarning($"{gameObject.name}의 range 값이 0입니다.");
                SkillAtk(target);
            }
            else
            {
                PlSkillCapsuleAreaAtk(TargetPos, effectAreaCollider.size, effectAreaCollider.direction);
            }
        }
    }
}