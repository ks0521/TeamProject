using Base.Data;
using Base.Managers;
using Base.Save;
using Growth.StatUpgrade;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Scripts
{
    public class Ability : MonoBehaviour
    {
        private StatUpgradeManager manager;
        private ProgressManager progressManager;
        private EventHub hub;
        private GameDataProvider gameDB;
        private RuntimeStatus _data;

        [Header("스텟 UI 목록")]
        [SerializeField] StatItemView[] statItemViews;

        [Header("능력치 구매 버튼")]
        [SerializeField] private Button[] Upbtn;

        [Header("곱하기 버튼")]
        [SerializeField] private Button_Set btnX;

        [Header("능력치 상세창")]
        [SerializeField] private AbilityDetailView AbilityDetailView;

        private enum XState
        {
            X1, X10, X100
        }
        private XState multiState;
        private int multiPress = 1;

        private void OnEnable()
        {
            manager = GameManager.Instance.GetGameSystem<StatUpgradeManager>();
            progressManager = GameManager.Instance.GetGameSystem<ProgressManager>();
            hub = GameManager.Instance.GetGameSystem<EventHub>();
            gameDB = GameManager.Instance.GetGameSystem<GameDataProvider>();
            _data = GameManager.Instance.GetGameSystem<RuntimeStatus>();

            BindAllButtons();
            ReFreshAllUI(1);
            ChangeState(XState.X1);
            AbilityDetailView.RefreshDetailView(_data);

            if (hub != null)
            {
                hub.OnCurrencyChange += EventChain;
                hub.OnLevelChange += ReFreshAllUI;
                hub.OnStatusEnhanced += DetailViewEventChain;
                Debug.Log("장비창 이벤트 구독");
            }
        }
        private void OnDisable()
        {
            if (hub == null) return;
            hub.OnCurrencyChange -= EventChain;
            hub.OnLevelChange -= ReFreshAllUI;
            hub.OnStatusEnhanced -= DetailViewEventChain;
            Debug.Log("장비창 이벤트 해제");
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
                gameDB.statusTable.TryGetStatEntry(type, out var entry);
                stat.BindLevelUp(() => OnClickLevelUp(type, entry));
            }
            btnX.Xbtn[0].onClick.AddListener(() => ChangeState(XState.X1));
            btnX.Xbtn[1].onClick.AddListener(() => ChangeState(XState.X10));
            btnX.Xbtn[2].onClick.AddListener(() => ChangeState(XState.X100));
        }//능력치 구매 버튼 , 곱하기 버튼 OnClick 에 자동으로 함수 넣어주기
        void EventChain(CurrencyType type, int value)
        {
            ReFreshAllUI(1);
        }//이벤트 연결용
        void DetailViewEventChain(StatusType statusType)
        {
            AbilityDetailView.RefreshDetailView(_data);
        }
        void ReFreshAllUI(int fake)
        {
            foreach (var stat in statItemViews)
            {
                ReFreshStatUI(stat.statusType);
            }
        }//모든 UI 새로고침
        void ReFreshStatUI(StatusType type)
        {
            if (!gameDB.statusTable.TryGetStatEntry(type, out var statEntry))
            {
                Debug.Log($"{type} : statEntry를 찾지 못함");
                return;
            }//null 방지

            int playerGold = progressManager.Currency.statStone;
            int currentLevle = manager.GetStatUpgradeLevel(type);

            float currentValue = currentLevle * statEntry.increasePerEnhance; //현재 스텟 수치
            float nextValue = (math.min(currentLevle + multiPress, statEntry.maxLevel)) * statEntry.increasePerEnhance; //증가 후 스텟 수치

            int cost = 0;
            for (int i = 1; i <= multiPress; i++)
            {
                cost += (currentLevle + i) * statEntry.enhanceCost;
            }
            bool isUnLock = progressManager.PlayerInfo.level >= statEntry.unlockLevel;

            bool canLevelUp = manager.CanUpgradeStat(type, multiPress) && (currentLevle < statEntry.maxLevel) && isUnLock;

            StatItemView itemView = GetType(type);
            if (itemView == null)
            {
                Debug.Log($"{type}에 해당되는 스텟 UI가 없음");
                return;
            }

            itemView.RefreshUI(statEntry, currentLevle, currentValue, nextValue, cost, canLevelUp, isUnLock);


        }//능력치팝업창 UI 갱신용 함수

        private void OnClickLevelUp(StatusType type, StatEntry stat)
        {
            int currntLevel = manager.GetStatUpgradeLevel(type);
            if (currntLevel + multiPress <= stat.maxLevel)
            {
                manager.TryUpgradeStat(type, multiPress);
            }
            else
            {
                manager.TryUpgradeStat(type, stat.maxLevel - currntLevel);
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
