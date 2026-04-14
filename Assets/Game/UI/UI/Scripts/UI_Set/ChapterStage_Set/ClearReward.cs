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
    [SerializeField] TextMeshProUGUI rewardTypeText;
    [SerializeField] Image[] rewardImg;
    [SerializeField] TextMeshProUGUI[] rewardText;

    [SerializeField] Button checkButton;

    public void SetReward(List<DropReward> rewards, string titleText)
    {
        for (int i = 0; i < rewardImg.Length; i++)
        {
            rewardImg[i].gameObject.SetActive(false);

            if (i < rewardText.Length && rewardText[i] != null)
            {
                rewardText[i].text = null;

            }
        }

        if (rewards.Count <= 0)
        {
            Debug.LogError(rewards.Count);
        }
        rewardTypeText.text = titleText;

        int currentSlot = 0;

        for (int i = 0; i < rewards.Count; i++)
        {
            var reward = rewards[i];
            if (reward.amount == 0)
            {
                Debug.LogWarning($"{i} 번째 보상이 0 입니다?");
            }
            switch (reward.rewardType)
            {
                case DropRewardType.Currency:
                    SetSlot(ref currentSlot, reward.amount.ToString(), reward.currencySO.icon);
                    break;

                case DropRewardType.Item:
                    SetSlot(ref currentSlot, reward.amount.ToString(), reward.itemSO.icon);
                    break;
            }
        }
    }
    private void SetSlot(ref int slot, string valueText, Sprite icon)
    {
        if (slot >= rewardImg.Length) return;

        rewardImg[slot].sprite = icon;
        rewardImg[slot].gameObject.SetActive(true);

        if (icon == null)
        {
            Debug.LogError($"{rewardImg[slot].sprite} 비었음");

        }

        if (slot < rewardText.Length && rewardText[slot] != null)
        {
            rewardText[slot].text = valueText;
            if (rewardText[slot].text == null)
            {
                Debug.LogError(rewardText[slot].text);
            }
        }

        slot++;
    }

}
