using Base.Data;
using Base.Managers;
using Base.Save;
using Cysharp.Threading.Tasks;
using Personal.HagYun;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Battle
{
    public class Player : Character
    {
        // outside component
        public override float Hp
        {
            get => hp;
            protected set
            {
                hp = value;
                if (CurrentBattleStatStat.maxHp <= hp)
                {
                    hp = CurrentBattleStatStat.maxHp;
                }
                eventHub.HpChanged(hp,MaxHp);
                if (hp <= 0f)
                {
                    OnDead();
                }
            }
        }
        public PlayerEquipSkillController ESController => equipSkillController;
        public override BattleStat CurrentBattleStatStat => runtimeStatus.finalBattleStatStatus;
        protected override float AttackRange => runtimeStatus.finalBattleStatStatus.atkRange;
        [SerializeField] private RuntimeProgressState runtimeProgress;
        [SerializeField] protected PlayerRuntimeStatus runtimeStatus;
        [SerializeField] protected PlayerEquipSkillController equipSkillController;
        [SerializeField] private StageManager stageManager;
        [SerializeField] private EventHub hub;
        [SerializeField] private List<Monster> stageMonsters; //현재 스테이지에 존재하는 몬스터의 리스트

        public int Level { 
            get => runtimeProgress.currency.level;
            set => runtimeProgress.currency.level = value;
        }

        public override void Init()
        {
            base.Init();
            equipSkillController.Init(this);
            hub = GameManager.Instance.GetGameSystem<EventHub>();
            stageManager = GameManager.Instance.GetGameSystem<StageManager>();
            runtimeProgress = GameManager.Instance.GetGameSystem<PlayerProgressManager>().Progress;
        }

        void Rebirth()
        {
            isDead = false;
            Hp = CurrentBattleStatStat.maxHp;
        }
        /// <summary> 플레이어에게 처치당했을 시 실행</summary>
        protected override void OnDead()
        {
            if (isDead) return;
            isDead = true;

            if (hub != null && stageManager != null)
            {
                hub.PlayerDead(this);
            }

            DeadMotionAsync().Forget();
        }

        public void RecoveryHP(int value)
        {
            Hp += value;
        }
        async UniTaskVoid DeadMotionAsync()
        {
            var cts = this.GetCancellationTokenOnDestroy();

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
            Rebirth();
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
            if (target == null || target.IsDead || !target.isActiveAndEnabled)
            {
                if (!FindTarget()) return;
            }

            FixedUpdateMoveFeat();
        }

        void UpdateMoveFeat()
        {
            cm.UpdateMoveInput(CurrentBattleStatStat.moveSpeed);
            // TestMoveTargetSet();
            //AtkFeat();
        }

        private bool FindTarget()
        {
            if (GameManager.Instance == null) return false;

            if (stageManager == null)
            {
                stageManager = GameManager.Instance.GetGameSystem<StageManager>();
                //아직 GameManager가 StageManager를 등록하지 못했다면 재시도
                if (stageManager == null) return false;
            }

            if (!stageManager.TryGetTarget(this.transform, out var result))
            {
                return false;
            }

            target = result;
            targetTransform = result.transform;
            return true;
            //target = targetMonster;
            /*stageMonsters = stageManager.Monsters;
            float minDist = Single.MaxValue;
            float dist;
            if (stageMonsters is null || stageMonsters.Count == 0)
            {
                return false;
            }

            target = stageMonsters[0]; // 일단 버그 방지
            foreach (var monster in stageMonsters)
            {
                if (monster.IsDead) continue;
                dist = Vector2.Distance(transform.position, monster.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    target = monster;
                    targetTransform = monster.transform;
                }
            }

            return true;*/
        }

        void FixedUpdateMoveFeat()
        {
            if (target == null || isDead) return;
            cm.FixedMove();
            UpdateFacing(target.transform.position.x - transform.position.x);
            //if (!CheckAtkRangeCollision(ref monColArr))
            if (!CheckTargetIsClose())
            {
                state = CharacterState.Move;
                cm.ChaseMove(DirFromPosToTarget(), CurrentBattleStatStat.moveSpeed);
                if (spumController != null)
                {
                    spumController.PlayAnimation(PlayerState.MOVE, 0);
                }
                // cm.VChaseMove(DirFromPosToTarget());
            }
            else
            {
                if (isAtkCooltime) return; 
                state = CharacterState.Attack;
                AtkFeat();
                //cm.VChaseMove(DirFromPosToTarget());
            }
        }

        void AtkFeat()
        {
            if (target == null)
            {
                if (!FindTarget()) return;
            }

            NormalAttack(target);
        }
        protected override void SendHitSignal(float resultDamage, HitType type)
        {
            if (isDead) return;
            eventHub?.PlayerHit();
            eventHub?.RequestDamageText(damageAnchor.position, (int)resultDamage, type, isMonster: false);
        }
    }
}