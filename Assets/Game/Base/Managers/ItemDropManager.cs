using Base.Data;
using Base.Managers;
using Base.Save;
using Growth.Currency;
using Growth.Equipment;
using System.Collections.Generic;
using UnityEngine;

public class RewardBatch
{
    public HashSet<CurrencyType> CurrenciesChanged = new();
    public bool itemChanged;
    public bool equipmentChanged;
    public bool newEquipmentChanged;
    public bool playerDropSound; //필드 드랍아이템 주울때만 활성화(오디오용)
}
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

    void FlushBatch(RewardBatch batch)
    {
        if(batch.CurrenciesChanged.Count != 0 && batch.playerDropSound) hub.GetCurrency(); //재화 획득 사운드 출력용
        if(batch.CurrenciesChanged.Contains(CurrencyType.EXP)) hub.CurrencyChange(CurrencyType.EXP,progress.currency.exp);
        if(batch.CurrenciesChanged.Contains(CurrencyType.GOLD)) hub.CurrencyChange(CurrencyType.GOLD,progress.currency.gold);
        if(batch.CurrenciesChanged.Contains(CurrencyType.STATSTONE)) hub.CurrencyChange(CurrencyType.STATSTONE,progress.currency.statStone);
        if(batch.CurrenciesChanged.Contains(CurrencyType.FAME)) hub.CurrencyChange(CurrencyType.FAME, progress.currency.fame);
        if(batch.itemChanged) hub.GetItems();
        if(batch.equipmentChanged) hub.GetEquipments();
        if(batch.newEquipmentChanged) hub.GetNewEquipment();
    }
    
    /// <summary> 아이템 1종 획득하는 메서드</summary>
    /// <param name="playerDropSound">획득 효과음 출력 여부 </param>
    public void GetReward(DropReward reward, bool playerDropSound = true)
    {
        RewardBatch batch = new();
        ApplyReward(reward, batch);
        FlushBatch(batch);
    }
    /// <summary> 아이템 여러종 획득하는 메서드</summary>
    /// <param name="playerDropSound">획득 효과음 출력 여부</param>
    public void GetRewards(List<DropReward> rewards, bool playerDropSound = false)
    {
        RewardBatch batch = new();
        foreach (var reward in rewards)
        {
            ApplyReward(reward,batch);
        }
        FlushBatch(batch);
    }

    void ApplyReward(in DropReward reward, RewardBatch batch)
    {
        switch (reward.rewardType)
        {
            case DropRewardType.Currency:
                ApplyCurrency(reward, batch);
                break;
            case DropRewardType.Item:
                ApplyItem(reward, batch);
                break;
        }
    }

    void ApplyCurrency(DropReward reward, RewardBatch batch)
    {
        switch (reward.currencyType)
        {
            case CurrencyType.EXP:
                int finalExp = (int)(reward.amount * (1 + stat.FinalRewardStatStatus.expGain));
                progress.currency.exp += finalExp;
                while (progress.currency.exp > 100) { LevelUp(); }
                break;
            case CurrencyType.GOLD:
                int finalGold = (int)(reward.amount * (1 + stat.FinalRewardStatStatus.goldGain));
                progress.currency.gold += finalGold;
                break;
            case CurrencyType.STATSTONE:
                int finalStatStone = (int)(reward.amount * (1 + stat.FinalRewardStatStatus.statStoneGain));
                progress.currency.statStone += finalStatStone;
                break;
            case CurrencyType.FAME:
                progress.currency.fame += reward.amount;
                break;
        }
        batch.CurrenciesChanged.Add(reward.currencyType);
    }

    void ApplyItem(DropReward reward, RewardBatch batch)
    {
        if (reward.itemSO is EquipmentSO)
        {
            ApplyEquipment(reward, batch);
        }
        
        //이미 아이템이 있으면 획득수량만 추가
        if (progress.itemInventory.ownedItemCounts.ContainsKey(reward.itemSO.key))
        {
            progress.itemInventory.ownedItemCounts[reward.itemSO.key] += reward.amount;
            //Debug.Log($"{droppedItem.itemSO.name} 추가 획득");
        }
        //없으면 개수까지 추가
        else
        {
            progress.itemInventory.ownedItemCounts.Add(reward.itemSO.key, reward.amount);
            Debug.Log($"{reward.itemSO.name} 신규 획득");
        }
        batch.itemChanged = true;
    }
    void ApplyEquipment(DropReward reward, RewardBatch batch)
    {
        if (progress.equipmentInventory.equipmentEntries.ContainsKey(reward.itemSO.key))
        {
            progress.equipmentInventory.equipmentEntries[reward.itemSO.key].ownedCount += reward.amount;
            //Debug.Log($"{droppedItem.itemSO.name} 추가 획득");
        }
        else
        {
            progress.equipmentInventory.equipmentEntries.Add(reward.itemSO.key,
                new EquipmentEntryState()
                {
                    enhancementLevel = 0, isDiscovered = true, ownedCount = reward.amount
                });
            batch.newEquipmentChanged = true; //신규장비 장착 -> 계산기 돌리기
            //Debug.Log($"{droppedItem.itemSO.name} 신규 획득");
        }
        batch.equipmentChanged = true;
    }
    void LevelUp()
        {
            progress.playerInfo.level++;
            progress.playerInfo.skillPoint++;
            progress.playerInfo.maxSkillPoint++;
            progress.currency.exp -= 100;
            //Debug.Log($"레벨 상승, 경험치 -100, 남은 경험치 : {progress.currency.exp}");
            hub.LevelChanged(progress.playerInfo.level);
        }
    void GetGold(int dropGold)
    {
        int finalGold = (int)(dropGold * (1 + stat.FinalRewardStatStatus.goldGain));
        progress.currency.gold += finalGold;
        /*Debug.Log($"{dropGold} 획득, 플레이어 골드획득량 증가 {stat.finalRewardStatStatus.goldGain}적용되어 최종 {finalGold} 획득\n" +
                  $"현재 소유 골드 : {progress.currency.gold}");*/
        hub.CurrencyChange(CurrencyType.GOLD, progress.currency.gold);
        hub.GetCurrency();
    }

    void GetStatStone(int dropStatStone)
    {
        int finalStatStone = (int)(dropStatStone * (1 + stat.FinalRewardStatStatus.goldGain));
        progress.currency.statStone += finalStatStone;
        /*Debug.Log(
            $"스탯강화석 {dropStatStone} 획득, 플레이어 스탯강화석 증가 {stat.finalRewardStatStatus.goldGain}적용되어 최종 {finalStatStone} 획득\n" +
            $"현재 소유 스탯강화석 : {progress.currency.statStone}");*/
        hub.CurrencyChange(CurrencyType.STATSTONE, progress.currency.statStone);
        hub.GetCurrency();
    }

    public void GetExp(int dropExp)
    {
        int finalExp = (int)(dropExp * (1 + stat.FinalRewardStatStatus.expGain));
        progress.currency.exp += finalExp;
        /*Debug.Log($"경험치 {dropExp} 획득, 플레이어 경험치 증가 {stat.finalRewardStatStatus.expGain}적용되어 최종 {finalExp} 획득\n" +
                  $"현재 소유 경험치 : {progress.currency.exp}");*/
        while (progress.currency.exp > 100)
        {
            LevelUp();
        }

        hub.CurrencyChange(CurrencyType.EXP, progress.currency.exp);
    }

    
    public void GetItem(DropReward droppedItem)
    {
        if (droppedItem.itemSO is EquipmentSO)
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
        //Debug.Log($"{droppedItem} 장비 획득");
        if (progress.equipmentInventory.equipmentEntries.ContainsKey(droppedItem.itemSO.key))
        {
            progress.equipmentInventory.equipmentEntries[droppedItem.itemSO.key].ownedCount += droppedItem.amount;
            //Debug.Log($"{droppedItem.itemSO.name} 추가 획득");
        }
        else
        {
            progress.equipmentInventory.equipmentEntries.Add(droppedItem.itemSO.key,
                new EquipmentEntryState()
                {
                    enhancementLevel = 0, isDiscovered = true, ownedCount = droppedItem.amount
                });
            hub.GetNewEquipment(); //신규장비 장착 -> 계산기 돌리기
            //Debug.Log($"{droppedItem.itemSO.name} 신규 획득");
        }
        hub.GetEquipments();
    }


    public int GetOrder() => 4; //ItemDropTable은 stage이전에 생성 필요
}