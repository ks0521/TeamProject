using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class Atk_Set : MonoBehaviour
{
    [Header("UI 참조")]
    [SerializeField] private TextMeshProUGUI statsLevelText;
    [SerializeField] private TextMeshProUGUI currentStats;
    [SerializeField] private TextMeshProUGUI nextStats;
    [SerializeField] private TextMeshProUGUI levelupcost;
    [SerializeField] private Button levelUpButton;

    [Header("잠금 UI")]
    [SerializeField] private GameObject lockPanel;
    [SerializeField] private TextMeshProUGUI unlockLevelText;

    public void RefreshUI(int level, float currentValue ,float nextValue , int cost , bool isInteractable , bool isLock , int unlockLevel)
    {
        statsLevelText.text = $"Lv.{level}";
        currentStats.text = currentValue.ToString("0");
        nextStats.text = nextValue.ToString("0");
        levelupcost.text = cost.ToString();
        levelUpButton.interactable = isInteractable;

        if (isInteractable)
        {
            levelUpButton.image.color = Color.yellow;
        }
        else
        {
            levelUpButton.image.color = Color.gray;
        }
        lockPanel.SetActive(isLock);
        unlockLevelText.text = $"Lv : {unlockLevel} 개방";

    }//함수가 너무 많아서 하나로 묶은 버전
    public void SetLevelText(int level)
    {
        statsLevelText.text = $"Lv.{level}";
    }
    public void SetCurrentStatText(float value)
    {
        currentStats.text = value.ToString("0");
    }
    public void SetNextStatText(float value)
    {
        nextStats.text = value.ToString("0");
    }
    public void SetCostText(int cost)
    {
        levelupcost.text = cost.ToString();
    }
    public void SetButtonInteractable(bool isInteractable)
    {
        levelUpButton.interactable = isInteractable;

        if (isInteractable)
        {
            levelUpButton.image.color = Color.yellow;
        }
        else
        {
            levelUpButton.image.color = Color.gray;
        }
    }
    public void BindLevelUp(Action action)
    {
        levelUpButton.onClick.RemoveAllListeners();
        levelUpButton.onClick.AddListener(() => action?.Invoke());
    }//버튼 잠금용 함수(비용이 부족할때 완전 클릭이 안되게 하는 함수)
    public void SetLockStat(bool isLock , int unlockLevel)
    {
        lockPanel.SetActive(isLock);
        unlockLevelText.text = $"Lv : {unlockLevel} 개방";
    }
}
