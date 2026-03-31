using System;

public enum QuestCategory
{
    Daily, Lineal, Recurring, Achievement
}

[System.Serializable]
public class QuestData
{
    public int questID;
    public QuestCategory category;
    public string description; //예: "몬스터 {0}마리 처치"
    public GoalType type;
    public int targetID;
    public int targetValue; //기본 목표 수치(예: 50마리)
    public int RewardGroupID;
    public int PrevQuestID; //선행 퀘스트
    public int StartEventID; //튜토리얼에서 컷신 등의 연출용
    //public int rewardGold;

    public enum GoalType { Hunt, LevelUp, PlayTime, Upgrade, SkillUse, StageClear }

    /*
    // 현재 플레이어 레벨에 따른 보상 계산 (선형적 증가)
    public int GetScaledReward(int playerLevel)
    {
        int multiplier = 10; // 레벨당 보상 가중치
        return rewardGold + (playerLevel * multiplier);
    }
    */
}
