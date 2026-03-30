using Growth.Equipment;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentDetailView : MonoBehaviour
{
    [Header("UI 표시")]
    [SerializeField] private Image equipmentIcon;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI retentionEffectText;
    [SerializeField] private TextMeshProUGUI equipEffectText;

    [Header("버튼")]
    [SerializeField] private Button equipBtn;
    [SerializeField] private Button levelupBtn;
    [SerializeField] private Button composingBtn;


    public void SetActive(bool isActive)
    {
        gameObject.SetActive(isActive);
    }
    public void Refresh(EquipmentSO equipmentSO, int currentLevel)
    {
        equipmentIcon.sprite = equipmentSO.icon;
        nameText.text = equipmentSO.itemName;
        levelText.text = $"Lv : {currentLevel}";

        //장비 보유효과 , 장착 효과 표시해야함
        retentionEffectText.text = RetentionEffect(equipmentSO, currentLevel);
        equipEffectText.text = EquipEffect(equipmentSO);
    }

    string RetentionEffect(EquipmentSO so, int level) // level 은 나중에 강화 수치로 변경하시면 될거 같습니다.
    {
        StringBuilder sb = new StringBuilder();//자동으로 줄 바꿈 해주는 역할

        if (so.incAtk > 0) sb.AppendLine($"공격력 +{so.incAtk * level}"); //AppendLine : 기존 내용 뒤에 내용추가하고 줄바꿈해주는 역할
        if (so.multipleAtk > 0) sb.AppendLine($"공격력 +{so.multipleAtk * level}%");

        if (so.incHp > 0) sb.AppendLine($"체력 +{so.incHp * level}");
        if (so.multipleHp > 0) sb.AppendLine($"체력 +{so.multipleHp * level}%");
        if (so.dmgReduce > 0) sb.AppendLine($"받는 피해 비율 감소 +{so.dmgReduce * level}");

        if (so.itemDropRateBonus > 0) sb.AppendLine($"아이템 드랍률 +{so.itemDropRateBonus * level}%");
        if (so.incGold > 0) sb.AppendLine($"골드 획득량 +{so.incGold * level}%");
        if (so.incExp > 0) sb.AppendLine($"경험치 획득량 +{so.incExp * level}%");
        if (so.incStat > 0) sb.AppendLine($"스테이터스 포인트 +{so.incStat * level}%");

        if (so.incSpeed > 0) sb.AppendLine($"이동속도 증가 + {so.incSpeed * level}%");
        if (so.atkSpeed > 0) sb.AppendLine($"공속 증가 + {so.atkSpeed * level}%");


        return sb.Length > 0 ? sb.ToString().TrimEnd() : "보유 효과 없음";

    } // 보유 효과 표시 // 일단 임시로 모든 효과 넣어놨습니다.

    string EquipEffect(EquipmentSO so)
    {
        StringBuilder sb = new StringBuilder();



        return sb.Length > 0 ? sb.ToString().TrimEnd() : "장착 효과 없음";
    } // 장착 효과 표시 // RetentionEffect 에 있는것들중에 장착 효과로 할 스텟들 여기 넣으시면 됩니다.

}
