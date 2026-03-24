using Base.Data;
using Base.Managers;
using Base.Save;
using Battle;
using Growth.StatUpgrade;
using UnityEngine;
namespace UI.Scripts.UiPresenter
{
    public class UiPresenter : MonoBehaviour, IManager
    {

        [Header("UI 참조")]
        [SerializeField] Hp_Set hp;
        [SerializeField] Exp_Set expBar;
        [SerializeField] Power_Set powerText;
        [SerializeField] Gold_Set goldText;
        [SerializeField] Stone_Set stoneText;
        [SerializeField] MainUIStage_Set stageText;
        [SerializeField] Auto_Set autoButton;
        [SerializeField] Skill_Set skillIcons;
        PlayerProgressManager manager;
        private EventHub hub;
        
        public void Init()
        {
            manager = GameManager.Instance.GetGameSystem<PlayerProgressManager>();
            hub = GameManager.Instance.GetGameSystem<EventHub>(); 

            RefreshAll();

            hub.OnHpChange += hp.SetHp;
            hub.OnExpChange += expBar.SetExp;
            hub.OnGoldChange += goldText.SetGold;
            hub.OnStatStoneChange += stoneText.SetGrowthStone; 
            hub.OnChangeStage += stageText.SetStage;
            //스킬 부분 미구현
            //자동전투 버튼 미구현
        }

        public int GetOrder() => 201;

        void RefreshAll()
        {
            StageManager stageManager = GameManager.Instance.GetGameSystem<StageManager>();
            Character player = FindAnyObjectByType<Character>();

            if (player != null)
            {
                hp.SetHp(player.Hp, player.MaxHp);
            }
            goldText.SetGold(manager.progress.currency.gold);
            stoneText.SetGrowthStone(manager.progress.currency.statStone);

            if (stageManager != null)
            {
                stageText.SetStage(stageManager.CurStageSO);
            }
        }//초기값 세팅
    }
}







