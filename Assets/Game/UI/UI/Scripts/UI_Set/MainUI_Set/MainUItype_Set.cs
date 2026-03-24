using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainUItype_Set : MonoBehaviour
{
    [Header("담당 타입")]
    [SerializeField] GameObject types;

    [Header("UI 연결용")]
    [SerializeField] private TextMeshProUGUI valueText;
    [SerializeField] private Image uiImage;
    [SerializeField] private Slider slider;
        
   public void SetUI()
    {

    }
}
