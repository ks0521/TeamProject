using Base.Data;
using Base.Managers;
using Battle;
using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Growth.Currency;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public abstract class StageRule
{
    protected StageSO stage;
    protected int killScore;
    public int KillScore => killScore;
    protected ItemDropManager dropManager;
    protected EventHub eventHub;

    public StageRule(StageSO stage)
    {
        dropManager = GameManager.Instance.GetGameSystem<ItemDropManager>();
        eventHub = GameManager.Instance.GetGameSystem<EventHub>();
        this.stage = stage;
    }

    public abstract void Enter();
    public abstract void MonsterKilledInStage(Monster monster); //몬스터 사망시 스테이지 룰별 실행되는 메서드 
    public abstract void Destroy();
}
#region 일반 스테이지 
[Serializable]
public class NormalStageRule : StageRule
{
    private ItemPoolManager itemPool;
    private PlayerManager player;

    public NormalStageRule(StageSO stage) : base(stage)
    {
        itemPool = GameManager.Instance.GetGameSystem<ItemPoolManager>();
        player = GameManager.Instance.GetGameSystem<PlayerManager>();
    }

    public override void Enter()
    {
        Debug.Log($"일반 스테이지{stage.chapter} - {stage.stage} StageRule 시작");
    }

    public override void MonsterKilledInStage(Monster monster)
    {
        ++killScore;
        //몬스터 처치에 대한 기타 작동기전 구현
        ItemDrop(monster);
    }

    public void ItemDrop(Monster monster)
    {
        List<DropReward> items =
            stage.dropTable.GetDroppedItems(RuntimeStatus.Instance.finalRewardStatStatus.itemDropRate);
        dropManager.GetExp(stage.dropTable.GetExp());
        foreach (var item in items)
        {
            GameObject dropItem = itemPool.UsePool();
            dropItem.GetComponent<DroppedItem>().Init(item, player.transform, itemPool);
            float randX = monster.transform.position.x + Random.Range(-0.5f, 0.5f);
            float randY = monster.transform.position.y + Random.Range(-0.5f, 0.5f);
            dropItem.transform.position = new Vector3(randX, randY, 0);
            dropItem.SetActive(true);
        }
    }

    public override void Destroy()
    {
    }
}
#endregion

#region 도전 스테이지
[Serializable]
public class ChallengeStageRule : StageRule
{
    public bool isCleared;
    public event Action ChallengeSuccess;
    public event Action ChallengeFail;
    public float RemainTime => remainTime;
    protected float remainTime;
    protected CancellationTokenSource token;
    
    public ChallengeStageRule(StageSO stage) : base(stage)
    {
        remainTime = stage.deadLine;
        isCleared = false;
    }

    public override void Enter()
    {
        token = new CancellationTokenSource();
        ChallengeTimeAttack(token.Token).Forget();
    }

    protected void StageClear()
    {
        if (isCleared) return;
        isCleared = true;
        if (stage.rewardTable != null)
        {
            dropManager.GetRewards(stage.rewardTable);
            eventHub.GetClearRewards(stage.rewardTable);
        }
        ChallengeSuccess?.Invoke();
    }
    protected void StageFail()
    {
        ChallengeFail?.Invoke();
    }

    protected async UniTaskVoid ChallengeTimeAttack(CancellationToken token, bool timeOverIsFail = true)
    {
        while (remainTime > 0)
        {
            remainTime -= Time.deltaTime;
            await UniTask.Yield(cancellationToken: token);
        }
        if (timeOverIsFail)
        {
            StageFail();
        }
        else
        {
            StageClear();
        }
    }
    public override void MonsterKilledInStage(Monster monster)
    {
        if (++killScore >= stage.targetKillScore)
        {
            Debug.Log("목표처치 달성");
            StageClear();
            return;
        }
        Debug.Log($"현재 적 {killScore} 처치 / 목표 처치 : {stage.targetKillScore}");
    }
    public override void Destroy()
    {
        token?.Cancel();
        token?.Dispose();
    }
    
}
/// <summary> 특정 킬 수를 달성하면 클리어되는 스테이지</summary>
[Serializable]
public class KillCount : ChallengeStageRule
{
    public KillCount(StageSO stage) : base(stage){}
}
/// <summary> 도전 스테이지 중 보스를처치하면 클리어되는 스테이지</summary>
[Serializable]
public class BossKill : ChallengeStageRule
{
    [SerializeField] private float deadLine;

    public BossKill(StageSO stage) : base(stage){}

    public override void Enter()
    {
        base.Enter();
        Debug.Log($"{stage.chapter} - {stage.stage}(Challenge) BossStageRule 시작. \n" +
                  $"제한시간 : {stage.deadLine} / 목표 처치 대상 : {stage.targetKillScore}");
    }

    public override void MonsterKilledInStage(Monster monster)
    {
        if (monster is Boss)
        {
            Debug.Log("목표처치 달성");
            StageClear();
        }
    }
}
/// <summary> 특정시간 생존하면 달성되는 스테이지 </summary>
[Serializable]
public class Survival : ChallengeStageRule
{
    public Survival(StageSO stage) : base(stage) { }

    public override void Enter()
    {
        token = new CancellationTokenSource();
        ChallengeTimeAttack(token.Token, timeOverIsFail:false).Forget();
    }
    //해당 챌린지에서는 
    public override void MonsterKilledInStage(Monster monster) {  }
}
#endregion
