using Battle;
using Cysharp.Threading.Tasks;
using Base.Utils;
using UnityEngine;

namespace Growth.Skill
{
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
    public abstract class ActiveSkillObject : MonoBehaviour
    {
        protected ActiveSkill launcher;
        // target 설정
        [SerializeField] protected LayerMask targetMask = 1 << 8;
        public LayerMask TargetMask => targetMask;
        protected Vector2 ThisPos
        {
            get => transform.position;
            set => transform.position = value;
        }
        protected abstract Vector2 TargetPos { get; }
        // public Vector2 OwnerPos => owner.transform.position;
        // effect
        [SerializeField] protected Animator effectAnim;
        public virtual void Init(ActiveSkill launcher)
        {
            this.launcher = launcher;
        }
        public abstract void SkillUseTargeting(TargetChecker target);
        public abstract void SkillEffect();
        public void SkillAtk(Character cha)
        {
            launcher.Owner.SkillAttack(cha, launcher.ResultDamage);
        }
        public void PlAreaAtk(int inAreaTargetCnt)
        {
            if (inAreaTargetCnt <= 0) return;
            for (int i = 0; i < inAreaTargetCnt; i++)
            {
                if (OverlapChecker.GetTargetCol(i).GetComponent<Monster>() is Monster mon && !mon.IsDead)
                    launcher.Owner.SkillAttack(mon, launcher.ResultDamage);
            }
        }
        public void PlSkillCircleAreaAtk(Vector2 targetPos)
        {
            int cnt = OverlapChecker.GetCircleTargetsCount(targetPos, launcher.ActiveSkillData.effectArea, targetMask);
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
    }
}
