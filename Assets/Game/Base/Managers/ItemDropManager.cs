using System;
using Base.Managers;
using Base.Save;
using Growth.Equipment;
using UnityEngine;

public class ItemDropManager : MonoBehaviour, IManager
{
    [SerializeField] private RuntimeProgressState progress;
    [SerializeField] private PlayerRuntimeStatus stat => PlayerRuntimeStatus.Instance;
    public event Action OnGoldChanged;
    public event Action OnStatStoneChanged;
    public event Action OnExpChanged;

    public void GetGold(int dropGold)
    {
        int finalGold = (int)(dropGold * (1 + stat.finalRewardStatus.goldRate));
        progress.currency.gold += finalGold;
        Debug.Log($"{dropGold} 획득, 플레이어 골드획득량 증가 {stat.finalRewardStatus.goldRate}적용되어 최종 {finalGold} 획득\n" +
                  $"현재 소유 골드 : {progress.currency.gold}");
        OnGoldChanged?.Invoke();
    }

    public void GetStatStone(int dropStatStone)
    {
        int finalStatStone = (int)(dropStatStone * (1 + stat.finalRewardStatus.goldRate));
        progress.currency.statStone += finalStatStone;
        Debug.Log(
            $"스탯강화석 {dropStatStone} 획득, 플레이어 스탯강화석 증가 {stat.finalRewardStatus.goldRate}적용되어 최종 {finalStatStone} 획득\n" +
            $"현재 소유 스탯강화석 : {progress.currency.statStone}");
        OnStatStoneChanged?.Invoke();
    }

    public void GetExp(int dropExp)
    {
        int finalExp = (int)(dropExp * (1 + stat.finalRewardStatus.expRate));
        progress.currency.exp += finalExp;
        Debug.Log($"경험치 {dropExp} 획득, 플레이어 경험치 증가 {stat.finalRewardStatus.expRate}적용되어 최종 {finalExp} 획득\n" +
                  $"현재 소유 경험치 : {progress.currency.exp}");
        OnExpChanged?.Invoke();
    }

    public void GetItem(DropedItem droppedItem)
    {
        if (droppedItem.item is EquipmentSO)
        {
            GetEquip(droppedItem);
            return;
        }

        int itemKey = droppedItem.item.key;
        //이미 아이템이 있으면 획득수량만 추가
        if (progress.itemInventory.ownedItemCounts.ContainsKey(itemKey))
        {
            progress.itemInventory.ownedItemCounts[itemKey] += droppedItem.amount;
        }
        //없으면 개수까지 추가
        else
        {
            progress.itemInventory.ownedItemCounts.Add(itemKey, droppedItem.amount);
        }
        
    }

    public void GetEquip(DropedItem dropedItem)
    {
        //MVP 이후 개발
    }


    public void Init()
    {
        progress = GameManager.Instance.GetGameSystem<PlayerProgressManager>().progress;
    }

    public int GetOrder() => 10;
}