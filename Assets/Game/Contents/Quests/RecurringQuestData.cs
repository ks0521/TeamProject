using System;

[Serializable]
public class RecurringQuestData
{
    public string questId;
    public string descriptionFormat; // 예: "몬스터 {0}마리 처치"
    public QuestType type;
    public int baseTargetValue;      // 기본 목표 수치 (예: 50마리)
    public int baseRewardGold;       // 기본 보상 골드

    public enum QuestType { Click, Hunt, Levelup, Upgrade }

    // 현재 플레이어 레벨에 따른 보상 계산 (선형적 증가)
    public int GetScaledReward(int playerLevel)
    {
        int multiplier = 10; // 레벨당 보상 가중치
        return baseRewardGold + (playerLevel * multiplier);
    }
}
