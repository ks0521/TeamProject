using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuestDataReader
{
    public int questID;
    public string category;
    public string description;
    public string questStatus;
    public string goalType;
    public int targetID;
    public int targetValue;
    public int prevQuestID;
    public int rewardGroupID;
    public int valueModifier;
    public bool isInfinite;
    public bool isAbsoluteGoal;
    //ㄴ현재 값이 기준치 이상인가?
    //예: '레벨 30 달성' '누적 몬스터 3000마리 처치' 등의 1회성 퀘스트
    //아니오: '레벨업 1번 하기' '스테이지 1회 클리어' 등의 반복 가능한 퀘스트

    public QuestCategory CategoryEnum => (QuestCategory)System.Enum.Parse(typeof(QuestCategory), category);
    public GoalType GoalTypeEnum => (GoalType)System.Enum.Parse(typeof(GoalType), goalType);
    public QuestStatus StatusEnum => (QuestStatus)System.Enum.Parse(typeof(QuestStatus), goalType);
}

[Serializable]
public class QuestDataWrapper
{
    public System.Collections.Generic.List<QuestDataReader> quests;
}
