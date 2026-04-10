using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//퀘스트 보상 목록을 정의
[CreateAssetMenu(fileName = "Quest Reward SO", menuName = "Quest/Quest Reward SO")]
public class QuestRewardSO : ScriptableObject
{
    [System.Serializable]
    public class RewardGroup
    {
        public int groupID;
        public List<RewardItem> items;
    }

    [System.Serializable]
    public class RewardItem
    {
        public int itemID; //ItemDataSO에 등록된 ID
        public int amount;
    }

    public List<RewardGroup> rewardGroups;

    public RewardGroup GetGroup(int id)
    {
        return rewardGroups.Find(x => x.groupID == id);
    }
}
