using Growth.Equipment;
using UnityEngine;
using Base.Utils;

namespace Growth.Skill
{
    [CreateAssetMenu(menuName = "Game/Growth/PassiveSkill")]
    public class PassiveSkillSO : SkillSO
    {
        public override SkillType Type => SkillType.Passive;
        public StatIncrease baseAddStat;
        public StatIncrease lvPerIncreaseAddStat;
        public StatIncrease ResultAddStat(int curLv)
        {
            if (curLv < 0 || maxLv < curLv || curLv == 0)
            {
                return baseAddStat;
            }
            StatIncrease curLvIncreaseStat = StructMemberCalculator<StatIncrease>.Mul(lvPerIncreaseAddStat, curLv);
            return StructMemberCalculator<StatIncrease>.Add(baseAddStat, curLvIncreaseStat);
        }
    }
}