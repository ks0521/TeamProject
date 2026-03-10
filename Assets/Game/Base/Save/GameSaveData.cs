using System;
using System.Collections.Generic;
using Growth.StatUpgrade;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.UI;


namespace Base.Save
{
    [Serializable]
    public class GameSaveData
    {
        public StageProgressData stageProgress = new();
        public PlayerCurrencyData currencyData = new();
        public PlayerItemInventoryData itemInventory = new();
        public PlayerEquipmentInventoryData equipmentInventory = new();
        public PlayerEquipmentData equipment = new();
        public PlayerStatUpgradeData statUpgrade = new();
        public PlayerSkillData skill = new();
        public PlayerAccessTimeData lastAccess = new();
    }
    [Serializable]public class StageProgressData
    {
        public int curNormalStage; //직전 일반스테이지
        public int curNormalChapter;
        public int maxClearStage;
        public int maxClearChapter;
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
        //key : 아이템 key, value : 아이템 개수
        public Dictionary<int, int> itemInventory;
    }
    [Serializable]public class PlayerEquipmentInventoryData
    {
        public Dictionary<int, EquipmentData> equipmentInventory;
    }
    [Serializable]public class EquipmentData
    {
        public int enforce;
        public int count;
        public bool isDiscovered;
    }
    [Serializable]public class PlayerEquipmentData
    {
        public int equipWepon; //장비의 키를 저장
        public int equipArmor;
        public int equipAccessory;
    }
    [Serializable]public class PlayerStatUpgradeData
    {
        public Dictionary<StatusType, int> statUpgrade; //statusType은 StatusSo.cs에 존재
    }
    [Serializable]public class PlayerSkillData
    {
        public int skillSlots; //나중에 스킬슬롯 객체 만들면 수정
        public Dictionary<int, int> skills; //key : 스킬 키, value : 해당 스킬 레벨
    }
    [Serializable]public class PlayerAccessTimeData
    {
        public Time lastConnectTime;
    }
}