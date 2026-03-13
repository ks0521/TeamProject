using Battle;
using Cysharp.Threading.Tasks;
using Growth.Skill;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
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
        private EquipSkill[] equipSkillArr = new EquipSkill[6];
        private EquipSkillControllerEvent eventSet;
        int skillEquipMask;
        [Range(0f, 1f)] private float skillFireTimeValue = 0.5f;
        public bool IsCasting { get; private set; }
        //private Player owner;
        void Start()
        {
            SkillEquipCheck();
        }
        void SkillEquipCheck()
        {
            for(int i = 0; i < 6; i++)
            {
                skillEquipMask |= 1 << i;
            }
        }
        private void Update()
        {
            SkillInput();
        }
        public void SkillInput()
        {
            if(Input.GetKeyDown(KeyCode.Alpha1))
            {
                SkillUse(0);
            }
            else if(Input.GetKeyDown(KeyCode.Alpha2))
            {
                SkillUse(1);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                SkillUse(2);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                SkillUse(3);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                SkillUse(4);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                SkillUse(5);
            }
        }

        public void OwnerSet(Player pl)
        {
            Skill.SetPlOwner(pl);
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
            float baseCastingTime = equipSkillArr[index].skill.Data.castingTime;
            float curCastingTime = baseCastingTime;
            while (0 < curCastingTime)
            {
                float castingTimeValue = curCastingTime / baseCastingTime;
                if (castingTimeValue <= skillFireTimeValue)
                {
                    //equipSkillArr[index].skill.
                }
                curCastingTime -= Time.deltaTime; // * owner의 캐스팅 시간 감소
                await UniTask.Yield(Skill.PlOwner.GetCancellationTokenOnDestroy());
                if (Skill.PlOwner == null) return;
            }
            eventSet.RaiseOnCastingEnd();
            IsCasting = false;
        }
        bool CheckSkillUsePossible(int index)
        {
            if ((skillEquipMask & (1 << index)) == 0)
            {
                Debug.LogWarning($"{index + 1}번 자리에 장착된 스킬 없음");
                return false;
            }
            else if (IsCasting)
            {
                Debug.LogWarning("캐스팅중");
                return false;
            }
            else return true;

        }
        public void SkillUse(int index)
        {
            if (!CheckSkillUsePossible(index)) return;
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