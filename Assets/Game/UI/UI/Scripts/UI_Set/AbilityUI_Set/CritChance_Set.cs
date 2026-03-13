using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class CritChance_Set : MonoBehaviour
{
    [Header("UI 참조")]
    [SerializeField] private TextMeshProUGUI statsNameText;
    [SerializeField] private TextMeshProUGUI statsLevelText;
    [SerializeField] private TextMeshProUGUI currentStats;
    [SerializeField] private TextMeshProUGUI nextStats;
    [SerializeField] private TextMeshProUGUI levelupcost;
    [SerializeField] private Image costImage;
    [SerializeField] private Button levelUpButton;

    [Header("잠금 UI")]
    [SerializeField] private GameObject lockPanel;
    [SerializeField] private TextMeshProUGUI unlockLevelText;



    public void RefreshUI(int statslevel, int maxLevel, float currentValue, float nextValue, int cost, bool isInteractable, int playerLevel, int unlockLevel)
    {
        levelUpButton.interactable = isInteractable;

        if (isInteractable)
        {
            levelUpButton.image.color = Color.yellow;
        }
        else { levelUpButton.image.color = Color.gray; }

        if (playerLevel >= unlockLevel)
        {
            lockPanel.SetActive(false);
        }
        unlockLevelText.text = $"Lv : {unlockLevel} 개방";

        if (statslevel < maxLevel)
        {
            statsLevelText.text = $"{statslevel}";
            currentStats.text = currentValue.ToString("0");
            nextStats.text = nextValue.ToString("0");
            levelupcost.text = cost.ToString();
        }
        else
        {
            currentStats.color = Color.yellow;
            statsLevelText.color = Color.yellow;
            nextStats.enabled = false;
            levelupcost.enabled = false;
            levelUpButton.gameObject.SetActive(false);
            costImage.enabled = false;

            statsLevelText.text = $"MAX";

        }



    }

    public void BindLevelUp(Action action)
    {
        levelUpButton.onClick.RemoveAllListeners();
        levelUpButton.onClick.AddListener(() => action?.Invoke());
    }//버튼 OnClick 에 함수 넣어주는 함수
}
