using Base.Data;
using Battle;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public class Boss : Monster
{
    public SFXPlayer sfx;
    //public MonsterSO monsterSO;
    //public const float MonsterAttackRange = 0.6f;

    public override BattleStat CurrentBattleStat => monsterSO.battleStat;
    protected override float AttackRange => MonsterAttackRange;

    // 공격 대상의 스크립트를 미리 캐싱해둘 변수
    //private Character targetScript;

    [SerializeField] private Collider2D playerCollider;

    [Header("Skill1(돌진)")]
    [SerializeField] private GameObject atkRange1;
    [SerializeField] private BoxCollider2D chargeCollider;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private float skill1CoolTime = 10f;
    [SerializeField] private float skill1WarningDuration = 1.2f;
    [SerializeField] private float skill1Damage = 15f;
    [SerializeField] private float chargeDuration = 0.6f;
    [SerializeField] private float chargeSpeed = 9f;
    //[SerializeField] private float chargeCollisionDistance = 0.75f;
    //[SerializeField] private float skill1KnockbackForce = 9.9f;
    //[SerializeField] private float skill1KnockbackDuration = 0.2f;

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
    private bool isCharging = false;
    private float currentSkill1CoolTime = 0f;
    private float currentSkill2CoolTime = 0f;
    private float currentSkill3CoolTime = 0f;

    public void InitBoss()
    {
        playerCollider = target.GetComponent<Collider2D>();
    }
    
    bool CanUseSkill(float currentCoolTime)
    {
        //Debug.Log($"CanUseSkill Check: {currentCoolTime} <= 0 ? {currentCoolTime <= 0f}");
        return currentCoolTime <= 0f;
    }

    async UniTaskVoid UseMonsterSkill1Async()
    {
        var cts = this.GetCancellationTokenOnDestroy();
        if (target == null) return;

        currentSkill1CoolTime = skill1CoolTime;
        isUsingSkill = true;
        isCharging = true;

        Vector2 directionToTarget = (target.transform.position - transform.position).normalized;
        UpdateFacing(directionToTarget.x);
        float angle = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg;
        Quaternion skillRotation = Quaternion.Euler(new Vector3(0, 0, angle));

        if (atkRange1 != null)
        {
            atkRange1.transform.rotation = skillRotation;
            atkRange1.SetActive(true);
        }
        if (chargeCollider != null) chargeCollider.transform.rotation = skillRotation;

        Debug.Log("돌진 준비 중...");
        await UniTask.Delay(TimeSpan.FromSeconds(skill1WarningDuration), cancellationToken: cts);
        if (atkRange1 != null) atkRange1.SetActive(false);
        if (chargeCollider != null) chargeCollider.enabled = true;
        //sfx.PlayBossSkillSound(); sfx 미연결로 인한 주석처리
        

        float elapsed = 0f;
        //bool hasDamaged = false;

        if (spumController != null)
        {
            spumController.PlayAnimation(PlayerState.ATTACK, 1);
        }

        /*
        if (target == null)
        {
            isUsingSkill = false;
            if (atkRange1 != null) atkRange1.SetActive(false);
            return;
        }
        */

        float elapsed = 0f;
        bool hasDamaged = false;

        while (elapsed < chargeDuration && isCharging)
        {
            if (target == null || isDead) break;

            float distance = Vector2.Distance(transform.position, target.transform.position);
            RaycastHit2D wallHit = Physics2D.BoxCast(transform.position + (Vector3)directionToTarget * chargeCollider.size.x / 2f, chargeCollider.size, 0f, directionToTarget, chargeSpeed * Time.fixedDeltaTime, wallLayer);
            if (wallHit.collider != null) //벽과 충돌한 경우
            {
                Debug.Log("벽과 충돌");
                break;
            }

            cm.ChaseMove(directionToTarget, chargeSpeed);

            if (!hasDamaged)
            {
                // 자식 콜라이더의 실제 월드 위치, 크기, 회전값을 직접 가져옵니다.
                Vector2 boxWorldPos = chargeCollider.transform.position;
                Vector2 boxSize = new Vector2(
                    chargeCollider.size.x * chargeCollider.transform.lossyScale.x,
                    chargeCollider.size.y * chargeCollider.transform.lossyScale.y
                );
                float boxAngle = chargeCollider.transform.eulerAngles.z;

                // 해당 영역에 'targetLayer'를 가진 콜라이더가 있는지 '직접' 검사
                Collider2D hitCheck = Physics2D.OverlapBox(boxWorldPos, boxSize, boxAngle, targetLayer);

                if (hitCheck != null)
                {
                    Debug.Log($"돌진 적중! 감지된 대상: {hitCheck.name}");
                    target.Hit(skill1Damage);
                    hasDamaged = true;
                }
            }
            /*
            if (chargeCollider.IsTouching(playerCollider))
            {
                Debug.Log("돌진으로 피격되었습니다.");
                SkillAttack(target,1.2f);
                //target.Hit(skill1Damage);

                // 플레이어 스크립트 가져오기 (character1을 player1로 캐스팅)
                player1 player = targetScript as player1;
                if (player != null)
                {
                    // 넉백 방향 계산 (몬스터 -> 플레이어 방향)
                    Vector2 knockbackDir = (target.position - transform.position).normalized;
                    player.Knockback(knockbackDir, skill1KnockbackForce, skill1KnockbackDuration);
                }
                //hasDamaged = true;
                //break; //충돌 후 몬스터는 이동 중단
            }
            */

            elapsed += Time.fixedDeltaTime;
            await UniTask.WaitForFixedUpdate(cancellationToken: cts); //다음 프레임까지 대기
        }
        await WaitMotion("ATTACK", cts);
        if (chargeCollider != null) chargeCollider.enabled = false;

        isCharging = false;
        isUsingSkill = false;
    }
    /*
    void RotateTowards(Vector2 direction)
    {
        // Atan2는 벡터(y, x)를 입력받아 각도(라디안)를 반환합니다.
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        // 몬스터의 스프라이트가 기본적으로 오른쪽(0도)을 보고 있다고 가정합니다.
        // 만약 위쪽을 보고 있다면 angle - 90f 등으로 보정이 필요할 수 있습니다.
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }
    */
    void OnCollisionEnter2D(Collision2D collision)
    {
        // 충돌한 오브젝트가 "Wall" 태그를 가지고 있는지 확인
        if (isUsingSkill && isCharging && collision.gameObject.layer == wallLayer)
        {
            Debug.Log("돌진 중 벽에 부딪혀 중단됨");
            isCharging = false;
            cm.MoveStop();
        }
    }

    async UniTaskVoid UseMonsterSkill2Async()
    {
        var cts = this.GetCancellationTokenOnDestroy();
        if (target == null) return;

        currentSkill2CoolTime = skill2CoolTime;
        isUsingSkill = true;
        UpdateFacing(target.transform.position.x - transform.position.x);
        Debug.Log("화염 장막 준비 중...");

        if (atkRange2 != null) atkRange2.SetActive(true);
        await UniTask.Delay(TimeSpan.FromSeconds(skill2WarningDuration), cancellationToken: cts);
        if (spumController != null)
        {
            spumController.PlayAnimation(PlayerState.ATTACK, 3);
        }
        if (atkRange2 != null)
        {
            atkRange2.SetActive(false);
            float distance = Vector2.Distance(transform.position, target.transform.position);
            if (distance <= skill2Range)
            {
                Debug.Log("화염 장막에 피격되었습니다.");
                SkillAttack(target,1.5f);
                //target.Hit(skill2Damage);

                Player player = target as Player;
                if (player != null)
                {
                    //player.ApplyDotDamage(skill2TotalDotDamage, skill2DotDuration, skill2DotInterval);
                }
            }
        }
        //sfx.PlayBossSkillSound(); sfx 연결 제한으로 인한 주석처리
        await WaitMotion("ATTACK", cts);
        isUsingSkill = false;
    }

    async UniTaskVoid UseMonsterSkill3Async()
    {
        if (target == null) return;

        currentSkill3CoolTime = skill3CoolTime;
        var cts = this.GetCancellationTokenOnDestroy();
        skillTargetPosition = target.transform.position;
        isUsingSkill = true;
        UpdateFacing(target.transform.position.x - transform.position.x);
        Debug.Log("메테오 준비 중...");

        if (atkRange3 != null)
        {
            // 예고 이펙트의 월드 좌표를 설정
            atkRange3.transform.position = skillTargetPosition;
            atkRange3.SetActive(true);
        }

        await UniTask.Delay(TimeSpan.FromSeconds(skill3WarningDuration), cancellationToken: cts);
        if (spumController != null)
        {
            spumController.PlayAnimation(PlayerState.ATTACK, 3);
        }
        if (atkRange3 != null)
        {
            atkRange3.SetActive(false);
            float distance = Vector2.Distance(skillTargetPosition, target.transform.position);
            if (distance <= skill3Range)
            {
                Debug.Log("메테오 적중! 플레이어에게 데미지");
                SkillAttack(target,2f);
                //target.Hit(skill3Damage);
            }
        }
        //sfx.PlayBossSkillSound(); sfx 미연결로 인한 주석처리
        await WaitMotion("ATTACK", cts);
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

    /*protected override void OnDead()
    {
        if (isDead) //여러번 죽지 않게하기
            return;
        isDead = true;
        
        Debug.Log("몬스터 사망");
        //rb.velocity = Vector2.zero;
        //Destroy(gameObject);
        //Killed();
        //sfx.PlayBossDeadSound();
    }*/

    void Killed()
    {
        //보상 지급과 오브젝트 풀 반환에 대한 구현. 현재는 구현할 필요 없습니다. 
    }

    /// <summary>스테이지 변경등의 이유로 사라질 때 실행</summary>
    /*
    public void ForcedReturn()
    {
        //현재는 구현할 필요 없습니다. 
        Debug.Log("오브젝트 풀에 강제 반환");
    }
    */

    //처음 생성때 초기화되는 내용(불변)
    public override void Init() //(Transform tf)
    {
        //transform이라는 타입의 변수를 만들어 target에 주입
        //FindGameObjectWithTag보다 가벼운 연산을 찾을 것
        //몬스터를 풀링하는 시점에 static에 있는 정보를 1번만 주입해 앞으로는 그 정보만 보면 되게
        //몬스터스포너or매니저에 있는 static 정보 사용
        base.Init();
        //hp = CurrentBattleStat.maxHp;
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
        {
            target = playerObj.GetComponent<Character>();
            targetTransform = playerObj.transform;
            if (target == null)
            {
                Debug.LogWarning("Player에게 Character 스크립트가 없습니다");
            }
        }
        else
        {
            Debug.LogWarning("Player를 찾을 수 없습니다!");
        }
        hp = CurrentBattleStat.maxHp;
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
            float distance = Vector2.Distance(transform.position, target.transform.position);
            if (distance <= skill2Range) UseMonsterSkill2Async().Forget();
        }
        else if (CanUseSkill(currentSkill3CoolTime)) UseMonsterSkill3Async().Forget();
    }

    protected override void FixedUpdateFeat()
    {
        // 타겟이 없거나 이미 죽었다면 아무것도 하지 않음
        if (target == null || isDead || isUsingSkill) return;
        if (cm == null) Debug.LogError("cm(CharacterMove)이 Null입니다! base.Init()을 확인하세요.");
        if (monsterSO == null) Debug.LogError("monsterSO가 Null입니다! 인스펙터를 확인하세요.");
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
    protected override void SendHitSignal()
    {
        eventHub?.MonsterHit();
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
