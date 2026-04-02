using Battle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Personal.HagYun
{
    public class AreaSkillByProjectile : ProjectileSkill
    {
        // area check
        [SerializeField] CapsuleCollider2D effectAreaCollider;
        public override void Init(Character owner)
        {
            base.Init(owner);
            ProjectileAndAreaInit();
        }
        protected void ProjectileAndAreaInit()
        {
            float areaSize = data.effectArea;
            Transform effectTransform = effectAnim.transform;
            effectTransform.localScale *= areaSize;
            effectTransform.position *= areaSize;
        }
        public override void SkillEffect()
        {
            DisableProjectile();
            EnableEffect();
            SkillAtk(target);
            if (data.effectArea == 0)
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