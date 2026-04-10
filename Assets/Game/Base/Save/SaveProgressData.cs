using System;
using System.Collections.Generic;
using Growth.StatUpgrade;
using UnityEngine;
using UnityEngine.Serialization;

namespace Base.Save
{
    public enum CurrencyType
    {
        EXP, GOLD, STATSTONE, FAME
    }
    /// <summary> 실제 게임 저장을 위한 리스트 구조 데이터</summary>
    [Serializable]
    public class SaveProgressData
    {
        [Header("런타임 - 세이브 공용필드")]
        public StageProgressState stage = new(); //스테이지 진행정보
        public PlayerCurrencyState currency = new(); //재화 상태
        public PlayerInfo playerInfo = new();
        public LastSessionTime lastSession = new(); //마지막 접속 시간
        public PlayerEquipmentState equipment = new(); //플레이어 장착 장비 정보
        [Header("세이브 전용 필드")]
        public PlayerItemInventoryState itemInventory = new(); // 플레이어 인벤토리 정보
        public EquipmentInventoryState equipmentInventory = new();  //플레이어 장비 보유 정보
        public PlayerStatUpgradeState statUpgrades = new(); //플레이어 스탯 업그레이드 정보
        public PlayerSkillState skillProgress = new(); //플레이어 스킬 획득 정보
    }

    // 인벤토리 내 아이템 정보
    [Serializable]public class PlayerItemInventoryState
    {
        public List<ItemEntry> owneditemCounts;
    }
    /// <summary>아이템 중 장비의 인벤토리</summary>
    [Serializable]public class EquipmentInventoryState
    {
        public List<EquipmentEntry> equipmentEntries;
    }
    /// <summary> 현재 장착중인 장비 </summary>
    [Serializable]public class PlayerEquipmentState
    {
        public int equippedWeponKey; //장비의 키를 저장
        public int equippedArmorKey;
        public int equippedAccessoryKey;
    }
    /// <summary> 플레이어 스탯 업그레이드 상태</summary>
    [Serializable]public class PlayerStatUpgradeState
    {
        public List<StatusEntry> upgradeLevelsByType; //statusType은 StatusSo.cs에 존재
    }
    /// <summary> 플레이어 스킬 획득 및 슬롯정보 </summary>
    [Serializable]public class PlayerSkillState
    {
        public List<int> skillSlots; //나중에 스킬슬롯 객체 만들면 수정
        public List<SkillEntry> skillProgressState; //key : 스킬 키, value : 해당 스킬 레벨
    }
    
    /// <summary> 아이템의 필수정보</summary>
    [Serializable]
    public struct ItemEntry
    {
        public int key; //아이템 키
        public int ownedCount; //아이템 개수
    }
    /// <summary> 스탯의 필수정보</summary>
    [Serializable]
    public struct StatusEntry
    {
        public StatusType statType; //스탯 종류
        public int enhancementLevel; //강화 수치
    }
    /// <summary> 스킬의 필수정보</summary>
    [Serializable]
    public struct SkillEntry
    {
        public int key; //스킬 키
        public int enhancementCount; //강화 수치
    }
    
}