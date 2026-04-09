using Base.Data;
using Base.Managers;
using Base.Save;
using Battle;
using System;
using System.Collections;
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


        [SerializeField] private ProgressManager manager;
        [SerializeField] private EventHub hub;
        [SerializeField] private StageManager stageManager;
        [SerializeField] private PopupManager popup;

        private Monster currentBoss;

        private Coroutine timerCoroutine; //코루틴 저장용
        
        public void Init()
        {
            manager = GameManager.Instance.GetGameSystem<ProgressManager>();
            hub = GameManager.Instance.GetGameSystem<EventHub>();
            stageManager = GameManager.Instance.GetGameSystem<StageManager>();
            popup = GameManager.Instance.GetGameSystem<PopupManager>();

            RefreshAll();

            hub.OnHpChange += hp.SetHp;
            hub.OnLevelChange += LvText.SetLv;
            hub.OnCurrencyChange += ReFreshCurrency;

            hub.OnChangeStage += stageText.SetStage;

            hub.OnStageChangeClear += RefreshChallengeUIOnce;
            hub.OnMonsterKill += ReFreshMonsterKill;

            hub.OnBossSpawned += ReFreshBoss;

            //스킬 부분 미구현
            //자동전투 버튼 미구현
        }

        public int GetOrder() => 210;

        void RefreshAll()
        {
            StageManager stageManager = GameManager.Instance.GetGameSystem<StageManager>();
            Player player = FindAnyObjectByType<Player>();
            if (player == null)
            {
                LvText.SetLv(0);
            }
            if (player != null)
            {
                hp.SetHp(player.Hp, player.MaxHp);
                LvText.SetLv(player.Level);
            }

            foreach (var ui in mainUItype)
            {
                switch (ui.Currency)
                {
                    case CurrencyType.GOLD:
                        ui.SetUI(manager.Currency.gold);
                        break;

                    case CurrencyType.EXP:
                        ui.SetUI(manager.Currency.exp);
                        break;

                    case CurrencyType.STATSTONE:
                        ui.SetUI(manager.Currency.statStone);
                        break;
                }
            }

            if (stageManager != null)
            {
                stageText.SetStage(stageManager.CurrentStageSo);
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
        void RefreshChallengeUIOnce(StageSO stageSO)
        {
            if (popup == null || stageManager == null) return;
            if (!stageManager.TryGetChallengeData(out var data)) { Debug.Log("챌린지 데이터 못가져옴"); return; }
            if (!popup.TryGetTimer(out var timer)) { Debug.Log("timerInstance 못가져옴"); return; }
            if (!popup.TryGetMonster(out var kill)) { Debug.Log("monsterKillInstance 데이터 못가져옴"); return; }

            timer.SetTime(data.currentTime, data.maxTime);
            kill.UpdateKillText(data.currentKill, data.targetKill);

            StartTimerRoutine();
        }//챌린지 UI 초기값 세팅
        void ReFreshMonsterKill(MonsterSO monsterSO)
        {
            if(popup == null || stageManager == null) return;
            if (!stageManager.TryGetChallengeData(out var data)) return;
            if (!popup.TryGetMonster(out var kill)) return;

            kill.UpdateKillText(data.currentKill);
        }//몬스터 갱신용
        void ReFreshBoss(Monster monster)
        {
            if (popup == null) return;
            if (!popup.TryGetBossHpBar(out var bossHp)) return;

            if (currentBoss != null)
            {
                currentBoss.OnMonsterHpChanged -= OnBossHpChanged;
                currentBoss = null;
            }

            currentBoss = monster;

            bossHp.SetBoss(currentBoss.Hp, currentBoss.CurrentBattleStatStat.maxHp /* ,나중에 여기 보스 이름 추가*/);

            currentBoss.OnMonsterHpChanged += OnBossHpChanged;

        }//보스 생성될때 UI 세팅용
        private void OnBossHpChanged(float hp, float maxHp)
        {
            if (!popup.TryGetBossHpBar(out var bossHp)) return;
            bossHp.SetBoss(hp);
        }//보스 Hp 갱신용
        void StartTimerRoutine()
        {
            StopTimerRoutine();
            timerCoroutine = StartCoroutine(CoRefreshTimer());
        }
        void StopTimerRoutine()
        {
            if (timerCoroutine != null)
            {
                StopCoroutine(timerCoroutine);
                timerCoroutine = null;
            }
        }
        IEnumerator CoRefreshTimer()
        {
            while (true)
            {
                if (popup == null || stageManager == null)//팝업 , 스테이지 매니저 체크
                {
                    yield return null;
                    continue;
                }

                if (!stageManager.TryGetChallengeData(out var data))//챌린지 체크
                {
                    StopTimerRoutine();
                    yield break;
                }

                if (!popup.TryGetTimer(out var timer))//UI 생성 체크
                {
                    yield return null;
                    continue;
                }

                timer.SetTime(data.currentTime);
                yield return null;
            }
        }
    }
}







