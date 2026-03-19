using Base.Save;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace UI.Scripts.UiPresenter
{
    public class UiPresenter : MonoBehaviour
    {
        [Header("UI 참조")]
        [SerializeField] Hp_Set hp;
        [SerializeField] Exp_Set expBar;
        [SerializeField] Power_Set powerText;
        [SerializeField] Gold_Set goldText;
        [SerializeField] Stone_Set StoneText;
        [SerializeField] MainUIStage_Set stageText;
        [SerializeField] Auto_Set AutoButton;
        [SerializeField] Skill_Set skillIcons;

        bool autoType;
        private void Start()
        {
            autoType = false;
        }

        public void RefreshHp()
        {
            goldText.SetGold(PlayerProgressManager.Instance.progress.currency.gold);
          
        }
        public void AutoBattle()
        {
            AutoButton.SetAutoBattle(autoType);
            autoType = !autoType;
        }
       
       
    }

}
