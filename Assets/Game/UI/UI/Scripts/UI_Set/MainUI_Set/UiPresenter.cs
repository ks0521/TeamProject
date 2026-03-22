using Base.Data;
using Base.Managers;
using Base.Save;
using Battle;
using Growth.StatUpgrade;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace UI.Scripts.UiPresenter
{
    public class UiPresenter : MonoBehaviour , IManager
    {
        [Header("매니저")] 
        [SerializeField]PlayerProgressManager manager;
        
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
        public void Init()
        {
            //나중에 메인 화면에 있는 UI 초기화하는 함수 추가 예정
        }
        public int GetOrder() => 201;
        public void RefreshHp()
        {
            
        }
       

       

       
       
    }

}
