using Base.Data;
using Base.Managers;
using System;
using System.Collections.Generic;
using System.Threading;
using Battle;
using Cysharp.Threading.Tasks;
using Personal.GyuSeong;
using UnityEngine;

[Serializable]
public abstract class StageRule
{
    [SerializeField] protected StageSO stage;
    [SerializeField] protected int killScore;
    protected ItemDropManager dropManager;
    protected EventHub eventHub;

    public virtual void Init(StageSO stage)
    {
        dropManager = GameManager.Instance.GetGameSystem<ItemDropManager>();
        eventHub = GameManager.Instance.GetGameSystem<EventHub>();
        this.stage = stage;
    }
    public abstract void Enter();
    public abstract void MonsterKilled(TestMonster monster);
    public abstract void Destroy();
}


[Serializable]
public class ChallengeStageRule : StageRule
{
    [SerializeField] private float remainTime;
    public event Action<StageSO> ChallengeSuccess;
    public event Action<StageSO> ChallengeFail;
    private CancellationTokenSource token;
    
    public override void Init(StageSO stage)
    {
        base.Init(stage);
        remainTime = stage.deadLine;
    }

    public override void Enter()
    {
        Debug.Log($"{stage.chapter} - {stage.stage}(Challenge) StageRule 시작. \n" +
                  $"제한시간 : {stage.deadLine} / 목표 처치 수 : {stage.targetKillScore}");
        dropManager = GameManager.Instance.GetGameSystem<ItemDropManager>();
        token = new CancellationTokenSource();
        ChallengeTimeAttack(token.Token).Forget();
    }
        
    async UniTaskVoid ChallengeTimeAttack(CancellationToken token)
    {
        while (remainTime > 0)
        {
            remainTime -= Time.deltaTime;
            await UniTask.Yield(cancellationToken: token);
        }
            
        ChallengeFail?.Invoke(stage);
    }

    public override void MonsterKilled(TestMonster monster)
    {
        if (++killScore >= stage.targetKillScore)
        {
            Debug.Log("목표처치 달성");
            ChallengeSuccess?.Invoke(stage);
            return;
        }
        Debug.Log($"현재 적 {killScore} 처치 / 목표 처치 : {stage.targetKillScore}");
    }

    public override void Destroy()
    {
        token.Cancel();
        token.Dispose();
    }
}
[Serializable]
public class NormalStageRule : StageRule
{
    public override void Enter()
    {
        Debug.Log($"일반 스테이지{stage.chapter} - {stage.stage} StageRule 시작");
    }

    public override void MonsterKilled(TestMonster monster)
    {
        ++killScore;
        //몬스터 처치에 대한 기타 작동기전 구현
        ItemDrop();
    }

    public void ItemDrop()
    {
        List<DropedItem> items =
            stage.dropTable.GetDroppedItems(PlayerRuntimeStatus.Instance.finalRewardStatus.itemDropRateBonus);
        dropManager.GetGold(stage.dropTable.rewardGold);
        dropManager.GetStatStone(stage.dropTable.rewardStatStone);
        dropManager.GetExp(stage.dropTable.rewardExp);
        Debug.Log($"{items.Count}종 아이템 드랍");
    }
    public override void Destroy()
    {
        
    }
}

