using Base.Managers;
using Battle;
using Cysharp.Threading.Tasks.Triggers;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace UI.Scripts.Stage
{
    public class Stage_Set : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private Button button;
        [SerializeField] private TextMeshProUGUI stageName;

        [SerializeField] private GameObject spotlightIcon;
        [SerializeField] private GameObject challengeIcon;
        [SerializeField] private GameObject bossIcon;
        [SerializeField] private GameObject lockIcon;

        public void SetStage(StageEntry entry)
        {
            if (entry.stageSO == null)
            {
                Debug.Log("StageSO가 null입니다.");
                return;
            }

            if (stageName == null)
            {
                Debug.Log("stageName UI가 연결되지 않았습니다.");
                return;
            }

            stageName.text = entry.stageSO.stageName;

            switch (entry.type)
            {
                case StageType.Normal:
                    challengeIcon.SetActive(false); 
                    bossIcon.SetActive(false); 
                    lockIcon.SetActive(false);

                    button.interactable = true;
                    break;

                case StageType.Challenge:
                    challengeIcon.SetActive(true);
                    bossIcon.SetActive(false);
                    lockIcon.SetActive(false);

                    button.interactable = true;
                    break;

                case StageType.Boss:
                    challengeIcon.SetActive(false);
                    bossIcon.SetActive(true);
                    lockIcon.SetActive(false);

                    button.interactable = true;
                    break;

                default:
                    challengeIcon.SetActive(false);
                    bossIcon.SetActive(false);
                    lockIcon.SetActive(true);

                    button.interactable = false;
                    break;
            }
        }

        public void Bind(Action action)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => action?.Invoke());
        }//버튼 OnClick 에 함수 넣어주는 함수

        public void Spotlight(bool isSpotlight)
        {
            spotlightIcon.SetActive(isSpotlight);   
        }
    }

}
