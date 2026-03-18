using Base.Data;
using Base.Save;
using Growth.StatUpgrade;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.Scripts.Ability
{
    public class Ability : MonoBehaviour
    {
        [Header("매니저")]
        [SerializeField] GameManager gameManager;
        public enum StatusType
        {
            Atk, MaxHp, Def, AtkSpeed, CritChance, CritDmg, MoveSpeed, GoldRate, ExpRate, ItemDropRate
        }//나중에 참고
        [Header("능력치 구매 버튼")]
        [SerializeField] Button[] Upbtn;

        [Header("곱하기 버튼")]
        [SerializeField] Button_Set btnX;
        private enum XState
        {
            X1, X10, X100
        }
        private XState X_state;
        private float multiValue;

        [Header("UI 참조")]
        [SerializeField] Atk_Set atk;
        [SerializeField] MaxHp_Set maxHp;
        [SerializeField] Def_Set def;
        [SerializeField] AtkSpeed_Set atkspeed;
        [SerializeField] CritChance_Set critChance;
        [SerializeField] CritDmg_Set critDmg;
        [SerializeField] MoveSpeed_Set moveSpeed;
        [SerializeField] GoldRate_Set goldRate;
        [SerializeField] ExpRate_Set expRate;
        [SerializeField] ItemDropRate_Set itemDropRate;

      
        // Start is called before the first frame update
        public void OnEnable()
        {
            ReFreshUI();
        }
        private void Start()
        {
            atk.BindLevelUp(OnClickAtkLevelUp);
            maxHp.BindLevelUp(OnClickMaxHPLevelUp);
            def.BindLevelUp(OnClickDefLevelUp);
            atkspeed.BindLevelUp(OnClickAtkSpeedLevelUp);
            critChance.BindLevelUp(OnClickCritChanceLevelUp);
            critDmg.BindLevelUp(OnClickCritDmgLevelUp);
            moveSpeed.BindLevelUp(OnClickMoveSpeedLevelUp);
            goldRate.BindLevelUp(OnClickGoldRateLevelUp);
            expRate.BindLevelUp(OnClickExpRateLevelUp);
            itemDropRate.BindLevelUp(OnClickItmeDropRateLevelUp);

            ReFreshUI();
            ChangeState(XState.X1);
        }
        public void ReFreshUI()
        {
            
        }//능력치팝업창 UI 갱신용 함수(능력치 팝업창 안에있는 UI 갱신용 함수 추가 예정)

        private void OnClickAtkLevelUp()
        {
        }//테스트용(나중에 수정할 예정)
        private void OnClickMaxHPLevelUp()
        {

        }
        private void OnClickDefLevelUp()
        {

        }
        private void OnClickAtkSpeedLevelUp()
        {

        }
        private void OnClickCritChanceLevelUp()
        {

        }
        private void OnClickCritDmgLevelUp()
        {

        }
        private void OnClickMoveSpeedLevelUp()
        {

        }
        private void OnClickGoldRateLevelUp()
        {

        }
        private void OnClickExpRateLevelUp()
        {

        }
        private void OnClickItmeDropRateLevelUp()
        {

        }


        private void RefreshAtkUI(StatusType type)
        {
            
           

            
                        
            
        }//테스트용(나중에 수정할 예정)
        private void RefreshMaxHPUI()
        {

        }
        private void RefreshDef()
        {

        }
        private void RefreshAtkSpeedUI()
        {

        }
        private void RefreshCritChanceUI()
        {

        }
        private void RefreshCritDmgUI()
        {

        }
        private void RefreshMoveSpeed()
        {

        }
        private void RefreshGoldRate()
        {

        }
        private void RefreshExpRate()
        {

        }
        private void RefreshItemDropRate()
        {

        }

        void ChangeState(XState newState)
        {
            X_state = newState;

            switch (X_state)
            {
                case XState.X1:
                    multiValue = 1;
                    btnX.SelectButton(0);
                    break;

                case XState.X10:
                    multiValue = 10;
                    btnX.SelectButton(1);
                    break;

                case XState.X100:
                    multiValue = 100;
                    btnX.SelectButton(2);
                    break;
            }
            ReFreshUI();
        }//상태 전환 함수
        public void OnClickX1()
        {
            ChangeState(XState.X1);
        }//버튼 연결용 함수
        public void OnClickX10()
        {
            ChangeState(XState.X10);
        }
        public void OnClickX100()
        {
            ChangeState(XState.X100);
        }

        // Update is called once per frame
        private void Update()
        {
            
        }
    }

}
