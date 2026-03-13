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
        public static GameSaveData RuntimeToSave(RuntimeData runData)
        {
            GameSaveData saveData = new()
            {
                stageProgress =
                {
                    selectedNormalStage = runData.stageProgress.selectedNormalStage,
                    selectedNormalChapter = runData.stageProgress.selectedNormalChapter,
                    nextChallangeStage = runData.stageProgress.nextChallangeStage,
                    nextChallangeChapter = runData.stageProgress.nextChallangeChapter
                },
                currencyData =
                {
                    exp = runData.currencyData.exp,
                    gold = runData.currencyData.gold,
                    level = runData.currencyData.level,
                    statStone = runData.currencyData.statStone
                },
                itemInventory = { items = new List<ItemEntry>() },
                stat = { upgrade = new List<StatusEntry>() },
                skill =
                {
                    skillSlots = runData.skill.skillSlots, //임시용, 나중에 스킬슬롯 정보들어오면 추가
                    skills = new List<SkillEntry>()
                },
                lastAccess =
                {
                    lastConnectTime = runData.lastAccess.lastConnectTime
                }
            };
            //runtimedata의 딕셔너리를 savedata의 리스트로 변환
            foreach (var item in runData.itemInventory.items)
            {
                ItemEntry entry = new ItemEntry { key = item.Key, count = item.Value };
                saveData.itemInventory.items.Add(entry);
            }

            foreach (var stat in runData.stat.upgrade)
            {
                StatusEntry entry = new StatusEntry { type = stat.Key, count = stat.Value };
                saveData.stat.upgrade.Add(entry);
            }

            foreach (var skill in runData.skill.skills)
            {
                SkillEntry entry = new SkillEntry { key = skill.Key, count = skill.Value };
                saveData.skill.skills.Add(entry);
            }

            return saveData;
        }

        /// <summary> 세이브 데이터를 런타임용 데이터로 변경 </summary>
        /// <returns></returns>
        public static RuntimeData SaveToRuntime(GameSaveData saveData)
        {
            RuntimeData runData = new RuntimeData
            {
                stageProgress =
                {
                    selectedNormalStage = saveData.stageProgress.selectedNormalStage,
                    selectedNormalChapter = saveData.stageProgress.selectedNormalChapter,
                    nextChallangeStage = saveData.stageProgress.nextChallangeStage,
                    nextChallangeChapter = saveData.stageProgress.nextChallangeChapter
                },
                currencyData =
                {
                    exp = saveData.currencyData.exp,
                    gold = saveData.currencyData.gold,
                    level = saveData.currencyData.level,
                    statStone = saveData.currencyData.statStone
                },
                itemInventory = { items = new Dictionary<int, int>() },
                stat = { upgrade = new Dictionary<StatusType, int>() },
                skill =
                {
                    skillSlots = saveData.skill.skillSlots, //임시용, 나중에 스킬슬롯 정보들어오면 추가
                    skills = new Dictionary<int, int>()
                },
                lastAccess =
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
            foreach (var stat in saveData.stat.upgrade)
            {
                runData.stat.upgrade.TryAdd(stat.type, stat.count);
                Debug.Log($"{stat.type}키값을 가진 스탯 추가 : {runData.stat.upgrade[stat.type]}개 ");
            }

            foreach (var skill in saveData.skill.skills)
            {
                runData.skill.skills.TryAdd(skill.key, skill.count);
                Debug.Log($"{skill.key}키값을 가진 스킬 추가 : {runData.skill.skills[skill.key]}개 ");
            }

            return runData;
        }
    }
}