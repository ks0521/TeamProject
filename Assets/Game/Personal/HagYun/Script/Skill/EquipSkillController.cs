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

            // // 1. 스프라이트 자체의 순수 크기 (Local Bounds)
            // // import 설정에서 PPU(Pixels Per Unit)에 의해 결정된 월드 크기입니다.
            // Vector2 spriteSize = sr.sprite.bounds.size;

            // // 2. 타겟 크기 대비 비율 계산
            // float ratioX = SpriteSize.x / spriteSize.x;
            // float ratioY = SpriteSize.y / spriteSize.y;

            // // 3. 비율 유지 (전체 영역에 맞추기 위해 더 작은 비율 선택)
            // float minRatio = Mathf.Min(ratioX, ratioY);

            // // 4. Transform의 스케일을 직접 수정
            // sr.transform.localScale = new Vector3(minRatio, minRatio, 1f);
            //float spriteWidth = spriteSize.x;
            float spriteWidth = MathF.Max(0.1f, spriteSize.x);
            radius *= 2f;
            if (spriteWidth == 0) spriteWidth = 1;
            float scale = radius / spriteWidth;
            
            sr.transform.localScale = new Vector3(scale,scale,1f);
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
        public virtual void SkillEquip(int index, Skill targetSkill, bool isInit = false)
        {
            if (targetSkill == null)
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
            IsCasting = true;
            // eventSet.RaiseCastingStart();
            eventHub.CastingStarted();
            float alphaValue = 100f / 255f;
            if (sr != null) sr.color = new Color(0, 0, 1f, alphaValue);

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
                SkillRangeChange(equipSkillArr[index].Skill.Data.range);
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
    }
}