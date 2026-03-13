using Base.Data;
using Battle;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class monster1 : character1
{
    public MonsterSO monsterSO;
    public const float MonsterAttackRange = 0.6f;
    
    protected override BattleStat CurrentBattleStat => monsterSO.battleStat;
    protected override float AttackRange => MonsterAttackRange;

    // 공격 대상의 스크립트를 미리 캐싱해둘 변수
    private character1 targetScript;

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
    protected override void Init()
    {
        hp = CurrentBattleStat.maxHp;

        if (target != null)
        {
            targetScript = target.GetComponent<character1>();
            if (targetScript == null)
            {
                Debug.LogWarning($"Target {target.name}에게 character1 컴포넌트가 없습니다.");
            }
        }
        else
        {
            // 주말 이후 타겟을 설정하는 작업 필요
            Debug.LogWarning($"{gameObject.name}의 Target이 설정되지 않았습니다.");
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
        if (target == null || targetScript == null || isDead) return;

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
