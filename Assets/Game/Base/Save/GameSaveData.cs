using System;
using System.Collections.Generic;
using Growth.StatUpgrade;
using UnityEngine.Serialization;

namespace Base.Save
{
    [Serializable]
    public class GameSaveData
    {
        public StageProgressData stageProgress = new();
        public PlayerCurrencyData currencyData = new();
        public PlayerItemInventoryData itemInventory = new();
        //public PlayerEquipmentInventoryData equipmentInventory = new(); mvp에선 미구현
        //public PlayerEquipmentData equipment = new(); mvp에선 미구현
        public PlayerStatUpgradeData stat = new();
        public PlayerSkillData skill = new();
        public PlayerAccessTimeData lastAccess = new();
    }
    [Serializable]public class StageProgressData
    {
        public int selectedNormalStage; //직전 일반스테이지
        public int selectedNormalChapter; //현재 일반 스테이지
        public int nextChallangeStage; //도전 가능한 스테이지
        public int nextChallangeChapter; //도전 가능한 챕터
    }
    [Serializable]public class PlayerCurrencyData
    {
        public int level;
        public float exp;
        public int gold;
        public int statStone;
    }
    [Serializable]public class PlayerItemInventoryData
    {
        public List<ItemEntry> items;
    }
    [Serializable]public class PlayerEquipmentInventoryData
    {
        public List<EquipmentEntry> equipments;
    }
    [Serializable]public class PlayerEquipmentData
    {
        public int equipWepon; //장비의 키를 저장
        public int equipArmor;
        public int equipAccessory;
    }
    [Serializable]public class PlayerStatUpgradeData
    {
        public List<StatusEntry> upgrade; //statusType은 StatusSo.cs에 존재
    }
    [Serializable]public class PlayerSkillData
    {
        public int skillSlots; //나중에 스킬슬롯 객체 만들면 수정
        public List<SkillEntry> skills; //key : 스킬 키, value : 해당 스킬 레벨
    }
    [Serializable]public class EquipmentEntry
    {
        public int key; //아이템 키
        public int enforce; //강화 수치
        public int count; //개수
        public bool isDiscovered; //해금 여부
    }
    [Serializable]
    public struct ItemEntry
    {
        public int key; //아이템 키
        public int count; //아이템 개수
    }
    [Serializable]
    public struct StatusEntry
    {
        public StatusType type; //스탯 종류
        public int count; //강화 수치
    }
    [Serializable]
    public struct SkillEntry
    {
        public int key; //스킬 키
        public int count; //강화 수치
    }
    [Serializable]
    public class PlayerAccessTimeData
    {
        public long lastConnectTime;
    }
}