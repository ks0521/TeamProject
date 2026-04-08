using System.Collections.Generic;
using Growth.StatUpgrade;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Base.Save
{
    /*저장 - 런타임 데이터 분리목적
        - 런타임에서는 Dictionary가 사용하기 편함
        - 저장(JSON)에는 List 구조가 더 적합함
     리플렉션 구조 추가 목적
        - 공용 블록(stage, currency, playerInfo ...)이 늘어날 때마다 컨버터에 수동 복사 코드를 계속 추가하는 일이 번거로움
        - 공용 블록은 자동으로 복사하고 List <-> Dictionary 변환이 필요한 블록만 수동 처리하기 위해 사용
     저장 데이터 추가 규칙
        1. List <-> Dictionary 변환이 필요 없는 공용 블록은 CommonTypeData에 두고 [CommonType]을 붙인다
        2. 변환이 필요한 블록은 Runtime / Save에 따로 두고 기존 방식대로 컨버터에 수동 변환 코드를 추가한다
    */
    public static class DataConverter
    {
        private static readonly BindingFlags bindFlag = BindingFlags.Public | BindingFlags.Instance;
        /// <summary>
        /// [CommonType]이 붙은 타입 목록으로 자동 복사 가능한 공용 블록 타입만 여기에 들어간다
        /// <para>단, 타입에 [CommonType]이 붙어 있어도 RuntimeProgressData / SaveProgressData의 루트 필드에 실제로 존재해야 자동 복사된다</para>
        /// </summary>
        private static readonly HashSet<Type> commonType = HashCommonType();
        private static Dictionary<Type, FieldInfo[]> commonFieldCache = new();
        
        private static List<BlockMap> runtimeToSaveMap; //런타임 -> 세이브 데이터로 변환하기 위한 리플렉션 정보
        private static List<BlockMap> saveToRuntimeMap; //세이트 -> 런타임 데이터로 변환하기 위한 리플렉션 정보
        private static bool isReflectionReady;
        /// <summary> 현재 어셈블리 안의 Base.Save 네임스페이스를 검사해서 [CommonType]이 붙은 타입들을 모아온다
        /// <para> => 자동 복사 후보 타입 목록을 만드는 역할</para> </summary>
        private static HashSet<Type> HashCommonType()
        {
            var assembly = typeof(StageProgressState).Assembly;
            HashSet<Type> result = new();

            foreach (var type in assembly.GetTypes())
            {
                if (type.Namespace == "Base.Save" && type.IsDefined(typeof(CommonType), false))
                {
                    result.Add(type);
                }
            }
            return result;
        }
        /// <summary> 공용 블록 1개를 복사하기 위한 정보 묶음
        /// <para>Ex. RuntimeProgressData.stage -> SaveProgressData.stage 를 복사할 때 필요한 정보 저장</para></summary>
        private class BlockMap
        {
            public FieldInfo sourceField; 
            // source 루트 객체에서 꺼낼 필드 정보
            // 예: RuntimeProgressData.stage
            
            public FieldInfo destinationField; 
            // destination 루트 객체에서 값을 넣을 필드 정보
            // 예: SaveProgressData.stage
            
            public Type blockType; 
            // 공용 블록의 실제 타입
            // 예: StageProgressState
            
            public FieldInfo[] innerField; 
            // 공용 블록 내부 필드 목록
            // 예: selectedNormalStage, selectedNormalChapter ...
        }
        /// <summary>
        /// 공용 블록 복사 계획을 처음 1번만 만든다
        /// <para>이후 저장/로드에서는 캐시된 맵을 재사용</para>
        /// </summary>
        private static void EnsureReflectionReady()
        {
            if (isReflectionReady) return;
            runtimeToSaveMap = BuildBlockMaps(typeof(RuntimeProgressData), typeof(SaveProgressData));
            saveToRuntimeMap = BuildBlockMaps(typeof(SaveProgressData), typeof(RuntimeProgressData));

            isReflectionReady = true;
        }
        /// <summary>
        /// sourceType -> destinationType 사이의 공용 블록 복사 계획 생성
        ///<para></para>
        /// 동작 순서
        /// <para>1. sourceType의 루트 필드(stage, currency, playerInfo ...)를 순회</para>
        /// 2. 그 중 [CommonType]으로 등록된 타입만 추림
        /// <para>3. destinationType에서 같은 이름의 필드를 찾음</para>
        /// 4. 이름과 타입이 모두 맞으면 BlockMap에 추가
        /// <para>5. 그 블록 내부 필드 목록도 함께 저장</para>
        ///
        /// 현재 구현에서는 루트 필드 이름이 같아야 자동 매핑된다.
        /// </summary>
        private static List<BlockMap> BuildBlockMaps(Type sourceType, Type destinationType)
        {
            List<BlockMap> maps = new();
            FieldInfo[] sourceFields = sourceType.GetFields(bindFlag); //sourceType의 필드를 가져옴(stage,playerInfo...)
            FieldInfo[] destinationFields = destinationType.GetFields(bindFlag); //destinationType의 필드를 가져옴
            //순회 간 탐색효율을 늘리기 위한 딕셔너리
            Dictionary<string,FieldInfo> destinationFieldDic = destinationFields.ToDictionary(f => f.Name, f => f);

            foreach (var sourceField in sourceFields)
            {
                Type blockType = sourceField.FieldType; 
                //sourceType필드 중 공용타입(StageProgressState, PlayerInfo ...)만 맵에 추가
                if (!commonType.Contains(blockType))
                {
                    continue;
                }
                //sourceField의 필드명(stage,currency,playerInfo...)과 destinationField의 필드명이 동일하면 복사
                //그래서 SaveProgressData와 RuntimeProgressData 공용으로 사용하는 필드의 이름은 통일시켜줘야함
                //sourceField로 찾으면 이름 상관없어지려나??
                if (!destinationFieldDic.TryGetValue(sourceField.Name, out FieldInfo destinationField))
                {
                    Debug.LogWarning($"DataConverter : {destinationType.Name}에 {sourceField.Name}이 없습니다! ");
                    continue;
                }
                //이름이 같은데 실제 타입은 다르면 (currency = currency지만 source는 PlayerCurrencyState타입, destination은 PlayerInfo타입인 경우같이) 추가하지 않음
                if (destinationField.FieldType != blockType)
                {
                    Debug.LogWarning($"DataConverter : source: {sourceField.Name}와 dest : {destinationField.FieldType.Name}의 타입이 서로 다릅니다! ");
                    continue;
                }

                maps.Add(new BlockMap()
                {
                    sourceField = sourceField,
                    destinationField = destinationField,
                    blockType = blockType,
                    innerField = GetCommonFields(blockType)
                });
            }

            return maps;
        }
        /// <summary>
        /// 공용 블록 타입 내부의 public instance field 목록을 가져온다
        /// <para>한 번 찾은 결과는 commonFieldCache에 저장해서 재사용</para></summary>
        /// <param name="type">공용 블록 타입 (PlayerInfo, PlayerCurrencyState ...)</param>
        /// <returns>해당 타입 내부 필드 목록(PlayerInfo.level / exp ...)</returns>
        private static FieldInfo[] GetCommonFields(Type type)
        {
            //이미 캐싱되어있으면 그대로 가져다 쓰기
            if (commonFieldCache.TryGetValue(type, out FieldInfo[] cache))
            {
                return cache;
            }
            //클래스 필드 내의 모든 필드 가져오고 캐싱
            FieldInfo[] fields = type.GetFields(bindFlag);
            commonFieldCache[type] = fields;
            return fields;
        }
        /// <summary>
        /// source 루트 객체의 공용 블록 값을 destination 루트 객체로 복사한다.
        ///
        /// <para>주의:</para>
        /// - 블록 객체 참조를 통째로 넘기는 것이 아니라 블록 내부 필드 값만 하나씩 복사한다.
        /// <para>- 예:
        ///   runProgressData.stage.selectedNormalStage
        ///   -> saveProgressData.stage.selectedNormalStage</para></summary>
        private static void CopyCommonBlock(object source, object destination,List<BlockMap> maps)
        {
            foreach (var map in maps)
            {
                object sourceBlock = map.sourceField.GetValue(source);
                // 예: source = runProgressData 일 때 sourceBlock = runProgressData.stage
                if (sourceBlock == null) continue;

                object destinationBlock = map.destinationField.GetValue(destination);
                // 예: source = runProgressData 일 때 sourceBlock = runProgressData.stage
                
                if (destinationBlock == null)
                {
                    destinationBlock = Activator.CreateInstance(map.blockType);
                    map.destinationField.SetValue(destination,destinationBlock);
                    // destination 쪽 블록이 없으면 새로 만들어 넣는다
                }

                foreach (var innerField in map.innerField)
                {
                    object value = innerField.GetValue(sourceBlock);
                    innerField.SetValue(destinationBlock,value);
                    // 예: runProgressData.stage.selectedNormalStage 값을 읽어서
                    // saveProgressData.stage.selectedNormalStage에 넣는다
                }
            }
        }
        /// <summary> 런타임 데이터를 세이브용 데이터로 변경 (저장)</summary>
        /// <returns></returns>
        public static SaveProgressData RuntimeToSave(RuntimeProgressData runProgressData)
        {
            
            SaveProgressData saveProgressData = new()
            {
                
                // stage =
                // {
                //     selectedNormalStage = runProgressData.stage.selectedNormalStage,
                //     selectedNormalChapter = runProgressData.stage.selectedNormalChapter,
                //     nextChallangeStage = runProgressData.stage.nextChallangeStage,
                //     nextChallangeChapter = runProgressData.stage.nextChallangeChapter
                // },
                // currency =
                // {
                //     exp = runProgressData.currency.exp,
                //     gold = runProgressData.currency.gold,
                //     statStone = runProgressData.currency.statStone
                // },
                // playerInfo =
                // {
                //     level = runProgressData.playerInfo.level,
                //     maxSkillPoint = runProgressData.playerInfo.maxSkillPoint,
                //     skillPoint = runProgressData.playerInfo.skillPoint,
                //     weaponGachaLevel = runProgressData.playerInfo.weaponGachaLevel
                // },
                // equipment = runProgressData.equipment,
                // lastSession =
                // {
                //     lastConnectTime = runProgressData.lastSession.lastConnectTime
                // },
                
                equipmentInventory = {equipmentEntries = new List<EquipmentEntry>()},
                itemInventory = { owneditemCounts = new List<ItemEntry>() },
                statUpgrades = { upgradeLevelsByType = new List<StatusEntry>() },
                skillProgress =
                {
                    skillSlots = runProgressData.skillProgress.skillSlots, //임시용, 나중에 스킬슬롯 정보들어오면 추가
                    skillProgressState = new List<SkillEntry>()
                },
            };
            EnsureReflectionReady();
            CopyCommonBlock(runProgressData,saveProgressData,runtimeToSaveMap);
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

        /// <summary> 세이브 데이터를 런타임용 데이터로 변경 (로드)</summary>
        public static RuntimeProgressData SaveToRuntime(SaveProgressData saveProgressData)
        {
            RuntimeProgressData runProgressData = new RuntimeProgressData
            {
                // stage =
                // {
                //     selectedNormalStage = saveProgressData.stage.selectedNormalStage,
                //     selectedNormalChapter = saveProgressData.stage.selectedNormalChapter,
                //     nextChallangeStage = saveProgressData.stage.nextChallangeStage,
                //     nextChallangeChapter = saveProgressData.stage.nextChallangeChapter
                // },
                // currency =
                // {
                //     exp = saveProgressData.currency.exp,
                //     gold = saveProgressData.currency.gold,
                //     statStone = saveProgressData.currency.statStone
                // },
                // equipment = saveProgressData.equipment,
                // lastSession =
                // {
                //     lastConnectTime = saveProgressData.lastSession.lastConnectTime
                // },
                
                equipmentInventory = {equipmentEntries = new Dictionary<int, EquipmentEntryState>()},
                itemInventory = { ownedItemCounts = new Dictionary<int, int>() },
                statUpgrades = { upgradeLevelsByType = new Dictionary<StatusType, int>() },
                skillProgress =
                {
                    skillSlots = saveProgressData.skillProgress.skillSlots, //임시용, 나중에 스킬슬롯 정보들어오면 추가
                    skillProgressState = new Dictionary<int, int>()
                },
            };
            EnsureReflectionReady();
            CopyCommonBlock(saveProgressData,runProgressData,saveToRuntimeMap);
            //saveData의 리스트를 runtimeData의 딕셔너리로 변환

            //장비 부분은 mvp 이후 구현
             foreach (var item in saveProgressData.itemInventory.owneditemCounts)
            {
                runProgressData.itemInventory.ownedItemCounts.TryAdd(item.key, item.ownedCount);
                Debug.Log($"{item.key}키값을 가진 장비 추가 : {runProgressData.itemInventory.ownedItemCounts[item.key]}개 ");
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
                Debug.Log($"{stat.statType}키값을 가진 스탯 추가 : {runProgressData.statUpgrades.upgradeLevelsByType[stat.statType]}개 ");
            }

            foreach (var skill in saveProgressData.skillProgress.skillProgressState)
            {
                runProgressData.skillProgress.skillProgressState.TryAdd(skill.key, skill.enhancementCount);
                Debug.Log($"{skill.key}키값을 가진 스킬 추가 : {runProgressData.skillProgress.skillProgressState[skill.key]}개 ");
            }
            Debug.Log("저장된 정보 변환 완료");
            return runProgressData;
        }
    }
}