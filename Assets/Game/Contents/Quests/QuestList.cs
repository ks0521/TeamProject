using Base.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using QuestSystem;

public static class QuestList
{
    public static List<IQuestStep> GetRecurringQuests()
    {
        return new List<IQuestStep>
        {
            new Quest_LevelUp(), // 레벨업 퀘스트
            new Quest_SkillUse() // 스킬 사용 퀘스트
        };
    }
}

public class Quest_LevelUp : IQuestStep
{
    public string Description => "레벨을 1 올리세요!";

    public async Task ExecuteStepAsync()
    {
        var tcs = new TaskCompletionSource<bool>();

        // 이벤트 핸들러 정의
        Action<int> handler = null;
        handler = (newLevel) =>
        {
            Debug.Log($"[이벤트 수신] 레벨업 감지! 현재 레벨: {newLevel}");
            // 이벤트 구독 해제 (중요: 메모리 누수 방지)
            EventHub.Instance.OnLevelChange -= handler;
            tcs.TrySetResult(true); // 대기 종료
        };

        // 이벤트 구독
        EventHub.Instance.OnLevelChange += handler;

        await tcs.Task;
    }

    public void OnStartQuest() => Debug.Log($">>> 시작: {Description}");
    public void OnCompleteQuest() => Debug.Log("<<< 완료: 레벨업 보상을 획득했습니다!");
}

public class Quest_SkillUse : IQuestStep
{
    public string Description => "아무 스킬이나 5번 사용하세요!";
    private int currentCount = 0;
    private const int TARGET_COUNT = 5;

    public async Task ExecuteStepAsync()
    {
        var tcs = new TaskCompletionSource<bool>();
        currentCount = 0;

        Action<int> handler = null;
        handler = (skillOrder) =>
        {
            Debug.Log($"[이벤트 수신] 스킬 {skillOrder}번 사용됨. (현황: {currentCount}/{TARGET_COUNT})");
            // 이벤트 구독 해제 (중요: 메모리 누수 방지)
            EventHub.Instance.OnLevelChange -= handler;
            tcs.TrySetResult(true); // 대기 종료
        };

        // 이벤트 구독
        EventHub.Instance.OnLevelChange += handler;

        await tcs.Task;
    }

    public void OnStartQuest() => Debug.Log($">>> 시작: {Description}");
    public void OnCompleteQuest() => Debug.Log("<<< 완료: 스킬 사용 보상을 획득했습니다!");
}

