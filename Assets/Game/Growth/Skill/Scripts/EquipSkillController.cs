using Base.Data;
using Battle;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
namespace Growth.Skill
{
    public struct SpriteShower
    {
        Vector2 spriteSize;
        float radius;
        public SpriteShower(Vector2 spriteSize, float radius)
        {
            this.spriteSize = spriteSize;
            this.radius = radius;
        }
        public void FitSpriteToSize(SpriteRenderer sr)
        {
            if (sr == null || sr.sprite == null) return;

            float spriteWidth = MathF.Max(0.1f, spriteSize.x);
            radius *= 2f;
            if (spriteWidth == 0) spriteWidth = 1;
            float scale = radius / spriteWidth;

            sr.transform.localScale = new Vector3(scale, scale, 1f);
        }
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
        public int SkillCnt => skillCnt;
        // skill event
        protected EventHub eventHub;

        [Range(0f, 1f)] protected float skillFireTimeValue = 0.5f;
        [field: SerializeField] public bool IsCasting { get; private set; }
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
            equipSkillArr[index].priority = pri;
        }
        [SerializeField] Vector2 testAreaOffset;
        protected void SkillRangeChange(float range)
        {
            if (sr == null) return;
            SpriteShower tss = new SpriteShower(testAreaOffset, range);
            tss.FitSpriteToSize(sr);
        }
        public virtual void SkillEquip(int slotIndex, ActiveSkill targetSkill, bool isInit = false)
        {
            if (targetSkill == null)
            {
                Debug.Log($"{slotIndex}번에 장착할 스킬 없음");
                return;
            }
            int targetSkillEquipNum = targetSkill.EquipSlotIndex;
            if (slotIndex == targetSkillEquipNum)
            {
                Debug.Log($"{slotIndex}번에 이미 같은 스킬 장착됨");
                return;
            }
            else if (targetSkillEquipNum != -1)
            {
                SkillUnequip(targetSkillEquipNum);
            }

            EquipSkill eSkill = equipSkillArr[slotIndex];
            eSkill.SkillEquip(targetSkill, isInit);
            eSkill.Skill.EquipSkillSlotIndexUpdate(slotIndex);
            PriorityUpdate(slotIndex, Priority.Low);
            eventHub.SkillSet(slotIndex);
            if (eSkill.isEquipped) return;
            eSkill.isEquipped = true;
            skillCnt++;

        }
        public virtual void SkillUnequip(int index)
        {
            EquipSkill eSkill = equipSkillArr[index];
            if (!eSkill.isEquipped) return;
            eSkill.Skill.EquipSkillSlotIndexUpdate(-1);
            eSkill.SkillUnequip();
            eventHub.SkillUnset(index);
            eSkill.isEquipped = false;
            skillCnt--;

        }
        async UniTaskVoid CastingStartTask(int index, Character cha)
        {
            IsCasting = true;
            // eventSet.RaiseCastingStart();
            eventHub.CastingStarted();
            float alphaValue = 100f / 255f;
            if (sr != null) sr.color = new Color(0, 0, 1f, alphaValue);

            float baseCastingTime = equipSkillArr[index].Skill.ActiveSkillData.castingTime;
            float curCastingTime = baseCastingTime;
            float castingTimeValue = 1f;

            while (skillFireTimeValue < castingTimeValue)
            {
                castingTimeValue = curCastingTime / baseCastingTime;
                curCastingTime -= Time.deltaTime; // * owner의 캐스팅 시간 감소 속도
                await UniTask.Yield(this.GetCancellationTokenOnDestroy());
                if (this == null) return;
            }

            if (sr != null) sr.color = new Color(0, 1f, 0, alphaValue);
            equipSkillArr[index].SkillUse(cha);

            while (0 < castingTimeValue)
            {
                castingTimeValue = curCastingTime / baseCastingTime;
                curCastingTime -= Time.deltaTime; // * owner의 캐스팅 시간 감소 속도
                await UniTask.Yield(this.GetCancellationTokenOnDestroy());
                if (this == null) return;
            }

            if (sr != null) sr.color = new Color(1f, 0, 0, alphaValue);

            IsCasting = false;
            // eventSet.RaiseCastingEnd();
            eventHub.CastingEnd();
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
            ActiveSkill tSkill = equipSkillArr[skillIndex].Skill;
            Vector2 plPos = tSkill.OwnerPos;
            int getNearMonCnt = OverlapChecker.GetCircleTargetsCount(plPos, tSkill.ActiveSkillData.range, tSkill.TargetMask);
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
                SkillRangeChange(equipSkillArr[index].Skill.ActiveSkillData.range);
                AtkSkillUse(index, mon);
                return true;
            }
            return false;
        }

        // event subscription Func for external use  
        public void SkillEquip1(ActiveSkill skill) => SkillEquip(0, skill);
        public void SkillEquip2(ActiveSkill skill) => SkillEquip(1, skill);
        public void SkillEquip3(ActiveSkill skill) => SkillEquip(2, skill);
        public void SkillEquip4(ActiveSkill skill) => SkillEquip(3, skill);
        public void SkillEquip5(ActiveSkill skill) => SkillEquip(4, skill);
        public void SkillEquip6(ActiveSkill skill) => SkillEquip(5, skill);
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
    }
}