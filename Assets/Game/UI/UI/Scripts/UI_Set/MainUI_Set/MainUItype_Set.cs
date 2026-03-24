using Base.Save;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainUItype_Set : MonoBehaviour
{
    [Header("타입 설정")]
    [SerializeField]  CurrencyType currencyType;
    public CurrencyType Currency
    {
        get {  return currencyType; }
    }

    [Header("Text 연결")]
    [SerializeField] private TextMeshProUGUI valueText;

    [Header("게이지 요소")]
    [SerializeField] private Slider slider;
        
   public void SetUI(int value , int maxValue = 1)
   {
        valueText.text = value.ToString();

        if (slider != null)
        {
            slider.value = value / maxValue;
        }
   }
}
