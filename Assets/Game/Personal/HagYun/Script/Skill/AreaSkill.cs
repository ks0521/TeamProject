using Battle;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using static UnityEditor.PlayerSettings;

namespace Personal.HagYun
{
    public class AreaSkill : Skill
    {
        [SerializeField] protected Vector2 targetPos;
        protected override Vector2 TargetPos => targetPos;

        [SerializeField] SpriteRenderer[] sprites;
        //Vector2 effectArea = new Vector2(2.5f, 2f);
        Vector2 effectArea = new Vector2(2.5f, 2f);
        // area check
        [SerializeField] CapsuleCollider2D overlapArea;
        // targeting test
        [SerializeField] Transform targetTrans;
        private void Start()
        {
            Init();
        }
        public virtual void Init()
        {
            AreaInit(new Vector2(2.5f, 2f));
        }
        protected void AreaInit(Vector2 area)
        {
            effectArea = new Vector2(1f, 1f) * Data.effectArea;
            sprites[0].transform.localScale = effectArea;
            if(sprites.Length == 2 && sprites[1] != null) sprites[1].transform.localScale = effectArea;
            effectArea = area * Data.effectArea;
            AreaShowColliderInit();
        }
        protected void AreaShowColliderInit()
        {
            overlapArea = GetComponent<CapsuleCollider2D>();
            overlapArea.size = effectArea;
        }
        public override void SkillUseTargeting(TargetChecker target)
        {
            // target 설정
            if (targetTrans == null)
            {
                targetPos = target.targetPos;
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
            PlSkillCapsuleAreaAtk(TargetPos, effectArea, CapsuleDirection2D.Horizontal);
        }
        protected override void EnableSkill()
        {
            EnableEffect();
            base.EnableSkill();
            SkillEffectTask().Forget();
        }
    }
}