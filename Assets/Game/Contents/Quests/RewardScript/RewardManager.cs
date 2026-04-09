using Base.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//퀘스트 보상을 지급하기 위한 RewardData 객체 제작
public class RewardManager : MonoBehaviour, IManager
{
    [SerializeField] private RewardItemRegistrySO itemData;
    [SerializeField] private TextAsset rewardJson;

    private Dictionary<int, List<RewardData>> rewardGroupDic = new Dictionary<int, List<RewardData>>();

    public void Init()
    {
        LoadRewards();
    }
    public int GetOrder() => 345;

    void LoadRewards()
    {
        //JSON 파싱 후 rewardGroupDic에 저장(itemID를 사용하여 Database에서 Sprite를 가져와 RewardData 완성)
    }

    public List<RewardData> GetRewards(int groupID)
    {
        return rewardGroupDic.ContainsKey(groupID) ? rewardGroupDic[groupID] : new List<RewardData>();
    }
}
