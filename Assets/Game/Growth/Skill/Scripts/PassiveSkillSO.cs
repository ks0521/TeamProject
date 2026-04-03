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
                atk = baseAddStat.atk + (lvPerIncreaseAddStat.atk * curLv),
                atkRate = baseAddStat.atkRate + (lvPerIncreaseAddStat.atkRate * curLv),
                maxHp = baseAddStat.maxHp + (lvPerIncreaseAddStat.maxHp * curLv),
                maxHpRate = baseAddStat.maxHpRate + (lvPerIncreaseAddStat.maxHpRate * curLv),
                damageReduction = baseAddStat.damageReduction + (lvPerIncreaseAddStat.damageReduction * curLv),
                itemDropRate = baseAddStat.itemDropRate + (lvPerIncreaseAddStat.itemDropRate * curLv),
                goldGain = baseAddStat.goldGain + (lvPerIncreaseAddStat.goldGain * curLv),
                expGain = baseAddStat.expGain + (lvPerIncreaseAddStat.expGain * curLv),
                statStoneGain = baseAddStat.statStoneGain + (lvPerIncreaseAddStat.statStoneGain * curLv),
                moveSpeed = baseAddStat.moveSpeed + (lvPerIncreaseAddStat.moveSpeed * curLv),
                atkSpeed = baseAddStat.atkSpeed + (lvPerIncreaseAddStat.atkSpeed * curLv),
            };
        }
    }
}