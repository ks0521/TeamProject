using Base.Managers;
using Battle;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClearReward : MonoBehaviour
{
    [SerializeField] Image [] rewardImg;
    [SerializeField] TextMeshProUGUI[] rewardText;

    [SerializeField] Button checkButton;

    public void SetReward(List<DropReward> rewards)
    {
        for (int i = 0; i < rewardImg.Length; i++)
        {
            rewardImg[i].gameObject.SetActive(false);

            if (i < rewardText.Length && rewardText[i] != null)
            {
                rewardText[i].text = null;

            }
        }

        int currentSlot = 0;

        for (int i = 0; i < rewards.Count; i++)
        {
            var reward = rewards[i];

            switch (reward.rewardType)
            {
                case DropRewardType.Currency:
                    SetSlot(ref currentSlot , reward.amount.ToString() , reward.currencySO.icon);
                    break;

                case DropRewardType.Item:
                    SetSlot(ref currentSlot, reward.amount.ToString(), reward.itemSO.icon);
                    break;
            }
        }
    }
    private void SetSlot(ref int slot , string valueText , Sprite icon)
    {
        if (slot >= rewardImg.Length) return;

        rewardImg[slot].sprite = icon;
        rewardImg[slot].gameObject.SetActive(true);

        if (slot < rewardText.Length && rewardText[slot] != null)
        {
            rewardText[slot].text = valueText;
        }

        slot++;
    }
   
}
