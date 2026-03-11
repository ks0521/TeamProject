using Cysharp.Threading.Tasks;
using Growth.Skill;
using System;
using System.Collections.Generic;
using UnityEngine;
using Battle;
namespace Personal.HagYun
{
    public class EquipSkillEvent
    {
        public event Action OnCooltimeStart;
        public event Action OnCooltimeEnd;
        public event Action<float, float> OnCooltimeUpdate;

        public void RaiseCooltimeStart() => OnCooltimeStart?.Invoke();
        public void RaiseCooltimeEnd() => OnCooltimeEnd?.Invoke();
        public void RaiseCooltimeUpdate(float curCooltime, float maxCooltime) => OnCooltimeUpdate?.Invoke(curCooltime, maxCooltime);
    }
    [Serializable]
    public class EquipSkill
    {
        public Skill skill;
        public float CurCooltime { get; private set; }
        public bool IsCooltime { get; private set; }
        public Priority priority;
        EquipSkillEvent eventSet = new EquipSkillEvent();
        static Player owner;
        public static void OwnerSet(Player pl) => owner = pl;
        public void SkillSet(Skill skill) => this.skill = skill;
        public void SkillUnset() => skill = null;
        public void SkillChange(Skill skill)
        {
            SkillUnset();
            SkillSet(skill);
            CooltimeStart();
        }
        async UniTaskVoid CooltimeStartTask()
        {
            IsCooltime = true;
            eventSet.RaiseCooltimeStart();
            float baseCooltime = skill.data.coolDown;
            CurCooltime = baseCooltime;
            while (0 < CurCooltime)
            {
                CurCooltime -= Time.deltaTime; // 쿨타임 감소 속도 증가 시, 해당 값 곱하기
                eventSet.RaiseCooltimeUpdate(CurCooltime, baseCooltime);
                await UniTask.Yield();
                if (owner == null) return;
            }
            IsCooltime = false;
            eventSet.RaiseCooltimeEnd();
        }
        public void CooltimeStart(float addTime = 0)
        {
            if (addTime == 0)
            {
                CurCooltime = skill.data.coolDown;
            }
            else
            {
                CurCooltime += addTime;
            }
            if (!IsCooltime) CooltimeStartTask().Forget();
        }
        // event add/remove
        public void AddEventCooltimeStart(Action func) => eventSet.OnCooltimeStart += func;
        public void AddEventCooltimeEnd(Action func) => eventSet.OnCooltimeEnd += func;
        public void AddEventCooltimeUpdate(Action<float, float> func) => eventSet.OnCooltimeUpdate += func;

        public void RemoveEventCooltimeStart(Action func) => eventSet.OnCooltimeStart -= func;
        public void RemoveEventCooltimeEnd(Action func) => eventSet.OnCooltimeEnd -= func;
        public void RemoveEventCooltimeUpdate(Action<float, float> func) => eventSet.OnCooltimeUpdate -= func;
    }
}