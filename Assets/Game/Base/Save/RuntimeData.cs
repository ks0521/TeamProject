using System;
using System.Collections.Generic;
using Growth.StatUpgrade;

namespace Base.Save
{
    /// <summary> GameSaveData와의 차이는 리스트 -> 딕셔너리 변환 </summary>
    [Serializable]
    public class RuntimeData 
    {
        public StageProgressData stageProgress = new();
        public PlayerCurrencyData currencyData = new();
        public RuntimeItemInventoryData itemInventory = new();
        //public RuntimeEquipmentInventoryData equipmentInventory = new(); mvp에선 미구현
        //public PlayerEquipmentData equipment = new(); mvp에선 미구현
        public RuntimeStatUpgradeData stat = new();
        public RuntimeSkillData skill = new();
        public PlayerAccessTimeData lastAccess = new();
    }
    
    [Serializable]public class RuntimeItemInventoryData
    {
        //key : 아이템 키, value : 아이템 개수
        public Dictionary<int, int> items;
    }
    [Serializable]public class RuntimeEquipmentInventoryData
    {
        //key : 장비 키, value : 장비 상세(개수 + 강화 + 해금여부)
        public Dictionary<int, RuntimeEquipmentEntry> equipments;
    }
    [Serializable]public class RuntimeStatUpgradeData
    {
        //key : 스탯 타입, value : 스탯 찍은 횟수
        public Dictionary<StatusType, int> upgrade; //statusType은 StatusSo.cs에 존재
    }
    [Serializable]public class RuntimeSkillData
    {
        public int skillSlots; //나중에 스킬슬롯 객체 만들면 수정
        //key : 스킬 키, value : 스킬 레벨
        public Dictionary<int, int> skills; 
    }
    [Serializable]public class RuntimeEquipmentEntry
    {
        public int enforce; //강화 수치
        public int count; //개수
        public bool isDiscovered; //해금 여부
    }
}