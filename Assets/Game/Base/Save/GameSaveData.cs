using System;
using System.Collections.Generic;
using Growth.StatUpgrade;
using UnityEngine.Serialization;

namespace Base.Save
{
    [Serializable]
    public class GameSaveData
    {
        public StageProgressState stage = new();
        public PlayerCurrencyState currency = new();
        public PlayerItemInventoryState itemInventory = new();
        //public PlayerEquipmentInventoryData equipmentInventory = new(); mvp에선 미구현
        //public PlayerEquipmentData equipment = new(); mvp에선 미구현
        public PlayerStatUpgradeState statUpgrades = new();
        public PlayerSkillState skillProgress = new();
        public LastSessionTime lastAccess = new();
    }
    [Serializable]public class StageProgressState
    {
        public int selectedNormalStage; //직전 일반스테이지
        public int selectedNormalChapter; //현재 일반 스테이지
        public int nextChallangeStage; //도전 가능한 스테이지
        public int nextChallangeChapter; //도전 가능한 챕터
    }
    [Serializable]public class PlayerCurrencyState
    {
        public int level;
        public float exp;
        public int gold;
        public int statStone;
    }
    [Serializable]public class PlayerItemInventoryState
    {
        public List<ItemEntry> owneditemCounts;
    }
    [Serializable]public class PlayerEquipmentInventoryState
    {
        public List<EquipmentEntry> equipmentEntries;
    }
    [Serializable]public class PlayerEquipmentState
    {
        public int equippedWeponKey; //장비의 키를 저장
        public int equippedArmorKey;
        public int equippedAccessoryKey;
    }
    [Serializable]public class PlayerStatUpgradeState
    {
        public List<StatusEntry> upgradeLevelsByType; //statusType은 StatusSo.cs에 존재
    }
    [Serializable]public class PlayerSkillState
    {
        public int skillSlots; //나중에 스킬슬롯 객체 만들면 수정
        public List<SkillEntry> skillProgressState; //key : 스킬 키, value : 해당 스킬 레벨
    }
    [Serializable]public class EquipmentEntry
    {
        public int key; //아이템 키
        public int enhancementLevel; //강화 수치
        public int ownedCount; //개수
        public bool isDiscovered; //해금 여부
    }
    [Serializable]
    public struct ItemEntry
    {
        public int key; //아이템 키
        public int ownedCount; //아이템 개수
    }
    [Serializable]
    public struct StatusEntry
    {
        public StatusType statType; //스탯 종류
        public int enhancementLevel; //강화 수치
    }
    [Serializable]
    public struct SkillEntry
    {
        public int key; //스킬 키
        public int enhancementCount; //강화 수치
    }
    [Serializable]
    public class LastSessionTime
    {
        public long lastConnectTime;
    }
}