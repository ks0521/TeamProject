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

    public ChallengeStageRule(StageSO stage)
    {
        this.stage = stage;
        remainTime = stage.deadLine;
    }

    public override void Enter()
    {
        token = new CancellationTokenSource();
        ChallengeTimeAttack(token.Token).Forget();
    }
        
    async UniTaskVoid ChallengeTimeAttack(CancellationToken token)
    {
        Debug.Log($"{stage.chapter} - {stage.stage}(Challenge) 입장. \n" +
                  $"제한시간 : {stage.deadLine} / 목표 처치 수 : {stage.targetKillScore}");
        while (remainTime > 0)
        {
            remainTime -= Time.deltaTime;
            await UniTask.Yield(cancellationToken: token);
        }
            
        ChallengeFail?.Invoke(stage);
    }

    public override void MonsterKilled(TestMonster monster)
    {
        Debug.Log($"현재 적 {killScore} 처치 / 목표 처치 : {stage.targetKillScore}");
        if (++killScore > stage.targetKillScore)
        {
            Debug.Log("목표처치 달성");
            ChallengeSuccess?.Invoke(stage);
        }
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

    public NormalStageRule(StageSO stage)
    {
        this.stage = stage;
    }

    public override void Enter()
    {
        
    }

    public override void MonsterKilled(TestMonster monster)
    {
        ++killScore;
        List<DropedItem> items =
            stage.dropTable.GetDroppedItems(PlayerRuntimeStatus.Instance.finalRewardStatus.itemDropRateBonus);
        Debug.Log($"{items.Count}종 아이템 드랍");
    }

    public override void Destroy()
    {
        
    }
}

