using Base.Save;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Base.Data
{
    [CreateAssetMenu(menuName = "Game/Growth/CurrencyDictionary")]
    public class CurrencyDataBaseSO : ScriptableObject
    {
        [Header("재화 전체")] 
        public List<CurrencySO> currencyList = new();
        Dictionary<CurrencyType, CurrencySO> currencyDic;

        void MakeDictionary()
        {
            currencyDic = new Dictionary<CurrencyType, CurrencySO>();
            foreach (var currency in currencyList)
            {
                currencyDic.Add(currency.type, currency);
            }
        }

        public CurrencySO GetSO(CurrencyType key)
        {
            if (currencyDic == null)
            {
                MakeDictionary();
                Debug.Log("딕셔너리를 생성했습니다. ");
            }

            if (!currencyDic.TryGetValue(key, out var currency))
            {
                Debug.LogWarning("키에 해당하는 아이템이 없습니다. ");
                return null;
            }

            Debug.Log($"{currency.name}");
            return currency;
        }
    }
}