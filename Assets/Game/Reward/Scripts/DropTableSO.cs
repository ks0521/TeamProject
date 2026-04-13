using Base.Data;
using Base.Save;
using Growth.Equipment;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

[Serializable]
public class DropEntry
{
    [FormerlySerializedAs("item")] public EquipmentSO equipment; //아이템 so
    [Range(0, 1)] public float chance; // 드랍 확률
    public int minAmount = 1; //최소 드랍 갯수
    public int maxAmount = 1; //최대 드랍 갯수
}

public enum DropRewardType
{
    Item, Currency
}
[Serializable]
public class DropRewardPreset
{
    public DropRewardType rewardType;
    public CurrencyType currencyType;
    public CurrencySO currencySO;
    [Range(0,1)]public float chance;
    public ItemSO itemSO;
    public int minAmount;
    public int maxAmount;
}

[Serializable]
public struct DropReward
{
    
    public DropRewardType rewardType;
    public CurrencyType currencyType;
    public CurrencySO currencySO;
    public ItemSO itemSO;
    public int amount;
    public DropReward(DropRewardPreset preset, int resultAmount)
    {
        rewardType = preset.rewardType;
        currencyType = preset.currencyType;
        currencySO = preset.currencySO;
        itemSO = preset.itemSO;
        amount = resultAmount;
    }
}
[CreateAssetMenu(menuName = "Game/Reward/DropTable")]
public class DropTableSO : ScriptableObject
{
    public int chapter; //챕터
    public int stage; //스테이지
    public int rewardExp; //경험치(드랍X, 바로 제공)
    public CurrencySO expSO; //경험치 SO넣기(나중에 어드레서블로 자동 불러오면 좋겠음)
    [Header("장비 드랍 테이블")]
    public List<DropRewardPreset> dropList = new();

    public int GetExp() => rewardExp; //지금은 드랍없이 즉시 반영되는 인자가 경험치밖에 없어서 단일로, 추가되면 공용 메서드로 확장
    /// <summary> 드롭률 감안해서 드랍테이블의 아이템 뽑기</summary>
    /// <param name="dropRate">최종 드랍률</param>
    /// <returns>드랍된 아이템 갯수</returns>
    public List<DropReward> GetDroppedItems(float dropRate)
    {
        List<DropReward> droppedItemList = new();
        foreach (var reward in dropList)
        {
            if (reward == null) continue; //드롭리스트에 아이템 없는 사건 방어
            float value = Random.Range(0f, 1f);
            if (value < Mathf.Clamp01(reward.chance * (1 + dropRate))) //확률 뽑아서 당첨이면 드롭되는 아이템 리스트에 추가
            {
                int amount = (reward.minAmount > reward.maxAmount) ?
                    1 : Random.Range(reward.minAmount, reward.maxAmount + 1);
                //* 고민해볼 사항 : 최소 ~ 최대 중 낮은 개수나 높은 개수에 높은 비중을 두고 만들기?
                //드랍될 아이템의 타입(재화 / 장비)에 따라 다른 dropReward생성
                DropReward droppedItem = new(reward, amount);
                droppedItemList.Add(droppedItem);
            }
        }
        return droppedItemList;
    }
    /// <summary> 비접속 보상을 위해 대량의 적 처치를 가정한 보상 리스트를 반환하는 메서드</summary>
    /// <param name="count">처치 수</param>
    /// <param name="dropRate">드랍률</param>
    /// <returns>Count만큼 적을 처치했을 때 획득한 최종 아이템 수</returns>
    public List<DropReward> GetDroppedItems(int count, float dropRate)
    {
        List<DropReward> droppedItemList = new();
        droppedItemList.Add(new DropReward()
        {
            rewardType = DropRewardType.Currency,
            currencyType = CurrencyType.EXP,
            currencySO = expSO,
            amount =  (int)(Random.Range(0.95f , 1.05f) * count * rewardExp),
        });
        foreach (var reward in dropList)
        {
            if (reward == null) continue; //드롭리스트에 아이템 없는 사건 방어
            if (reward.rewardType == DropRewardType.Currency)
            {
                droppedItemList.Add(new(reward,
                    (int)(Mathf.Clamp01(Random.Range(reward.chance - 0.05f,reward.chance + 0.05f)) * 
                        (reward.maxAmount + reward.minAmount) * count / 2)));
                continue;
            }
            float value;
            int totalAmount = 0;
            float finalChance = Mathf.Clamp01(reward.chance * (1 + dropRate));
            for (int i = 0; i < count; i++)
            {
                value = Random.Range(0f, 1f);
                if (value < finalChance) //확률 뽑아서 당첨이면 드롭되는 아이템 리스트에 추가
                {
                    totalAmount += (reward.minAmount > reward.maxAmount) ? 
                            1 : Random.Range(reward.minAmount, reward.maxAmount + 1);
                }
            }
            DropReward droppedItem = new(reward, totalAmount);
            droppedItemList.Add(droppedItem);
        }
        return droppedItemList;
    }
}