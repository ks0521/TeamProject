using Base.Data;
using Base.Managers;
using Base.Save;
using Growth.Equipment;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct GatchaLevels
{
    public int Equipment;
    //public int Pet;
}
public class testGatchaManager : MonoBehaviour,IManager
{
    private GameDataProvider dictionarys;
    private RuntimeProgressData _runtimeData;
    private EventHub eventhub;
    private ItemDropManager dropmanager;
    
    
    public TestGatchaSO GetProbabilityTable()
    {
        return null;
    }

    public struct probability
    {
        public EquipmentSO equipment;
        public int weights;
    }
    public void LoadGatchaLevel()
    {
        //저장소에서 불러오기
        //progress에서 뽑기레벨 받아오기, 이건 나중에 제가 하겠습니다. 
    }
    //현재 재화는 골드지만 나중에 바뀔 예정
    public bool TryGetCatcha(int count, out List<EquipmentSO> ResultList)
    {
        int cost = count * 100;
        if (_runtimeData.currency.gold < cost)
        {
            ResultList = null;
            return false;
        }

        ResultList = new();
        for (int i = 0; i < count; i++)
        {
            EquipmentSO result = Pick();
            ResultList.Add(result);
            dropmanager.GetItem(new DropReward(){rewardType = DropRewardType.Item, amount = 1, itemSO = result});
        }
        _runtimeData.currency.gold -= cost;
        eventhub.CurrencyChange(CurrencyType.GOLD, _runtimeData.currency.gold);
        return true;
    }
    //제네릭으로 바꿔도 좋을듯
    EquipmentSO Pick()
    {
        EquipmentSO result = null;
        //확률표 가져와서 뽑기(레벨에 맞게)
        return result;
    }
    public void Init()
    {
        dictionarys = GameManager.Instance.GetGameSystem<GameDataProvider>();
        _runtimeData = GameManager.Instance.GetGameSystem<ProgressManager>().Progress;
        eventhub = GameManager.Instance.GetGameSystem<EventHub>();
        dropmanager = GameManager.Instance.GetGameSystem<ItemDropManager>();
    }
    public int GetOrder() => 70;

}
