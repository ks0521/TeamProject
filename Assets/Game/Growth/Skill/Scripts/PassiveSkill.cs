using Battle;
using Growth.Equipment;
using UnityEngine;

namespace Growth.Skill
{
    public class PassiveSkill : Skill
    {
        [SerializeField] protected PassiveSkillSO skillData;
        public override SkillSO SkillData => skillData;
        public PassiveSkillSO PassiveSkillData => skillData;
        private StatIncrease resultSkillData;
        public StatIncrease ResultSkillData => resultSkillData;
        public PassiveSkill(Character cha, PassiveSkillSO skillData)
        {
            this.skillData = skillData;
            Init(cha);
        }
        public override void StatUpdate()
        {
            resultSkillData = skillData.ResultAddStat(curLv);
        }
    }
}