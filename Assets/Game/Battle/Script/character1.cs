using System;
using System.Threading;
using Base.Data;
using Battle;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

public abstract class character1 : MonoBehaviour
{
    public float hp;
    public float Hp
    {
        get => hp;
        set
        {
            hp = value;
            if (hp <= 0)
            {
                OnDead();
            }
        }
    }

    protected bool isDead; //객체의 사망 확인
    protected Rigidbody2D rb; 
    protected CharacterMove cm; //커스텀 클래스
    protected Transform target; //대상의 위치
    [SerializeField]protected LayerMask targetLayer; //대상 레이어마스크
    protected bool CanAttack; //지금 공격 가능한지(가능하면 true / 안되면 false )
    protected abstract BattleStat CurrentBattleStat { get; } //자식 (몬스터나 플레이어에서 전투 스탯을 구현)
    protected abstract float AttackRange { get; } //공격 거리
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        cm = new CharacterMove();
        cm.Init(rb);
    }
    
    private void OnEnable()
    {
        CanAttack = true;
        isDead = false;
        Init();
    }

    private void Update()
    {
        UpdateFeat();
    }
    private void FixedUpdate()
    {
        FixedUpdateFeat();
    }

    protected abstract void OnDead();
    protected abstract void Init();
    protected abstract void UpdateFeat();
    protected abstract void FixedUpdateFeat();



    protected Vector2 DirFromPosToTarget()
    {
        Vector2 targetPos = target.position;
        return (targetPos - rb.position).normalized;
    }

    protected bool CheckAtkRangeCollision(ref Collider2D[] colArr)
    {
        int cnt = Physics2D.OverlapCircleNonAlloc(transform.position, AttackRange, colArr, targetLayer);
        return cnt > 0;
    }

    public void Hit(float damage)
    {
        float finalDmg = Mathf.Max(1f, damage - CurrentBattleStat.def);
        Hp -= finalDmg;
        
        Debug.Log($"{finalDmg} Damage!\n{gameObject.name} HP : {Hp} 남음");
    }

    protected void Attack(character1 target)
    {
        if (!CanAttack || target == null) return;
        Debug.Log($"{gameObject.name}가 {target.name} 공격함!");
        float resultDmg = CurrentBattleStat.atk;
        if (Random.Range(0f, 1f) < CurrentBattleStat.critChance)
        {
            resultDmg *= CurrentBattleStat.critDamage;
            Debug.Log($"크리티컬! {CurrentBattleStat.critDamage}배의 피해!");
        }
        target.Hit(resultDmg);
        AtkCooltimeTask(CurrentBattleStat.atkSpeed,this.GetCancellationTokenOnDestroy()).Forget();
    }
    /// <summary> 공격 쿨타임 구현 유니태스크</summary>
    /// <param name="duration"> 정지 시간(초)</param>
    async UniTaskVoid AtkCooltimeTask(float duration, CancellationToken token)
    {
        CanAttack = false;
        await UniTask.Delay(TimeSpan.FromSeconds(duration),cancellationToken: token);
        CanAttack = true;
    }
}