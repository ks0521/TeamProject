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
    

    public virtual void Init(StageSO stage)
    {
        dropManager = GameManager.Instance.GetGameSystem<ItemDropManager>();
        eventHub = GameManager.Instance.GetGameSystem<EventHub>();
        this.stage = stage;
    }
    public abstract void Enter();
    public abstract void MonsterKilledInStage(Monster monster);
    public abstract void Destroy();
}


[Serializable]
public class ChallengeStageRule : StageRule
{
    public bool isCleared;
    public event Action ChallengeSuccess;
    public event Action ChallengeFail;
    public float RemainTime => remainTime;
    
    protected float remainTime;
    protected CancellationTokenSource token;
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

    protected void StageClear()
    {
        if (isCleared) return;
        isCleared = true;
        ChallengeSuccess?.Invoke();
    }
    protected void StageFail()
    {
        ChallengeFail?.Invoke();
    }
    
    protected async UniTaskVoid ChallengeTimeAttack(CancellationToken token)
    {
        while (remainTime > 0)
        {
            remainTime -= Time.deltaTime;
            await UniTask.Yield(cancellationToken: token);
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
        token.Cancel();
        token.Dispose();
    }
}

[Serializable]
public class BossStageRule : ChallengeStageRule
{
    [SerializeField] private float remainTime;
    
    public override void Enter()
    {
        Debug.Log($"{stage.chapter} - {stage.stage}(Challenge) BossStageRule 시작. \n" +
                  $"제한시간 : {stage.deadLine} / 목표 처치 대상 : {stage.targetKillScore}");
        dropManager = GameManager.Instance.GetGameSystem<ItemDropManager>();
        token = new CancellationTokenSource();
        ChallengeTimeAttack(token.Token).Forget();    
    }

    public override void MonsterKilledInStage(Monster monster)
    {
        if (monster is Boss)
        {
            isCleared = true;
            Debug.Log("목표처치 달성");
            StageClear();
        }
    }
}
[Serializable]
public class NormalStageRule : StageRule
{
    private ItemPoolManager itemPool;
    private PlayerManager player;
    public override void Enter()
    {
        Debug.Log($"일반 스테이지{stage.chapter} - {stage.stage} StageRule 시작");
        itemPool = GameManager.Instance.GetGameSystem<ItemPoolManager>();
        player = GameManager.Instance.GetGameSystem<PlayerManager>();
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
            stage.dropTable.GetDroppedItems(PlayerRuntimeStatus.Instance.finalRewardStatStatus.itemDropRate);
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

