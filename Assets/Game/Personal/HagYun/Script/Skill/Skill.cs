using Battle;
using Cysharp.Threading.Tasks;
using Growth.Skill;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;

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
        public static float Angle(this in Vector2 v) => (Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg) - 90f;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Quaternion LookTarget(this in Vector3 thisPos, in Vector3 targetPos)
        {
            return Quaternion.LookRotation(Vector3.forward, targetPos.ToV2() - thisPos.ToV2());
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
    public abstract class Skill : MonoBehaviour
    {
        // skill data
        [SerializeField] protected SkillSO data;
        // target 설정
        [SerializeField] protected LayerMask targetMask = 1 << 8;
        // property
        public SkillSO Data => data;
        public LayerMask TargetMask => targetMask;
        [field: SerializeField] public static Character PlOwner { get; protected set; }
        protected Vector2 ThisPos
        {
            get => transform.position;
            set => transform.position = value;
        }
        protected abstract Vector2 TargetPos { get; }
        public Vector2 OwnerPos => PlOwner.transform.position;
        // effect
        [SerializeField] protected Animator effectAnim;
        // etc
        //protected CancellationTokenSource cts;

        // test
        //public Player pl;
        public static void SetPlOwner(Character pl) => PlOwner = pl;
        //public void TargetSet(Character target) => this.target = target;
        private void Awake()
        {
            if (data == null)
            {
                Debug.LogWarning($"{gameObject.name}에 skill data 없음");
            }
            if (PlOwner == null)
            {
                //SetPlOwner(pl);
                if (PlOwner == null) Debug.LogWarning("왜 스킬 플레이어 저장 안됨?");
            }
        }
        public abstract void SkillUseTargeting(TargetChecker target);
        public abstract void SkillEffect();
        //public float PlSkillDmg()
        //{
        //    float resultDmg = PlOwner.atk;
        //    resultDmg *= Data.baseDamage;
        //    if (IsCriticalChance(PlOwner))
        //        resultDmg *= PlOwner.criDmgPower;
        //    Debug.Log($"{gameObject.name} 스킬 데미지 : {resultDmg}");
        //    return resultDmg;
        //}
        public void PlSkillAtk(Character cha)
        {
            //cha.Hit(PlSkillDmg());
            cha.Hit(PlOwner.SkillResultDmg(data.baseDamage));
        }
        public void AreaAtk(int inAreaTargetCnt)
        {
            if (inAreaTargetCnt <= 0) return;
            float resultDmg = PlOwner.SkillResultDmg(data.baseDamage);
            for (int i = 0; i < inAreaTargetCnt; i++)
            {
                if (OverlapChecker.GetTargetCol(i).GetComponent<Monster>() is Monster mon)
                    mon.Hit(resultDmg);
            }
        }
        public void PlSkillCircleAreaAtk(Vector2 targetPos)
        {
            int cnt = OverlapChecker.GetCircleTargetsCount(targetPos, Data.effectArea, targetMask);
            AreaAtk(cnt);
        }
        public void PlSkillCapsuleAreaAtk(Vector2 targetPos, Vector2 overlapCapsuleSize, CapsuleDirection2D capsuleDir)
        {
            int cnt = OverlapChecker.GetCapsuleTargetsCount(targetPos, overlapCapsuleSize, capsuleDir, targetMask);
            AreaAtk(cnt);
        }
        //public bool IsCriticalChance(Character ch)
        //{
        //    if (Random.Range(0f, 1f) < ch.criChance)
        //        return true;
        //    else
        //        return false;
        //}
        protected virtual void EnableSkill()
        {
            gameObject.SetActive(true);
        }
        protected virtual void DisableSkill()
        {
            //DisableEffect();
            gameObject.SetActive(false);
        }
        protected virtual void EnableEffect()
        {
            ThisPos = TargetPos;
            gameObject.SetActive(true);
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
            DisableSkill();
        }
    }
}
