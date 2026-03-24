using Base.Save;
using Battle;
using Growth.StatUpgrade;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;
using UnityEngine.UI;
public class StatItemView : MonoBehaviour
{
    [Header("담당 타입")]
    [SerializeField] public StatusType statusType;
    public StatusType StatType
    {
        get { return statusType; }
    }

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
    public void BindLevelUp(Action action)
    {
        levelUpButton.onClick.RemoveAllListeners();
        levelUpButton.onClick.AddListener(() => action?.Invoke());
    }//버튼 OnClick 에 함수 넣어주는 함수
    public void RefreshUI(StatEntry statEntry , int currentLevel, float currentValue ,float nextValue , float cost , bool canLevelUp , bool isUnlock)
    {
        levelUpButton.interactable = canLevelUp;
        if (canLevelUp)
        {
            levelUpButton.image.color = Color.yellow;
        }
        else
        {
            levelUpButton.image.color = Color.gray;
        }
        lockPanel.SetActive(!isUnlock);
        unlockLevelText.text = $"Lv : {statEntry.unlockLevel} 개방";

        statsNameText.text = statEntry.type.ToString();
        if (currentLevel < statEntry.maxLevel)
        {
            statsLevelText.color = Color.white;
            currentStats.color = Color.white;

            nextStats.enabled = true;
            levelupcost.enabled = true;
            costImage.enabled = true;
            levelUpButton.gameObject.SetActive(true);

            statsLevelText.text = currentLevel.ToString();
            currentStats.text = currentValue.ToString("0.00");
            nextStats.text = nextValue.ToString("0.00");
            levelupcost.text = cost.ToString("0");
        }
        else
        {
            statsLevelText.color = Color.yellow;
            currentStats.color = Color.yellow;

            nextStats.enabled = false;
            levelupcost.enabled = false;
            costImage.enabled = false;
            levelUpButton.gameObject.SetActive(false);

            statsLevelText.text = "Lv : MAX";
            currentStats.text = currentValue.ToString();
        }
    }//스텟 UI 표시



}
