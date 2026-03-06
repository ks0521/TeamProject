using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Personal.GyuSeong
{
    [Serializable]
    public class DropEntry
    {
        public ItemSO item; //아이템 so
        [Range(0,1)]public float chance; // 드랍 확률
        public int minAmount = 1; //최소 드랍 갯수
        public int maxAmount = 1; //최대 드랍 갯수
    }

    public struct DropedItem
    {
        public ItemSO item;
        public int amount;

        public DropedItem(ItemSO item, int amount)
        {
            this.item = item;
            this.amount = amount;
        }
    }

    [CreateAssetMenu]
    public class DropTableSO : ScriptableObject
    {
        public List<DropEntry> dropList = new();
        /// <summary> 드롭률 감안해서 드랍테이블의 아이템 뽑기</summary>
        /// <param name="dropRate">최종 드랍률</param>
        /// <returns>드랍된 아이템 갯수</returns>
        public List<DropedItem> GetDroppedItems(float dropRate)
        {
            List<DropedItem> droppedItemList = new();
            foreach (var items in dropList)
            {
                if (items.item == null) continue; //드롭리스트에 아이템 변수 없는 사건 방어
                float value = Random.Range(0f, 1f);
                if (value < items.chance * (1 + dropRate)) //확률 뽑아서 당첨이면 드롭되는 아이템 리스트에 추가
                {
                    int amount = Random.Range(items.minAmount, items.maxAmount + 1);
                    //* 고민해볼 사항 : 최소 ~ 최대 중 낮은 개수나 높은 개수에 높은 비중을 두고 만들기?
                    DropedItem dropedItem = new DropedItem(items.item, amount);
                    droppedItemList.Add(dropedItem);
                }
            }
            return droppedItemList;
        }
    }
}