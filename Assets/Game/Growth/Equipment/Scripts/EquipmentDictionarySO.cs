using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Growth.Equipment
{
    [CreateAssetMenu(menuName = ("Game/Growth/EquipmentDictionary"))]
    public class EquipmentDictionarySO : ScriptableObject
    {
        [Header("아이템 전체")] public List<EquipmentSO> allEquipments = new();
        Dictionary<int, EquipmentSO> equipmentsDic;
        private Dictionary<EquipType, List<EquipmentSO>> equipmentsTypeDic;
        private Dictionary<(EquipType, EquipRarity, EquipQuality), EquipmentSO> equipmentPickupDic;

        void MakeDictionary()
        {
            equipmentsDic = new Dictionary<int, EquipmentSO>();
            equipmentsTypeDic = new Dictionary<EquipType, List<EquipmentSO>>();
            equipmentPickupDic = new Dictionary<(EquipType, EquipRarity, EquipQuality), EquipmentSO>();
            foreach (EquipType type in Enum.GetValues(typeof(EquipType)))
            {
                equipmentsTypeDic.Add(type, new List<EquipmentSO>());
            }
            foreach (var equipment in allEquipments)
            {
                equipmentsDic.Add(equipment.key, equipment);
                equipmentsTypeDic[equipment.equipType].Add(equipment);
                equipmentPickupDic.Add((equipment.equipType,equipment.rarity,equipment.quality),equipment);
            }
        }
        public IReadOnlyList<EquipmentSO> GetEquipListByType(EquipType type)
        {
            if(equipmentsTypeDic == null) MakeDictionary();
            //Debug.Log($"{type} 형식의 장비 개수 : {equipmentsTypeDic[type].Count}");
            return equipmentsTypeDic[type];
        }

        public IReadOnlyList<EquipmentSO> GetEquipList()
        {
            return allEquipments;
        }

        public EquipmentSO GetSO(int key)
        {
            if (equipmentsDic == null)
            {
                MakeDictionary();
                Debug.Log("딕셔너리를 생성했습니다. ");
            }
            if (!equipmentsDic.TryGetValue(key, out var item))
            {
                Debug.LogWarning("키에 해당하는 아이템이 없습니다. ");
                return null;
            }

            //Debug.Log($"{item.itemName}");
            return item;
        }
        /// <summary> 특정함 등급과 품질, 아이템 타입을 가진 아이템SO를 제공 </summary>
        /// <returns>해당하는 아이템 SO</returns>
        public bool TryPickupItem(EquipType type, EquipRarity rarity, EquipQuality quality, out EquipmentSO result)
        {
            if (equipmentsDic == null)
            {
                MakeDictionary();
                Debug.Log("딕셔너리를 생성했습니다. ");
            }
            if (!equipmentPickupDic.TryGetValue((type, rarity, quality), out EquipmentSO so))
            {
                result = null;
                return false;
            }
            result = so;
            return true;
        }
    }
}