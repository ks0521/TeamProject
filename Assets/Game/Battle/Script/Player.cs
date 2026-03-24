using Base.Data;
using Base.Managers;
using Cysharp.Threading.Tasks;
using Personal.HagYun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
namespace Battle
{
    public class Player : Character
    {
        // outside component
        [SerializeField] protected PlayerRuntimeStatus runtimeStatus;
        [SerializeField] protected PlayerEquipSkillController equipSkillController;
        //StatusCalculator statCal;
        public override BattleStat CurrentBattleStat => runtimeStatus.finalBattleStatus;
        protected override float AttackRange => runtimeStatus.finalRange;
        [SerializeField] private StageManager stageManager;
        [SerializeField] private List<Monster> stageMonsters;//현재 스테이지에 존재하는 몬스터의 리스트

        public event Action<Player> OnPlayerKilled;
        protected override void Init()
        {
            base.Init();
            //equipSkillController.Init(this);
            //runtimeStatus = GetComponent<PlayerRuntimeStatus>();
            SyncHpAfterManagersReady().Forget();
        }
        async UniTaskVoid SyncHpAfterManagersReady()
        {
            // GameManager의 Start()가 실행되고 매니저들의 Init()이 끝날 때까지 넉넉히 대기
            // 보통 1~2프레임이면 충분합니다.
            await UniTask.DelayFrame(2);

            if (runtimeStatus != null)
            {
                float calculatedHp = CurrentBattleStat.maxHp;
                if (calculatedHp > 0)
                {
                    hp = calculatedHp;
                    Debug.Log($"매니저 계산 완료! 강화가 적용된 HP로 갱신되었습니다: {hp}");
                }
            }
        }
        /// <summary> 플레이어에게 처치당했을 시 실행</summary>
        protected override void OnDead()
        {
            if (isDead) return;
            isDead = true;

            DeadMotionAsync().Forget();
            Debug.Log("스테이지 실패");
        }
        async UniTaskVoid DeadMotionAsync()
        {
            var cts = this.GetCancellationTokenOnDestroy();

            Debug.Log("플레이어 사망...");
            state = CharacterState.Dead;
            if (spumController != null)
            {
                spumController.PlayAnimation(PlayerState.DEATH, 0);
                await WaitMotion("DEATH", cts);
            }
            else
            {
                await UniTask.Delay(TimeSpan.FromSeconds(1.0f), cancellationToken: cts);
            }
            OnPlayerKilled?.Invoke(this);
        }
        async UniTask WaitMotion(string stateName, CancellationToken token)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(0.1f), cancellationToken: token);
            if (spumController == null || spumController._anim == null) return;

            Animator ani = spumController._anim;

            //재생 중인 애니메이션의 진행도가 100% 미만일 때까지 대기하는 람다문
            await UniTask.WaitUntil(() =>
            {
                var stateInfo = ani.GetCurrentAnimatorStateInfo(0);
                //모션의 상태가 바뀌거나 애니메이션 재생 완료 시 종료
                return !stateInfo.IsName(stateName) || stateInfo.normalizedTime >= 0.99f;
            }, cancellationToken: token);
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
            if (!FindTarget()) return;
            FixedUpdateMoveFeat();
        }
        void UpdateMoveFeat()
        {
            cm.UpdateMoveInput(CurrentBattleStat.moveSpeed);
            // TestMoveTargetSet();
            //AtkFeat();
        }
        private bool FindTarget()
        {
            //GameManager의 Start()가 끝날 때까지 대기
            if (GameManager.Instance == null) return false;

            if (stageManager == null)
            {
                stageManager = GameManager.Instance.GetGameSystem<StageManager>();
                //아직 GameManager가 StageManager를 등록하지 못했다면 재시도
                if (stageManager == null) return false;
            }

            stageMonsters = stageManager.GetStageMonsters();
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
                    targetTransform = monster.transform;
                }
            }
            return true;
        }
        
        void FixedUpdateMoveFeat()
        {
            if (target == null || isDead) return;

            cm.FixedMove();
            UpdateFacing(target.transform.position.x - transform.position.x);
            //if (!CheckAtkRangeCollision(ref monColArr))
            if (!isAtkCooltime)
            {
                if (target != null)
                {
                    if (!CheckTargetIsClose())
                    {
                        state = CharacterState.Move;
                        cm.ChaseMove(DirFromPosToTarget(), CurrentBattleStat.moveSpeed);
                        if (spumController != null)
                        {
                            spumController.PlayAnimation(PlayerState.MOVE, 0);
                        }
                        // cm.VChaseMove(DirFromPosToTarget());
                    }
                    else
                    {
                        //Debug.Log(Vector2.Distance(target.transform.position, transform.position));
                        state = CharacterState.Attack;
                        AtkFeat();
                        //cm.VChaseMove(DirFromPosToTarget());
                    }
                }
            }
        }
        // void TestMoveTargetSet()
        // {
        //     if (target == null && MonsterSetComponent.ins.TryGetMonster(out GameObject obj))
        //     {
        //         target = obj.GetComponent<Monster>();
        //         targetTransform = obj.transform;
        //     }
        // }
        void AtkFeat()
        {
            if (target == null)
            {
                if (!FindTarget()) return;
            }
            NormalAttack(target);
        }

    }
}