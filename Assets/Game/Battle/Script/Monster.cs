using Base.Data;
using Base.Managers;
using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Battle
{
    public class Monster : Character
    {
        public MonsterSO monsterSO;
        public const float MonsterAttackRange = 0.6f;
        public const float ApproachStopRange = 0.15f;
        protected override float AttackRange => MonsterAttackRange; 
        public override BattleStat CurrentBattleStat => monsterSO.battleStat;
        private CancellationTokenSource monsterCts;
        public event Action<float, float> OnMonsterHpChanged; //내부이벤트로 허브등록 X
        public event Action<Monster> OnMonsterKilled;

        public override void Init()
        {
            base.Init();
            monsterCts?.Dispose();
            monsterCts = new CancellationTokenSource();

            PlayerManager playerRef = GameManager.Instance.GetGameSystem<PlayerManager>();
            if (playerRef == null)
            {
                Debug.LogWarning("플레이어가 존재하지 않습니다. ");
                return;
            }
            target = playerRef.GetComponent<Character>();
            targetTransform = playerRef.transform;
        }

        /// <summary>스테이지 변경등의 이유로 사라질 때 실행</summary>
        public void ForcedReturn()
        {
            Debug.Log("오브젝트 강제 정리");
            Destroy(gameObject);
        }

        /// <summary> 플레이어에게 처치당했을 시 실행</summary>
        protected override void OnDead()
        {
            if (isDead) //여러번 죽지 않게하기
                return;
            isDead = true;
            Debug.Log($"isDead : {isDead}");
            DeadMotionAsync(monsterCts.Token).Forget();
        }

        async UniTaskVoid DeadMotionAsync(CancellationToken cts)
        {
            //var cts = this.GetCancellationTokenOnDestroy();
            //3.24(규성) : 몬스터는 사망할때 Destroy가 아닌 비활성화이기 때문에 해당 토큰은 사용이 제한됩니다
            Debug.Log("몬스터 처치됨");
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
            OnMonsterKilled?.Invoke(this);
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
        protected override void UpdateFeat()
        {
        }

        public override void Hit(float damage)
        {
            base.Hit(damage);
            OnMonsterHpChanged?.Invoke(Hp,CurrentBattleStat.maxHp);
        }
        protected override void SendHitSignal()
        {
            eventHub?.MonsterHit();
        }
        /*
        private void FixedUpdate()
        {
            if (isDead || target is null)
                return;
        }
        */

        protected override void FixedUpdateFeat()
        {
            if (target == null || isDead) return;

            UpdateFacing(target.transform.position.x - transform.position.x);

            float distanceToTarget = Vector2.Distance(transform.position, target.transform.position);

            if (distanceToTarget <= AttackRange)
            {
                state = CharacterState.Attack;
                NormalAttack(target);
            }
            else
            {
                state = CharacterState.Move;
                cm.ChaseMove(target.transform, CurrentBattleStat.moveSpeed);
                if (spumController != null)
                {
                    spumController.PlayAnimation(PlayerState.MOVE, 0);
                }
            }
        }

        private void OnDisable()
        {
            monsterCts?.Cancel();
        }
        private void OnDestroy()
        {
            monsterCts?.Cancel();
            monsterCts?.Dispose();
        }
        // public void ChasePlayer()
        // {
        //     if(Vector2.Distance(transform.position, target.transform.position) > ApproachStopRange)
        //         transform.position = Vector2.MoveTowards(transform.position,target.transform.position, CurrentBattleStat.moveSpeed * Time.deltaTime);
        // }
    }
}