using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Personal.HagYun;
namespace Battle
{
    public class Player : Character
    {
        [SerializeField] PlayerBaseStatusSO baseStat;
        [SerializeField] Collider2D[] monColArr = new Collider2D[64];
        protected override void FixedUpdateFeat()
        {
            FixedUpdateMoveFeat();
        }

        protected override void UpdateFeat()
        {
            UpdateMoveFeat();
        }
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(rb.position, atkRange);
        }

        void FixedUpdateMoveFeat()
        {
            cm.FixedMove();
            if (!CheckAtkRangeCollision(ref monColArr))
            {
                if (moveTarget != null)
                {
                    cm.ChaseMove(DirFromPosToTarget(), speed);
                }
            }
        }
        void UpdateMoveFeat()
        {
            cm.UpdateMoveInput(speed);
            TestMoveTargetSet();
            AtkFeat();
        }
        void TestMoveTargetSet()
        {
            if (moveTarget == null && MonsterSetComponent.ins.TryGetMonster(out GameObject obj))
                moveTarget = obj.transform;
        }
        void AtkFeat()
        {
            if (!cm.IsInputMoving && CheckAtkRangeCollision(ref monColArr))
            {
                if (monColArr[0] != null)
                {
                    if (!isAtkCooltime)
                        AtkCharacter(monColArr[0].GetComponent<Character>());
                }
            }
        }
    }
}