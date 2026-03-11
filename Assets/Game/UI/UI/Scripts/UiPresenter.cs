using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace Game_UI_Scripts_MainUiView
{
    public class UiPresenter : MonoBehaviour
    {
        [Header("UI ÂüÁ¶")]
        [SerializeField] Hp_Set hp;
        [SerializeField] Exp_Set expBar;
        [SerializeField] Power_Set powerText;
        [SerializeField] Gold_Set goldText;
        [SerializeField] Stone_Set StoneText;
        [SerializeField] Stage_Set stageText;
        [SerializeField] Auto_Set AutoButton;
        [SerializeField] Skill_Set skillIcons;

        bool autoType;
        private void Start()
        {
            autoType = false;
        }

        public void RefreshHp()
        {
            
        }
        public void AutoBattle()
        {
            AutoButton.SetAutoBattle(autoType);
            autoType = !autoType;
        }
        float currentHp = 100f;
        float maxHp = 100f;

        private void Update()
        {

            if (Input.GetKeyDown(KeyCode.Q))
            {
                currentHp -= 10f;
                hp.SetHp(currentHp, maxHp);
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                currentHp += 10f;
                hp.SetHp(currentHp, maxHp);
            }
            
        }
    }

}
