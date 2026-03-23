using Base.Data;
using Base.Managers;
using Personal.HagYun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Battle
{
    public class Player : Character
    {
        // outside component
        [SerializeField] protected PlayerRuntimeStatus runtimeStatus;
        [SerializeField] protected PlayerEquipSkillController equipSkillController;
        //StatusCalculator statCal;
        protected override BattleStat CurrentBattleStat => runtimeStatus.finalBattleStatus;
        protected override float AttackRange => runtimeStatus.finalRange;
        protected static void StaticPlSet(Player newPl) => Pl = newPl;
        [SerializeField] private List<Monster> stageMonsters;//현재 스테이지에 존재하는 몬스터의 리스트
        void OnDestroy()
        {
            if (Pl == this) Pl = null;
        }
        protected override void Init()
        {
            base.Init();

            equipSkillController.Init(this);
            //runtimeStatus = GetComponent<PlayerRuntimeStatus>();
            stageMonsters = GameManager.Instance.GetGameSystem<StageManager>().GetStageMonsters();
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
            //TestMoveTargetSet();
            //AtkFeat();
        }
        private bool FindTarget()
        {
            //stageMonsters = stageManager.GetStageMonsters();
            float minDist = Single.MaxValue;
            float dist;
            if (stageMonsters is null || stageMonsters.Count == 0)
            {
                //Debug.LogWarning("현재 스테이지에 나와있는 몬스터가 없습니다. ");
                return false; 
            }

            target = stageMonsters[0]; // 일단 버그 방지
            foreach (var monster in stageMonsters)
            {
                dist = Vector2.Distance(transform.position, monster.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    target = monster;
                    //target = monster.transform;
                }
            }
            return true;
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
                        // cm.ChaseMove(DirFromPosToTarget(), CurrentBattleStat.moveSpeed);
                        cm.VChaseMove(DirFromPosToTarget());
                    }
                }
                else
                {
                    //Debug.Log(Vector2.Distance(target.transform.position, transform.position));
                    AtkFeat();
                }
            }
        }
        // void TestMoveTargetSet()
        // {
        //     if (target == null && MonsterSetComponent.ins.TryGetMonster(out GameObject obj))
        //         target = obj.GetComponent<Monster>();
        // }
        void AtkFeat()
        {
            NormalAttack(target);
        }
    }
}