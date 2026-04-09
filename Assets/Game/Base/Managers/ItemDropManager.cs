using Base.Data;
using Base.Managers;
using Base.Save;
using Growth.Currency;
using Growth.Equipment;
using UnityEngine;

public class ItemDropManager : MonoBehaviour, IManager
{
    [SerializeField] private RuntimeProgressData progress;
    [SerializeField] private EventHub hub;
    private RuntimeStatus stat => RuntimeStatus.Instance;

    public void Init()
    {
        progress = GameManager.Instance.GetGameSystem<ProgressManager>().Progress;
        hub = GameManager.Instance.GetGameSystem<EventHub>();
    }

    public void GetReward(DropReward reward)
    {
        if (reward.rewardType == DropRewardType.Item)
        {
            GetItem(reward);
            return;
        }

        switch (reward.currencyType)
        {
            case CurrencyType.GOLD:
                GetGold(reward.amount);
                break;
            case CurrencyType.STATSTONE:
                GetStatStone(reward.amount);
                break;
            case CurrencyType.EXP:
                GetExp(reward.amount);
                break;
        }
    }

    void GetGold(int dropGold)
    {
        int finalGold = (int)(dropGold * (1 + stat.finalRewardStatStatus.goldGain));
        progress.currency.gold += finalGold;
        Debug.Log($"{dropGold} 획득, 플레이어 골드획득량 증가 {stat.finalRewardStatStatus.goldGain}적용되어 최종 {finalGold} 획득\n" +
                  $"현재 소유 골드 : {progress.currency.gold}");
        hub.GetCurrency();
        hub.CurrencyChange(CurrencyType.GOLD, progress.currency.gold);
        hub.GetCurrency();
    }

    void GetStatStone(int dropStatStone)
    {
        int finalStatStone = (int)(dropStatStone * (1 + stat.finalRewardStatStatus.goldGain));
        progress.currency.statStone += finalStatStone;
        Debug.Log(
            $"스탯강화석 {dropStatStone} 획득, 플레이어 스탯강화석 증가 {stat.finalRewardStatStatus.goldGain}적용되어 최종 {finalStatStone} 획득\n" +
            $"현재 소유 스탯강화석 : {progress.currency.statStone}");
        hub.GetCurrency();
        hub.CurrencyChange(CurrencyType.STATSTONE, progress.currency.statStone);
        hub.GetCurrency();
    }

    //경험치는 드랍없이 바로 가서 일단 public으로 쓰긴 하는데 바꿔야함
    public void GetExp(int dropExp)
    {
        int finalExp = (int)(dropExp * (1 + stat.finalRewardStatStatus.expGain));
        progress.currency.exp += finalExp;
        Debug.Log($"경험치 {dropExp} 획득, 플레이어 경험치 증가 {stat.finalRewardStatStatus.expGain}적용되어 최종 {finalExp} 획득\n" +
                  $"현재 소유 경험치 : {progress.currency.exp}");
        while (progress.currency.exp > 100)
        {
            LevelUp();
        }

        hub.CurrencyChange(CurrencyType.EXP, progress.currency.exp);
    }

    void LevelUp()
    {
        progress.playerInfo.level++;
        progress.playerInfo.skillPoint++;
        progress.playerInfo.maxSkillPoint++;
        progress.currency.exp -= 100;
        Debug.Log($"레벨 상승, 경험치 -100, 남은 경험치 : {progress.currency.exp}");
        hub.LevelChanged(progress.playerInfo.level);
    }
    public void GetItem(DropReward droppedItem)
    {
        hub.GetItems();
        if (droppedItem.itemSO is Growth.Equipment.EquipmentSO)
        {
            GetEquip(droppedItem);
            return;
        }

        int itemKey = droppedItem.itemSO.key;
        //이미 아이템이 있으면 획득수량만 추가
        if (progress.itemInventory.ownedItemCounts.ContainsKey(itemKey))
        {
            progress.itemInventory.ownedItemCounts[itemKey] += droppedItem.amount;
            //Debug.Log($"{droppedItem.itemSO.name} 추가 획득");
        }
        //없으면 개수까지 추가
        else
        {
            progress.itemInventory.ownedItemCounts.Add(itemKey, droppedItem.amount);
            Debug.Log($"{droppedItem.itemSO.name} 신규 획득");
        }

        hub.GetItems();
    }

    public void GetEquip(DropReward droppedItem)
    {
        Debug.Log($"{droppedItem} 장비 획득");
        if (progress.equipmentInventory.equipmentEntries.ContainsKey(droppedItem.itemSO.key))
        {
            progress.equipmentInventory.equipmentEntries[droppedItem.itemSO.key].ownedCount += droppedItem.amount;
            Debug.Log($"{droppedItem.itemSO.name} 추가 획득");
        }
        else
        {
            progress.equipmentInventory.equipmentEntries.Add(droppedItem.itemSO.key,
                new EquipmentEntryState()
                {
                    enhancementLevel = 0, isDiscovered = true, ownedCount = droppedItem.amount
                });
            hub.GetNewEquipment(); //신규장비 장착 -> 계산기 돌리기
            Debug.Log($"{droppedItem.itemSO.name} 신규 획득");
        }
        hub.GetEquipments();
        //MVP 이후 개발
    }


    public int GetOrder() => 4; //ItemDropTable은 stage이전에 생성 필요
}