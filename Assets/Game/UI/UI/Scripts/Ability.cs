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
using System;

namespace UI.Scripts.Ability
{
    public class Ability : MonoBehaviour
    {
        [Header("매니저")]
        [SerializeField] private PlayerProgressManager gameManager;

        [Header("스텟 UI 목록")]
        [SerializeField] StatItemView[] statItemViews;
        
        [Header("능력치 구매 버튼")]
        [SerializeField] private Button[] Upbtn;

        [Header("곱하기 버튼")]
        [SerializeField] private Button_Set btnX;
        private enum XState
        {
            X1 , X10 , X100
        }
        private XState X_state;
        private float multiValue;


        // Start is called before the first frame update
        public void OnEnable()
        {
            ReFreshAllUI();
        }
        private void Start()
        {
            BindAllButtons();
            ReFreshAllUI();
            ChangeState(XState.X1);
        }
        private StatItemView GetType(StatusType type)
        {
            foreach (var view in statItemViews)
            {
                if (view.statusType == type)
                    return view;
            }

            return null;
        }//타입에 해당하는 StatItemView 찾아주는 함수
        private void BindAllButtons()
        {
            foreach (var stat in statItemViews)
            {
                StatusType type = stat.statusType;
                stat.BindLevelUp(() => OnClickLevelUp(type));
            }
        }
        public void ReFreshAllUI()
        {
            foreach (var stat in statItemViews)
            {
                ReFreshStatUI(stat.statusType);
            }
        }
        public void ReFreshStatUI(StatusType type)
        {
            if (!gameManager.statUpgradeConfig.TryGetStatEntry(type, out var statEntry))
            {
                Debug.Log($"{type} : statEntry를 찾지 못함");
                return;
            }//null 방지

            int playerLevel = gameManager.progress.currency.level;
            int playerGold = gameManager.progress.currency.gold;
            int currentLevle = gameManager.progress.statUpgrades.upgradeLevelsByType[type];

            float currentValue = currentLevle * statEntry.increasePerEnhance; //현재 스텟 수치
            float nextValue = (currentLevle + 1) * statEntry.increasePerEnhance; //증가 후 스텟 수치
            int cost = currentLevle * statEntry.enhanceCost; //스텟 가격 부분

            bool isUnLock = playerLevel <= statEntry.unlockLevel;
            bool canLevelUp = isUnLock && playerGold >= cost && currentLevle < statEntry.maxLevel;

            StatItemView itemView = GetType(type);

            if (itemView == null)
            {
                Debug.Log($"{type}에 해당되는 스텟 UI가 없음");
            }

            itemView.RefreshUI(statEntry , currentLevle , currentValue , nextValue ,cost , canLevelUp , isUnLock);
                

        }//능력치팝업창 UI 갱신용 함수

        private void OnClickLevelUp(StatusType type)
        {

        } // 미구현





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
            ReFreshAllUI();
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
