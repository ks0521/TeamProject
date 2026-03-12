using System.Collections.Generic;
using UnityEngine;

namespace Base.Data
{
    [CreateAssetMenu(menuName = ("Game/Growth/ItemDictionary"))]
    public class ItemDictionarySO : ScriptableObject
    {
        [Header("아이템 전체")] public List<ItemSO> allItems = new();
        Dictionary<int, ItemSO> itemDic;

        void MakeDictionary()
        {
            itemDic = new Dictionary<int, ItemSO>();
            foreach (var item in allItems)
            {
                itemDic.Add(item.key, item);
            }
        }

        public ItemSO GetSO(int key)
        {
            if (itemDic == null)
            {
                MakeDictionary();
                Debug.Log("딕셔너리를 생성했습니다. ");
            }

            if (!itemDic.TryGetValue(key, out var item))
            {
                Debug.LogWarning("키에 해당하는 아이템이 없습니다. ");
                return null;
            }

            Debug.Log($"{item.itemName}");
            return item;
        }
    }
}