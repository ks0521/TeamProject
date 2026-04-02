using Growth.Equipment;
using Growth.Skill;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            return new StatIncrease() {
                flatAttack = baseAddStat.flatAttack + (lvPerIncreaseAddStat.flatAttack * curLv),
                attackRate = baseAddStat.attackRate + (lvPerIncreaseAddStat.attackRate * curLv),
                flatMaxHp = baseAddStat.flatMaxHp + (lvPerIncreaseAddStat.flatMaxHp * curLv),
                maxHpRate = baseAddStat.maxHpRate + (lvPerIncreaseAddStat.maxHpRate * curLv),
                damageReductionRate = baseAddStat.damageReductionRate + (lvPerIncreaseAddStat.damageReductionRate * curLv),
                itemDropRateBonus = baseAddStat.itemDropRateBonus + (lvPerIncreaseAddStat.itemDropRateBonus * curLv),
                goldGainRate = baseAddStat.goldGainRate + (lvPerIncreaseAddStat.goldGainRate * curLv),
                expGainRate = baseAddStat.expGainRate + (lvPerIncreaseAddStat.expGainRate * curLv),
                statStoneGainRate = baseAddStat.statStoneGainRate + (lvPerIncreaseAddStat.statStoneGainRate * curLv),
                moveSpeedRate = baseAddStat.moveSpeedRate + (lvPerIncreaseAddStat.moveSpeedRate * curLv),
                attackSpeedRate = baseAddStat.attackSpeedRate + (lvPerIncreaseAddStat.attackSpeedRate * curLv),
            };
        }
    }
}