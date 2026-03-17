using System;
using System.Collections.Generic;
using Base.Data;
using Base.Managers;
using UnityEditor;
using UnityEngine;

namespace Personal.GyuSeong
{
    public class TestPlayer : TestCharacter
    {
        [SerializeField] private StageManager stageManager;
        [SerializeField] private PlayerRuntimeStatus runtimeStatus;
        protected override BattleStat CurrentBattleStat => runtimeStatus.finalBattleStatus;
        protected override float AttackRange => runtimeStatus.finalRange;
        [SerializeField] Collider2D[] monColArr = new Collider2D[64];
        private TestMonster targetMonster;
        [SerializeField] private List<TestMonster> stageMonsters;

        protected override void OnDead()
        {
            if (isDead)
                return;
            isDead = true;
            Debug.Log("스테이지 실패");
            //이후 기능 구현
        }

        /// <summary> 현재 스테이지에 있는 몬스터 중 가장 가까운 타겟을 curTarget에 설정 </summary>
        /// <returns>curTarget 갱신여부(true - 갱신성공 / false - 갱신실패)</returns>
        private bool FindTarget()
        {
            stageMonsters = stageManager.GetStageMonsters();
            float minDist = Single.MaxValue;
            float dist;
            if (stageMonsters is null || stageMonsters.Count == 0)
            {
                Debug.LogWarning("현재 스테이지에 나와있는 몬스터가 없습니다. ");
                return false;
            }

            targetMonster = stageMonsters[0]; // 일단 버그 방지
            foreach (var monster in stageMonsters)
            {
                dist = Vector2.Distance(transform.position, monster.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    targetMonster = monster;
                    target = monster.transform;
                }
            }

            return true;
        }

        protected override void Init()
        {
            runtimeStatus = GetComponent<PlayerRuntimeStatus>();
            hp = CurrentBattleStat.maxHp;
        }

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
            //Gizmos.color = Color.green;
            float range = runtimeStatus != null ? runtimeStatus.finalRange : 1.5f;
            //Gizmos.DrawWireSphere(transform.position, range);
#if UNITY_EDITOR
            Handles.color = Color.blue;

            // XZ 평면에 평평한 원
            Handles.DrawWireDisc(transform.position, Vector3.forward, range);

            // XY 평면에 그리고 싶으면
            // Handles.DrawWireDisc(transform.position, Vector3.forward, radius);

            // YZ 평면에 그리고 싶으면
            // Handles.DrawWireDisc(transform.position, Vector3.right, radius);
#endif
        }

        /// <summary></summary>
        void FixedUpdateMoveFeat()
        {
            if (cm.IsInputMoving) return; //플레이어 조작중에는 자동이동하지 않음
            if (target is null)
            {
                if (!FindTarget())
                {
                    Debug.Log("스테이지 내 적이없어 이동할 수 없습니다. ");
                    return;
                }
            }   
            //공격 사거리에 대상이 없으면 이동, 있으면 멈춰서 공격
            if (AttackRange < Vector2.Distance(transform.position, target.position))
            {
                cm.ChaseMove(DirFromPosToTarget(), CurrentBattleStat.moveSpeed);
                return;
            }
            AtkFeat(); //공격 판정

            // if (!CheckAtkRangeCollision(ref monColArr))
            // {
            //     if (target != null)
            //     {
            //         cm.ChaseMove(DirFromPosToTarget(), CurrentBattleStat.moveSpeed);
            //     }
            // }
        }

        void UpdateMoveFeat()
        {
            cm.UpdateMoveInput(CurrentBattleStat.moveSpeed); //입력받아서 속도 및 플레이어 조작여부 저장
            cm.FixedMove(); //플레이어 입력이 있으면 실제로 이동
        }

        void TestMoveTargetSet()
        {
            //if (target == null && MonsterSetComponent.ins.TryGetMonster(out GameObject obj))
            //target = obj.transform;
        }

        void AtkFeat()
        {
            if (targetMonster is null || targetMonster.IsDead || !targetMonster.isActiveAndEnabled)
            {
                if (!FindTarget())
                {
                    Debug.Log("스폰된 적 없음");
                    return;
                }
            }
            Attack(targetMonster);

            // if (!cm.IsInputMoving && CheckAtkRangeCollision(ref monColArr))
            // {
            //     if (monColArr[0] != null)
            //     {
            //         if (CanAttack)
            //             Attack(monColArr[0].GetComponent<character1>());
            //     }
            // }
        }
    }
}