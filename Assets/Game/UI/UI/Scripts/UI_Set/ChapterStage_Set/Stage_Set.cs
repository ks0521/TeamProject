using Base.Managers;
using Battle;
using Cysharp.Threading.Tasks.Triggers;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Scripts.Stage
{
    public class Stage_Set : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private Button button;
        [SerializeField] private TextMeshProUGUI stageName;

        [SerializeField] private Image normalIcon;
        [SerializeField] private Image challengeIcon;
        [SerializeField] private Image bossIcon;
        [SerializeField] private Image lockIcon;

        public void SetStage(StageEntry entry)
        {
            if (entry.stageSO == null)
            {
                Debug.LogError("StageSO가 null입니다.");
                return;
            }

            if (stageName == null)
            {
                Debug.LogError("stageName UI가 연결되지 않았습니다.");
                return;
            }

            stageName.text = entry.stageSO.stageName;

            switch (entry.type)
            {
                case StageType.Normal:
                    button.image = normalIcon;
                    button.interactable = true;
                    break;

                case StageType.Challenge:
                    button.image = challengeIcon;
                    button.interactable = true;
                    break;

                case StageType.Boss:
                    button.image = bossIcon;
                    button.interactable = true;
                    break;

                default:
                    button.interactable = false;
                    button.image = lockIcon;
                    break;
            }
        }
    }

}
