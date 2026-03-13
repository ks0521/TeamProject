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
        public void SkillSet(Skill skill)
        {
            this.skill = skill;
        }
        public void SkillUnset()
        {
            skill = null;
            IsCooltime = false;
        }
        public void SkillChange(Skill skill)
        {
            SkillUnset();
            SkillSet(skill);
            CooltimeStart();
        }
        public void SkillUse(Vector2 targetPos)
        {
            if(skill == null)
            {
                Debug.LogWarning("스킬 없음");
                return;
            }
            switch (skill.Data.Targeting)
            {
                case TargetingMode.Self:
                    break;
                case TargetingMode.Homing:
                    break;
                case TargetingMode.GroundTarget:
                    break;
                default:
                    break;
            }
            CooltimeStart();
        }
        async UniTaskVoid CooltimeStartTask()
        {
            IsCooltime = true;
            eventSet.RaiseCooltimeStart();
            float baseCooltime = skill.Data.coolDown;
            CurCooltime = baseCooltime;
            while (0 < CurCooltime)
            {
                CurCooltime -= Time.deltaTime; // 쿨타임 감소 속도 증가 시, 해당 값 곱하기
                eventSet.RaiseCooltimeUpdate(CurCooltime, baseCooltime);
                await UniTask.Yield();
                if (Skill.PlOwner == null) return;
            }
            IsCooltime = false;
            eventSet.RaiseCooltimeEnd();
        }
        public void CooltimeSet(float cooltime)
        {
            CurCooltime = cooltime;
        }
        public void ColltimeAdd(float cooltime)
        {
            CurCooltime += cooltime;
            if(!IsCooltime) CooltimeStartTask().Forget();
        }
        public void CooltimeStart()
        {
            CooltimeSet(skill.Data.coolDown);
            CooltimeStartTask().Forget();
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