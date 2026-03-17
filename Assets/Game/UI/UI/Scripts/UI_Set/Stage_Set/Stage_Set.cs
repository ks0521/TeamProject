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

   
        

        public void SetStage(int stage, string stgName, StageType type)
        {
            switch (type)
            {
                case StageType.Normal:
                    button[stage].image = normalIcon;
                    break;

                case StageType.Challenge:
                    button[stage].image = challengeIcon;
                    break;

                case StageType.Boss:
                    button[stage].image = bossIcon;
                    break;

                default:
                    button[stage].interactable = false;
                    break;
            }
            stageName[stage].text = stgName;
            
        }
    }

}
