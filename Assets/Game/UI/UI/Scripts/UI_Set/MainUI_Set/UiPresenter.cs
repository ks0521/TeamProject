using Base.Data;
using Base.Managers;
using Base.Save;
using Battle;
using UnityEngine;
namespace UI.Scripts.UiPresenter
{
    public class UiPresenter : MonoBehaviour, IManager
    {

        [Header("UI 참조")]
        [SerializeField] Hp_Set hp;
        [SerializeField] Lv_Set LvText;

        [Header("골드 , 성장석 , 경험치 넣기")]
        [SerializeField] MainUItype_Set[] mainUItype;

        [SerializeField] MainUIStage_Set stageText;
        [SerializeField] Auto_Set autoButton;
        [SerializeField] Skill_Set skillIcons;

        [Header("챌린지 모드")]
        [SerializeField] GameObject challengePanel;
        [SerializeField] SetViewer timer;
        [SerializeField] SetViewer monsterKill;

        private PlayerProgressManager manager;
        private EventHub hub;
        private StageManager stageManager;
        public void Init()
        {
            manager = GameManager.Instance.GetGameSystem<PlayerProgressManager>();
            hub = GameManager.Instance.GetGameSystem<EventHub>();
            stageManager = GameManager.Instance.GetGameSystem<StageManager>();

            RefreshAll();
            challengePanel.SetActive(false);
            hub.OnHpChange += hp.SetHp;
            hub.OnLevelChange += LvText.SetLv; 
            hub.OnCurrencyChange += ReFreshCurrency;
            //hub.OnExpChange += expBar.SetExp; <- 해당 부분을 바꾸시면 됩니다
            //hub.OnGoldChange += goldText.SetGold;
            //hub.OnStatStoneChange += stoneText.SetGrowthStone; 
            hub.OnChangeStage += stageText.SetStage;
            //스킬 부분 미구현
            //자동전투 버튼 미구현
        }

        public int GetOrder() => 210;

        void RefreshAll()
        {
            StageManager stageManager = GameManager.Instance.GetGameSystem<StageManager>();
            Character player = FindAnyObjectByType<Character>();
            

            if (player != null)
            {
                hp.SetHp(player.Hp, player.MaxHp);
                LvText.SetLv(((Player)player).Level);
            }

            foreach (var ui in mainUItype)
            {
                switch (ui.Currency)
                {
                    case CurrencyType.GOLD:
                        ui.SetUI(PlayerProgressManager.Instance.progress.currency.gold);
                        break;

                    case CurrencyType.EXP:
                        ui.SetUI(PlayerProgressManager.Instance.progress.currency.exp);
                        break;

                    case CurrencyType.STATSTONE:
                        ui.SetUI(PlayerProgressManager.Instance.progress.currency.statStone);
                        break;
                }
            }
            
            if (stageManager != null)
            {
                stageText.SetStage(stageManager.CurStageSO);
            }

        }//초기값 세팅
        void ReFreshCurrency(CurrencyType type, int currency)
        {
            foreach (var ui in mainUItype)
            {
                if (ui.Currency == type)
                {
                    ui.SetUI(currency);
                }
            }
        }

        private void Update()
        {
            if (stageManager.TryGetChallengeData(out var data))
            {
                timer.SetTime(data.maxTime, data.currentTime);
                monsterKill.UpdateKillText(target : data.targetKill,current: data.currentKill);
            }
        }
        //챌린지 전용 UI 활성화 / 비활성화
        public void SetChallengeUI(bool isCheck)
        {
            if (isCheck == challengePanel.activeSelf) return; //동일한 현상(켜져있을때 키기 / 꺼져있을때 끄기)에서는 작동 X
            challengePanel.SetActive(isCheck);
        }
    }
}







