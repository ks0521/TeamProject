using Base.Data;
using Base.Managers;
using Base.Save;
using Battle;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OfflineRewardManager : MonoBehaviour,IManager
{
    private RuntimeProgressData progress;
    private ItemDropManager dropManager;
    private GameDataProvider dic;
    private StageManager stage;
    private DateTime time;
    [SerializeField] private List<DropReward> drops;
    public void Init()
    {
        progress = GameManager.Instance.GetGameSystem<ProgressManager>().Progress;
        dropManager = GameManager.Instance.GetGameSystem<ItemDropManager>();
        dic = GameManager.Instance.GetGameSystem<GameDataProvider>();
        stage = GameManager.Instance.GetGameSystem<StageManager>();
        time = DateTime.FromBinary(progress.lastSession.lastConnectTime);
        OfflineKillReward();
    }

    public void OfflineKillReward()
    {
        int sec = (DateTime.Now - time).Seconds;
        if (sec > 28800) sec = 28800; //8시간 (8 * 60 * 60 ) 한도
        int offlineKillCount = (int)(sec / 60f * 50);
        if (offlineKillCount <= 0)
        {
            Debug.Log("처치한 보상이 없습니다. ");
            return;
        }
        StageProgress stageProgress = stage.GetStageProgress();
        StageSO maxStage = dic.stageTable.GetSO
        (chapter : stageProgress.nextChallengeChapter, 
            stage : stageProgress.nextChallengeStage - 1 ,
            stageType : StageType.Normal);
        Debug.Log($"{maxStage.chapter} - {maxStage.stage} Reward : {offlineKillCount}마리 처치");
        drops = maxStage.dropTable.GetDroppedItems(offlineKillCount, 0f);
        dropManager.GetRewards(drops);
    }
    
    public void AutoClearReward(int sec)
    {
        int offlineKillCount = (int)(sec / 60f * 50);
        if (offlineKillCount <= 0)
        {
            Debug.Log("소탕 시간이 너무 짧습니다. ");
            return;
        }
        StageProgress stageProgress = stage.GetStageProgress();
        StageSO maxStage = dic.stageTable.GetSO
        (chapter : stageProgress.nextChallengeChapter, 
            stage : stageProgress.nextChallengeStage - 1 ,
            stageType : StageType.Normal);
        Debug.Log($"{maxStage.chapter} - {maxStage.stage} Reward : {offlineKillCount}마리 처치");
        drops = maxStage.dropTable.GetDroppedItems(offlineKillCount, 0f);
        dropManager.GetRewards(drops);
    }
    public int GetOrder() => 50;
}
