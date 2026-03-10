using System.Collections.Generic;
using Presonal.GyuSeong;
using UnityEngine;

namespace Personal.GyuSeong
{
    public struct GameSaveData
    {
        public StageProgressData stageProgress;
        public PlayerCurrencyData currencyData;
        public PlayerItemInventoryData itemInventory;
        public PlayerEquipmentInventoryData equipmentInventory;
        public PlayerEquipmentData equipment;
        public PlayerStatUpgradeData statUpgrade;
        public PlayerSkillData skill;
        public PlayerAccessTimeData lastAccess;
    }

    public struct StageProgressData
    {
        public int curNormalStage; //직전 일반스테이지
        public int curNormalChapter;
        public int maxClearStage;
        public int maxClearChapter;
    }

    public struct PlayerCurrencyData
    {
        public int level;
        public float exp;
        public int gold;
        public int statStone;
    }

    public struct PlayerItemInventoryData
    {
        //key : 아이템 key, value : 아이템 개수
        public Dictionary<int, int> itemInventory;
    }

    public struct PlayerEquipmentInventoryData
    {
        public Dictionary<int, EquipmentData> equipmentInventory;
    }

    public struct EquipmentData
    {
        public int enforce;
        public int count;
        public bool isDiscovered;
    }
    public struct PlayerEquipmentData
    {
        public int equipWepon; //장비의 키를 저장
        public int equipArmor;
        public int equipAccessory;
    }

    public struct PlayerStatUpgradeData
    {
        public Dictionary<StatusType, int> statUpgrade; //statusType은 StatusSo.cs에 존재
    }

    public struct PlayerSkillData
    {
        public int skillSlots; //나중에 스킬슬롯 객체 만들면 수정
        public Dictionary<int, int> skills; //key : 스킬 키, value : 해당 스킬 레벨
    }

    public struct PlayerAccessTimeData
    {
        public Time lastConnectTime;
    }
}