using Base.Data;
using Battle;
using Cysharp.Threading.Tasks;
using System;
using Base.Utils;
using UnityEngine;
using System.Collections.Generic;
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
    public struct NeedsDataFromEquipSkillController
    {
        public SkillManager skillMgr;
        public Character cha;
        public EventHub hub;
        public NeedsDataFromEquipSkillController(SkillManager skillMgr, Character cha, EventHub hub)
        {
            this.skillMgr = skillMgr;
            this.cha = cha;
            this.hub = hub;
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
        public SkillPool SkPool => skillPool;
        [SerializeField] protected SkillObjectPool skillObjPool;
        public SkillObjectPool SkObjPool => skillObjPool;
        public SkillPool Pool => skillPool;
        [SerializeField] protected EquipSkill[] equipSkillArr;
        // public EquipSkill[] EquipSkillArr => equipSkillArr;
        public IReadOnlyList<EquipSkill> EquipSkillList => equipSkillArr;
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
            if (index < 0 || 6 <= index) return;
            equipSkillArr[index].priority = pri;
        }
        [SerializeField] Vector2 testAreaOffset;
        protected void SkillRangeChange(float range)
        {
            if (sr == null) return;
            SpriteShower tss = new SpriteShower(testAreaOffset, range);
            tss.FitSpriteToSize(sr);
        }
        public void SkillEquip(int slotIndex, int skillKey) => SkillEquipByKey(slotIndex, skillKey);
        public bool IsThisSlotEquipped(int slotIndex, int skillKey)
        {
            if (equipSkillArr[slotIndex].EquippedSkillKey == skillKey)
            {
                Debug.LogWarning($"{slotIndex}번에 이미 같은 스킬 장착됨");
                return true;
            }
            return false;
        }
        public bool IsThisSkillEquippedOtherSlot(ActiveSkill aSkill)
        {
            for (int i = 0; i < 6; i++)
            {
                if (!equipSkillArr[i].isEquipped) continue;
                else if (equipSkillArr[i].Skill == aSkill) return true;
                // else if (equipSkillArr[i].EquippedSkillKey == aSkill.SkillData.key) return true;
            }
            return false;
        }
        public bool IsThisSkillEquippedOtherSlot(ActiveSkill aSkill, out int equippedSlotIndex)
        {
            for (int i = 0; i < 6; i++)
            {
                if (!equipSkillArr[i].isEquipped) continue;
                else if (equipSkillArr[i].Skill == aSkill)
                // else if (equipSkillArr[i].EquippedSkillKey == aSkill.SkillData.key)
                {
                    equippedSlotIndex = i;
                    return true;
                }
            }
            equippedSlotIndex = -1;
            return false;
        }
        public bool IsOtherSlotEquipped(int slotIndex, int skillKey, out int otherEquippedSlotIndex)
        {
            otherEquippedSlotIndex = -1;
            for (int i = 0; i < 6; i++)
            {
                if (slotIndex == i) continue;
                else if (equipSkillArr[i].EquippedSkillKey == skillKey)
                {
                    // Debug.LogWarning($"{skillKey}번 스킬은 {i}번 슬롯에 장착됨");
                    otherEquippedSlotIndex = i;
                    return true;
                }
            }
            // Debug.Log($"{skillKey}번 스킬은 다른 슬롯에 장착되지 않음");
            return false;
        }
        protected virtual void SkillEquipFeat(int slotIndex, ActiveSkill targetSkill, bool isInit = false)
        {
            EquipSkill eSkill = equipSkillArr[slotIndex];
            eSkill.SkillEquip(targetSkill, isInit);
            PriorityUpdate(slotIndex, Priority.Low);
            if (eSkill.isEquipped) return;
            eSkill.isEquipped = true;
            skillCnt++;
        }
        public void SkillEquipByKey(int slotIndex, int skillKey, bool isInit = false)
        {
            if (!skillPool.TryGetActiveSkillByKey(skillKey, out ActiveSkill aSkill))
            {
                // Debug.Log($"{slotIndex}번에 장착할 스킬 없음");
                return;
            }
            else if (IsThisSlotEquipped(slotIndex, skillKey)) return;
            else if (IsOtherSlotEquipped(slotIndex, skillKey, out int otherEquippedSlotIndex))
            {
                // Debug.Log($"{slotIndex}번 슬롯 스킬 장착 해제");
                SkillUnequip(otherEquippedSlotIndex);
            }
            SkillEquipFeat(slotIndex, aSkill, isInit);
        }
        public void SkillEquip(int slotIndex, ActiveSkill targetSkill, bool isInit = false)
        {
            if (targetSkill == null)
            {
                // Debug.Log($"{slotIndex}번에 장착할 스킬 없음");
                return;
            }
            int targetSkillKey = targetSkill.SkillData.key;
            if (IsThisSlotEquipped(slotIndex, targetSkillKey)) return;
            else if (IsOtherSlotEquipped(slotIndex, targetSkillKey, out int otherEquippedSlotIndex))
            {
                // Debug.Log($"{slotIndex}번 슬롯 스킬 장착 해제");
                SkillUnequip(otherEquippedSlotIndex);
            }

            SkillEquipFeat(slotIndex, targetSkill, isInit);

        }
        protected virtual void SkillUnequipFeat(int index, EquipSkill eSkill)
        {
            eSkill.SkillUnequip();
            eSkill.isEquipped = false;
            skillCnt--;
        }
        public void SkillUnequip(int index)
        {
            EquipSkill eSkill = equipSkillArr[index];
            if (!eSkill.isEquipped) return;
            SkillUnequipFeat(index, eSkill);
        }
        async UniTaskVoid CastingStartTask(int index, Character cha)
        {
            Debug.Log("캐스팅 시작");
            IsCasting = true;
            eventHub.CastingStarted();
            float alphaValue = 100f / 255f;
            if (sr != null) sr.color = new Color(0, 0, 1f, alphaValue);
            var ct = this.GetCancellationTokenOnDestroy();

            float baseCastingTime = equipSkillArr[index].Skill.ActiveSkillData.castingTime;
            float curCastingTime = baseCastingTime;
            float castingTimeValue = 1f;

            // while (skillFireTimeValue < castingTimeValue)
            // {
            //     castingTimeValue = curCastingTime / baseCastingTime;
            //     curCastingTime -= Time.deltaTime; // * owner의 캐스팅 시간 감소 속도
            //     await UniTask.Yield(this.GetCancellationTokenOnDestroy());
            //     if (this == null) return;
            // }


            while (0 < castingTimeValue)
            {
                castingTimeValue = curCastingTime / baseCastingTime;
                curCastingTime -= Time.deltaTime;
                await UniTask.DelayFrame(1, PlayerLoopTiming.Update, ct);
            }

            if (sr != null) sr.color = new Color(0, 1f, 0, alphaValue);
            equipSkillArr[index].SkillUse(cha);

            await UniTask.Delay(TimeSpan.FromSeconds(1), DelayType.DeltaTime, PlayerLoopTiming.Update, ct);
            // curCastingTime = 1;
            // while (0 < castingTimeValue)
            // {
            //     castingTimeValue = curCastingTime / baseCastingTime;
            //     curCastingTime -= Time.deltaTime; // * owner의 캐스팅 시간 감소 속도
            //     await UniTask.Yield(this.GetCancellationTokenOnDestroy());
            //     if (this == null) return;
            // }

            if (sr != null) sr.color = new Color(1f, 0, 0, alphaValue);

            IsCasting = false;
            eventHub.CastingEnd();
            Debug.Log("캐스팅 완료");
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
            mon = null;

            Vector2 plPos = owner.transform.position;

            int getNearMonCnt = OverlapChecker.GetCircleTargetsCount(plPos,
            equipSkillArr[skillIndex].Skill.ActiveSkillData.range, owner.TargetLayer);

            // if (OverlapChecker.TryGetNearTarget(plPos, getNearMonCnt, out Collider2D targetCol))
            // {
            //     mon = targetCol.GetComponent<Monster>();
            //     return mon != null;
            // }
            // mon = null;
            // return false;
            if(getNearMonCnt <= 0) return false;

            var monsters = OverlapChecker.GetTargetColArr;
            float curMinDis = float.MaxValue;

            for (int i = 0; i < getNearMonCnt; i++)
            {
                if (!monsters[i].gameObject.activeSelf) continue;

                var tMon = monsters[i].GetComponent<Monster>();
                if (tMon.IsDead) continue;

                Vector2 monPos = tMon.transform.position;
                float tMinDis = (monPos - plPos).sqrMagnitude;
                
                if (tMinDis < curMinDis)
                {
                    curMinDis = tMinDis;
                    mon = tMon;
                }
            }
            return mon != null;
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
                Debug.Log("몬스터 찾음");
                SkillRangeChange(equipSkillArr[index].Skill.ActiveSkillData.range);
                AtkSkillUse(index, mon);
                return true;
            }
            Debug.Log("몬스터 찾지 못함");
            return false;
        }
    }
}