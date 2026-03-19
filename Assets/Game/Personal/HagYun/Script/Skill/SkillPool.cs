using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Personal.HagYun
{
    public class SkillPool : MonoBehaviour
    {
        [SerializeField] Skill[] skillArr;
        public bool TryGetSkill(int index, out Skill getSkill)
        {
            if(index < 0 || skillArr.Length <= index)
            {
                getSkill = null;
                return false;
            }
            getSkill = skillArr[index];
            return true;
        }
    }
}