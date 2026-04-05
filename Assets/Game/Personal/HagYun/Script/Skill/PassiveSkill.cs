using Battle;
using Growth.Equipment;
using Growth.Skill;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace Personal.HagYun
{
    public class PassiveSkill : Skill
    {
        [SerializeField] protected PassiveSkillSO skillData;
        public override SkillSO SkillData => skillData;
        public PassiveSkillSO PassiveSkillData => skillData;
        private StatIncrease resultSkillData;
        public StatIncrease ResultSkillData => resultSkillData;
        public override void StatUpdate()
        {
            resultSkillData = skillData.ResultAddStat(curLv);
        }
    }
}