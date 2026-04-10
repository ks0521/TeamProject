using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

namespace UI.Ability_Set
{
    public class AbilityDetailView : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI statText;

        public void RefreshDetailView(RuntimeStatus status)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"공격력 : {status.finalBattleStatStatus.atk}");
            sb.AppendLine($"최대체력 : {status.finalBattleStatStatus.maxHp}");
            sb.AppendLine($"방어력 : {status.finalBattleStatStatus.def}");
            sb.AppendLine($"공격속도 : {status.finalBattleStatStatus.atkSpeed * 100}%");
            sb.AppendLine($"이동속도 : {status.finalBattleStatStatus.moveSpeed * 100}%");
            sb.AppendLine($"크리티컬확률 : {status.finalBattleStatStatus.critChance}%");
            sb.AppendLine($"크리티컬데미지 : {status.finalBattleStatStatus.critDamage}%");

            sb.AppendLine($"골드획득률 : {status.finalRewardStatStatus.goldGain * 100}%");
            sb.AppendLine($"경험치획득률 : {status.finalRewardStatStatus.expGain * 100}%");
            sb.AppendLine($"아이템드랍률 : {status.finalRewardStatStatus.itemDropRate * 100}%");

            statText.text = sb.ToString().TrimEnd();
        }
    }

}

