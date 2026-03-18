using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using Base.Data;
using Battle;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class monster1 : character1
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
    [SerializeField] private float chargePrepareTime = 1.2f;
    [SerializeField] private float chargeDuration = 0.6f;
    [SerializeField] private float chargeSpeed = 9f;

    [Header("Skill2(화염 장막)")]
    [SerializeField] private GameObject atkRange2;
    [SerializeField] private float skill2WarningDuration = 1.5f;
    [SerializeField] private float skill2Range = 4.0f;

    [Header("Skill3(메테오)")]
    [SerializeField] private GameObject atkRange3;
    [SerializeField] private float skill3warningDuration = 1.5f;
    [SerializeField] private float skill3Range = 3.0f;

    private Vector3 skillTargetPosition; //시전 시점의 플레이어 위치
    private bool isUsingSkill = false;

    public void Update()
    {
        //Q: 예고 후 플레이어를 향한 돌진
        //W: 예고 후 원형 범위 데미지
        //E: 시전 시점의 플레이어 위치에 예고 후 메테오
        if (Input.GetKeyDown(KeyCode.Q)) UseMonsterSkill1Async().Forget();
        if (Input.GetKeyDown(KeyCode.W)) UseMonsterSkill2Async().Forget();
        if (Input.GetKeyDown(KeyCode.E)) UseMonsterSkill3Async().Forget();
    }

    async UniTaskVoid UseMonsterSkill1Async()
    {
        // 안전장치: 이 오브젝트가 파괴되면 비동기 작업도 취소하기 위한 토큰을 가져옵니다.
        var cts = this.GetCancellationTokenOnDestroy();

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
        await UniTask.Delay(TimeSpan.FromSeconds(chargePrepareTime), cancellationToken: cts);

        if (atkRange1 != null) atkRange1.SetActive(false);
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

            if (!hasDamaged && Vector2.Distance(transform.position, target.position) < 0.3f)
            {
                Debug.Log("돌진으로 피격되었습니다.");
                hasDamaged = true;
                Attack(targetScript);
            }

            elapsed += Time.fixedDeltaTime;
            // 다음 물리 프레임까지 대기 (이게 있어야 부드럽게 이동함)
            await UniTask.WaitForFixedUpdate(cancellationToken: cts);
        }

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
        if (target == null || targetScript == null) return;

        var cts = this.GetCancellationTokenOnDestroy();
        isUsingSkill = true;
        Debug.Log("화염 장막 준비 중...");

        if (atkRange2 != null) atkRange2.SetActive(true);
        await UniTask.Delay(TimeSpan.FromSeconds(1.5f), cancellationToken: cts);
        if (atkRange2 != null)
        {
            atkRange2.SetActive(false);
            float distance = Vector2.Distance(transform.position, target.position);
            if (distance <= skill2Range)
            {
                Debug.Log("화염 장막에 피격되었습니다.");
                Attack(targetScript);
            }
        }
        
        sfx.PlayBossSkillSound();
        isUsingSkill = false;
    }

    async UniTaskVoid UseMonsterSkill3Async()
    {
        if (target == null) return;

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

        await UniTask.Delay(TimeSpan.FromSeconds(skill2WarningDuration), cancellationToken: cts);
        if (atkRange3 != null)
        {
            atkRange3.SetActive(false);
            float distance = Vector2.Distance(skillTargetPosition, target.position);
            if (distance <= skill3Range)
            {
                Debug.Log("메테오 적중! 플레이어에게 데미지");
                Attack(targetScript);
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
}
