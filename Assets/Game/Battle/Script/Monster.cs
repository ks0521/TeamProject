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
    public enum CharacterState
    {
        Idle, Move, Attack
    }

    public class Monster : Character
    {
        public MonsterSO monsterSO;
        public const float MonsterAttackRange = 0.6f;
        public override BattleStat CurrentBattleStat => monsterSO.battleStat;
        protected override float AttackRange => MonsterAttackRange;
        public const float ApproachStopRange = 0.15f;
        public event Action<float, float> OnMonsterHpChanged; //내부이벤트로 허브등록 X
        private EventHub eventHub;

        public CharacterState state;
        public event Action<Monster> OnMonsterKilled;
        //public Transform player;

        //공격 대상의 스크립트를 미리 캐싱해둘 변수
        private Character targetCharacter;

        protected override void Init()
        {
            state = CharacterState.Idle;
            base.Init();

            GameManager gm = GameObject.FindAnyObjectByType<GameManager>();
            if (gm != null)
            {
                eventHub = gm.GetGameSystem<EventHub>();
                Debug.Log($"{gameObject.name}: GameManager를 찾아 EventHub를 연결했습니다.");
            }
            else
            {
                Debug.LogError($"{gameObject.name}: 씬에서 GameManager를 찾을 수 없습니다!");
            }

            var playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                target = playerObj.GetComponent<Character>();
                targetTransform = target.transform;
                targetCharacter = target;
            }
            else
            {
                Debug.LogWarning($"{gameObject.name}: 플레이어를 찾지 못했습니다. 태그를 확인하세요.");
            }

            if (GameManager.Instance != null)
            {
                eventHub = GameManager.Instance.GetGameSystem<EventHub>();
            }
        }

        /// <summary>스테이지 변경등의 이유로 사라질 때 실행</summary>
        public void ForcedReturn()
        {
            //현재는 구현할 필요 없습니다. 
            Debug.Log("오브젝트 강제 정리");
            Destroy(gameObject);
        }

        /// <summary> 플레이어에게 처치당했을 시 실행</summary>
        protected override void OnDead()
        {
            if (isDead) //여러번 죽지 않게하기
                return;
            isDead = true;
            
            DeadMotionAsync().Forget();
        }
        async UniTaskVoid DeadMotionAsync()
        {
            var cts = this.GetCancellationTokenOnDestroy();

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
            if (target == null || targetCharacter == null || isDead) return;

            UpdateFacing(target.transform.position.x - transform.position.x);

            float distanceToTarget = Vector2.Distance(transform.position, target.transform.position);

            if (distanceToTarget <= AttackRange)
            {
                state = CharacterState.Attack;
                NormalAttack(targetCharacter);
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

        // public void ChasePlayer()
        // {
        //     if(Vector2.Distance(transform.position, target.transform.position) > ApproachStopRange)
        //         transform.position = Vector2.MoveTowards(transform.position,target.transform.position, CurrentBattleStat.moveSpeed * Time.deltaTime);
        // }
    }
}