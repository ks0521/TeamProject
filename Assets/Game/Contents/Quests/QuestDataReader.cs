using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuestDataReader
{
    //선행 조건을 판단하는 bool 값을 여기에 넣고,
    //그 값이 true인 경우에만 UI를 띄우는 방식으로 한다면
    //잠금 처리되어 있는 퀘스트도 쉽게 표기하게 할 수 있음
    public int questID;
    public QuestCategory category;
    public string description;
    public QuestStatus questStatus;
    public GoalType goalType;
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

    public QuestCategory CategoryEnum => category;
    public GoalType GoalTypeEnum => goalType;
    public QuestStatus StatusEnum => questStatus;
}
