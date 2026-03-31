using Base.Data;
using Base.Managers;
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
        [SerializeField] protected PlayerRuntimeStatus runtimeStatus;
        [SerializeField] protected PlayerEquipSkillController equipSkillController;
        public PlayerEquipSkillController ESController => equipSkillController;
        public override BattleStat CurrentBattleStat => runtimeStatus.finalBattleStatus;
        protected override float AttackRange => runtimeStatus.finalRange;
        [SerializeField] private StageManager stageManager;
        [SerializeField] private EventHub hub;
        [SerializeField] private List<Monster> stageMonsters; //현재 스테이지에 존재하는 몬스터의 리스트
        public override float Hp
        {
            get => hp;
            protected set
            {
                hp = value;
                if (CurrentBattleStat.maxHp <= hp)
                {
                    hp = CurrentBattleStat.maxHp;
                }
                eventHub.HpChanged(hp,MaxHp);
                if (hp <= 0f)
                {
                    OnDead();
                }
            }
        }

        public int Level => runtimeStatus.Level;
        public override void Init()
        {
            base.Init();
            equipSkillController.Init(this);
            hub = GameManager.Instance.GetGameSystem<EventHub>();
            stageManager = GameManager.Instance.GetGameSystem<StageManager>();
        }

        void Rebirth()
        {
            isDead = false;
            Hp = CurrentBattleStat.maxHp;
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
            Debug.Log("플레이어 사망");
        }

        public void RecoveryHP(int value)
        {
            Hp += value;
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
                //Debug.Log("새로운 타겟 지정완료");
            }

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
            if (GameManager.Instance == null) return false;

            if (stageManager == null)
            {
                stageManager = GameManager.Instance.GetGameSystem<StageManager>();
                //아직 GameManager가 StageManager를 등록하지 못했다면 재시도
                if (stageManager == null) return false;
            }

            stageMonsters = stageManager.Monsters;
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
                if (monster.IsDead) continue;
                //3.23(규성) : 몬스터가 사망모션이 생겨서 죽어도 타겟에 남아있어서 이를 해결하려고 합니다.
                //이 방식은 비효율적인것 같아서 이후 바꾸려고 합니다
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
            //3.24(규성) : 조건문이 살짝 이상하게 걸려있는것 같아서 수정합니다. 
            //isAtkCooltime은 명칭만 보면 공격에만 영향을 줘야하지만 현재는 이동에도 영향을 주고 있습니다
            //따라서 isAtkCooltime 조건문을 공격부분에만 지정했습니다
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
                if (isAtkCooltime) return; 
                //3.24(규성) : state 변화때문에 isAtkCooltime 조건을 걸어놓았습니다. 
                //원래는 Character.NormalAtk에 isAtkCooltime 조건이 걸려있어 불필요한 조건입니다
                state = CharacterState.Attack;
                AtkFeat();
                //cm.VChaseMove(DirFromPosToTarget());
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
        protected override void SendHitSignal()
        {
            if(!isDead) eventHub?.PlayerHit();
        }
    }
}