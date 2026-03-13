using System.Collections;
using System.Collections.Generic;
using Base.Data;
using Personal.HagYun;
using UnityEngine;

public class player1 : character1
{
    [SerializeField]
    private PlayerRuntimeStatus runtimeStatus;
    protected override BattleStat CurrentBattleStat => runtimeStatus.finalBattleStatus;
    protected override float AttackRange => runtimeStatus.finalRange;
    [SerializeField] Collider2D[] monColArr = new Collider2D[64];
    protected override void OnDead()
    {
        if (isDead) 
            return;
        isDead = true;
        Debug.Log("스테이지 실패");
        //이후 기능 구현
    }

    protected override void Init()
    {
        runtimeStatus = GetComponent<PlayerRuntimeStatus>();
        hp = CurrentBattleStat.maxHp;
    }

    protected override void FixedUpdateFeat()
    {
        FixedUpdateMoveFeat();
    }

    protected override void UpdateFeat()
    {
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