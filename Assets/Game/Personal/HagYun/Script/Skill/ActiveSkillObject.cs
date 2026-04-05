using Battle;
using Growth.Skill;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Personal.HagYun
{
    public struct SkillParams
    {
        public Character target; 
        public LayerMask lm;
    }
    public abstract class ActiveSkillObject : MonoBehaviour
    {
        [SerializeField] protected Character owner;
        [SerializeField] protected int executorKey;

        SkillParams sParams;
        public void Init(Character owner, int executorKey, SkillParams sParams)
        {
            this.owner = owner;
            this.executorKey = executorKey;
            this.sParams = sParams;
        }
        public abstract void SkillOn();
        public virtual void SkillOn(SkillParams sParams)
        {
            this.sParams = sParams;
            SkillOn();
        }
    }
}