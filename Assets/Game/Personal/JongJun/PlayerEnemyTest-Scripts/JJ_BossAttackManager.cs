using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using Base.Data;
using Battle;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class JJ_BossAttackManager : character1
{
    public MonsterSO monsterSO;
    public SFXPlayer sfx;
    public const float MonsterAttackRange = 0.6f;

    protected override BattleStat CurrentBattleStat => monsterSO.battleStat;
    protected override float AttackRange => MonsterAttackRange;

    // 공격 대상의 스크립트를 미리 캐싱해둘 변수
    private character1 targetScript;

    [SerializeField] private Collider2D playerCollider;

    [Header("Skill1(돌진)")]
    [SerializeField] private GameObject atkRange1;
    [SerializeField] private BoxCollider2D chargeCollider;
    [SerializeField] private float skill1CoolTime = 10f;
    [SerializeField] private float skill1WarningDuration = 1.2f;
    [SerializeField] private float skill1Damage = 15f;
    [SerializeField] private float chargeDuration = 0.6f;
    [SerializeField] private float chargeSpeed = 9f;
    //[SerializeField] private float chargeCollisionDistance = 0.75f;
    [SerializeField] private float skill1KnockbackForce = 9.9f;
    [SerializeField] private float skill1KnockbackDuration = 0.2f;

    [Header("Skill2(화염 장막)")]
    [SerializeField] private GameObject atkRange2;
    [SerializeField] private float skill2CoolTime = 15f;
    [SerializeField] private float skill2WarningDuration = 1.5f;
    [SerializeField] private float skill2Damage = 10f;
    [SerializeField] private float skill2Range = 2.0f;
    [SerializeField] private float skill2TotalDotDamage = 10f;
    [SerializeField] private float skill2DotDuration = 5f;
    [SerializeField] private float skill2DotInterval = 0.5f;

    [Header("Skill3(메테오)")]
    [SerializeField] private GameObject atkRange3;
    [SerializeField] private float skill3CoolTime = 12f;
    [SerializeField] private float skill3WarningDuration = 1.5f;
    [SerializeField] private float skill3Damage = 33f;
    [SerializeField] private float skill3Range = 4.0f;

    private Vector3 skillTargetPosition; //시전 시점의 플레이어 위치
    private bool isUsingSkill = false;
    private float currentSkill1CoolTime = 0f;
    private float currentSkill2CoolTime = 0f;
    private float currentSkill3CoolTime = 0f;

    bool CanUseSkill(float currentCoolTime)
    {
        Debug.Log($"CanUseSkill Check: {currentCoolTime} <= 0 ? {currentCoolTime <= 0f}");
        return currentCoolTime <= 0f;
    }

    async UniTaskVoid UseMonsterSkill1Async()
    {
        // 안전장치: 이 오브젝트가 파괴되면 비동기 작업도 취소하기 위한 토큰을 가져옵니다.
        var cts = this.GetCancellationTokenOnDestroy();
        if (target == null || targetScript == null) return;

        currentSkill1CoolTime = skill1CoolTime;
        isUsingSkill = true;
        Debug.Log("돌진 준비 중...");

        if (atkRange1 != null) atkRange1.SetActive(true);

        // 타겟이 도중에 사라졌을 경우를 대비한 방어 코드
        if (target == null)
        {
            isUsingSkill = false;
            if (atkRange1 != null) atkRange1.SetActive(false);
            return;
        }

        Vector2 directionToTarget = (target.position - transform.position).normalized;
        RotateTowards(directionToTarget);
        await UniTask.Delay(TimeSpan.FromSeconds(skill1WarningDuration), cancellationToken: cts);

        if (atkRange1 != null) atkRange1.SetActive(false);
        if (chargeCollider != null) chargeCollider.enabled = true;
        sfx.PlayBossSkillSound();

        float elapsed = 0f;
        bool hasDamaged = false;

        while (elapsed < chargeDuration)
        {
            if (target == null || isDead) break;
            // CharacterMove에 있는 방향 기반 이동 함수를 재활용합니다.
            // 이렇게 하면 CharacterMove를 수정하지 않고도 물리 기반 이동이 가능합니다.
            // 중요: 물리 이동이므로 WaitForFixedUpdate와 짝을 맞춰야 합니다.
            cm.ChaseMove(directionToTarget, chargeSpeed);
            float distance = Vector2.Distance(transform.position, target.position);

            if (chargeCollider.IsTouching(playerCollider))
            {
                Debug.Log("돌진으로 피격되었습니다.");
                targetScript.Hit(skill1Damage);

                // 플레이어 스크립트 가져오기 (character1을 player1로 캐스팅)
                player1 player = targetScript as player1;
                if (player != null)
                {
                    // 넉백 방향 계산 (몬스터 -> 플레이어 방향)
                    Vector2 knockbackDir = (target.position - transform.position).normalized;
                    player.Knockback(knockbackDir, skill1KnockbackForce, skill1KnockbackDuration);
                }
                hasDamaged = true;
                break; //충돌 후 몬스터는 이동 중단
            }
            
            elapsed += Time.fixedDeltaTime;
            await UniTask.WaitForFixedUpdate(cancellationToken: cts); //다음 프레임까지 대기
        }
        if (chargeCollider != null) chargeCollider.enabled = false;
        isUsingSkill = false;
    }
    private void RotateTowards(Vector2 direction)
    {
        // Atan2는 벡터(y, x)를 입력받아 각도(라디안)를 반환합니다.
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        // 몬스터의 스프라이트가 기본적으로 오른쪽(0도)을 보고 있다고 가정합니다.
        // 만약 위쪽을 보고 있다면 angle - 90f 등으로 보정이 필요할 수 있습니다.
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    async UniTaskVoid UseMonsterSkill2Async()
    {
        var cts = this.GetCancellationTokenOnDestroy();
        if (target == null || targetScript == null) return;

        currentSkill2CoolTime = skill2CoolTime;
        isUsingSkill = true;
        Debug.Log("화염 장막 준비 중...");

        if (atkRange2 != null) atkRange2.SetActive(true);
        await UniTask.Delay(TimeSpan.FromSeconds(skill2WarningDuration), cancellationToken: cts);
        if (atkRange2 != null)
        {
            atkRange2.SetActive(false);
            float distance = Vector2.Distance(transform.position, target.position);
            if (distance <= skill2Range)
            {
                Debug.Log("화염 장막에 피격되었습니다.");
                targetScript.Hit(skill2Damage);

                player1 player = targetScript as player1;
                if (player != null)
                {
                    player.ApplyDotDamage(skill2TotalDotDamage, skill2DotDuration, skill2DotInterval);
                }
            }
        }

        sfx.PlayBossSkillSound();
        isUsingSkill = false;
    }

    async UniTaskVoid UseMonsterSkill3Async()
    {
        if (target == null || targetScript == null) return;

        currentSkill3CoolTime = skill3CoolTime;
        var cts = this.GetCancellationTokenOnDestroy();
        skillTargetPosition = target.position;
        isUsingSkill = true;
        Debug.Log("메테오 준비 중...");

        if (atkRange3 != null)
        {
            // 예고 이펙트의 월드 좌표를 설정
            atkRange3.transform.position = skillTargetPosition;
            atkRange3.SetActive(true);
        }

        await UniTask.Delay(TimeSpan.FromSeconds(skill3WarningDuration), cancellationToken: cts);
        if (atkRange3 != null)
        {
            atkRange3.SetActive(false);
            float distance = Vector2.Distance(skillTargetPosition, target.position);
            if (distance <= skill3Range)
            {
                Debug.Log("메테오 적중! 플레이어에게 데미지");
                targetScript.Hit(skill3Damage);
            }
        }

        sfx.PlayBossSkillSound();
        isUsingSkill = false;
    }

    public void UseMonsterSkill1()
    {
        UseMonsterSkill1Async().Forget();
    }
    public void UseMonsterSkill2()
    {
        UseMonsterSkill2Async().Forget();
    }
    public void UseMonsterSkill3()
    {
        UseMonsterSkill3Async().Forget();
    }

    protected override void OnDead()
    {
        if (isDead) //여러번 죽지 않게하기
            return;
        isDead = true;
        Debug.Log("몬스터 사망");
        rb.velocity = Vector2.zero;
        Destroy(gameObject);
        //Killed();
    }

    void Killed()
    {
        //보상 지급과 오브젝트 풀 반환에 대한 구현. 현재는 구현할 필요 없습니다. 
    }

    /// <summary>스테이지 변경등의 이유로 사라질 때 실행</summary>
    public void ForcedReturn()
    {
        //현재는 구현할 필요 없습니다. 
        Debug.Log("오브젝트 풀에 강제 반환");
    }
    //처음 생성때 초기화되는 내용(불변)
    protected override void Init() //(Transform tf)
    {
        //transform이라는 타입의 변수를 만들어 target에 주입
        //FindGameObjectWithTag보다 가벼운 연산을 찾을 것
        //몬스터를 풀링하는 시점에 static에 있는 정보를 1번만 주입해 앞으로는 그 정보만 보면 되게
        //몬스터스포너or매니저에 있는 static 정보 사용


        hp = CurrentBattleStat.maxHp;
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        //ㄴ이름으로 찾으니 오타 주의

        if (playerObj != null)
        {
            //target = tf;
            target = playerObj.transform;
            targetScript = playerObj.GetComponent<character1>();
            if (targetScript == null)
            {
                Debug.LogWarning($"Target {target.name}에게 character1 스크립트가 없습니다");
            }
        }
        else
        {
            Debug.LogWarning("Player를 찾을 수 없습니다!");
        }
        ///[summary] 이전 코드
        ///var playerObj = GameObject.FindGameObjectWithTag("Player");
        ///if (playerObj != null) target = playerObj.transform;
        ///[/summary]
    }

    protected override void UpdateFeat()
    {
        //if (Input.GetKeyDown(KeyCode.Q)) UseMonsterSkill1Async().Forget();
        //if (Input.GetKeyDown(KeyCode.W)) UseMonsterSkill2Async().Forget();
        //if (Input.GetKeyDown(KeyCode.E)) UseMonsterSkill3Async().Forget();

        //체력 40% = 돌진(30% 이상)과 화염 장막(50% 이하) 조건 모두 만족
        if (Input.GetKeyDown(KeyCode.H))
        {
            hp = CurrentBattleStat.maxHp * 0.4f;
            Debug.Log($"보스 체력: {Hp} / {CurrentBattleStat.maxHp}");
        }

        if (currentSkill1CoolTime > 0) currentSkill1CoolTime = Mathf.Max(0, currentSkill1CoolTime - Time.deltaTime);
        if (currentSkill2CoolTime > 0) currentSkill2CoolTime = Mathf.Max(0, currentSkill2CoolTime - Time.deltaTime);
        if (currentSkill3CoolTime > 0) currentSkill3CoolTime = Mathf.Max(0, currentSkill3CoolTime - Time.deltaTime);

        if (target == null || isUsingSkill || isDead) return;

        //돌진: 체력 30% 이상일 때만 발동 가능
        //화염 장막: 체력 50% 이하, 범위 내에 플레이어가 있을 때만 발동 가능
        if (CanUseSkill(currentSkill1CoolTime) && hp >= CurrentBattleStat.maxHp * 0.3) UseMonsterSkill1Async().Forget();
        else if (CanUseSkill(currentSkill2CoolTime) && hp <= CurrentBattleStat.maxHp * 0.5)
        {
            float distance = Vector2.Distance(transform.position, target.position);
            if (distance <= skill2Range) UseMonsterSkill2Async().Forget();
        }
        else if (CanUseSkill(currentSkill3CoolTime)) UseMonsterSkill3Async().Forget();
    }

    protected override void FixedUpdateFeat()
    {
        // 타겟이 없거나 이미 죽었다면 아무것도 하지 않음
        if (target == null || targetScript == null || isDead || isUsingSkill) return;

        // 거리 계산
        float distanceToTarget = Vector2.Distance(transform.position, target.position);

        if (distanceToTarget <= AttackRange)
        {
            // 사거리 내: 이동을 멈추고 공격 시도
            Attack(targetScript);
        }
        else
        {
            cm.ChaseMove(target, CurrentBattleStat.moveSpeed);
        }
    }

    void OnDrawGizmosSelected()
    {
        //공격 사거리(MonsterAttackRange)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, MonsterAttackRange);

        /*
        //스킬 1: 돌진 범위(실제 충돌 범위는 정사각형)
        Gizmos.color = new Color(0f, 0.5f, 1f); //하늘색
        Gizmos.DrawWireSphere(transform.position, chargeCollisionDistance);
        */

        //스킬 2: 화염 장막 범위
        Gizmos.color = new Color(1f, 0.5f, 0f); //주황색
        Gizmos.DrawWireSphere(transform.position, skill2Range);
    }
}
