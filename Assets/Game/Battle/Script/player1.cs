using System.Collections;
using System.Collections.Generic;
using Base.Data;
using Personal.HagYun;
using UnityEngine;

public class player1 : character1
{
    
    [SerializeField] Collider2D[] monColArr = new Collider2D[64];
    protected override BattleStat CurrentBattleStat { get; }
    protected override float AttackRange { get; }

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
        hp = 100;
    }

    protected override void FixedUpdateFeat()
    {
    }

    protected override void UpdateFeat()
    {
    }

}
