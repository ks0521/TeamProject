using Battle;
using Growth.Skill;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Personal.HagYun
{
    public class ActiveSkill : Skill
    {
        // skill data
        [SerializeField] private ActiveSkillSO skillData;
        public override SkillSO SkillData => skillData;
        public ActiveSkillSO ActiveSkillData => skillData;
        [SerializeField] private LayerMask targetLayer;
        public LayerMask TargetLayer { get; private set; }
        protected float resultDamage;
        public float ResultDamage => resultDamage;
        // public bool IsHomingSkill => skillData.Targeting == TargetingMode.Homing;
        public ActiveSkillObject skillObj;
        public ActiveSkill(Character cha, ActiveSkillSO skillData)
        {
            this.skillData = skillData;
            targetLayer = cha.TargetLayer;
            Init(cha);
        }
        public override void StatUpdate()
        {
            resultDamage = skillData.ResultDamage(curLv);
        }
        // public void SkillUseTargeting(TargetChecker target)
        // {
        //     skillObj.SkillUseTargeting(target);
        // }
    }
}