using Base.Data;
using Base.Managers;
using Base.Save;
using Cysharp.Threading.Tasks;
using Growth.Equipment;
using Shop.Gacha;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

namespace Shop.Gacha
{
    public class GachaManager : MonoBehaviour, IManager
    {
        private GameDataProvider gameDataProvider;
        private RuntimeProgressData progressData;
        private EventHub hub;
        private ItemDropManager dropManager;

        [Header("가챠 SO 모음")]
        [SerializeField] private List<GachaConfigSO> gachaConfigSO;

        private Dictionary<EquipType, GachaConfigSO> gachaDic = new(); //해당 타입의 가챠 SO


        public int GetOrder() => 220;
        public void Init()
        {
            gameDataProvider = GameManager.Instance.GetGameSystem<GameDataProvider>();
            progressData = GameManager.Instance.GetGameSystem<ProgressManager>().progress;
            hub = GameManager.Instance.GetGameSystem<EventHub>();
            dropManager = GameManager.Instance.GetGameSystem<ItemDropManager>();


            gachaDic.Clear();

            if (gachaConfigSO == null || gachaConfigSO.Count == 0)
            {
                Debug.LogWarning("가챠 SO 가 비어있음");
                return;
            }

            for (int i = 0; i < gachaConfigSO.Count; i++)
            {
                GachaConfigSO config = gachaConfigSO[i];

                if (config == null)
                {
                    Debug.LogWarning($"gachaConfigSO {i} 가 null");
                    continue;
                }

                EquipType type = config.targetEquipType;

                if (gachaDic.ContainsKey(config.targetEquipType))
                {
                    Debug.LogWarning($"{config.targetEquipType} 타입 SO 가 중복 등록됨");
                    continue;
                }

                gachaDic.Add(type, config);
                CheckDataConfig(config);
            }
        }
        private void CheckDataConfig(GachaConfigSO config)
        {
            if (config.maxLevel <= 0)
            {
                Debug.Log($"{config.name} : maxLevel 은 1 이상이어야함");
            }
            if (config.defaultLevel <= 0)
            {
                Debug.Log($"{config.name} : defaultLevel 은 1 이상이어야함");
            }
            if (config.maxLevel < config.defaultLevel)
            {
                Debug.Log($"{config.name} : maxLevel 이 defaultLevel 보다 작음");
            }
            if (config.levelUpDraw == null)
            {
                Debug.Log($"{config.name} : levelUpDrawRequirements 가 null 입니다.");
                return;
            }

            if (config.levelUpDraw.Count != config.maxLevel - config.defaultLevel)
            {
                Debug.LogWarning(
                    $"{config.name} : levelUpDraw 개수({config.levelUpDraw.Count})가 maxLevel 개수 ({config.maxLevel - 1})가 다릅니다.");

            }
            if (config.drawCosts == null || config.drawCosts.Count == 0)
            {
                Debug.LogWarning($"{config.name} : drawCosts 가 비어 있습니다.");
            }

            if (config.baseRarityWeights == null || config.baseRarityWeights.Count == 0)
            {
                Debug.LogWarning($"{config.name} : baseRarityWeights 가 비어 있습니다.");
            }
        }//SO 데이터 검사 (레벨 , 비용 등등 전체 확인용)


        private void SetGachaLevel(EquipType equipType, int level)
        {
            switch (equipType)
            {
                case EquipType.Weapon:
                    progressData.playerInfo.weaponGachaLevel = level;
                    break;

                    /*case EquipType.Armor:
                        progressData.playerInfo.armorGachaLevel = level;
                        break;

                    case EquipType.Accessory:
                        progressData.playerInfo.accessoryGachaLevel = level;
                        break;*/
            }
        }//가챠 레벨 저장용
        private void SetCurrentGachaCount(EquipType equipType, int count)
        {
            switch (equipType)
            {
                case EquipType.Weapon:
                    progressData.playerInfo.curWeaponGachaCount = count;
                    break;

                    /*case EquipType.Armor:
                        progressData.playerInfo.curArmorGachaCount = count;
                        break;

                    case EquipType.Accessory:
                        progressData.playerInfo.curAccessoryGachaCount = count;
                        break;*/
            }
        }//가챠 횟수 저장용


        public GachaConfigSO GetGachaSO(EquipType equipType)
        {
            if (gachaDic.TryGetValue(equipType, out var configSO))
            {
                return configSO;
            }

            Debug.Log($"{equipType} 타입 SO를 못 찾음");
            return null;
        }//해당 타입의 가챠 SO 찾기
        public int GetGachaLevel(EquipType equipType)
        {
            switch (equipType)
            {
                case EquipType.Weapon:
                    return progressData.playerInfo.weaponGachaLevel;

                    /*case EquipType.Armor:
                        break;

                    case EquipType.Accessory:
                        break;*/
            }

            return 1;
        }//타입별 현재 가챠 레벨 반환(방어구 , 악세서리 추가예정)
        public int GetCurrentGachaCount(EquipType equipType)
        {
            switch (equipType)
            {
                case EquipType.Weapon:
                    return progressData.playerInfo.curWeaponGachaCount;

                    /*case EquipType.Armor:
                    break;

                    case EquipType.Accessory:
                    break;*/
            }

            return 0;
        }//타입별 누적 가챠 횟수 반환
        public int GetNextLevelUpCount(EquipType equipType)
        {
            GachaConfigSO config = GetGachaSO(equipType);
            if (config == null) return 1;

            int currentLevel = GetGachaLevel(equipType);

            if (currentLevel >= config.maxLevel)
            {
                return 1;
            }

            int index = currentLevel - config.defaultLevel;

            if (index < 0 || index >= config.levelUpDraw.Count)
            {
                return 1;
            }

            return config.levelUpDraw[index];
        }//현재 레벨 기준 레벨업에 필요한 가챠 횟수 반환
        public int GetDrawCost(EquipType equipType, GachaDrawType drawType)
        {
            GachaConfigSO config = GetGachaSO(equipType);

            if (config == null) return 0;

            for (int i = 0; i < config.drawCosts.Count; i++)
            {
                var data = config.drawCosts[i];

                if (data.drawType != drawType) continue;

                return data.cost;
            }

            return 0;
        }//가챠 비용 반환
        public bool GetCanDraw(EquipType equipType, GachaDrawType drawType)
        {
            GachaConfigSO config = GetGachaSO(equipType);
            if (config == null) return false;

            int cost = GetDrawCost(equipType, drawType);
            if (cost <= 0) return false;

            if (progressData == null) return false;
            if (progressData.currency.gold < cost) return false;

            List<EquipmentSO> typeEquipments = GetEquipmentsByType(equipType);
            if (typeEquipments == null || typeEquipments.Count == 0) return false;

            return true;
        }//가챠 가능 여부 반환
        public string GetProbabilityTableText(EquipType equipType, int level)
        {
            GachaConfigSO configSO = GetGachaSO(equipType);
            if (configSO == null) return string.Empty;

            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            GachaConfigSO.GachaLevelRarityBonus bonusData = null;

            if (configSO.levelRarityBonuses != null)
            {
                for (int i = 0; i < configSO.levelRarityBonuses.Count; i++)
                {
                    if (configSO.levelRarityBonuses[i].level == level)
                    {
                        bonusData = configSO.levelRarityBonuses[i];
                        break;
                    }
                }
            }

            if (configSO.baseRarityWeights == null) return string.Empty;

            for (int i = 0; i < configSO.baseRarityWeights.Count; i++)
            {
                var baseWeight = configSO.baseRarityWeights[i];
                int bonusWeight = 0;

                if (bonusData != null && bonusData.bonusWeights != null)
                {
                    for (int j = 0; j < bonusData.bonusWeights.Count; j++)
                    {
                        if (bonusData.bonusWeights[j].rarity == baseWeight.rarity)
                        {
                            bonusWeight = bonusData.bonusWeights[j].weight;
                            break;
                        }
                    }
                }

                int finalWeight = baseWeight.weight + bonusWeight;

                switch (baseWeight.rarity)
                {
                    case EquipRarity.Common:
                        sb.AppendLine($"<color=#646464>{baseWeight.rarity}</color> : {finalWeight / 100}%");
                        break;

                    case EquipRarity.UnCommon:
                        sb.AppendLine($"<color=#64E6FF>{baseWeight.rarity}</color> : {finalWeight / 100}%");
                        break;

                    case EquipRarity.Rare:
                        sb.AppendLine($"<color=#A500FF>{baseWeight.rarity}</color> : {finalWeight / 100}%");
                        break;

                    case EquipRarity.Unique:
                        sb.AppendLine($"<color=#FFC800>{baseWeight.rarity}</color> : {finalWeight / 100}%");
                        break;
                }
            }

            return sb.ToString();
        }//확률표 반환 함수


        public int GetDrawCountByType(GachaDrawType drawType)
        {
            switch (drawType)
            {
                case GachaDrawType.One:
                    return 1;

                case GachaDrawType.Ten:
                    return 10;

                case GachaDrawType.Hundred:
                    return 100;
            }

            return 1;
        }//뽑기 횟수 타입에서 int 로 변환



        private List<EquipmentSO> GetEquipmentsByType(EquipType equipType)
        {
            List<EquipmentSO> result = new List<EquipmentSO>();

            if (gameDataProvider == null) return result;
            if (gameDataProvider.equipmentTable == null) return result;
            if (gameDataProvider.equipmentTable.allEquipments == null) return result;

            for (int i = 0; i < gameDataProvider.equipmentTable.allEquipments.Count; i++)
            {
                EquipmentSO equipment = gameDataProvider.equipmentTable.allEquipments[i];

                if (equipment == null) continue;
                if (equipment.equipType != equipType) continue;

                result.Add(equipment);
            }

            return result;
        }//타입에 맞는 장비만 가져오기
        private EquipQuality GetRandomQuality(GachaConfigSO configSO)
        {
            if (configSO == null) return EquipQuality.Low;
            if (configSO.qualityWeights == null || configSO.qualityWeights.Count == 0) return EquipQuality.Low;

            int totalWeight = 0;

            for (int i = 0; i < configSO.qualityWeights.Count; i++) //전체 가중치 합산
            {
                totalWeight += configSO.qualityWeights[i].weight;
            }

            if (totalWeight <= 0)
            {
                Debug.LogWarning("GachaSO 파일의 품질 가중치 이상함");
                return EquipQuality.Low;
            }

            int randomValue = UnityEngine.Random.Range(0, totalWeight);

            int currentWeight = 0;

            for (int i = 0; i < configSO.qualityWeights.Count; i++)
            {
                currentWeight += configSO.qualityWeights[i].weight;//누적 가중치 방식

                if (randomValue < currentWeight)
                {
                    return configSO.qualityWeights[i].quality;
                }
            }

            return EquipQuality.Low;
        }//품질 결정
        private EquipRarity GetRandomRarity(GachaConfigSO configSO, int level)
        {
            List<GachaConfigSO.GachaRarityWeight> finalWeights = new List<GachaConfigSO.GachaRarityWeight>();

            if (configSO.baseRarityWeights != null)
            {
                for (int i = 0; i < configSO.baseRarityWeights.Count; i++)
                {
                    GachaConfigSO.GachaRarityWeight baseData = configSO.baseRarityWeights[i];

                    GachaConfigSO.GachaRarityWeight newData = new GachaConfigSO.GachaRarityWeight();
                    newData.rarity = baseData.rarity;
                    newData.weight = baseData.weight;

                    finalWeights.Add(newData);
                }
            }

            if (configSO.levelRarityBonuses != null)
            {
                for (int i = 0; i < configSO.levelRarityBonuses.Count; i++)
                {
                    GachaConfigSO.GachaLevelRarityBonus bonusData = configSO.levelRarityBonuses[i];

                    if (bonusData.level != level) continue;
                    if (bonusData.bonusWeights == null) continue;

                    for (int j = 0; j < bonusData.bonusWeights.Count; j++)
                    {
                        GachaConfigSO.GachaRarityWeight bonusWeight = bonusData.bonusWeights[j];
                        bool found = false;

                        for (int k = 0; k < finalWeights.Count; k++)
                        {
                            if (finalWeights[k].rarity == bonusWeight.rarity)
                            {
                                finalWeights[k].weight += bonusWeight.weight;
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            GachaConfigSO.GachaRarityWeight newBonus = new GachaConfigSO.GachaRarityWeight();
                            newBonus.rarity = bonusWeight.rarity;
                            newBonus.weight = bonusWeight.weight;
                            finalWeights.Add(newBonus);
                        }
                    }

                    break;
                }
            }

            int totalWeight = 0;

            for (int i = 0; i < finalWeights.Count; i++)
            {
                totalWeight += finalWeights[i].weight;
            }

            if (totalWeight <= 0)
                return EquipRarity.Common;

            int randomValue = UnityEngine.Random.Range(0, totalWeight);
            int currentWeight = 0;

            for (int i = 0; i < finalWeights.Count; i++)
            {
                currentWeight += finalWeights[i].weight;

                if (randomValue < currentWeight)
                {
                    return finalWeights[i].rarity;
                }
            }

            return EquipRarity.Common;
        }//등급 결정
        public List<EquipmentSO> TryDrawGacha(EquipType equipType, int count)
        {
            List<EquipmentSO> results = new List<EquipmentSO>();

            if (count <= 0) return results;

            GachaConfigSO configSO = GetGachaSO(equipType);
            if (configSO == null) return results;

            List<EquipmentSO> typeEquipments = GetEquipmentsByType(equipType);
            if (typeEquipments == null || typeEquipments.Count == 0) return results;

            int currentLevel = GetGachaLevel(equipType);

            for (int i = 0; i < count; i++)
            {
                EquipRarity equipRarity = GetRandomRarity(configSO, currentLevel);
                EquipQuality equipQuality = GetRandomQuality(configSO);

                EquipmentSO selectedEquipment = null;

                for (int z = 0; z < typeEquipments.Count; z++)
                {
                    EquipmentSO equipment = typeEquipments[z];

                    if (equipment == null) continue;
                    if (equipment.rarity != equipRarity) continue;
                    if (equipment.quality != equipQuality) continue;

                    selectedEquipment = equipment;
                    break;
                }

                if (selectedEquipment != null)
                {
                    results.Add(selectedEquipment);
                }
            }

            return results;
        }//랜덤 결과 생성
        private bool TrySpendCurrency(int amount)
        {
            if (progressData == null) return false;
            if (amount <= 0) return false;

            if (progressData.currency.gold < amount)
            {
                return false;
            }

            progressData.currency.gold -= amount;

            return true;
        }//골드 차감 함수


        private void AddDrawCount(EquipType equipType, int addCount)
        {
            if (addCount <= 0) return;

            int currentCount = GetCurrentGachaCount(equipType);
            currentCount += addCount;

            SetCurrentGachaCount(equipType, currentCount);
        }//가챠 횟수 누적용
        private void AddEquipment(EquipmentSO equipment)
        {
            if (equipment == null) return;
            if (dropManager == null) return;

            DropReward droppedItem = new DropReward();
            droppedItem.itemSO = equipment;
            droppedItem.amount = 1;

            dropManager.GetItem(droppedItem);
        }//장비 지급하는 함수

        private void ProcessLevelUp(EquipType equipType)
        {
            GachaConfigSO config = GetGachaSO(equipType);
            if (config == null) return;

            int currentLevel = GetGachaLevel(equipType);
            int currentCount = GetCurrentGachaCount(equipType);

            while (currentLevel < config.maxLevel)
            {
                int needCount = GetNextLevelUpCount(equipType);

                if (currentCount < needCount)
                    break;

                currentCount -= needCount;
                currentLevel++;

                SetGachaLevel(equipType, currentLevel);
                SetCurrentGachaCount(equipType, currentCount);
            }

            if (currentLevel >= config.maxLevel)
            {
                SetGachaLevel(equipType, config.maxLevel);
                SetCurrentGachaCount(equipType, 0);
            }
        }//가챠 레벨 상승용

        public List<EquipmentSO> ExecuteGacha(EquipType equipType, GachaDrawType drawType)
        {
            List<EquipmentSO> results = new List<EquipmentSO>();

            if (!GetCanDraw(equipType, drawType))
                return results;

            int drawCount = GetDrawCountByType(drawType);
            int cost = GetDrawCost(equipType, drawType);

            if (drawCount <= 0) return results;
            if (cost <= 0) return results;

            results = TryDrawGacha(equipType, drawCount);
            if (results == null || results.Count == 0)
                return results;

            if (!TrySpendCurrency(cost))
                return results;


            for (int i = 0; i < results.Count; i++)
            {
                AddEquipment(results[i]);//이곳 아직 미구현
            }

            AddDrawCount(equipType, results.Count);
            ProcessLevelUp(equipType);

            return results;
        }//가챠 함수
    }
}
