using Base.Managers;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Reward_Set : MonoBehaviour
{
    [SerializeField] Image [] rewardImg;
    [SerializeField] private TextMeshProUGUI [] text;
    


    public void SetReward(StageEntry stageEntry)
    {
        text[0].text = stageEntry.stageSO.dropTable.rewardGold.ToString();
        text[1].text = stageEntry.stageSO.dropTable.rewardStatStone.ToString();

    }
}
