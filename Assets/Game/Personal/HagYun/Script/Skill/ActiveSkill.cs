using Battle;
using Cysharp.Threading.Tasks;
using Growth.Skill;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace Personal.HagYun
{
    public static class TransformMoveExtensionsClass
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 ToV2(this in Vector3 v) => new Vector2(v.x, v.y);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 DirThisToTarget(this in Vector3 thisPos, in Vector3 targetPos, float speed)
        {
            return Vector2.MoveTowards(thisPos.ToV2(), targetPos.ToV2(), speed * Time.deltaTime);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Angle(this in Vector3 v) => (Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg) - 90f;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Quaternion LookTarget(this in Vector3 thisPos, in Vector3 targetPos)
        {
            Vector3 dir = targetPos - thisPos;
            dir.z = 0;
            // return Quaternion.LookRotation(Vector3.forward, targetPos.ToV2() - thisPos.ToV2());
            return Quaternion.LookRotation(Vector3.forward, dir);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LookTarget(this Transform thisTrans, in Vector3 targetPos)
        {
            Vector3 dir = targetPos - thisTrans.position;
            thisTrans.rotation = Quaternion.Euler(0, 0, dir.Angle());
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CheckDirZeroToTarget(this Transform thisTrans, in Vector3 targetPos)
        {
            return thisTrans.position.ToV2() != targetPos.ToV2();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void MoveToTarget(this Transform thisTrans, in Vector3 targetPos, float speed)
        {
            thisTrans.position = DirThisToTarget(thisTrans.position, targetPos, speed);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LookToTarget(this Transform thisTrans, in Vector3 targetPos)
        {
            thisTrans.rotation = LookTarget(thisTrans.position, targetPos);
        }
    }
    public struct TargetChecker
    {
        // Projectile Skill Target
        public Character targetCha;
        // Area Skill Target
        public Vector2 targetPos;
        public TargetChecker(Character cha)
        {
            targetCha = cha;
            targetPos = Vector2.zero;
        }
        public TargetChecker(Vector2 pos)
        {
            targetCha = null;
            targetPos = pos;
        }
    }
    public abstract class ActiveSkill : Skill
    {
        // skill data
        // target 설정
        [SerializeField] protected ActiveSkillSO data;
        public override SkillSO SkillData => data;
        public ActiveSkillSO ActiveSkillData => data;
        protected float resultDamage;
        public float ResultDamage => resultDamage;
        public override void StatUpdate()
        {
            resultDamage = data.ResultDamage(curLv);
        }
        [SerializeField] protected LayerMask targetMask = 1 << 8;
        public LayerMask TargetMask => targetMask;
        private int equipSlotIndex = -1;
        public int EquipSlotIndex => equipSlotIndex;
        public bool IsHomingSkill => data.Targeting == TargetingMode.Homing;
        protected Vector2 ThisPos
        {
            get => transform.position;
            set => transform.position = value;
        }
        protected abstract Vector2 TargetPos { get; }
        public Vector2 OwnerPos => owner.transform.position;
        // effect
        [SerializeField] protected Animator effectAnim;
        // etc
        //protected CancellationTokenSource cts;

        //public void TargetSet(Character target) => this.target = target;
        public abstract void SkillUseTargeting(TargetChecker target);
        public abstract void SkillEffect();
        public void EquipSkillSlotIndexUpdate(int index) => equipSlotIndex = index;
        public void SkillAtk(Character cha)
        {
            // cha.Hit(owner.SkillResultDmg(ResultDamage));
            owner.SkillAttack(cha, ResultDamage);
        }
        public void PlAreaAtk(int inAreaTargetCnt)
        {
            if (inAreaTargetCnt <= 0) return;
            for (int i = 0; i < inAreaTargetCnt; i++)
            {
                if (OverlapChecker.GetTargetCol(i).GetComponent<Monster>() is Monster mon && !mon.IsDead)
                    owner.SkillAttack(mon, ResultDamage);
            }
        }
        public void PlSkillCircleAreaAtk(Vector2 targetPos)
        {
            int cnt = OverlapChecker.GetCircleTargetsCount(targetPos, data.effectArea, targetMask);
            PlAreaAtk(cnt);
        }
        public void PlSkillCapsuleAreaAtk(Vector2 targetPos, Vector2 overlapCapsuleSize, CapsuleDirection2D capsuleDir)
        {
            int cnt = OverlapChecker.GetCapsuleTargetsCount(targetPos, overlapCapsuleSize, capsuleDir, targetMask);
            PlAreaAtk(cnt);
        }
        protected virtual void EnableSkill()
        {
            gameObject.SetActive(true);
        }
        protected virtual void DisableSkill()
        {
            gameObject.SetActive(false);
        }
        protected virtual void EnableEffect()
        {
            ThisPos = TargetPos;
            effectAnim.gameObject.SetActive(true);
            effectAnim.Rebind();
        }
        protected virtual void DisableEffect()
        {
            effectAnim.gameObject.SetActive(false);
        }
        protected async UniTask CurAnimTimerTask(float timerValue)
        {
            float curAnimStateTimeValue = effectAnim.GetCurrentAnimatorStateInfo(0).normalizedTime;
            while (curAnimStateTimeValue < timerValue)
            {
                await UniTask.Yield(this.GetCancellationTokenOnDestroy());
                curAnimStateTimeValue = effectAnim.GetCurrentAnimatorStateInfo(0).normalizedTime;
                if (this == null) return;
            }
        }
        protected async UniTaskVoid ObjDisableTimerTask()
        {
            await CurAnimTimerTask(1f);
            if (this == null) return;
            DisableEffect();
            DisableSkill();
        }
        public override void SkillImgSet(Image img)
        {
            base.SkillImgSet(img);
            if (IsHomingSkill)
                img.rectTransform.localEulerAngles = new Vector3(0, 0, 135f);
            else
                img.rectTransform.localEulerAngles = Vector3.zero;

        }
    }
}
