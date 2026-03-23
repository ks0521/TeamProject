using Base.Managers;
using Base.Save;
using Battle;
using UnityEngine;
namespace UI.Scripts.UiPresenter
{
    public class UiPresenter : MonoBehaviour, IManager
    {
        [Header("매니저")]
        [SerializeField] PlayerProgressManager manager;

        [Header("UI 참조")]
        [SerializeField] Hp_Set hp;
        [SerializeField] Exp_Set expBar;
        [SerializeField] Power_Set powerText;
        [SerializeField] Gold_Set goldText;
        [SerializeField] Stone_Set stoneText;
        [SerializeField] MainUIStage_Set stageText;
        [SerializeField] Auto_Set autoButton;
        [SerializeField] Skill_Set skillIcons;

        bool autoType;
        private void Start()
        {
            autoType = false;
            RefreshAll();
        }
        public void Init()
        {
            manager = GameManager.Instance.GetGameSystem<PlayerProgressManager>();
            //나중에 메인 화면에 있는 UI 초기화하는 함수 추가 예정
        }
        public int GetOrder() => 201;

        void RefreshAll()
        {
            RefreshGoldText();
            RefreshStageText();
            RefreshStoneText();
        }
        public void RefreshHp()
        {
           Character player = FindObjectOfType<Character>();

            //hp.SetHp(player.Hp ,);
        }
        public void RefreshExpBar()
        {
            //expBar.SetExp(manager.progress.currency.exp ,  ); <- MaxExp 가 아직 없음
        }
        public void RefreshPowerText()
        {

        }
        public void RefreshGoldText()
        {
            goldText.SetGold(manager.progress.currency.gold);
        }
        public void RefreshStoneText()
        {
            stoneText.SetGrowthStone(manager.progress.currency.statStone);
        }
        public void RefreshStageText()
        {
            
        }
        public void IsAutoType()
        {
           
        }


    }
}



    



