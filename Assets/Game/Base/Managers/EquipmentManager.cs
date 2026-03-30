using Base.Data;
using Base.Managers;
using Base.Save;
using Growth.Equipment;
using System;
using System.Collections.Generic;
using UnityEngine;

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
        public int NEED_COMBINE = 5;
        
        [SerializeField] private EquipmentSO equipItem;
        private GameDataProvider dictionarys;
        private RuntimeProgressState runtimeState;
        private ItemDropManager dropManager;
        private EventHub eventHub;
        private RuntimeEquipmentInventoryState EquipmentInventory => runtimeState.equipmentInventory;

        /// <summary> 현재 보유중인 모든 장비를 조회 </summary>
        /// <returns>EquipmentCatalog 리스트</returns>
        public List<EquipmentCatalog> AllEquipmentCatalogs()
        {
            List<EquipmentCatalog> catalogs = new();
            //장비 딕셔너리에 있는 모든 아이템 순회
            foreach (var equipment in dictionarys.equipmentTable.allEquipments)
            {
                //장비 딕셔너리에 있는 장비를 플레이어가 가지고 있을 때
                if (runtimeState.equipmentInventory.equipmentEntries.TryGetValue(equipment.key, out var value))
                {
                    catalogs.Add(new EquipmentCatalog(
                        equipment.key,
                        equipment,
                        value));
                    continue;
                }
                catalogs.Add(new EquipmentCatalog(
                    equipment.key,
                    equipment,
                    GetDefaultEquipmentEntryState()));
            }
            return catalogs;
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
        
        public bool CanEquipmentCombine(int key)
        {
            //장비가 없으면 합성불가
            if (!EquipmentInventory.equipmentEntries.ContainsKey(key)) return false;
            //장비 개수가 조합개수보다 낮으면 합성불가
            if (EquipmentInventory.equipmentEntries[key].ownedCount < 
                dictionarys.equipmentTable.GetSO(key).combineNeedAmount) return false;
            //장비도감에 다음 장비가 없으면(합성 결과의 장비가 없으면) 합성불가 
            if (!EquipmentInventory.equipmentEntries.ContainsKey(key + 1)) return false;
            return true;
        }

        public bool TryEquipmentCombine(int key)
        {
            if (!CanEquipmentCombine(key)) return false;
            EquipmentInventory.equipmentEntries[key].ownedCount -=
                dictionarys.equipmentTable.GetSO(key).combineNeedAmount;
            //해당 장비보다 +1 키 높은 아이템 획득
            dropManager.GetEquip(new DropReward()
            {
                amount = 1,
                itemSO = dictionarys.equipmentTable.GetSO(key + 1),
                rewardType = DropRewardType.Item
            });
            Debug.Log("장비 합성 완료");
            return true;
        }
        /// <summary> 기본 상태(미획득) 장비의 EquipmentEntryState 획득용 </summary>
        EquipmentEntryState GetDefaultEquipmentEntryState()
        {
            return new EquipmentEntryState() { ownedCount = 0, enhancementLevel = 0, isDiscovered = false };
        }
        
        public void Init()
        {
            eventHub = GameManager.Instance.GetGameSystem<EventHub>();
            runtimeState = GameManager.Instance.GetGameSystem<PlayerProgressManager>().Progress;
            dictionarys = GameManager.Instance.GetGameSystem<GameDataProvider>();
            dropManager = GameManager.Instance.GetGameSystem<ItemDropManager>();
        }

        public int GetOrder() => 15;

    }
}