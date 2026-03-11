using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
namespace Battle
{
    public abstract class Character : MonoBehaviour
    {
        // stat
        //[SerializeField] PlayerBaseStatusSO baseStat;
        public float speed = 1;
        public float atkRange = 1;
        public float atkSpeed = 1;
        public float maxHp = 20;
        public float curHp;
        public float atk = 5;
        public float def = 2;
        public float criChance = 0.1f;
        public float criDmgPower = 1.5f;
        // get component
        [SerializeField] protected Rigidbody2D rb;

        // action class (component X)
        protected CharacterMove cm = new CharacterMove();

        // target
        [SerializeField] protected Transform moveTarget;
        [SerializeField] protected LayerMask targetLayer;

        // battle element
        [SerializeField] protected bool isAtkCooltime;
        private void Start()
        {
            InitSetting();
        }
        void InitSetting()
        {
            curHp = maxHp;
            //InitGetComponent();
            InitActionClass();
        }
        void InitGetComponent()
        {
            //rb = GetComponent<Rigidbody2D>();
        }
        void InitActionClass()
        {
            //cm.Init(rb, baseStat.baseBattle.moveSpeed);
            cm.Init(rb);
        }
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
            Vector2 targetPos = moveTarget.position;
            return (targetPos - rb.position).normalized;
        }
        protected bool CheckAtkRangeCollision(ref Collider2D[] colArr)
        {
            int cnt = Physics2D.OverlapCircleNonAlloc(transform.position, atkRange, colArr, targetLayer);
            return cnt > 0;
        }
        public void Hit(float damage)
        {
            float resultDmg = damage - def;
            curHp -= damage - def;
            if (curHp <= 0)
            {
                Destroy(gameObject);
                Debug.Log($"{gameObject.name} 죽음!");
            }
            else
            {
                Debug.Log($"{resultDmg} Damage!\n{gameObject.name} HP {curHp} 남음");
            }
        }
        protected void AtkCharacter(Character target)
        {
            AtkCooltimeTask().Forget();
            Debug.Log($"{target.name} 공격당함!");
            float resultDmg = atk;
            if(Random.Range(0f, 1f) < criChance)
            {
                resultDmg *= criDmgPower;
                Debug.Log("크리티컬!");
            }
            target.Hit(resultDmg);
        }
        async UniTaskVoid AtkCooltimeTask()
        {
            isAtkCooltime = true;
            float curAtkCooltime = 1; // 공격 쿨타임 추가?
            while (curAtkCooltime > 0)
            {
                curAtkCooltime -= Time.deltaTime * (atkSpeed);
                await UniTask.Yield();
                if (this == null) return;
            }
            isAtkCooltime = false;
        }
    }
}