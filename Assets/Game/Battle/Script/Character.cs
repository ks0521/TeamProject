using Base.Data;
using Base.Managers;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Battle
{
    //데미지 텍스트 결정용 타입
    public enum HitType
    {
        Normal, Critical //, Dot, HpHeal, MpHeal
    }

    public enum CharacterState
    {
        Idle, Move, Attack, Dead
    }

    public abstract class Character : MonoBehaviour
    {
        // stat
        //[SerializeField] PlayerBaseStatusSO baseStat;
        //자식 (몬스터나 플레이어)에서 전투 스탯을 구현
        public abstract BattleStat CurrentBattleStatStat { get; }
        public Transform damageAnchor;
        [SerializeField] protected float hp;

        public virtual float Hp
        {
            get => hp;
            protected set
            {
                hp = value;
                if (CurrentBattleStatStat.maxHp <= hp)
                {
                    hp = CurrentBattleStatStat.maxHp;
                }

                //cEvent.RaiseHPValueChange(hp, CurrentBattleStat.maxHp);
                if (hp <= 0f)
                {
                    OnDead();
                }
            }
        }

        public float MaxHp => CurrentBattleStatStat.maxHp;

        // battle element
        // action split class (component X)
        protected CharacterMove cm;
        [SerializeField] protected LayerMask targetLayer;
        public LayerMask TargetLayer => targetLayer; // active skill 에서 타겟 탐지를 위한 layer를 위한 getter
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

        [Header("Animation (SPUM)")] [SerializeField]
        protected SPUM_Prefabs spumController;

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
            hp = CurrentBattleStatStat.maxHp;
            eventHub = GameManager.Instance.GetGameSystem<EventHub>();
            damageAnchor = GetComponentInChildren<DamageTextMarker>()?.transform;
            //마커 없으면 기본 오브젝트 위치를 마커로 함
            if (damageAnchor == null)
            {
                damageAnchor = gameObject.transform;
            }
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

        public virtual void Hit(float damage, HitType type)
        {
            float resultDmg = Mathf.Max(1, damage - CurrentBattleStatStat.def);
            // Hp -= damage - CurrentBattleStat.def;
            SendHitSignal(resultDmg, type);
            Hp -= resultDmg;
            if (Hp <= 0)
            {
                //Debug.Log($"{gameObject.name} 죽음!");
            }
            else
            {
                Debug.Log($"{resultDmg} Damage!\n{gameObject.name} HP {Hp} 남음");
            }
        }

        protected abstract void SendHitSignal(float resultDamage, HitType type);

        bool IsCriticalChance()
        {
            if (UnityEngine.Random.Range(0f, 1f) < CurrentBattleStatStat.critChance) return true;
            return false;
        }

        protected void NormalAttack(Character hitTarget)
        {
            if (hitTarget == null || !canAtk || isDead || isAtkCooltime) return;
            HitType type = HitType.Normal;
            AtkCooltimeTask().Forget();
            //Debug.Log($"{name} 이 {target.name}에게 일반공격!");
            if (spumController != null)
            {
                spumController.PlayAnimation(PlayerState.ATTACK, 0);
            }

            float resultDmg = CurrentBattleStatStat.atk;
            if (IsCriticalChance())
            {
                type = HitType.Critical;
                resultDmg *= CurrentBattleStatStat.critDamage;

                //Debug.Log("크리티컬!");
            }

            //resultDmg *= Random.Range(0.9f, 1.1f);
            hitTarget.Hit(resultDmg, type);
        }

        public void SkillAttack(Character hitTarget, float multiplier)
        {
            if (hitTarget == null || target.isDead || !canAtk || isDead) return;
            HitType type = HitType.Normal;
            //AtkCooltimeTask().Forget();
            //Debug.Log($"{name} 이 {target.name}에게 스킬공격!");

            //float resultDmg = CurrentBattleStat.atk * (1 + multiplier);
            float resultDmg = CurrentBattleStatStat.atk * multiplier;
            if (IsCriticalChance())
            {
                type = HitType.Critical;
                resultDmg *= CurrentBattleStatStat.critDamage;
                // Debug.Log("크리티컬!");
            }
            resultDmg *= Random.Range(0.95f, 1.05f);
            hitTarget.Hit(resultDmg, type);
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
            if (isAtkCooltime) return;
            //Debug.Log("공격 쿨타임 시작");
            isAtkCooltime = true;
            float curAtkCooltime = 1; // 공격 쿨타임 추가?
            while (curAtkCooltime > 0)
            {
                //Debug.Log($"{curAtkCooltime}");
                curAtkCooltime -= Time.deltaTime * CurrentBattleStatStat.atkSpeed;
                await UniTask.Yield();
                if (this == null) return;
            }

            //Debug.Log("공격 쿨타임 종료");
            isAtkCooltime = false;
        }
    }
}