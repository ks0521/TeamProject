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

            sb.AppendLine($"공격력 : {status.FinalBattleStatStatus.atk}");
            sb.AppendLine($"공격력% : {status.FinalBattleStatStatus.atkRate * 100}%");
            sb.AppendLine($"피해량 증가 : {status.finalStatus.extra.damageDealtRate * 100}%");
            sb.AppendLine($"최대체력 : {status.FinalBattleStatStatus.maxHp}");
            sb.AppendLine($"방어력 : {status.FinalBattleStatStatus.def}");
            sb.AppendLine($"피해량 감소율 : {status.finalStatus.extra.damageReduceRate * 100}%");
            sb.AppendLine($"공격속도 : {status.FinalBattleStatStatus.atkSpeed}");
            sb.AppendLine($"이동속도 : {status.FinalBattleStatStatus.moveSpeed}");
            sb.AppendLine($"크리티컬확률 : {status.FinalBattleStatStatus.critChance * 100}%");
            sb.AppendLine($"크리티컬데미지 : {status.FinalBattleStatStatus.critDamage * 100}%");

            sb.AppendLine($"골드획득률 : {status.FinalRewardStatStatus.goldGain * 100}%");
            sb.AppendLine($"경험치획득률 : {status.FinalRewardStatStatus.expGain * 100}%");
            sb.AppendLine($"아이템드랍률 : {status.FinalRewardStatStatus.itemDropRate * 100}%");

            statText.text = sb.ToString().TrimEnd();
        }
    }
}

