using Growth.Equipment;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Shop.Gacha
{
    [CreateAssetMenu(menuName = "Game/Gacha/Gacha Config")]
    public class GachaConfigSO : ScriptableObject
    {

        [Header("가챠 타입")]
        public EquipType targetEquipType;

        [Header("가챠 레벨")]
        public int defaultLevel = 1;
        public int maxLevel = 100;

        [Header("레벨업 필요 뽑기 횟수")]
        public List<int> levelUpDraw = new();

        [Header("뽑기 비용")]
        public List<GachaCostData> drawCosts = new();

        [Header("기본 등급 가중치")]
        public List<GachaRarityWeight> baseRarityWeights = new();

        [Header("레벨별 등급 확률 가중치")]
        public List<GachaLevelRarityBonus> levelRarityBonuses = new();

        [Header("품질 확률")]
        public List<GachaQualityWeight> qualityWeights = new();

        public enum GachaDrawType
        {
            One,
            Ten,
            Hundred
        }

        [Serializable]
        public class GachaCostData
        {
            public GachaDrawType drawType;
            public int count;
            public int cost;
        }
        [Serializable]
        public class GachaRarityWeight
        {
            public EquipRarity rarity;
            public int weight;
        }
        [Serializable]
        public class GachaQualityWeight
        {
            public EquipQuality quality;
            public int weight;
        }
        [Serializable]
        public class GachaLevelRarityBonus
        {
            public int level;
            public List<GachaRarityWeight> bonusWeights = new List<GachaRarityWeight>();
        }


    }

}
