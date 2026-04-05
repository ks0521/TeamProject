using Battle;
using Cysharp.Threading.Tasks;
using Growth.Skill;
using UnityEngine;

namespace Personal.HagYun
{
    public class AreaSkill : ActiveSkill
    {
        [SerializeField] protected Vector2 targetPos;
        protected override Vector2 TargetPos => targetPos;
        [SerializeField] Animator[] addEffectAnim;
        [SerializeField] CapsuleCollider2D effectAreaCollider;
        // sprite check
        [SerializeField] bool isSpriteYPosUp = false;
        // targeting test
        [SerializeField] Transform targetTrans;
        public override void Init(Character owner)
        {
            base.Init(owner);
            AreaInit();
        }
        protected void AreaInit()
        {
            float areaSize = ActiveSkillData.effectArea;
            Vector2 effectSize = effectAnim.transform.localScale;
            effectAnim.transform.localScale = effectSize * areaSize;
            if (isSpriteYPosUp)
            {
                Vector2 effectPos = effectAnim.transform.position;
                effectAnim.transform.position = effectPos * areaSize;
            }
            if (addEffectAnim != null)
            {
                for (int i = 0; i < addEffectAnim.Length; i++)
                {
                    Vector2 addEffectSize = addEffectAnim[i].transform.localScale;
                    addEffectAnim[i].transform.localScale = addEffectSize * areaSize;
                    if (isSpriteYPosUp)
                    {
                        Vector2 addEffectPos = addEffectAnim[i].transform.position;
                        addEffectAnim[i].transform.position = addEffectPos * areaSize;
                    }
                }
            }
            AreaShowColliderInit();
        }
        protected void AreaShowColliderInit()
        {
            effectAreaCollider = GetComponent<CapsuleCollider2D>();
            effectAreaCollider.size *= ActiveSkillData.effectArea;
        }
        public override void SkillUseTargeting(TargetChecker target)
        {
            // target 설정
            if (targetTrans == null)
            {
                targetPos = target.targetPos;
            }
            else if (data.Targeting == TargetingMode.Self)
            {
                targetPos = ThisPos;
            }
            else
            {
                targetPos = targetTrans.position;
            }
            EnableSkill();
        }
        protected async UniTaskVoid SkillEffectTask()
        {
            await CurAnimTimerTask(0.5f);
            SkillEffect();
            ObjDisableTimerTask().Forget();
        }
        public override void SkillEffect()
        {
            Debug.Log("area skill 이펙트");
            PlSkillCapsuleAreaAtk(TargetPos, effectAreaCollider.size, effectAreaCollider.direction);
        }
        protected override void EnableSkill()
        {
            EnableEffect();
            base.EnableSkill();
            if (addEffectAnim != null)
            {
                for (int i = 0; i < addEffectAnim.Length; i++)
                {
                    addEffectAnim[i].Rebind();
                }
            }
            // SkillEffectTask().Forget();
            SkillEffect();
            ObjDisableTimerTask().Forget();
        }
    }
}