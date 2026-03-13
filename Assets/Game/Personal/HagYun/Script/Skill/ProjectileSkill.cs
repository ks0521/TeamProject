using Battle;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

namespace Personal.HagYun
{
    public class ProjectileSkill : Skill
    {
        [SerializeField] BoxCollider2D col;
        [SerializeField] Animator projectileAnim;
        bool isHoming;
        private void OnEnable()
        {
            EnableProjectile();
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.GetComponent<Monster>() is Monster mon)
            {
                target = mon;
                SkillEffect();
            }
        }
        private void Update()
        {
            MoveToTarget();
        }
        void MoveToTarget()
        {
            if (target == null)
            {
                //gameObject.SetActive(false);
                return;
            }
            else if (!isHoming)
            {
                return;
            }
            transform.MoveToTarget(TargetPos, Data.speed);
        }
        public override void SkillEffect()
        {
            DisableProjectile();
            EnableEffect(TargetPos);
            if (Data.SoE == Growth.Skill.ScopeOfEffect.Single)
            {
                PlSkillAtk(target);
            }
            else if(Data.effectArea == 0)
            {
                Debug.LogWarning($"{gameObject.name}의 range 값이 0입니다.");
            }
            else
            {
                PlSkillCircleAreaAtk(TargetPos);
            }
        }
        void EnableProjectile()
        {
            col.enabled = true;
            projectileAnim.gameObject.SetActive(true);
            projectileAnim.Rebind();
            isHoming = true;
            MoveToTarget();
        }
        void DisableProjectile()
        {
            col.enabled = false;
            projectileAnim.gameObject.SetActive(false);
            isHoming = false;
        }
        protected override void EnableEffect(Vector2 targetPos)
        {
            base.EnableEffect(targetPos);
            ObjDisableTimerTask().Forget();
        }
    }
}