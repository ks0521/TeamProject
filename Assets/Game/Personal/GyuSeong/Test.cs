using Base.Data;
using Base.Manager;
using Base.Managers;
using Base.Save;
using Growth.Equipment;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour, IManager
{
    // Update is called once per frame
    [SerializeField]private SaveProgressData saveProgressData;
    [SerializeField] private RuntimeProgressData runProgressData;
    [SerializeField] private RuntimeStatus runStat;
    [SerializeField] private StatusCalculator calc;
    [SerializeField] private EquipmentManager equip;
    [SerializeField] private EquipmentDictionarySO dic;
    [SerializeField] private ItemDropManager dropManager;
    [SerializeField] private EventHub hub;
    private void Start()
    {
        Debug.Log("1. 현재 가지고 있는 아이템 출력 / 2. 모든 아이템 획득 / 3. 현재 가지고 있는 모든 아이템 제거");
    }

    void Update()
    {
        #if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.F1))
        {
            Debug.Log("모든 장비 확인");
            PrintAllItems();
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            Debug.Log("모든 아이템 획득");
            foreach (var VARIABLE in dic.allEquipments)
            {
                dropManager.GetReward(new DropReward(){amount = 1, itemSO = VARIABLE, rewardType = DropRewardType.Item});
            }
            PrintAllItems();
        }
        if (Input.GetKeyDown(KeyCode.F3))
        {
            Debug.Log("모든 아이템 제거");
            runProgressData.equipmentInventory.equipmentEntries = new Dictionary<int, EquipmentEntryState>() ;
            PrintAllItems();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            dropManager.GetReward
            (new DropReward(){
                amount = 10000, currencyType = CurrencyType.GOLD, rewardType = DropRewardType.Currency
            });
            dropManager.GetReward
            (new DropReward(){
                amount = 10000, currencyType = CurrencyType.STATSTONE, rewardType = DropRewardType.Currency
            });
            hub.GetCurrency();
        }
        #endif
    }

    void PrintAllItems()
    {
        List<EquipmentCatalog> catalogs = equip.GetAllEquipmentCatalogs();
        foreach (var catalog in catalogs)
        {
            Debug.Log($"{catalog.key}, {catalog.equipment.name}, {catalog.state.ownedCount}, {catalog.state.isDiscovered}");
        }
    }
    public int GetOrder() => 999;

    public void Init()
    {
        equip = GameManager.Instance.GetGameSystem<EquipmentManager>();
        runProgressData = GameManager.Instance.GetGameSystem<ProgressManager>().progress;
        dic = GameManager.Instance.GetGameSystem<GameDataProvider>().equipmentTable;
        dropManager = GameManager.Instance.GetGameSystem<ItemDropManager>();
        hub = GameManager.Instance.GetGameSystem<EventHub>();
    }
}
