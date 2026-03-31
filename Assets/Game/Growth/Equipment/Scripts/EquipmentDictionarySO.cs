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

        void MakeDictionary()
        {
            equipmentsDic = new Dictionary<int, EquipmentSO>();
            equipmentsTypeDic = new Dictionary<EquipType, List<EquipmentSO>>();
            foreach (EquipType type in Enum.GetValues(typeof(EquipType)))
            {
                equipmentsTypeDic.Add(type, new List<EquipmentSO>());
            }
            foreach (var equipment in allEquipments)
            {
                equipmentsDic.Add(equipment.key, equipment);
                equipmentsTypeDic[equipment.equipType].Add(equipment);
            }
        }
        public IReadOnlyList<EquipmentSO> GetEquipListByType(EquipType type)
        {
            if(equipmentsTypeDic == null) MakeDictionary();
            Debug.Log($"{type} 형식의 장비 개수 : {equipmentsTypeDic[type].Count}");
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

            Debug.Log($"{item.itemName}");
            return item;
        }
    }
}