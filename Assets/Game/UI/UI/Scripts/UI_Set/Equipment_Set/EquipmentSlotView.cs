using Growth.Equipment;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentSlotView : MonoBehaviour
{
    public EquipType equipType;

    [Header("기본 UI")]
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Button btn;

    [Header("등급 표시")]
    [SerializeField] private Image frame;

    [Header("개수 표시")]
    [SerializeField] private Image numberfill;
    [SerializeField] private TextMeshProUGUI numberText;
    public void SetSlot(EquipmentSO equipmentSO , int level , Action action)
    {
        icon.sprite = equipmentSO.icon;
        levelText.text = level.ToString();

        ApplyRarity(equipmentSO);

        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() => action?.Invoke());

        //numberfill.fillAmount = 현재 장비 개수 / 합성에 필요한 장비 개수 <- 이미지 파일 표시
        //numberText.Text = 현재 장비 / 합성에 필요한 장비 <- 텍스트 표시
    }
    void ApplyRarity(EquipmentSO so)
    {
        if (frame == null) return;

        switch (so.rarity)
        {
            case EquipRarity.Common:
                frame.color = Color.gray;
                break;

            case EquipRarity.UnCommon:
                frame.color = Color.green;
                break;

            case EquipRarity.Rare:
                frame.color = Color.blue;
                break;

            case EquipRarity.Unique:
                frame.color = Color.yellow;
                break;
        }
    }//등급 별로 아이템 프레임 색깔 변경
}
