using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Growth.Equipment
{
    [CreateAssetMenu(menuName = ("Game/Growth/EquipmentDictionary"))]
    public class EquipmentDictionarySO : ScriptableObject
    {
        [FormerlySerializedAs("allItems")] [Header("아이템 전체")] public List<EquipmentSO> allEquipments = new();
        Dictionary<int, EquipmentSO> equipmentsDic;

        void MakeDictionary()
        {
            equipmentsDic = new Dictionary<int, EquipmentSO>();
            foreach (var euqipment in allEquipments)
            {
                equipmentsDic.Add(euqipment.key, euqipment);
            }
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