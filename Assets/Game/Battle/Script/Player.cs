using Base.Data;
using Personal.HagYun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Battle
{
    public class Player : Character
    {
        [SerializeField] private PlayerRuntimeStatus runtimeStatus;
        //StatusCalculator statCal;
        protected override BattleStat CurrentBattleStat => runtimeStatus.finalBattleStatus;
        protected override float AttackRange => runtimeStatus.finalRange;
        [SerializeField] Collider2D[] monColArr = new Collider2D[64];
        protected override void AwakeInit()
        {
            base.AwakeInit();
            //runtimeStatus = GetComponent<PlayerRuntimeStatus>();
        }
        protected override void OnEnableInit()
        {

            base.OnEnableInit();
        }
        protected override void StartInit()
        {

            base.StartInit();
        }
        /// <summary> 플레이어에게 처치당했을 시 실행</summary>
        protected override void OnDead()
        {
            if (isDead) return;
            isDead = true;
            Debug.Log("스테이지 실패");
        }
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, AttackRange);
        }
#endif
        protected override void UpdateFeat()
        {
            UpdateMoveFeat();
        }

        protected override void FixedUpdateFeat()
        {
            FixedUpdateMoveFeat();
        }
        void UpdateMoveFeat()
        {
            cm.UpdateMoveInput(CurrentBattleStat.moveSpeed);
            TestMoveTargetSet();
            //AtkFeat();
        }
        void FixedUpdateMoveFeat()
        {
            cm.FixedMove();
            //if (!CheckAtkRangeCollision(ref monColArr))
            if (!isAtkCooltime)
            {
                if (!CheckTargetIsClose())
                {
                    if (target != null)
                    {
                        cm.ChaseMove(DirFromPosToTarget(), CurrentBattleStat.moveSpeed);
                    }
                }
                else
                {
                    //Debug.Log(Vector2.Distance(target.transform.position, transform.position));
                    AtkFeat();
                }
            }
        }
        void TestMoveTargetSet()
        {
            if (target == null && MonsterSetComponent.ins.TryGetMonster(out GameObject obj))
                target = obj.GetComponent<Monster>();
        }
        void AtkFeat()
        {
            if (!cm.IsInputMoving && !isAtkCooltime)
            {
                NormalAttack(target);
            }
        }
    }
}