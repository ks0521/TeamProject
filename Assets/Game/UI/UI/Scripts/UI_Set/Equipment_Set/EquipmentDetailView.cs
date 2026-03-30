using Growth.Equipment;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentDetailView : MonoBehaviour
{
    [SerializeField] private Image equipmentIcon;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI retentionEffectText;
    [SerializeField] private TextMeshProUGUI equipEffectText;

    public void SetActive(bool isActive)
    {
        gameObject.SetActive(isActive);
    }
    public void Refresh(EquipmentSO equipmentSO , int currentLevel)
    {
        equipmentIcon.sprite = equipmentSO.icon;
        nameText.text = equipmentSO.itemName;
        levelText.text = $"Lv : {currentLevel}";

        //장비 보유효과 , 장착 효과 표시해야함

    }
}
