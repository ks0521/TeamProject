using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Growth.Skill;
using Cysharp.Threading.Tasks;

namespace Personal.HagYun
{
    public abstract class Skill : MonoBehaviour
    {
        // skill data
        public SkillSO data;
        // target 설정
        protected Vector2 targetPos;
        [SerializeField] protected LayerMask targetMask;

        public void TargetSet(Vector2 pos) => targetPos = pos;
    }
    public class SkillType1 : Skill
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.gameObject.layer == targetMask)
            {
                switch (data.SoE)
                {
                    case ScopeOfEffect.Single:
                        break;
                    case ScopeOfEffect.Area:
                        break;
                }
                // 스킬 데미지 입히기
                // 스킬 이펙트
            }
        }
    }
}
