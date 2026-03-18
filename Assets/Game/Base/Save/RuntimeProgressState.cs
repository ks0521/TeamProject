using System;
using System.Collections.Generic;
using Growth.StatUpgrade;

namespace Base.Save
{
    /// <summary> GameSaveData와의 차이는 리스트 -> 딕셔너리 변환 </summary>
    [Serializable]
    public class RuntimeProgressState 
    {
        public StageProgressState stage = new();
        public PlayerCurrencyState currency = new();
        public ItemInventoryState itemInventory = new();
        //public RuntimeEquipmentInventoryData equipmentInventory = new(); mvp에선 미구현
        //public PlayerEquipmentData equipment = new(); mvp에선 미구현
        public RuntimeStatUpgradeData statUpgrades = new();
        public RuntimeSkillData skillProgress = new();
        public LastSessionTime lastSession = new();
    }
    //인벤토리 내 아이템 정보
    [Serializable]public class ItemInventoryState
    {
        //key : 아이템 키, value : 아이템 개수
        public Dictionary<int, int> ownedItemCounts;
    }
    //장비 정보
    [Serializable]public class EquipmentInventoryState
    {
        //key : 장비 키, value : 장비 상세(개수 + 강화 + 해금여부)
        public Dictionary<int, EquipmentEntryState> equipmentEntries;
    }
    //플레이어 스탯 강화 상황
    [Serializable]public class RuntimeStatUpgradeData
    {
        //key : 스탯 타입, value : 스탯 찍은 횟수
        public Dictionary<StatusType, int> upgradeLevelsByType; //statusType은 StatusSo.cs에 존재
    }
    //플레이어 스킬 획득 상황
    [Serializable]public class RuntimeSkillData
    {
        public int skillSlots; //나중에 스킬슬롯 객체 만들면 수정
        //key : 스킬 키, value : 스킬 레벨
        public Dictionary<int, int> skillProgressState; 
    }
    //장비 상세정보
    [Serializable]public class EquipmentEntryState
    {
        public int enhancementLevel; //강화 수치
        public int ownedCount; //개수
        public bool isDiscovered; //해금 여부
    }
}