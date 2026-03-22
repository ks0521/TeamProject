using Base.Data;
using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Battle
{
    public class CharacterCommonEvent
    {
        public event Action<Character> OnDead;
        public event Action<float, float> OnHPValueChange;
        public void RaiseDead(Character cha) => OnDead?.Invoke(cha);
        public void RaiseHPValueChange(float curHp, float maxHp) => OnHPValueChange?.Invoke(curHp, maxHp);
    }
    public abstract class Character : MonoBehaviour
    {
        // stat
        //[SerializeField] PlayerBaseStatusSO baseStat;
        //자식 (몬스터나 플레이어)에서 전투 스탯을 구현
        protected abstract BattleStat CurrentBattleStat { get; }
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
                cEvent.RaiseHPValueChange(hp, CurrentBattleStat.maxHp);
                if (hp <= 0f)
                {
                    OnDead();
                }
            }
        }
        // get component
        protected Rigidbody2D rb;


        // action class (component X)
        protected CharacterMove cm;

        // target
        //[SerializeField] protected Transform moveTarget;
        [SerializeField] protected LayerMask targetLayer;

        // battle element
        protected Character target;
        [SerializeField] protected bool isAtkCooltime;
        protected bool isDead;
        protected abstract float AttackRange { get; } //공격 거리
        protected float TargetSqrMagnitudeRange => AttackRange * AttackRange;
        // event
        protected CharacterCommonEvent cEvent;
        private void Awake()
        {
            AwakeInit();
        }
        private void OnEnable()
        {
            OnEnableInit();
        }
        private void Start()
        {
            StartInit();
        }
        /// <summary> 적합 시점 : Awake - 처음 / Component, 일반 클래스 등 할당 (GetComponent, new 등) </summary>
        protected virtual void AwakeInit()
        {
            rb = GetComponent<Rigidbody2D>();
            cm = new CharacterMove();
            cEvent = new CharacterCommonEvent();
        }
        /// <summary> 적합 시점 : OnEnable - MemberInit 실행 후 및 비활성화 된 몬스터 등이 새로운 스탯을 부여받을 때 / Character class의 멤버 변수에 값 할당 </summary>
        protected virtual void OnEnableInit()
        {
            hp = CurrentBattleStat.maxHp;
            isAtkCooltime = false;
        }
        /// <summary> 적합 시점 : Start 등 MemberInit 실행 후 / Component, 일반 클래스 등의 Init 실행 </summary>
        protected virtual void StartInit()
        {
            cm.Init(rb);
            //targetSqrMagnitudeRange = AttackRange * AttackRange;
        }
        protected abstract void OnDead();
        private void Update()
        {
            UpdateFeat();
        }
        protected abstract void UpdateFeat();
        private void FixedUpdate()
        {
            FixedUpdateFeat();
        }
        protected abstract void FixedUpdateFeat();
        protected Vector2 DirFromPosToTarget()
        {
            //Vector2 targetPos = moveTarget.position;
            Vector2 targetPos = target.transform.position;
            return (targetPos - rb.position).normalized;
        }
        protected bool CheckAtkRangeCollision(ref Collider2D[] colArr)
        {
            int cnt = Physics2D.OverlapCircleNonAlloc(transform.position, AttackRange, colArr, targetLayer);
            return cnt > 0;
        }
        protected bool CheckTargetIsClose()
        {
            if (target == null) return false;
            Vector2 thisPos = transform.position;
            Vector2 targetPos = target.transform.position;
            // targetPos - thisPos 거리가 TargetSqrMagnitudeRange 보다 작을 때 true
            // 사이 거리 / 판정 거리 : 판정 거리보다 짧아야 true
            return (targetPos - thisPos).sqrMagnitude <= TargetSqrMagnitudeRange;
        }
        public void Hit(float damage)
        {
            float resultDmg = damage - CurrentBattleStat.def;
            Hp -= damage - CurrentBattleStat.def;
            if (Hp <= 0)
            {
                Destroy(gameObject);
                Debug.Log($"{gameObject.name} 죽음!");
            }
            else
            {
                Debug.Log($"{resultDmg} Damage!\n{gameObject.name} HP {Hp} 남음");
            }
        }
        bool IsCriticalChance()
        {
            if (UnityEngine.Random.Range(0f, 1f) < CurrentBattleStat.critChance) return true;
            return false;
        }
        protected void NormalAttack(Character target)
        {
            if (target == null) return;
            AtkCooltimeTask().Forget();
            Debug.Log($"{name} 이 {target.name}에게 일반공격!");
            float resultDmg = CurrentBattleStat.atk;
            if(IsCriticalChance())
            {
                resultDmg *= CurrentBattleStat.critDamage;
                Debug.Log("크리티컬!");
            }
            target.Hit(resultDmg);
        }
        public float SkillResultDmg(float increasePower)
        {
            float resultDmg = CurrentBattleStat.atk;
            resultDmg *= increasePower;
            if(IsCriticalChance())
            {
                resultDmg *= CurrentBattleStat.critDamage;
            }
            return resultDmg;
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

        public void AddEventDead(Action<Character> func) => cEvent.OnDead += func;
        public void RemoveEventDead(Action<Character> func) => cEvent.OnDead -= func;
        public void AddEventHPValueChange(Action<float, float> func) => cEvent.OnHPValueChange += func;
        public void RemoveEventHPValueChange(Action<float, float> func) => cEvent.OnHPValueChange -= func;
    }
}