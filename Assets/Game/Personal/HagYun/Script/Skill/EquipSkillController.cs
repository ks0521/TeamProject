using Base.Data;
using Battle;
using Cysharp.Threading.Tasks;
using Growth.Skill;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
namespace Personal.HagYun
{
    public class EquipSkillControllerEvent
    {
        public event Action OnCastingStart;
        public event Action OnCastingEnd;
        public void RaiseCastingStart() => OnCastingStart?.Invoke();
        public void RaiseCastingEnd() => OnCastingEnd?.Invoke();
    }
    public abstract class EquipSkillController : MonoBehaviour
    {
        // test
        public SpriteRenderer sr;
        
        // owner
        protected Character owner;

        // skill pool for get skill
        [SerializeField] protected SkillPool skillPool;
        [SerializeField] protected EquipSkill[] equipSkillArr;
        public EquipSkill[] EquipSkillArr => equipSkillArr;
        public EquipSkill this[int index] => equipSkillArr[index];
        protected int skillCnt;
        // skill event
        protected EquipSkillControllerEvent eventSet = new EquipSkillControllerEvent();
        protected EventHub eventHub;
        // skill ready
        public bool IsSkillReady { get; private set; }

        [Range(0f, 1f)] protected float skillFireTimeValue = 0.5f;
        [field : SerializeField] public bool IsCasting { get; private set; }
        public void OwnerSet(Character owner) => this.owner = owner;
        public abstract void Init(Character cha);
        private void Update()
        {
            UpdateFeat();
        }
        protected virtual void UpdateFeat() { }
        public void SkillReady(int index)
        {
            
        }
        public virtual void PriorityUpdate(int index, Priority pri)
        {
            // equipSkillSetArr[index].priority = pri;
            equipSkillArr[index].priority = pri;
        }
        public virtual void SkillEquip(int index, Skill targetSkill, bool isInit = false)
        {
            if(targetSkill == null)
            {
                Debug.Log($"{index}번에 장착할 스킬 없음");
                return;
            }
            PriorityUpdate(index, Priority.Low);
            equipSkillArr[index].SkillSet(targetSkill, isInit);
            if (equipSkillArr[index].isEquipped) return;
            equipSkillArr[index].isEquipped = true;
            skillCnt++;
        }
        public virtual void SkillUnequip(int index)
        {
            equipSkillArr[index].SkillUnset();
            if (!equipSkillArr[index].isEquipped) return;
            equipSkillArr[index].isEquipped = false;
            skillCnt--;
        }
        async UniTaskVoid CastingStartTask(int index, Character cha)
        {
            eventSet.RaiseCastingStart();
            IsCasting = true;

            sr.color = Color.blue;

            // float baseCastingTime = equipSkillSetArr[index].Skill.Data.castingTime;
            float baseCastingTime = equipSkillArr[index].Skill.Data.castingTime;
            float curCastingTime = baseCastingTime;
            float castingTimeValue = 1f;

            while (skillFireTimeValue < castingTimeValue)
            {
                castingTimeValue = curCastingTime / baseCastingTime;
                curCastingTime -= Time.deltaTime; // * owner의 캐스팅 시간 감소 속도
                await UniTask.Yield(this.GetCancellationTokenOnDestroy());
                if (this == null) return;
            }

            sr.color = Color.yellow;
            // equipSkillSetArr[index].ESkill.SkillUse(cha);
            equipSkillArr[index].SkillUse(cha);

            while (0 < castingTimeValue)
            {
                castingTimeValue = curCastingTime / baseCastingTime;
                curCastingTime -= Time.deltaTime; // * owner의 캐스팅 시간 감소 속도
                await UniTask.Yield(this.GetCancellationTokenOnDestroy());
                if (this == null) return;
            }

            sr.color = Color.white;

            eventSet.RaiseCastingEnd();
            IsCasting = false;
        }
        bool CheckSkillUsePossible(int index)
        {
            if (!equipSkillArr[index].IsSkillUsePossible)
            {
                Debug.LogWarning($"{index}번 자리에 장착된 스킬 없음 or 쿨타임");
                return false;
            }
            else if (IsCasting)
            {
                Debug.LogWarning("캐스팅중");
                return false;
            }
            else return true;

        }
        public bool TryGetMonsterTargetToAtk(int skillIndex, out Monster mon)
        {
            Skill tSkill = equipSkillArr[skillIndex].Skill;
            Vector2 plPos = tSkill.OwnerPos;
            int getNearMonCnt = OverlapChecker.GetCircleTargetsCount(plPos, tSkill.Data.range, tSkill.TargetMask);
            if (OverlapChecker.TryGetNearTarget(plPos, getNearMonCnt, out Collider2D targetCol))
            {
                mon = targetCol.GetComponent<Monster>();
                return mon != null;
            }
            mon = null;
            return false;
        }
        public void AtkSkillUse(int index, Monster mon)
        {
            CastingStartTask(index, mon).Forget();
        }
        public bool TryAtkSkillUseToMonster(int index)
        {
            if (!CheckSkillUsePossible(index)) return false;
            else if (TryGetMonsterTargetToAtk(index, out Monster mon))
            {
                AtkSkillUse(index, mon);
                return true;
            }
            return false;
        }

        // event subscription Func for external use  
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
        public void SkillUse1() => TryAtkSkillUseToMonster(0);
        public void SkillUse2() => TryAtkSkillUseToMonster(1);
        public void SkillUse3() => TryAtkSkillUseToMonster(2);
        public void SkillUse4() => TryAtkSkillUseToMonster(3);
        public void SkillUse5() => TryAtkSkillUseToMonster(4);
        public void SkillUse6() => TryAtkSkillUseToMonster(5);

        // event add/remove for external use
        public void AddEventCastingStart(Action func) => eventSet.OnCastingStart += func;
        public void AddEventCastingEnd(Action func) => eventSet.OnCastingEnd += func;
        public void RemoveEventCastingStart(Action func) => eventSet.OnCastingStart -= func;
        public void RemoveEventCastingEnd(Action func) => eventSet.OnCastingEnd -= func;
    }
}