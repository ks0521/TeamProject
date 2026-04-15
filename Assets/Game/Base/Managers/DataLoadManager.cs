using Base.Data;
using Battle;
using Cysharp.Threading.Tasks;
using Growth.Equipment;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Base.Manager
{
    public class DataLoadManager : MonoBehaviour
    {
        public static DataLoadManager Instance;
        private GameDataDictionaries dic;
        private bool isLoaded = false;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        public async UniTask InitAllData(GameDataDictionaries dic)
        {
            if (isLoaded) return;
            this.dic = dic;
            await UniTask.WhenAll
            (
                LoadAllStage(),
                LoadAllEquipment(),
                LoadAllItems(),
                LoadAllCurrency()
            );


            isLoaded = true;
        }

        private async UniTask LoadAllStage()
        {
            var handle = Addressables.LoadAssetsAsync<StageSO>("SO/Stage", null);
            await handle.Task;
            if (handle.Status != AsyncOperationStatus.Succeeded)
            {
                throw new Exception("StageSO 전체 로드 실패");
            }
            dic.stageTable.stageList = new List<StageSO>(handle.Result);
            Debug.Log($"로딩된 Stage 개수 : {dic.stageTable.stageList.Count}");
        }

        private async UniTask LoadAllEquipment()
        {
            var handle = Addressables.LoadAssetsAsync<EquipmentSO>("SO/Equipment", null);
            await handle.Task;
            if (handle.Status != AsyncOperationStatus.Succeeded)
            {
                throw new Exception("EquipmentSO 전체 로드 실패");
            }
            dic.equipmentTable.allEquipments = new List<EquipmentSO>(handle.Result);
            Debug.Log($"로딩된 Equipment 개수 : {dic.equipmentTable.allEquipments.Count}");
        }

        private async UniTask LoadAllItems()
        {
            var handle = Addressables.LoadAssetsAsync<ItemSO>("SO/Item", null);
            await handle.Task;
            if (handle.Status != AsyncOperationStatus.Succeeded)
            {
                throw new Exception("ItemSO 전체 로드 실패");
            }
            dic.itemTable.allItems = new List<ItemSO>(handle.Result);
            Debug.Log($"로딩된 Item 개수 : {dic.itemTable.allItems.Count}");
        }

        private async UniTask LoadAllCurrency()
        {
            var handle = Addressables.LoadAssetsAsync<CurrencySO>("SO/Currency", null);
            await handle.Task;
            if (handle.Status != AsyncOperationStatus.Succeeded)
            {
                throw new Exception("CurrencySO 전체 로드 실패");
            }
            dic.currencyTable.currencyList = new List<CurrencySO>(handle.Result);
            Debug.Log($"로딩된 Currency 개수 : {dic.itemTable.allItems.Count}");
        }
    }
}