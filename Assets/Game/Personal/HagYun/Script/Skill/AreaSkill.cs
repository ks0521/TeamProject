using Battle;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Personal.HagYun
{
    public class AreaSkill : Skill
    {
        [SerializeField] SpriteRenderer[] sprites;
        //Vector2 effectArea = new Vector2(2.5f, 2f);
        Vector2 effectArea = new Vector2(2.5f, 2f);
        // area check collider
        [SerializeField] CapsuleCollider2D overlapArea;
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
            sprites[1].transform.localScale = effectArea;
            effectArea = area * Data.effectArea;

            overlapArea = GetComponent<CapsuleCollider2D>();
            overlapArea.size = effectArea;
        }
        private void OnEnable()
        {
            if (target == null)
            {
                Debug.LogWarning($"타겟 없음, 플레이어 위치에서 {gameObject.name} skill 생성");
                target = PlOwner;
            }
            if (target == null)
            {
                Debug.LogWarning("플레이어로도 타겟이 되지 않음");
            }
            EnableEffect(TargetPos);
        }
        protected override void EnableEffect(Vector2 targetPos)
        {
            base.EnableEffect(targetPos);
            OnEffecteTask(targetPos).Forget();
        }
        protected async UniTaskVoid OnEffecteTask(Vector2 targetPos)
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
    }
}