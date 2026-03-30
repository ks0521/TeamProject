using Growth.Equipment;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentSlotView : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Button btn;

    public void SetSlot(EquipmentSO equipmentSO , int level , Action action)
    {
        icon.sprite = equipmentSO.icon;
        levelText.text = level.ToString();

        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() => action?.Invoke());
    }
}
