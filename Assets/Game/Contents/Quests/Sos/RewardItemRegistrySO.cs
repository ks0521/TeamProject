using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//제공할 아이템들의 도감
[CreateAssetMenu(fileName = "RewardItem Registry SO", menuName = "Quest/RewardItem Registry SO")]
public class RewardItemRegistrySO : ScriptableObject
{
    [System.Serializable]
    public class ItemInfo
    {
        public int itemID;
        public string itemName;
        public Sprite itemIcon;
    }

    public List<ItemInfo> allItems;

    //ID로 아이템 정보 탐색
    public ItemInfo GetItem(int id) => allItems.Find(i => i.itemID == id);
}
