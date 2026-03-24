using Base.Data;
using System;
using Base.Managers;
using Base.Save;
using Growth.Equipment;
using UnityEditor.Rendering;
using UnityEngine;

public class ItemDropManager : MonoBehaviour, IManager
{
    [SerializeField] private RuntimeProgressState progress;
    [SerializeField] private PlayerRuntimeStatus stat => PlayerRuntimeStatus.Instance;
    private EventHub hub;

    private void Start()
    {
        hub = GameManager.Instance.GetGameSystem<EventHub>();
    }

    public void GetGold(int dropGold)
    {
        int finalGold = (int)(dropGold * (1 + stat.finalRewardStatus.goldRate));
        progress.currency.gold += finalGold;
        Debug.Log($"{dropGold} 획득, 플레이어 골드획득량 증가 {stat.finalRewardStatus.goldRate}적용되어 최종 {finalGold} 획득\n" +
                  $"현재 소유 골드 : {progress.currency.gold}");
        hub.CurrencyChange(CurrencyType.GOLD,progress.currency.gold);
    }

    public void GetStatStone(int dropStatStone)
    {
        int finalStatStone = (int)(dropStatStone * (1 + stat.finalRewardStatus.goldRate));
        progress.currency.statStone += finalStatStone;
        Debug.Log(
            $"스탯강화석 {dropStatStone} 획득, 플레이어 스탯강화석 증가 {stat.finalRewardStatus.goldRate}적용되어 최종 {finalStatStone} 획득\n" +
            $"현재 소유 스탯강화석 : {progress.currency.statStone}");
        hub.CurrencyChange(CurrencyType.STATSTONE,progress.currency.statStone);
    }

    public void GetExp(int dropExp)
    {
        int finalExp = (int)(dropExp * (1 + stat.finalRewardStatus.expRate));
        progress.currency.exp += finalExp;
        Debug.Log($"경험치 {dropExp} 획득, 플레이어 경험치 증가 {stat.finalRewardStatus.expRate}적용되어 최종 {finalExp} 획득\n" +
                  $"현재 소유 경험치 : {progress.currency.exp}");
        while (progress.currency.exp > 100)
        {
            progress.currency.level++;
            progress.currency.exp -= 100;
            Debug.Log($"레벨 상승, 경험치 -100, 남은 경험치 : {progress.currency.exp}");
        }
        hub.CurrencyChange(CurrencyType.EXP,progress.currency.exp);
        hub.LevelChanged(progress.currency.level);
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

    public int GetOrder() => 3; //ItemDropTable은 stage이전에 생성 필요
}