using Base.Data;
using Base.Managers;
using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Battle
{
    public enum CharacterState
    {
        Idle, Move, Attack, Dead
    }
    public abstract class Character : MonoBehaviour
    {
        // stat
        //[SerializeField] PlayerBaseStatusSO baseStat;
        //자식 (몬스터나 플레이어)에서 전투 스탯을 구현
        public abstract BattleStat CurrentBattleStat { get; }
        [SerializeField] protected float hp;
        public virtual float Hp
        {
            get => hp;
            protected set
            {
                hp = value;
                if (CurrentBattleStat.maxHp <= hp)
                {
                    hp = CurrentBattleStat.maxHp;
                }
                //cEvent.RaiseHPValueChange(hp, CurrentBattleStat.maxHp);
                if (hp <= 0f)
                {
                    OnDead();
                }
            }
        }
        public float MaxHp => CurrentBattleStat.maxHp;
        // battle element
        // action split class (component X)
        protected CharacterMove cm;
        [SerializeField] protected LayerMask targetLayer;
        [SerializeField] protected Character target;
        [SerializeField] protected Transform targetTransform;
        [SerializeField] protected bool isAtkCooltime;
        protected bool isDead;
        public bool IsDead => isDead;
        protected abstract float AttackRange { get; } //공격 거리
        protected float TargetSqrMagnitudeRange => AttackRange * AttackRange;
        // event
        protected EventHub eventHub;
        // test
        public bool canMove;
        public bool canAtk;
        // SPUM Animation
        protected CharacterState state;
        [Header("Animation (SPUM)")]
        [SerializeField] protected SPUM_Prefabs spumController;
        [SerializeField] protected Transform uniRoot;
        private void OnEnable()
        {
            canMove = true;
            canAtk = true;
            isDead = false;
            isAtkCooltime = false;
            state = CharacterState.Idle;
            cm = new CharacterMove();
            cm.Init(GetComponent<Rigidbody2D>());
            if (spumController == null)
            {
                spumController = GetComponentInChildren<SPUM_Prefabs>();
            }
            if (spumController != null)
            {
                Animator anim = spumController._anim;
                if (anim != null && anim.runtimeAnimatorController != null)
                {
                    //현 애니메이터의 컨트롤러가 이미 있는지 확인(중첩 방지)
                    if (anim.runtimeAnimatorController is AnimatorOverrideController existingOverride)
                    {
                        anim.runtimeAnimatorController = existingOverride.runtimeAnimatorController;
                    }
                }
                spumController.OverrideControllerInit();
            }
        }
        public virtual void Init()
        {
            // rb = GetComponent<Rigidbody2D>();
            hp = CurrentBattleStat.maxHp;
            
            eventHub = GameManager.Instance.GetGameSystem<EventHub>();
        }
        protected void TargetSet(Character target)
        {
            this.target = target;
            targetTransform = target.transform;
        }
        protected abstract void OnDead();
        private void Update()
        {
            UpdateFeat();
            // test
            //cm.canMove = canMove;
        }
        protected abstract void UpdateFeat();
        private void FixedUpdate()
        {
            FixedUpdateFeat();
        }
        protected abstract void FixedUpdateFeat();
        protected Vector2 DirFromPosToTarget()
        {
            Vector2 thisPos = transform.position;
            Vector2 targetPos = targetTransform.position;
            return (targetPos - thisPos).normalized;
        }
        protected bool CheckTargetIsClose()
        {
            if (target == null) return false;
            Vector2 thisPos = transform.position;
            Vector2 targetPos = targetTransform.position;
            // targetPos - thisPos 거리가 TargetSqrMagnitudeRange 보다 작을 때 true
            // 사이 거리 / 판정 거리 : 판정 거리보다 짧아야 true
            return (targetPos - thisPos).sqrMagnitude <= TargetSqrMagnitudeRange;
        }
        public virtual void Hit(float damage)
        {
            float resultDmg = Mathf.Max(1 , damage - CurrentBattleStat.def);
            // Hp -= damage - CurrentBattleStat.def;
            Hp -= resultDmg;
            if (Hp <= 0)
            {
                //Debug.Log($"{gameObject.name} 죽음!");
            }
            else
            {
                Debug.Log($"{resultDmg} Damage!\n{gameObject.name} HP {Hp} 남음");
            }
            SendHitSignal();
        }
        protected abstract void SendHitSignal();
        bool IsCriticalChance()
        {
            if (UnityEngine.Random.Range(0f, 1f) < CurrentBattleStat.critChance) return true;
            return false;
        }
        protected void NormalAttack(Character target)
        {
            if (target == null || !canAtk || isDead || isAtkCooltime) return;
            
            AtkCooltimeTask().Forget();
            //Debug.Log($"{name} 이 {target.name}에게 일반공격!");
            if (spumController != null)
            {
                spumController.PlayAnimation(PlayerState.ATTACK, 0);
            }

            float resultDmg = CurrentBattleStat.atk;
            if (IsCriticalChance())
            {
                resultDmg *= CurrentBattleStat.critDamage;
                // Debug.Log("크리티컬!");
            }
            target.Hit(resultDmg);
        }

        public void SkillAttack(Character target, float multiplier)
        {
            // if (target == null|| !canAtk || isDead || isAtkCooltime) return;

            // AtkCooltimeTask().Forget();
            //Debug.Log($"{name} 이 {target.name}에게 스킬공격!");
            if(!canAtk || target == null || isDead || target.isDead)return;
            float resultDmg = CurrentBattleStat.atk * multiplier;
            if (IsCriticalChance())
            {
                resultDmg *= CurrentBattleStat.critDamage;
                // Debug.Log("크리티컬!");
            }
            target.Hit(resultDmg);
        }
        protected void UpdateFacing(float horizontalDir)
        {
            if (uniRoot == null) return;

            if (horizontalDir > 0) //오른쪽으로 이동/공격 시
            {
                uniRoot.localScale = new Vector3(-1, 1, 1);
            }
            else if (horizontalDir < 0) //왼쪽으로 이동/공격 시
            {
                uniRoot.localScale = new Vector3(1, 1, 1);
            }
        }
        async UniTaskVoid AtkCooltimeTask()
        {
            //Debug.Log("공격 쿨타임 시작");
            isAtkCooltime = true;
            float curAtkCooltime = 1; // 공격 쿨타임 추가?
            while (curAtkCooltime > 0)
            {
                //Debug.Log($"{curAtkCooltime}");
                curAtkCooltime -= Time.deltaTime * CurrentBattleStat.atkSpeed;
                await UniTask.Yield();
                if (this == null) return;
            }
            //Debug.Log("공격 쿨타임 종료");
            isAtkCooltime = false;
        }
    }
}