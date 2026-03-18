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
        public static GameSaveData RuntimeToSave(RuntimeProgressState runProgressState)
        {
            GameSaveData saveData = new()
            {
                stage =
                {
                    selectedNormalStage = runProgressState.stage.selectedNormalStage,
                    selectedNormalChapter = runProgressState.stage.selectedNormalChapter,
                    nextChallangeStage = runProgressState.stage.nextChallangeStage,
                    nextChallangeChapter = runProgressState.stage.nextChallangeChapter
                },
                currency =
                {
                    exp = runProgressState.currency.exp,
                    gold = runProgressState.currency.gold,
                    level = runProgressState.currency.level,
                    statStone = runProgressState.currency.statStone
                },
                itemInventory = { owneditemCounts = new List<ItemEntry>() },
                statUpgrades = { upgradeLevelsByType = new List<StatusEntry>() },
                skillProgress =
                {
                    skillSlots = runProgressState.skillProgress.skillSlots, //임시용, 나중에 스킬슬롯 정보들어오면 추가
                    skillProgressState = new List<SkillEntry>()
                },
                lastAccess =
                {
                    lastConnectTime = runProgressState.lastSession.lastConnectTime
                }
            };
            //runtimedata의 딕셔너리를 savedata의 리스트로 변환
            foreach (var item in runProgressState.itemInventory.ownedItemCounts)
            {
                ItemEntry entry = new ItemEntry { key = item.Key, ownedCount = item.Value };
                saveData.itemInventory.owneditemCounts.Add(entry);
            }

            foreach (var stat in runProgressState.statUpgrades.upgradeLevelsByType)
            {
                StatusEntry entry = new StatusEntry { statType = stat.Key, enhancementLevel = stat.Value };
                saveData.statUpgrades.upgradeLevelsByType.Add(entry);
            }

            foreach (var skill in runProgressState.skillProgress.skillProgressState)
            {
                SkillEntry entry = new SkillEntry { key = skill.Key, enhancementCount = skill.Value };
                saveData.skillProgress.skillProgressState.Add(entry);
            }

            return saveData;
        }

        /// <summary> 세이브 데이터를 런타임용 데이터로 변경 </summary>
        /// <returns></returns>
        public static RuntimeProgressState SaveToRuntime(GameSaveData saveData)
        {
            RuntimeProgressState runProgressState = new RuntimeProgressState
            {
                stage =
                {
                    selectedNormalStage = saveData.stage.selectedNormalStage,
                    selectedNormalChapter = saveData.stage.selectedNormalChapter,
                    nextChallangeStage = saveData.stage.nextChallangeStage,
                    nextChallangeChapter = saveData.stage.nextChallangeChapter
                },
                currency =
                {
                    exp = saveData.currency.exp,
                    gold = saveData.currency.gold,
                    level = saveData.currency.level,
                    statStone = saveData.currency.statStone
                },
                itemInventory = { ownedItemCounts = new Dictionary<int, int>() },
                statUpgrades = { upgradeLevelsByType = new Dictionary<StatusType, int>() },
                skillProgress =
                {
                    skillSlots = saveData.skillProgress.skillSlots, //임시용, 나중에 스킬슬롯 정보들어오면 추가
                    skillProgressState = new Dictionary<int, int>()
                },
                lastSession =
                {
                    lastConnectTime = saveData.lastAccess.lastConnectTime
                }
            };
            //saveData의 리스트를 runtimeData의 딕셔너리로 변환

            /*장비 부분은 mvp 이후 구현
             foreach (var item in saveData.itemInventory.items)
            {
                runData.itemInventory.items.TryAdd(item.key, item.count);
                Debug.Log($"{item.key}키값을 가진 장비 추가 : {runData.itemInventory.items[item.key]}개 ");
            }*/
            foreach (var stat in saveData.statUpgrades.upgradeLevelsByType)
            {
                runProgressState.statUpgrades.upgradeLevelsByType.TryAdd(stat.statType, stat.enhancementLevel);
                Debug.Log($"{stat.statType}키값을 가진 스탯 추가 : {runProgressState.statUpgrades.upgradeLevelsByType[stat.statType]}개 ");
            }

            foreach (var skill in saveData.skillProgress.skillProgressState)
            {
                runProgressState.skillProgress.skillProgressState.TryAdd(skill.key, skill.enhancementCount);
                Debug.Log($"{skill.key}키값을 가진 스킬 추가 : {runProgressState.skillProgress.skillProgressState[skill.key]}개 ");
            }

            return runProgressState;
        }
    }
}