using Base.Data;
using Battle;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections;

public class monster1 : character1
{
    public MonsterSO monsterSO;
    //public SFXPlayer sfx;
    public const float MonsterAttackRange = 0.6f;

    protected override BattleStat CurrentBattleStat => monsterSO.battleStat;
    protected override float AttackRange => MonsterAttackRange;

    // 공격 대상의 스크립트를 미리 캐싱해둘 변수
    private character1 targetScript;

    [Header("Skill1 Settings")]
    [SerializeField] private GameObject atkRange1;
    [SerializeField] private float chargePrepareTime = 1.2f;
    [SerializeField] private float chargeDuration = 0.3f;
    [SerializeField] private float chargeSpeed = 0.5f;

    [Header("Skill2 Settings")]
    [SerializeField] private GameObject atkRange2;

    private bool isUsingSkill = false;

    public void Update()
    {
        //1번: 예고 후 플레이어를 향한 돌진
        //2번: 예고 후 원형 범위 데미지
        if(Input.GetKeyDown(KeyCode.Q))
        {
            UseMonsterSkill1();
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            UseMonsterSkill2();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            UseMonsterSkill3();
        }
    }

    IEnumerator MonsterSkillCo1()
    {
        isUsingSkill = true;
        Debug.Log("돌진 준비 중...");

        atkRange1.SetActive(true);

        Vector2 directionToTarget = (target.position - transform.position).normalized;
        RotateTowards(directionToTarget);
        yield return new WaitForSeconds(chargePrepareTime);

        atkRange1.SetActive(false);
        //실제 스킬 이펙트 구현할 자리
        //sfx.PlayBossAttackSound();
        float elapsed = 0f;
        while (elapsed < chargeDuration)
        {
            // CharacterMove에 있는 방향 기반 이동 함수를 재활용합니다.
            // 이렇게 하면 CharacterMove를 수정하지 않고도 물리 기반 이동이 가능합니다.
            // 중요: 물리 이동이므로 WaitForFixedUpdate와 짝을 맞춰야 합니다.
            cm.ChaseMove(directionToTarget, chargeSpeed);

            elapsed += Time.fixedDeltaTime;
            // 다음 물리 프레임까지 대기 (이게 있어야 부드럽게 이동함)
            yield return new WaitForFixedUpdate();
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
    IEnumerator MonsterSkillCo2()
    {
        isUsingSkill = true;
        Debug.Log("범위공격 준비 중...");

        atkRange2.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        atkRange2.SetActive(false);
        //실제 스킬 이펙트 구현할 자리
        //sfx.PlayBossSkillSound();

        isUsingSkill = false;
    }

    public void UseMonsterSkill1()
    {
        StartCoroutine(MonsterSkillCo1());
    }
    public void UseMonsterSkill2()
    {
        StartCoroutine(MonsterSkillCo2());
    }
    public void UseMonsterSkill3()
    {
        StartCoroutine(MonsterSkillCo3());
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
    //추가사항으로, 보스 몬스터 공격 3가지 
    //1. 보스 중심으로 일정 범위 경고 후 데미지
    //2. 스킬 시전 시 플레이어 위치기준 일정 범위 경고 후 데미지
    //3. 플레이어 위치 기준 일정범위 경고 후 돌진
}
