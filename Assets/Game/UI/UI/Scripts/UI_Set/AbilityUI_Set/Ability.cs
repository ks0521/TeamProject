using Base.Data;
using Base.Managers;
using Base.Save;
using Growth.StatUpgrade;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Unity.VisualScripting;
using Unity.Mathematics;

namespace UI.Scripts.Ability
{
    public class Ability : MonoBehaviour
    {
        private StatUpgradeManager manager;
        private EventHub hub;

        [Header("스텟 UI 목록")]
        [SerializeField] StatItemView[] statItemViews;

        [Header("능력치 구매 버튼")]
        [SerializeField] private Button[] Upbtn;

        [Header("곱하기 버튼")]
        [SerializeField] private Button_Set btnX;

        private enum XState
        {
            X1, X10, X100
        }
        private XState multiState;
        private int multiPress = 1;

        private void OnEnable()
        {
            hub.OnGoldChange += ReFreshAllUI;
            hub.OnStatStoneChange += ReFreshAllUI;
            hub.OnLevelChange += ReFreshAllUI;
        }
        private void OnDisable()
        {
            hub.OnGoldChange -= ReFreshAllUI;
            hub.OnStatStoneChange -= ReFreshAllUI;
            hub.OnLevelChange -= ReFreshAllUI;
        }
        public void Init() 
        {
            manager = GameManager.Instance.GetGameSystem<StatUpgradeManager>();
            hub = GameManager.Instance.GetGameSystem<EventHub>();
            //ReFreshAllUI(1); //3.23(규성) : 처음에 팝업 열려있는게 아니라 refresh할 필요가 없을것 같아 닫아두었습니다
            BindAllButtons();
            ChangeState(XState.X1);
            gameObject.SetActive(false);
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
                GameData.StatusDB.TryGetStatEntry(type, out var entry);
                stat.BindLevelUp(() => OnClickLevelUp(type , entry));
            }
            btnX.Xbtn[0].onClick.AddListener(() => ChangeState(XState.X1));
            btnX.Xbtn[1].onClick.AddListener(() => ChangeState(XState.X10));
            btnX.Xbtn[2].onClick.AddListener(() => ChangeState(XState.X100));
        }//능력치 구매 버튼 , 곱하기 버튼 OnClick 에 자동으로 함수 넣어주기
        public void ReFreshAllUI(int fake)
        {
            foreach (var stat in statItemViews)
            {
                ReFreshStatUI(stat.statusType);
            }
        }//모든 UI 새로고침
        public void ReFreshStatUI(StatusType type)
        {
            if (!GameData.StatusDB.TryGetStatEntry(type, out var statEntry))
            {
                Debug.Log($"{type} : statEntry를 찾지 못함");
                return;
            }//null 방지
            
            int playerGold = PlayerProgressManager.Instance.progress.currency.statStone;
            int currentLevle = manager.GetStatUpgradeLevel(type);

            float currentValue = currentLevle * statEntry.increasePerEnhance; //현재 스텟 수치
            float nextValue = (math.min(currentLevle + multiPress ,statEntry.maxLevel)) * statEntry.increasePerEnhance; //증가 후 스텟 수치

            int cost = 0;
            for (int i = 1; i <= multiPress; i++)
            {
                cost += (currentLevle + i) * statEntry.enhanceCost;
            }
            bool isUnLock = PlayerProgressManager.Instance.progress.currency.level >= statEntry.unlockLevel;
            
            bool canLevelUp = manager.CanUpgradeStat(type,multiPress) && (currentLevle < statEntry.maxLevel) && isUnLock;

            StatItemView itemView = GetType(type);
            if (itemView == null)
            {
                Debug.Log($"{type}에 해당되는 스텟 UI가 없음");
                return; 
            }

            itemView.RefreshUI(statEntry, currentLevle, currentValue, nextValue, cost, canLevelUp, isUnLock);


        }//능력치팝업창 UI 갱신용 함수

        private void OnClickLevelUp(StatusType type ,  StatEntry stat)
        {
            int currntLevel = manager.GetStatUpgradeLevel(type);
            if (currntLevel + multiPress <= stat.maxLevel)
            {
                manager.TryUpgradeStat(type, multiPress);
            }
            else
            {
                manager.TryUpgradeStat(type , stat.maxLevel - currntLevel);
            }
            ReFreshAllUI(1);
        } //능력치 강화

        void ChangeState(XState newState)
        {
            switch (newState)
            {
                case XState.X1:
                    multiPress = 1;
                    btnX.SelectButton(0);
                    break;

                case XState.X10:
                    multiPress = 10;
                    btnX.SelectButton(1);
                    break;

                case XState.X100:
                    multiPress = 100;
                    btnX.SelectButton(2);
                    break;
            }
            ReFreshAllUI(1);
        }//버튼 상태 전환 함수
    }

}
