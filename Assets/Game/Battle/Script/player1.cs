using Base.Data;
using Cysharp.Threading.Tasks; // UniTask 사용을 위해 추가
using Personal.HagYun;
using System; // TimeSpan 사용을 위해 추가
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player1 : character1
{
    [SerializeField] private PlayerRuntimeStatus runtimeStatus;
    [SerializeField] Collider2D[] monColArr = new Collider2D[64];

    protected override BattleStat CurrentBattleStat => runtimeStatus.finalBattleStatus;
    protected override float AttackRange => runtimeStatus.finalRange;

    private bool isKnockback = false;
    private bool isDotDamage = false;

    protected override void OnDead()
    {
        if (isDead)
            return;
        isDead = true;
        Debug.Log("스테이지 실패");
        spumController.PlayAnimation(PlayerState.DEATH, 0);
        //SFXPlayer가 사망 이벤트를 구독받고 효과음 재생
    }

    protected override void Init()
    {
        runtimeStatus = GetComponent<PlayerRuntimeStatus>();
        //hp = CurrentBattleStat.maxHp;
        SyncHpAfterManagersReady().Forget();
    }
    //팀원 영역을 건드리지 않는 선에서 Player의 hp를 가져오는 로직
    async UniTaskVoid SyncHpAfterManagersReady()
    {
        // GameManager의 Start()가 실행되고 매니저들의 Init()이 끝날 때까지 넉넉히 대기
        // 보통 1~2프레임이면 충분합니다.
        await UniTask.DelayFrame(2);

        if (runtimeStatus != null)
        {
            float calculatedHp = CurrentBattleStat.maxHp;
            if (calculatedHp > 0)
            {
                hp = calculatedHp;
                Debug.Log($"매니저 계산 완료! 강화가 적용된 HP로 갱신되었습니다: {hp}");
            }
        }
    }

    /*
    public void Knockback(Vector2 direction, float force, float duration)
    {
        if (isKnockback) return;

        isKnockback = true;
        rb.AddForce(direction.normalized * force, ForceMode2D.Impulse);
        KnockbackRoutine(duration).Forget(); //넉백 도중 플레이어의 조작 봉인
    }

    // 넉백 시간 동안 기다렸다가 상태를 복구하는 비동기 함수
    private async UniTaskVoid KnockbackRoutine(float duration)
    {
        //넉백 지속 시간만큼 대기
        await UniTask.Delay(TimeSpan.FromSeconds(duration), cancellationToken: this.GetCancellationTokenOnDestroy());

        //넉백 종료: 속도 초기화 및 조작 가능 상태로 변경
        rb.velocity = Vector2.zero;
        isKnockback = false;
    }
    */

    //도트 데미지
    public void ApplyDotDamage(float totalDamage, float duration, float interval)
    {
        if (isDotDamage) return;
        DotDamageRoutine(totalDamage, duration, interval).Forget();
    }
    async UniTaskVoid DotDamageRoutine(float totalDamage, float duration, float interval)
    {
        isDotDamage = true;

        int tickCount = Mathf.FloorToInt(duration / interval);
        if (tickCount <= 0) tickCount = 1; //1회는 보장
        float damagePerTick = totalDamage / tickCount;

        float elapsed = 0f;
        int ticks = 0;
        var cts = this.GetCancellationTokenOnDestroy();

        while (elapsed < duration && ticks < tickCount)
        {
            Hit(damagePerTick);
            ticks++;
            elapsed += interval;
            await UniTask.Delay(TimeSpan.FromSeconds(interval), cancellationToken: cts);
        }

        isDotDamage = false;
    }



    protected override void FixedUpdateFeat()
    {
        if (isKnockback) return;

        FixedUpdateMoveFeat();
    }

    protected override void UpdateFeat()
    {
        if (isKnockback) return;

        UpdateMoveFeat();
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        float range = runtimeStatus != null ? runtimeStatus.finalRange : 1.5f;
        Gizmos.DrawWireSphere(transform.position, range);
    }

    void FixedUpdateMoveFeat()
    {
        cm.FixedMove();
        if (!CheckAtkRangeCollision(ref monColArr))
        {
            if (target != null)
            {
                cm.ChaseMove(DirFromPosToTarget(), CurrentBattleStat.moveSpeed);
            }
        }
    }
    void UpdateMoveFeat()
    {
        cm.UpdateMoveInput(CurrentBattleStat.moveSpeed);
        TestMoveTargetSet();
        AtkFeat();
    }
    void TestMoveTargetSet()
    {
        if (target == null && MonsterSetComponent.ins.TryGetMonster(out GameObject obj))
            target = obj.transform;
    }

    void AtkFeat()
    {
        if (!cm.IsInputMoving && CheckAtkRangeCollision(ref monColArr))
        {
            if (monColArr[0] != null)
            {
                if (CanAttack)
                    Attack(monColArr[0].GetComponent<character1>());
            }
        }
    }
}