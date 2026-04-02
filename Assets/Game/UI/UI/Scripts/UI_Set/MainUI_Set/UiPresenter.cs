using Base.Data;
using Base.Managers;
using Base.Save;
using Battle;
using UnityEngine;
namespace UI.Scripts
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


        [SerializeField] private PlayerProgressManager manager;
        [SerializeField] private EventHub hub;
        [SerializeField ]private StageManager stageManager;

        private void Update()
        {
            PopupManager popup = GameManager.Instance.GetGameSystem<PopupManager>();
            if (popup == null) return;

            if (stageManager.TryGetChallengeData(out var data))
            {
                if (popup.TryGetChallengeUI(out var timer, out var kill))
                {
                    timer.SetTime(data.maxTime, data.currentTime);
                    kill.UpdateKillText(target: data.targetKill, current: data.currentKill);
                }
            }
        }
        public void Init()
        {
            manager = GameManager.Instance.GetGameSystem<PlayerProgressManager>();
            hub = GameManager.Instance.GetGameSystem<EventHub>();
            stageManager = GameManager.Instance.GetGameSystem<StageManager>();

            RefreshAll();
            
            hub.OnHpChange += hp.SetHp;
            hub.OnLevelChange += LvText.SetLv; 
            hub.OnCurrencyChange += ReFreshCurrency;

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
    }
}







