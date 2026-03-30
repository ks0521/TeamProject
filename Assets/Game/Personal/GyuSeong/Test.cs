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
    [SerializeField]private GameSaveData saveData;
    [SerializeField] private RuntimeProgressState runProgressState;
    [SerializeField] private PlayerRuntimeStatus runStat;
    [SerializeField] private StatusCalculator calc;
    [SerializeField] private EquipmentManager equip;
    [SerializeField] private EquipmentDictionarySO dic;
    [SerializeField] private ItemDropManager dropManager;
    private void Start()
    {
        Debug.Log("1. 모든 아이템 획득 / 2. 현재 가지고 있는 아이템 출력 / 3. 현재 가지고 있는 모든 아이템 제거");
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
                dropManager.GetEquip(new DropReward(){amount = 1, itemSO = VARIABLE, rewardType = DropRewardType.Item});
            }
            PrintAllItems();
        }
        if (Input.GetKeyDown(KeyCode.F3))
        {
            Debug.Log("모든 아이템 제거");
            runProgressState.equipmentInventory.equipmentEntries = new Dictionary<int, EquipmentEntryState>() ;
            PrintAllItems();
        }
        #endif
    }

    void PrintAllItems()
    {
        List<EquipmentCatalog> catalogs = equip.AllEquipmentCatalogs();
        foreach (var catalog in catalogs)
        {
            Debug.Log($"{catalog.key}, {catalog.equipment.name}, {catalog.state.ownedCount}, {catalog.state.isDiscovered}");
        }
    }
    public int GetOrder() => 999;

    public void Init()
    {
        equip = GameManager.Instance.GetGameSystem<EquipmentManager>();
        runProgressState = GameManager.Instance.GetGameSystem<PlayerProgressManager>().progress;
        dic = GameManager.Instance.GetGameSystem<GameDataProvider>().equipmentTable;
        dropManager = GameManager.Instance.GetGameSystem<ItemDropManager>();
    }
}
