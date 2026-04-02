using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class AbilityDetailView : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI statText;

    public void RefreshDetailView(PlayerRuntimeStatus status)
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine($"공격력 : {status.finalBattleStatus.atk}");
        sb.AppendLine($"최대체력 : {status.finalBattleStatus.maxHp}");
        sb.AppendLine($"방어력 : {status.finalBattleStatus.def}");
        sb.AppendLine($"공격속도 : {status.finalBattleStatus.atkSpeed * 100}%");
        sb.AppendLine($"이동속도 : {status.finalBattleStatus.moveSpeed * 100}%");
        sb.AppendLine($"크리티컬확률 : {status.finalBattleStatus.critChance}%");
        sb.AppendLine($"크리티컬데미지 : {status.finalBattleStatus.critDamage}%");

        sb.AppendLine($"골드획득률 : {status.finalRewardStatus.goldRate * 100}%");
        sb.AppendLine($"경험치획득률 : {status.finalRewardStatus.expRate * 100}%");
        sb.AppendLine($"아이템드랍률 : {status.finalRewardStatus.itemDropRateBonus * 100}%");

        statText.text = sb.ToString().TrimEnd();
    }
}
