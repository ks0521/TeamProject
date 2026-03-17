using Battle;
using Cysharp.Threading.Tasks;
using Growth.Skill;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

namespace Personal.HagYun
{
    public static class TransformMoveExtensionsClass
    {
        public static Vector2 ToV2(this Vector3 v) => new Vector2(v.x, v.y);
        public static Vector2 DirThisToTarget(this Vector3 thisPos, Vector3 targetPos, float speed)
        {
            return Vector2.MoveTowards(thisPos.ToV2(), targetPos.ToV2(), speed * Time.deltaTime);
        }
        public static float Angle(this Vector2 v) => (Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg) - 90f;
        public static Quaternion LookTarget(this Vector3 thisPos, Vector3 targetPos)
        {
            return Quaternion.LookRotation(Vector3.forward, targetPos.ToV2() - thisPos.ToV2());
        }
        public static void MoveToTarget(this Transform thisTrans, Vector3 targetPos, float speed)
        {
            thisTrans.position = DirThisToTarget(thisTrans.position, targetPos, speed);
            thisTrans.rotation = LookTarget(thisTrans.position, targetPos);
        }
    }
    public abstract class Skill : MonoBehaviour
    {
        // skill data
        [SerializeField] protected SkillSO data;
        // target 설정
        [SerializeField] protected Character target;
        [SerializeField] protected LayerMask targetMask = 1 << 8;
        // property
        public SkillSO Data => data;
        [field: SerializeField] public static Character PlOwner { get; protected set; }
        protected Vector2 ThisPos
        {
            get => transform.position.ToV2();
            set => transform.position = value;
        }
        protected Quaternion ThisRot
        {
            get => transform.rotation;
            set => transform.rotation = value;
        }
        protected Vector2 TargetPos => target.transform.position.ToV2();
        // effect
        [SerializeField] protected Animator effectAnim;
        // etc
        protected CancellationTokenSource cts;

        // test
        public Player pl;
        public static void SetPlOwner(Character pl) => PlOwner = pl;
        public void TargetSet(Character target) => this.target = target;
        private void Awake()
        {
            if (data == null)
            {
                Debug.LogWarning($"{gameObject.name}에 skill data 없음");
            }
            if (PlOwner == null)
                SetPlOwner(pl);
            if (PlOwner == null) Debug.LogWarning("왜 스킬 플레이어 저장 안됨?");
        }
        public abstract void SkillEffect();
        public float PlSkillDmg()
        {
            float resultDmg = PlOwner.atk;
            resultDmg *= Data.baseDamage;
            if (IsCriticalChance(PlOwner))
                resultDmg *= PlOwner.criDmgPower;
            return resultDmg;
        }
        public void PlSkillAtk(Character cha)
        {
            cha.Hit(PlSkillDmg());
        }
        public void AreaAtk(int inAreaTargetCnt)
        {
            if (inAreaTargetCnt <= 0) return;
            float resultDmg = PlSkillDmg();
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
        public bool IsCriticalChance(Character ch)
        {
            if (Random.Range(0f, 1f) < ch.criChance)
                return true;
            else
                return false;
        }
        protected virtual void EnableEffect(Vector2 targetPos)
        {
            ThisPos = targetPos;
            ThisRot = Quaternion.Euler(0, 0, 0);
            effectAnim.gameObject.SetActive(true);
            effectAnim.Rebind();
        }
        protected virtual void EnableSkill(Character ch)
        {
            EnableEffect(ch.transform.position.ToV2());
        }
        protected virtual void DisableEffect()
        {
            effectAnim.gameObject.SetActive(false);
        }
        protected virtual void DisableSkill()
        {
            DisableEffect();
            gameObject.SetActive(false);
        }
        protected async UniTask CurAnimTimerTask(float timerValue)
        {
            float curAnimStateTimeValue = effectAnim.GetCurrentAnimatorStateInfo(0).normalizedTime;
            while (curAnimStateTimeValue < timerValue)
            {
                await UniTask.Yield(pl.GetCancellationTokenOnDestroy());
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
