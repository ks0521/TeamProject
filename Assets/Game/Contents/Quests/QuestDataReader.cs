using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuestDataReader
{
    public int questID;
    public string category; // JSON에서는 문자열로 적고 내부에서 Enum으로 변환합니다.
    public string description;
    public string questStatus;
    public string goalType;  // JSON에서는 문자열로 관리하는 것이 오타 수정에 유리합니다.
    public int targetID;
    public int targetValue;
    public int prevQuestID;
    public int rewardGroupID;
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
