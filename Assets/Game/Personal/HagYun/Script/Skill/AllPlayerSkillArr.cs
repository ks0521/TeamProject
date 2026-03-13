using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Personal.HagYun
{
    public class AllPlayerSkillArr : MonoBehaviour
    {
        [SerializeField] Skill[] skillArr;
        int getSkillMask;
        private void Start()
        {
            for (int i = 0; i < skillArr.Length; i++)
            {
                if(skillArr[i] != null) // 해당 스킬 얻었는지 여부 체크 필요
                {
                    getSkillMask |= 1 << i;
                }
            }
        }
        public bool TryGetSkill(int index, out Skill getSkill)
        {
            if((getSkillMask & 1 << index) == 0)
            {
                getSkill = null;
                return false;
            }
            getSkill = skillArr[index];
            return true;
        }
    }
}