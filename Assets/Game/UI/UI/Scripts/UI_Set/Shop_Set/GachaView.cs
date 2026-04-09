using Growth.Equipment;
using Shop.Gacha;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Shop.Gacha.GachaConfigSO;

public class GachaView : MonoBehaviour
{
    [SerializeField] EquipType equipType; //가챠 구별용
    public EquipType EquipType => equipType; //Presenter 가 가챠 타입 읽을때 사용할 예정

    [Header("뽑기 버튼")]
    [SerializeField] private Button oneDrawButton;
    [SerializeField] private Button tenDrawButton;
    [SerializeField] private Button hundredDrawButton;

    [Header("버튼 가격 텍스트")]
    [SerializeField] private TextMeshProUGUI oneDrawCostText;
    [SerializeField] private TextMeshProUGUI tenDrawCostText;
    [SerializeField] private TextMeshProUGUI hundredDrawCostText;

    [Header("가챠 정보 표시")]
    [SerializeField] private TextMeshProUGUI gachaCountText;
    [SerializeField] private TextMeshProUGUI gachaLevelText;
    [SerializeField] private Slider GachaSlider;

    private Color canTextColor = Color.white;
    private Color cannotTextColor = Color.red;


    private void BindButton(Action onOneDraw, Action onTenDraw, Action onHundredDraw)
    {
        if (oneDrawButton != null)
        {
            oneDrawButton.onClick.RemoveAllListeners();
            oneDrawButton.onClick.AddListener(()=> onOneDraw?.Invoke());
        }
        if (tenDrawButton != null)
        {
            tenDrawButton.onClick.RemoveAllListeners();
            tenDrawButton.onClick.AddListener(() => onTenDraw?.Invoke());
        }
        if (hundredDrawButton != null)
        {
            hundredDrawButton.onClick.RemoveAllListeners();
            hundredDrawButton.onClick.AddListener(() => onHundredDraw?.Invoke());
        }
    }
   
    public void SetGachaCount(int totalCount , int maxCount)
    {
        if (GachaSlider != null)
        {
            GachaSlider.value = Mathf.Clamp01((float)totalCount / maxCount);
        }
        if (gachaCountText)
        {
            gachaCountText.text = $"{totalCount} / {maxCount}";
        }
    }

    public void SetGachaLevel(int currentLevel)
    {
        gachaLevelText.text = currentLevel.ToString();
    }

    public void SetDrawCosts(int oneCost, int tenCost, int hundredCost)
    {
        if (oneDrawCostText != null)
            oneDrawCostText.text = oneCost.ToString();

        if (tenDrawCostText != null)
            tenDrawCostText.text = tenCost.ToString();

        if (hundredDrawCostText != null)
            hundredDrawCostText.text = hundredCost.ToString();
    }
    public void SetButtonStates(bool canOne, bool canTen, bool canHundred)
    {
        SetButtonState(oneDrawButton, oneDrawCostText, canOne);
        SetButtonState(tenDrawButton, tenDrawCostText, canTen);
        SetButtonState(hundredDrawButton, hundredDrawCostText, canHundred);
    }
   
    private void SetButtonState(Button button, TextMeshProUGUI costText, bool canAfford)
    {
        if (button != null)
        {
            button.interactable = canAfford;
        }

        if (costText != null)
        {
            costText.color = canAfford ? canTextColor : cannotTextColor;
        }
    }//버튼 활성화/비활성화 , 가격 색상 변경

}

