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
        [SerializeField] private Button[] button;
        [SerializeField] private TextMeshProUGUI[] stageName;

        [SerializeField] private Image normalIcon;
        [SerializeField] private Image challengeIcon;
        [SerializeField] private Image bossIcon;
        [SerializeField] private Image lockIcon;

        private void OnEnable()
        {
            
        }
        public void ReFreshStage(int currentStage)
        {
            foreach (var stageButton in button)
            {
                stageButton.interactable = false;
            }
            for (int i = 0; i < currentStage; i++)
            {
                button[i].interactable = true;
            }
        }

        public void SetStage(int stage, string stgName, StageType type)
        {
            switch (type)
            {
                case StageType.Normal:
                    button[stage].image = normalIcon;
                    button[stage].interactable = true;
                    break;

                case StageType.Challenge:
                    button[stage].image = challengeIcon;
                    button[stage].interactable = true;
                    break;

                case StageType.Boss:
                    button[stage].image = bossIcon;
                    button[stage].interactable = true;
                    break;

                default:
                    button[stage].interactable = false;
                    button[stage].image = lockIcon;
                    break;
            }
            stageName[stage].text = stgName;
            
        }
    }

}
