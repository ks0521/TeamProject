using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.Scripts.Ability
{
    public class Ability : MonoBehaviour
    {
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

        // 테스트용 데이터(나중에 삭제 예정)
        private int playerLevel = 1;
        private int playerGold = 1000;

        private int atkLevel = 1;
        private int atkMaxLevel = 10;
        private int atkUnlockLevel = 10;

        private int cost;
        private float currentValue;
        private float nextValue;

        // Start is called before the first frame update
        public void OnEnable()
        {
            RefreshUI();
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

            RefreshUI();
            ChangeState(XState.X1);
        }
        public void RefreshUI()
        {
            RefreshAtkUI();
        }//능력치팝업창 UI 갱신용 함수(능력치 팝업창 안에있는 UI 갱신용 함수 추가 예정)

        private void OnClickAtkLevelUp()
        {
            Debug.Log("공격력 레벨업");
            if (playerLevel < atkUnlockLevel)
            {
                Debug.Log("플레이어 레벨이 부족해서 공격력 해금이 안됨");
                return;
            }

            if (atkLevel >= atkMaxLevel)
            {
                Debug.Log("이미 최대 레벨");
                return;
            }

            if (playerGold < cost)
            {
                Debug.Log("골드 부족");
                return;
            }

            playerGold -= cost;
            atkLevel++;

            Debug.Log($"공격력 레벨업 성공 / 현재 레벨 : {atkLevel} / 남은 골드 : {playerGold}");

            RefreshAtkUI();
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


        private void RefreshAtkUI()
        {
            currentValue = atkLevel * 5;
            cost = atkLevel * 500;
            nextValue = (atkLevel + 1) * 5;

            bool canLevelUp = playerLevel >= atkUnlockLevel && playerGold >= cost &&  atkLevel < atkMaxLevel;

            atk.RefreshUI( atkLevel, atkMaxLevel, currentValue, nextValue, cost, canLevelUp, playerLevel, atkUnlockLevel);
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
            RefreshUI();
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

        private void UnlockStats()
        {
            /*bool isLock = playerLv < unlockLv;   
              if(isLock){return;}

            */
        }
        // Update is called once per frame
        private void Update()
        {
            if (Input.GetKey(KeyCode.Q))
            {
                playerGold += 100;
                Debug.Log($"골드 획득 : 현재 골드 {playerGold}");
                RefreshAtkUI();
            }

            if (Input.GetKeyDown(KeyCode.W))
            {
                playerLevel++;
                Debug.Log($"플레이어 레벨업 : 현재 레벨 {playerLevel}");
                RefreshAtkUI();
            }
        }
    }

}
