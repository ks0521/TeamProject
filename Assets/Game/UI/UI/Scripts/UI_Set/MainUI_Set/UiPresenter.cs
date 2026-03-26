using Base.Data;
using Base.Managers;
using Base.Save;
using Battle;
using Growth.StatUpgrade;
using System.Linq;
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
        [SerializeField] Hp_Set timer;
        [SerializeField] Hp_Set monsterKill;

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
        public void SetChallengeUI(bool isCheck)
        {

            challengePanel.SetActive(isCheck);

            timer.SetHp(stageManager.RemainTime , stageManager.RemainTimeRatio);

            monsterKill.SetHp(stageManager.TargetKillScore, stageManager.CurStageSO.targetKillScore);
        }
    }
}







