using System;
using System.Collections.Generic;
using Growth.StatUpgrade;

namespace Base.Save
{
    /// <summary> 게임 내에서 사용하는 진행도, GameSaveData와의 차이는 리스트 -> 딕셔너리 변환 </summary>
    [Serializable]
    public class RuntimeProgressState
    {
        public StageProgressState stage = new(); //플레이어 스테이지 진행 정보
        public PlayerCurrencyState currency = new(); //플레이어 재화 획득 정보
        public ItemInventoryState itemInventory = new(); //플레이어 인벤토리 정보
        public RuntimeEquipmentInventoryState equipmentInventory = new(); //플레이어 장비창 정보
        public PlayerEquipmentState equipment = new(); //플레이어 장착 장비 정보
        public RuntimeStatUpgradeData statUpgrades = new(); //플레이어 스탯 업그레이드 정보
        public RuntimeSkillData skillProgress = new(); // 플레이어 스킬 획득정보
        public LastSessionTime lastSession = new(); //플레이어 마지막 접속시간 정보
    }
    //인벤토리 내 아이템 정보
    [Serializable]public class ItemInventoryState
    {
        //key : 아이템 키, value : 아이템 개수
        public Dictionary<int, int> ownedItemCounts;
    }
    //장비 정보
    [Serializable]public class RuntimeEquipmentInventoryState
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
        public List<int> skillSlots; //스킬슬롯에 저장된 스킬리스트
        //key : 스킬 키, value : 스킬 레벨
        public Dictionary<int, int> skillProgressState; //찍힌 스킬
    }
    /// <summary> 특정 장비의 상세 정보(개수, 해금여부, 강화수치)</summary>
    [Serializable]public class EquipmentEntryState
    {
        public int ownedCount; //개수
        public bool isDiscovered; //해금 여부
        public int enhancementLevel; //강화 수치
    }
}