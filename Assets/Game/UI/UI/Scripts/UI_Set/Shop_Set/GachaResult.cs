using Growth.Equipment;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Shop.Gacha
{
    public class GachaResult : MonoBehaviour
    {
        [Header("확인 닫기 버튼")]
        [SerializeField] Button checkButton;
        [SerializeField] Button retryButton;

        [Header("장비 프리팹")]
        [SerializeField] ResultItem prefab;

        [Header("프리팹 생성 위치")]
        [SerializeField] private Transform canvas;

        private List<GameObject> spawnedItems = new List<GameObject>();

        public void Show(List<EquipmentSO> results)
        {
            for (int i = 0; i < spawnedItems.Count; i++)
            {
                Destroy(spawnedItems[i]);
            }
            spawnedItems.Clear();

            for (int i = 0; i < results.Count; i++)
            {
                ResultItem item = Instantiate(prefab, canvas);

                ApplyItem(item, results[i]);

                spawnedItems.Add(item.gameObject);
            }
        }
        private void ApplyItem(ResultItem item, EquipmentSO so)
        {
            if (item == null || so == null) return;

            item.SetData(so);
        }


        public void BindButton(Action check , Action retry)
        {
            checkButton.onClick.RemoveAllListeners();
            retryButton.onClick.RemoveAllListeners();

            checkButton.onClick.AddListener(() => check?.Invoke());
            retryButton.onClick.AddListener(() => retry?.Invoke());
        }

    }

}
