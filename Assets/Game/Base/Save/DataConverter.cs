using System.Collections;
using System.Collections.Generic;
using Base.Save;
using Growth.StatUpgrade;
using UnityEngine;

namespace Base.Save
{
/*데이터 구조 변경의 묵적
 1. 런타임 데이터와 저장용 데이터를 분리해 혼용 방지
 2. 실제 구동간 사용하기 좋은 딕셔너리와 저장 양식으로 사용하기 좋은 리스트로 분화
*/
    public static class DataConverter
    {
        /// <summary> 런타임 데이터를 세이브용 데이터로 변경</summary>
        /// <returns></returns>
        public static SaveProgressData RuntimeToSave(RuntimeProgressData runProgressData)
        {
            SaveProgressData saveProgressData = new()
            {
                stage =
                {
                    selectedNormalStage = runProgressData.stage.selectedNormalStage,
                    selectedNormalChapter = runProgressData.stage.selectedNormalChapter,
                    nextChallangeStage = runProgressData.stage.nextChallangeStage,
                    nextChallangeChapter = runProgressData.stage.nextChallangeChapter
                },
                currency =
                {
                    exp = runProgressData.currency.exp,
                    gold = runProgressData.currency.gold,
                    statStone = runProgressData.currency.statStone
                },
                playerInfo =
                {
                    level = runProgressData.playerInfo.level,
                    maxSkillPoint = runProgressData.playerInfo.maxSkillPoint,
                    skillPoint = runProgressData.playerInfo.skillPoint,
                    weaponGachaLevel = runProgressData.playerInfo.weaponGachaLevel
                },
                equipmentInventory = {equipmentEntries = new List<EquipmentEntry>()},
                equipment = runProgressData.equipment,
                itemInventory = { owneditemCounts = new List<ItemEntry>() },
                statUpgrades = { upgradeLevelsByType = new List<StatusEntry>() },
                skillProgress =
                {
                    skillSlots = runProgressData.skillProgress.skillSlots, //임시용, 나중에 스킬슬롯 정보들어오면 추가
                    skillProgressState = new List<SkillEntry>()
                },
                lastSession =
                {
                    lastConnectTime = runProgressData.lastSession.lastConnectTime
                }
            };
            //runtimedata의 딕셔너리를 savedata의 리스트로 변환
            foreach (var item in runProgressData.itemInventory.ownedItemCounts)
            {
                ItemEntry entry = new ItemEntry { key = item.Key, ownedCount = item.Value };
                saveProgressData.itemInventory.owneditemCounts.Add(entry);
            }

            foreach (var equipment in runProgressData.equipmentInventory.equipmentEntries)
            {
                EquipmentEntry entry = new EquipmentEntry()
                {
                    key = equipment.Key,
                    enhancementLevel = equipment.Value.enhancementLevel,
                    ownedCount = equipment.Value.ownedCount,
                    isDiscovered = equipment.Value.isDiscovered
                };
                saveProgressData.equipmentInventory.equipmentEntries.Add(entry);
            }
            foreach (var stat in runProgressData.statUpgrades.upgradeLevelsByType)
            {
                StatusEntry entry = new StatusEntry { statType = stat.Key, enhancementLevel = stat.Value };
                saveProgressData.statUpgrades.upgradeLevelsByType.Add(entry);
            }

            foreach (var skill in runProgressData.skillProgress.skillProgressState)
            {
                SkillEntry entry = new SkillEntry { key = skill.Key, enhancementCount = skill.Value };
                saveProgressData.skillProgress.skillProgressState.Add(entry);
            }

            return saveProgressData;
        }

        /// <summary> 세이브 데이터를 런타임용 데이터로 변경 </summary>
        /// <returns></returns>
        public static RuntimeProgressData SaveToRuntime(SaveProgressData saveProgressData)
        {
            RuntimeProgressData runProgressData = new RuntimeProgressData
            {
                stage =
                {
                    selectedNormalStage = saveProgressData.stage.selectedNormalStage,
                    selectedNormalChapter = saveProgressData.stage.selectedNormalChapter,
                    nextChallangeStage = saveProgressData.stage.nextChallangeStage,
                    nextChallangeChapter = saveProgressData.stage.nextChallangeChapter
                },
                currency =
                {
                    exp = saveProgressData.currency.exp,
                    gold = saveProgressData.currency.gold,
                    statStone = saveProgressData.currency.statStone
                },
                equipmentInventory = {equipmentEntries = new Dictionary<int, EquipmentEntryState>()},
                equipment = saveProgressData.equipment,
                itemInventory = { ownedItemCounts = new Dictionary<int, int>() },
                statUpgrades = { upgradeLevelsByType = new Dictionary<StatusType, int>() },
                skillProgress =
                {
                    skillSlots = saveProgressData.skillProgress.skillSlots, //임시용, 나중에 스킬슬롯 정보들어오면 추가
                    skillProgressState = new Dictionary<int, int>()
                },
                lastSession =
                {
                    lastConnectTime = saveProgressData.lastSession.lastConnectTime
                }
            };
            //saveData의 리스트를 runtimeData의 딕셔너리로 변환

            //장비 부분은 mvp 이후 구현
             foreach (var item in saveProgressData.itemInventory.owneditemCounts)
            {
                runProgressData.itemInventory.ownedItemCounts.TryAdd(item.key, item.ownedCount);
                //Debug.Log($"{item.key}키값을 가진 장비 추가 : {runProgressData.itemInventory.ownedItemCounts[item.key]}개 ");
            }

            foreach (var equipment in saveProgressData.equipmentInventory.equipmentEntries)
            {
                runProgressData.equipmentInventory.equipmentEntries.TryAdd(
                    equipment.key,
                    new EquipmentEntryState()
                    {
                        enhancementLevel = equipment.enhancementLevel,
                        isDiscovered = equipment.isDiscovered,
                        ownedCount = equipment.ownedCount
                    });
            }
            foreach (var stat in saveProgressData.statUpgrades.upgradeLevelsByType)
            {
                runProgressData.statUpgrades.upgradeLevelsByType.TryAdd(stat.statType, stat.enhancementLevel);
                //Debug.Log($"{stat.statType}키값을 가진 스탯 추가 : {runProgressData.statUpgrades.upgradeLevelsByType[stat.statType]}개 ");
            }

            foreach (var skill in saveProgressData.skillProgress.skillProgressState)
            {
                runProgressData.skillProgress.skillProgressState.TryAdd(skill.key, skill.enhancementCount);
                //Debug.Log($"{skill.key}키값을 가진 스킬 추가 : {runProgressData.skillProgress.skillProgressState[skill.key]}개 ");
            }
            Debug.Log("저장된 정보 변환 완료");
            return runProgressData;
        }
    }
}