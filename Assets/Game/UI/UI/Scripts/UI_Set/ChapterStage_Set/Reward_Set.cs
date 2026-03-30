using Base.Managers;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Reward_Set : MonoBehaviour
{
    [SerializeField] Image[] rewardImg;

    [SerializeField] Sprite gold;
    [SerializeField] Sprite stone;
    [SerializeField] Sprite exp;

    public void SetReward(StageEntry stageEntry)
    {
        for (int i = 0; i < rewardImg.Length; i++)
        {
            rewardImg[i].gameObject.SetActive(false);
        }

        var dropTable = stageEntry.stageSO.dropTable;

        if (dropTable == null)
        {
            return;
        }

        int currentSlot = 0;

        if (dropTable.rewardExp > 0)
        {
            SetSlot(ref currentSlot , exp);
        }
        
        foreach (var reward in dropTable.dropList)
        {
            switch (reward.rewardType)
            {
                case DropRewardType.Currency:
                    SetSlot(ref currentSlot, reward.currencySO.img);
                    break;
               
                case DropRewardType.Item:
                    SetSlot(ref currentSlot, reward.itemSO.icon);
                    break;
            }
        }
        //3.24 (규성) 드랍테이블 SO의 요소가 바뀌게되면서 코드 수정할 소요가 생겨 임시로 변경해놓았습니다. 
        //내일 아침에 이부분 해명하라고 말씀해주세요!! 
        /*if (dropTable.rewardGold > 0 && gold != null)
        {
            SetSlot(ref currentSlot , gold);
        }
        if (dropTable.rewardStatStone > 0 && stone != null)
        {
            SetSlot(ref currentSlot , stone);
        }
        if (dropTable.rewardExp > 0 && exp != null)
        {
            SetSlot (ref currentSlot , exp);
        }

        for (int i= 0; i < dropTable.dropList.Count; i++)
        {
            var dropItem = dropTable.dropList[i].item;
            SetSlot(ref currentSlot , dropItem.icon);
        }*/
    }
    private void SetSlot(ref int slot, Sprite icon)
    {
        if (slot < rewardImg.Length)
        {
            rewardImg[slot].sprite = icon;
            rewardImg[slot].gameObject.SetActive(true);
            slot++;
        }
    }
}
