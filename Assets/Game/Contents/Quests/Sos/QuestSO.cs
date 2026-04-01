using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QuestSystem
{
    [CreateAssetMenu(fileName = "NewQuest", menuName = "Quest/QuestData")]
    public class QuestSO : ScriptableObject
    {
        [Header("퀘스트 정보")]
        public int questID;
        public QuestCategory category;
        public string description; //퀘스트 제목을 겸하고 있음. 분리 가능성 o

        [Header("퀘스트 목표")]
        public GoalType goalType;
        public int targetID; //몬스터ID, 스킬ID 등
        public int targetValue;

        [Header("선행조건 및 보상")]
        public int prevQuestID;
        public int rewardGroupID;

        public bool isAbsoluteGoal; //레벨업 등은 true, 사냥은 false
    }
}

