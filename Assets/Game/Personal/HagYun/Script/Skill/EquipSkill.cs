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

        public void RaiseCooltimeStart() => OnCooltimeStart?.Invoke();
        public void RaiseCooltimeEnd() => OnCooltimeEnd?.Invoke();

        // public event Action<float> OnCooltimeUpdate;
        // public void RaiseCooltimeUpdate(float value) => OnCooltimeUpdate?.Invoke(value);
    }
    [Serializable]
    public class EquipSkill
    {
        //Owner
        Character owner;

        [SerializeField] Skill skill;
        public Skill Skill => skill;
        public float CurCooltime { get; private set; }
        public float MaxCooltime { get; private set; }
        public bool IsCooltime { get; private set; }
        EquipSkillEvent eventSet = new EquipSkillEvent();
        public Vector2 SkillOwnerPos => skill.OwnerPos;
        public void Init(Character owner)
        {
            this.owner = owner;
        }
        public void SkillSet(Skill skill, bool isInit = false)
        {
            this.skill = skill;
            skill.Init(owner);
            MaxCooltime = skill.Data.coolDown;
            if (!isInit) CooltimeStart();
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
        }
        public void SkillUse(Character target)
        {
            if (skill == null)
            {
                Debug.LogWarning("스킬 없음");
                return;
            }
            else if (IsCooltime)
            {
                //Debug.Log($"{skill.name} 스킬 쿨타임");
                return;
            }
            else if (target == null)
            {
                Debug.LogWarning("타겟 없음");
                return;
            }
            switch (skill.Data.Targeting)
            {
                case TargetingMode.Self:
                    skill.SkillUseTargeting(new TargetChecker(skill.OwnerPos));
                    break;
                case TargetingMode.Homing:
                    skill.SkillUseTargeting(new TargetChecker(target));
                    break;
                case TargetingMode.GroundTarget:
                    skill.SkillUseTargeting(new TargetChecker(target.transform.position));
                    break;
            }
            CooltimeStart();
        }
        async UniTaskVoid CooltimeStartTask()
        {
            IsCooltime = true;
            eventSet.RaiseCooltimeStart();
            // float baseCooltime = skill.Data.coolDown;
            CurCooltime = MaxCooltime;
            while (0 < CurCooltime)
            {
                CurCooltime -= Time.deltaTime; // 쿨타임 감소 속도 증가 시, 해당 값 곱하기
                // eventSet.RaiseCooltimeUpdate(1 - (CurCooltime / baseCooltime));
                await UniTask.Yield(owner.GetCancellationTokenOnDestroy());
                // if (Skill.PlOwner == null) return;
                if (owner == null) return;
            }
            // eventSet.RaiseCooltimeUpdate(1);
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
            if (!IsCooltime) CooltimeStartTask().Forget();
        }
        public void CooltimeStart()
        {
            CooltimeSet(skill.Data.coolDown);
            CooltimeStartTask().Forget();
        }

        // event add/remove
        public void AddEventCooltimeStart(Action func) => eventSet.OnCooltimeStart += func;
        public void AddEventCooltimeEnd(Action func) => eventSet.OnCooltimeEnd += func;

        public void RemoveEventCooltimeStart(Action func) => eventSet.OnCooltimeStart -= func;
        public void RemoveEventCooltimeEnd(Action func) => eventSet.OnCooltimeEnd -= func;

        // public void AddEventCooltimeUpdate(Action<float> func) => eventSet.OnCooltimeUpdate += func;
        // public void RemoveEventCooltimeUpdate(Action<float> func) => eventSet.OnCooltimeUpdate -= func;
    }
}