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
        private RuntimeProgressData runtimeData;
        private ItemDropManager dropManager;
        private EventHub eventHub;
        private Dictionary<int, EquipmentEntryState> EquipmentInventory => runtimeData.equipmentInventory.equipmentEntries;

        #region 탐색
        /// <summary> 특정 타입의 장비 관리 상태 가져오기 </summary>
        /// <param name="type"> 찾고자하는 장비 타입</param>
        /// <returns>type의 모든 아이템 카탈로그</returns>
        public List<EquipmentCatalog> GetEquipmentCatalogs(EquipType type)
        {
            List<EquipmentCatalog> catalogs = new();
            //type 장비를 모두 가져오기
            foreach (var equipment in dictionarys.equipmentTable.GetEquipListByType(type))
            {
                //장비 딕셔너리에 있는 장비를 플레이어가 가지고 있을 때
                if (runtimeData.equipmentInventory.equipmentEntries.TryGetValue(equipment.key, out var value))
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
        /// <summary> 현재 도감에 있는 모든 장비를 조회 </summary>
        /// <returns>EquipmentCatalog 리스트</returns>
        public List<EquipmentCatalog> GetAllEquipmentCatalogs()
        {
            List<EquipmentCatalog> catalogs = new();
            //type 장비를 모두 가져오기
            foreach (var equipment in dictionarys.equipmentTable.GetEquipList())
            {
                //장비 딕셔너리에 있는 장비를 플레이어가 가지고 있을 때
                if (runtimeData.equipmentInventory.equipmentEntries.TryGetValue(equipment.key, out var value))
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
        /// <returns>있으면 true, 없으면 false (catalog = default)</returns>
        public bool TryGetEquipmentCatalog(int key, out EquipmentCatalog catalog)
        {
            EquipmentSO equip = dictionarys.equipmentTable.GetSO(key);
            if (equip == null)
            {
                Debug.LogWarning("EquipMentManager : 찾으려는 키의 장비가 없습니다. ");
                catalog = null;
                return false;
            }
            //장비 획득정보가 있으면 해당 정보를 카탈로그화시켜서 반환
            if (runtimeData.equipmentInventory.equipmentEntries.TryGetValue(key, out EquipmentEntryState state))
            {
                catalog = new EquipmentCatalog(inputKey: equip.key, inputEquipment: equip, inputState: state);
                return true;
            }
            //장비 획득정보가 없으면 기본값을 카탈로그화시켜서 반환
            catalog = new EquipmentCatalog(inputKey: equip.key, inputEquipment: equip,
                inputState: GetDefaultEquipmentEntryState());
            return true;
        }
        #endregion

        #region 장착
        /// <summary> 장비 장착 </summary>
        /// <param name="equipment"></param>
        public void Equip(EquipmentSO equipment)
        {
            if (equipItem == equipment)
            {
                Debug.Log("이미 장착중인 아이템입니다 ");
                return;
            }
            runtimeData.equipment.equippedWeponKey = equipment.key;
            eventHub.EquipChenged(equipment);
            //장작후 스탯 계산 필요
        }
        #endregion

        #region 합성
        /// <summary> 장비 합성이 가능한지 확인</summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool CanEquipmentCombine(int key)
        {
            //장비가 없으면 합성불가
            if (!EquipmentInventory.ContainsKey(key)) return false;
            //장비도감에 다음 장비가 없으면(합성 결과의 장비가 없으면) 합성불가 
            if (!dictionarys.equipmentTable.GetSO(key+1)) return false;
            //장비 개수가 조합개수보다 낮으면 합성불가
            if (EquipmentInventory[key].ownedCount < 
                dictionarys.equipmentTable.GetSO(key).combineNeedAmount) return false;
            return true;
        }
        public bool TryEquipmentCombine(int key)
        {
            if (!CanEquipmentCombine(key)) return false;
            EquipmentInventory[key].ownedCount -=
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
        #endregion

        #region 강화
        /// <summary> 해당 장비 강화가 가능한지 확인하는 메서드</summary>
        /// <returns></returns>
        public bool CanEnhanceEquipment(EquipmentSO equipmentSo)
        {
            //최대레벨 제한 처리 추가필요
            if (!EquipmentInventory.ContainsKey(equipmentSo.key)) return false;
            int cost = (EquipmentInventory[equipmentSo.key].enhancementLevel + 1) * equipmentSo.UpgradeNeedCost;
            //Debug.Log($"CanEnhance : {equipmentSo.itemName}, {cost}, {runtimeData.currency.gold}");
            if (cost > runtimeData.currency.gold) return false;
            return true;
        }
        
        public bool TryEnhanceEquipment(EquipmentSO equipmentSo)
        {
            if (!CanEnhanceEquipment(equipmentSo)) return false;
            EquipmentInventory[equipmentSo.key].enhancementLevel += 1;
            int cost = (EquipmentInventory[equipmentSo.key].enhancementLevel + 1) * equipmentSo.UpgradeNeedCost;
            runtimeData.currency.gold -= cost;
            eventHub.CurrencyChange(CurrencyType.GOLD, runtimeData.currency.gold);
            eventHub.EquipEnhanced(equipmentSo);
            
            return true;
        }
        #endregion
        
        
        
        /// <summary> 기본 상태(미획득) 장비의 EquipmentEntryState 획득용 </summary>
        EquipmentEntryState GetDefaultEquipmentEntryState()
        {
            return new EquipmentEntryState() { ownedCount = 0, enhancementLevel = 0, isDiscovered = false };
        }
        
        public void Init()
        {
            eventHub = GameManager.Instance.GetGameSystem<EventHub>();
            runtimeData = GameManager.Instance.GetGameSystem<ProgressManager>().Progress;
            dictionarys = GameManager.Instance.GetGameSystem<GameDataProvider>();
            dropManager = GameManager.Instance.GetGameSystem<ItemDropManager>();
        }

        public int GetOrder() => 15;

    }
}