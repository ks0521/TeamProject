using Base.Data;
using Base.Managers;
using Base.Save;
using Growth.Equipment;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace Base.Manager
{
    /// <summary> UI에서 사용하기 위한 장비들 정보 묶음
    /// 키와 SO, 해당 장비의 상세수치까지 연결된 데이터</summary>
    [Serializable]
    public class EquipmentCatalog
    {
        public int key; //장비의 키
        public EquipmentSO equipment; //장비의 SO
        public EquipmentEntryState state; //개수, 획득여부, 강화횟수
        public EquipmentCatalog(int inputKey, EquipmentSO inputEquipment, EquipmentEntryState inputState)
        {
            key = inputKey;
            equipment = inputEquipment;
            state = inputState;
        }
    }

    /// <summary> 플레이어가 가지고 있는 장비를 관리하고 장비 강화, 장착 시 스탯 변화등을 관리한다. </summary>
    public class EquipmentManager : MonoBehaviour, IManager
    {
        [SerializeField] private Growth.Equipment.EquipmentSO equipItem;
        private ScriptableObjectHub dictionarys;
        private RuntimeProgressState runtimeState;
        private EventHub eventHub;
        private RuntimeEquipmentInventoryState equipmentInventory => runtimeState.equipmentInventory;

        /// <summary> 현재 보유중인 모든 장비를 조회 </summary>
        /// <returns>EquipmentCatalog 리스트</returns>
        public List<EquipmentCatalog> AllEquipmentCatalogs()
        {
            List<EquipmentCatalog> catalogs = new();
            foreach (var equipment in runtimeState.equipmentInventory.equipmentEntries)
            {
                catalogs.Add(new EquipmentCatalog(
                    equipment.Key,
                    dictionarys.equipmentTable.GetSO(equipment.Key),
                    equipment.Value));
            }
            return new List<EquipmentCatalog>();
        }
        /// <summary> 찾으려 하는 특정 키의 장비 정보를 확인함</summary>
        /// <returns>있으면 true, 없으면 false (catalog = null)</returns>
        public bool TryGetEquipmentCatalog(int key, out EquipmentCatalog catalog)
        {
            EquipmentSO equip = dictionarys.equipmentTable.GetSO(key);
            EquipmentEntryState state = runtimeState.equipmentInventory.equipmentEntries[key];
            catalog = null;
            if (equip == null || state == null)
            {
                return false;
            }
            catalog = new(key, equip, state);
            return true;
        }
        
        public void Init()
        {
            eventHub = GameManager.Instance.GetGameSystem<EventHub>();
            runtimeState = GameManager.Instance.GetGameSystem<PlayerProgressManager>().Progress;
            dictionarys = GameManager.Instance.GetGameSystem<GameDataProvider>().hub;
        }

        /// <summary> 장비 장착 </summary>
        /// <param name="equipment"></param>
        public void Equip(EquipmentSO equipment)
        {
            if (equipItem == equipment)
            {
                Debug.Log("이미 장착중인 아이템입니다 ");
                return;
            }
            runtimeState.equipment.equippedWeponKey = equipment.key;
            //장작후 스탯 계산 필요
        }

        public int GetOrder() => 15;

    }
}