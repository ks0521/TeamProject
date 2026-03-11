using Battle;
using Cysharp.Threading.Tasks;
using Growth.Skill;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace Personal.HagYun
{
    public enum ESCEventType
    {
        CooltimeStart,
        CooltimeUpdate,
        CooltimeEnd,
        CastingStart,
        CastingEnd
    }
    public class EquipSkillControllerEvent
    {
        public event Action OnCastingStart;
        public event Action OnCastingEnd;
        public void RaiseCastingStart() => OnCastingStart?.Invoke();
        public void RaiseOnCastingEnd() => OnCastingEnd?.Invoke();
    }
    public class EquipSkillController : MonoBehaviour
    {
        EquipSkill[] equipSkillArr = new EquipSkill[6];
        EquipSkillControllerEvent eventSet;
        int skillEquipMask;
        Player owner;
        [Range(0f, 1f)] float skillFireTimeValue = 0.5f;
        public bool IsCasting { get; private set; }
        void Start()
        {

        }


        public void OwnerSet(Player pl)
        {
            owner = pl;
            EquipSkill.OwnerSet(pl);
        }
        public void SkillEquip(int index, Skill targetSkill)
        {
            equipSkillArr[index].SkillSet(targetSkill);
            equipSkillArr[index].priority = Priority.Mid;
            skillEquipMask |= (1 << index);
        }
        public void SkillUnequip(int index)
        {
            equipSkillArr[index].SkillUnset();
            skillEquipMask &= ~(1 << index);
        }

        async UniTaskVoid CastingStartTask(int index)
        {
            eventSet.RaiseCastingStart();
            IsCasting = true;
            float baseCastingTime = equipSkillArr[index].skill.data.castingTime;
            float curCastingTime = baseCastingTime;
            while (0 < curCastingTime)
            {
                float castingTimeValue = curCastingTime / baseCastingTime;
                if (castingTimeValue <= skillFireTimeValue)
                {
                    // 스킬 시전
                }
                //Debug.Log("쿨타임 감소");
                curCastingTime -= Time.deltaTime; // * owner의 캐스팅 시간 감소
                await UniTask.Yield();
                if (owner == null) return;
            }
            eventSet.RaiseOnCastingEnd();
            IsCasting = false;
        }
        public void SkillUse(int index)
        {
            if (IsCasting) return;
            CastingStartTask(index).Forget();
            equipSkillArr[index].CooltimeStart();
        }
        // Func for event subscription
        public void SkillEquip1(Skill skill) => SkillEquip(0, skill);
        public void SkillEquip2(Skill skill) => SkillEquip(1, skill);
        public void SkillEquip3(Skill skill) => SkillEquip(2, skill);
        public void SkillEquip4(Skill skill) => SkillEquip(3, skill);
        public void SkillEquip5(Skill skill) => SkillEquip(4, skill);
        public void SkillEquip6(Skill skill) => SkillEquip(5, skill);
        public void SkillUnequip1() => SkillUnequip(0);
        public void SkillUnequip2() => SkillUnequip(1);
        public void SkillUnequip3() => SkillUnequip(2);
        public void SkillUnequip4() => SkillUnequip(3);
        public void SkillUnequip5() => SkillUnequip(4);
        public void SkillUnequip6() => SkillUnequip(5);
        public void SkillUse1() => SkillUse(0);
        public void SkillUse2() => SkillUse(1);
        public void SkillUse3() => SkillUse(2);
        public void SkillUse4() => SkillUse(3);
        public void SkillUse5() => SkillUse(4);
        public void SkillUse6() => SkillUse(5);

        // event add/remove
        public void AddEventCastingStart(Action func) => eventSet.OnCastingStart += func;
        public void AddEventCastingEnd(Action func) => eventSet.OnCastingEnd += func;
        public void RemoveEventCastingStart(Action func) => eventSet.OnCastingStart -= func;
        public void RemoveEventCastingEnd(Action func) => eventSet.OnCastingEnd -= func;
    }
}