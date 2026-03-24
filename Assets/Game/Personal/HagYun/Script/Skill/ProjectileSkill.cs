using Battle;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

namespace Personal.HagYun
{
    public class ProjectileSkill : Skill
    {
        [SerializeField] protected Character target;
        protected override Vector2 TargetPos => target.transform.position;
        [SerializeField] protected Animator projectileAnim;
        bool isHoming;
        //private void OnEnable()
        //{
        //    EnableProjectile();
        //}
        private void Update()
        {
            MoveToTarget();
        }
        void MoveToTarget()
        {
            if (!isHoming) return;
            else if (target == null)
            {
                Debug.LogWarning($"target 없어짐, {gameObject.name} skill 비활성화");
                gameObject.SetActive(false);
                return;
            }
            transform.MoveToTarget(TargetPos, Data.speed);
            transform.LookToTarget(TargetPos);
            if (!transform.CheckDirZeroToTarget(TargetPos))
            {
                SkillEffect();
            }
        }
        public override void SkillUseTargeting(TargetChecker target)
        {
            if (target.targetCha == null)
            {
                Debug.LogWarning($"target이 설정되지 않아, {gameObject.name} skill 활성화 하지 않음");
                return;
            }
            this.target = target.targetCha;
            //EnableProjectile();
            EnableSkill();
        }
        public override void SkillEffect()
        {
            DisableProjectile();
            EnableEffect();
            SkillAtk(target);
            // if (Data.SoE == Growth.Skill.ScopeOfEffect.Single)
            // {
            //     SkillAtk(target);
            // }
            // else if (Data.effectArea == 0)
            // {
            //     Debug.LogWarning($"{gameObject.name}의 range 값이 0입니다.");
            //     SkillAtk(target);
            // }
            // else
            // {
            //     // PlSkillCircleAreaAtk(TargetPos);
            //     PlSkillCapsuleAreaAtk(TargetPos, effectRangeOffset, CapsuleDirection2D.Horizontal);
            // }
        }
        void EnableProjectile()
        {
            if (effectAnim.gameObject.activeSelf) effectAnim.gameObject.SetActive(false);
            ThisPos = OwnerPos;
            transform.LookToTarget(TargetPos);
            projectileAnim.gameObject.SetActive(true);
            projectileAnim.Rebind();
            isHoming = true;
            //MoveToTarget();
        }
        protected void DisableProjectile()
        {
            isHoming = false;
            projectileAnim.gameObject.SetActive(false);
        }
        protected override void EnableEffect()
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
            base.EnableEffect();
            ObjDisableTimerTask().Forget();
        }
        protected override void EnableSkill()
        {
            EnableProjectile();
            base.EnableSkill();
        }
    }
}