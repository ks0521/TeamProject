using Base.Data;
using Base.Managers;
using QuestSystem.TutorialSteps;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.UI;
using static QuestSystem.TutorialSteps.Tutorial_Click;

namespace QuestSystem
{
    public class ActiveQuest
    {
        public QuestDataReader Data { get; private set; } //연결된 원본 데이터
        public int CurrentValue; //현재 진행 수치
        public bool isCompleted;

        public ActiveQuest(QuestDataReader data)
        {
            Data = data;
            CurrentValue = 0;
            isCompleted = false;
        }
    }
    public class QuestManager : MonoBehaviour, IManager
    {
        public static QuestManager Instance { get; private set; }
        private EventHub eventHub;
        private QuestSO questSO;
        private List<ActiveQuest> activeQuests = new List<ActiveQuest>(); //진행 중
        private HashSet<int> completedQuestIds = new HashSet<int>(); //완료
        //private Dictionary<int, QuestData> questDatabase = new Dictionary<int, QuestData>();

        [Header("퀘스트 데이터")]
        [SerializeField] private QuestDatabaseSO questDatabase;
        [SerializeField] private TextAsset questJsonFile;

        [Header("튜토리얼용 오브젝트들")]
        public Button autoHuntButton;
        public Button bossMapButton;
        public Button newContentButton;
        public GameObject guideArrow;
        public GameObject TutorialPanel; //튜토리얼 설명 텍스트가 있는 패널
        public Text TutorialDescText;

        [Header("선형 퀘스트")]
        [SerializeField] private GameObject linealQuestPanel;
        [SerializeField] private TextMeshProUGUI linealQuestTitle;
        [SerializeField] private TextMeshProUGUI linealQuestProgress;

        [Header("순환 퀘스트")]
        [SerializeField] private GameObject recurringQuestPanel;
        [SerializeField] private TextMeshProUGUI recurringQuestTitle;
        [SerializeField] private TextMeshProUGUI recurringQuestProgress;

        [Header("일일 퀘스트")]
        [SerializeField] private GameObject dailyQuestPanel;
        [SerializeField] private TextMeshProUGUI dailyQuestTitle;
        [SerializeField] private TextMeshProUGUI dailyQuestProgress;

        //private Queue<IQuestStep> questQueue = new Queue<IQuestStep>(); //튜토
        //private List<IQuestStep> activeQuests = new List<IQuestStep>(); //순환
        private int currentIndex = 0;
        private bool doingQuest = false;

        void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            if (recurringQuestPanel != null) recurringQuestPanel.SetActive(false);
            if (questJsonFile != null) questDatabase.LoadFromJson(questJsonFile.text);
        }

        public void Init()
        {
            eventHub = GameManager.Instance.GetGameSystem<EventHub>();
        }

        public int GetOrder() => 300;

        void Start()
        {
            //InitializeQuestList();
            //await RunQuestLoopAsync();
            /*
            activeQuests = QuestList.GetRecurringQuests();
            if (activeQuests == null || activeQuests.Count == 0)
            {
                Debug.LogError("퀘스트 목록이 비어있습니다! QuestList를 확인하세요.");
                return;
            }
            Debug.Log("순환형 퀘스트 시스템 가동...");
            await RunCycleQuestLoop();
            */
            eventHub.OnLevelChange += (level) => UpdateQuest(GoalType.LevelUp, 0, level);
            eventHub.OnSkillUsed += (skillID) => UpdateQuest(GoalType.SkillUse, skillID, 1);
            eventHub.OnClearStage += (stageSO) => UpdateQuest(GoalType.StageClear, stageSO.stageKey, 1);

            RefreshQuests(); //초기 퀘스트
            RefreshUI();
        }

        void UpdateQuest(GoalType type, int targetID, int amount)
        {
            for (int i = activeQuests.Count - 1; i >= 0; i--)
            {
                var quest = activeQuests[i];
                if (quest.isCompleted) continue;
                
                if (quest.Data.GoalTypeEnum == type && (quest.Data.targetID == 0 || quest.Data.targetID == targetID))
                {
                    if (type == GoalType.LevelUp) quest.CurrentValue = amount;
                    else if (type == GoalType.Hunt) quest.CurrentValue += amount;
                    else if (type == GoalType.SkillUse) quest.CurrentValue += amount;
                    else if (type == GoalType.StageClear) quest.CurrentValue += amount;

                    RefreshUI();

                    if (quest.CurrentValue >= quest.Data.targetValue)
                    {
                        quest.isCompleted = true;
                        Debug.Log($"퀘스트 [{quest.Data.description}] 완료 가능");

                        TryCompleteQuest(quest);
                    }
                }
            }
        }
        public void TryCompleteQuest(ActiveQuest quest)
        {
            //GiveReward(quest.Data.RewardGroupID);
            QuestCategory currentCategory = quest.Data.CategoryEnum;
            completedQuestIds.Add(quest.Data.questID);
            activeQuests.Remove(quest);

            RefreshQuests();
            bool isAllCleared = false;
            //선형or데일리 퀘스트 목록이 비었다면 올클리어로 인식
            if (currentCategory == QuestCategory.Lineal || currentCategory == QuestCategory.Daily)
            {
                isAllCleared = !activeQuests.Any(q => q.Data.CategoryEnum == currentCategory);
            }
            if (eventHub != null) eventHub.QuestCompleted(quest.Data, isAllCleared); //퀘 완료 이벤트

            switch (currentCategory)
            {
                case QuestCategory.Lineal:
                    Debug.Log($"선형 퀘스트 {quest.Data.questID} 완료");
                    break;
                case QuestCategory.Recurring:
                    //여기서는 퀘스트 완료를 바로 초기화하지만 고민 필요
                    completedQuestIds.Add(quest.Data.questID);
                    if (quest.Data.questID == 102)
                    {
                        completedQuestIds.Remove(101);
                        completedQuestIds.Remove(102);
                        Debug.Log("<color=cyan>순환 사이클이 초기화되었습니다. 사이클의 처음으로 돌아갑니다.</color>");
                        RefreshQuests();
                    }
                    break;
                case QuestCategory.Daily:
                    Debug.Log($"일일 퀘스트 {quest.Data.questID} 완료");
                    break;
                case QuestCategory.Achievement:
                    // 업적은 보통 다음 단계 업적이 바로 나오도록 설계
                    // 예: 몬스터 100마리 -> 완료 -> 몬스터 500마리 업적 등장
                    break;
            }

            RefreshUI();
        }
        void RefreshQuests()
        {
            bool isChanged = false;
            foreach (var data in questDatabase.allQuests)
            {
                if (completedQuestIds.Contains(data.questID)) continue;
                if (activeQuests.Any(q => q.Data.questID == data.questID)) continue;

                if (data.prevQuestID == 0 || completedQuestIds.Contains(data.prevQuestID))
                {
                    ActiveQuest newQuest = new ActiveQuest(data);

                    //레벨 n 달성 퀘스트는 현재 레벨을 받아옴
                    if (data.isAbsoluteGoal && data.GoalTypeEnum == GoalType.LevelUp)
                    {
                        newQuest.CurrentValue = PlayerRuntimeStatus.Instance.Level;
                    }

                    activeQuests.Add(newQuest);
                    isChanged = true;
                }
            }
            // 수락하자마자 완료 조건인지 체크 (이미 레벨이 높은 경우 등)
            CheckCompleteCondition();
            if (isChanged) RefreshUI();
        }
        void CheckCompleteCondition()
        {

            for (int i = activeQuests.Count - 1; i >= 0; i--)
            {
                var quest = activeQuests[i];
                if (quest.CurrentValue >= quest.Data.targetValue)
                {
                    quest.isCompleted = true;
                    TryCompleteQuest(quest);
                }
            }
        }
        public void RefreshUI()
        {
            var linealQuest = activeQuests.FirstOrDefault(q => q.Data.CategoryEnum == QuestCategory.Lineal);
            UpdateSlot(linealQuest, linealQuestPanel, linealQuestTitle, linealQuestProgress);

            var recurringQuest = activeQuests.FirstOrDefault(q => q.Data.CategoryEnum == QuestCategory.Recurring);
            UpdateSlot(recurringQuest, recurringQuestPanel, recurringQuestTitle, recurringQuestProgress);

            var dailyQuest = activeQuests.FirstOrDefault(q => q.Data.CategoryEnum == QuestCategory.Daily);
            UpdateSlot(dailyQuest, dailyQuestPanel, dailyQuestTitle, dailyQuestProgress);
        }
        void UpdateSlot(ActiveQuest quest, GameObject panel, TextMeshProUGUI title, TextMeshProUGUI progress)
        {
            if (quest != null)
            {
                panel.SetActive(true);
                if (title != null) title.text = quest.Data.description;
                if (progress != null) progress.text = $"{quest.CurrentValue}/{quest.Data.targetValue}";
            }
            else panel.SetActive(false);
        }
        public void ResetDailyQuests()
        {
            activeQuests.RemoveAll(q => q.Data.CategoryEnum == QuestCategory.Daily);
            //ID 900~999번은 일일 퀘스트라고 가정
            completedQuestIds.RemoveWhere(id => id >= 900 && id < 999);
            RefreshQuests();
            RefreshUI();
        }
        void GiveReward(int rewardGroupID)
        {
            // 멘토님 조언대로 RewardData와 연동하는 로직이 들어갈 자리
            Debug.Log($"보상 그룹 {rewardGroupID}번 지급됨.");
        }
        public void UpdateQuestUI(string description)
        {
            if (linealQuestPanel != null) linealQuestPanel.SetActive(true);
            if (linealQuestTitle != null) linealQuestTitle.text = description;

            if (recurringQuestPanel != null) recurringQuestPanel.SetActive(true);
            if (recurringQuestTitle != null) recurringQuestTitle.text = description;

            if (dailyQuestPanel != null) dailyQuestPanel.SetActive(true);
            if (dailyQuestTitle != null) dailyQuestTitle.text = description;
        }

#if UNITY_EDITOR
        private int _debugLevel = 1;
        //Pull Request하기 전에 PlayerRuntimeStatus를 Player로 바꿀 것
        int GetCurrentPlayerLevel() //삭제 예정인 인스턴스에 의존하고 있음
        {
            if (PlayerRuntimeStatus.Instance != null) return PlayerRuntimeStatus.Instance.Level;
            return 1; //인스턴스가 없다면 나오는 기본값
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Delete))
            {
                _debugLevel++;
                Debug.Log($"<color=orange>[Test] 레벨업 조작! 현재 레벨: {_debugLevel}</color>");
                if (PlayerRuntimeStatus.Instance != null)
                    PlayerRuntimeStatus.Instance.Level = _debugLevel;
                eventHub.LevelChanged(_debugLevel);
            }
            if (Input.GetKeyDown(KeyCode.End))
            {
                Debug.Log("<color=orange>[Test] 스킬 사용!</color>");
                // EventHub에 알림 -> QuestManager가 이를 수신하여 UpdateQuest 실행
                eventHub.SkillUsed(1);
            }
            if (Input.GetKeyDown(KeyCode.PageDown))
            {
                Debug.Log("<color=orange>[Test] 몬스터 100마리 사냥 조작</color>");
                UpdateQuest(GoalType.Hunt, 901, 100);
            }
        }
#endif

        /*
        void InitializeQuestList() //모두 임시로 구현해놓은 것들입니다!
        {
            questQueue.Enqueue(new Tutorial_Click(
                "[튜토리얼]자동사냥 활성화해보기",
                autoHuntButton, guideArrow,
                () => GiveItem("기본 장비", 1)
                ));

            questQueue.Enqueue(new Tutorial_Action(
                "[튜토리얼]장비창을 열고 기본 장비 장착하기",
                "EquipItem",
                () => GiveCurrency("골드", 100)
                ));
            questQueue.Enqueue(new Tutorial_Action(
                "[튜토리얼]기본 장비 강화하기",
                "EnhanceItem",
                () => GiveCurrency("스탯 강화석", 100)
                ));
            
            questQueue.Enqueue(new Tutorial_Hunt(
                "[튜토리얼]일반 몬스터 5마리 처치하기", 5,
                () => GiveItem("스탯 강화석", 100)
                ));

            questQueue.Enqueue(new Tutorial_Action(
                "[튜토리얼]공격력 1회 강화하기", "Upgrade_ATK"
                ));
            questQueue.Enqueue(new Tutorial_Action(
                "[튜토리얼]체력 1회 강화하기", "Upgrade_HP"
                ));

            questQueue.Enqueue(new Tutorial_Action(
                "[튜토리얼]레벨업하기", "LevelUp"
                //여기서는 레벨업으로 인한 SP 획득이 이미 있음
                ));

            questQueue.Enqueue(new Tutorial_Action(
                "[튜토리얼]스킬 레벨 올리기", "Skill_LevelUp"
                ));
            questQueue.Enqueue(new Tutorial_Action(
                "[튜토리얼]스킬을 슬롯에 배치하기", "Skill_Equip"
                //이 단계에서 스킬 초기화도 언급해주기
                ));

            questQueue.Enqueue(new Tutorial_Action(
                "[튜토리얼]일반 스테이지 클리어하기", "Stage_Clear"
                ));

            questQueue.Enqueue(new Tutorial_Click(
                "[튜토리얼]보스 스테이지 진입하기",
                bossMapButton, guideArrow,
                () => GiveItem("", 0)
                ));

            questQueue.Enqueue(new Tutorial_Hunt(
                "[튜토리얼]보스 몬스터 처치하기", 1, null, "Boss_01"
                ));

            questQueue.Enqueue(new Tutorial_Click(
                "[튜토리얼]신규 컨텐츠가 해금되었습니다!",
                newContentButton, guideArrow,
                () => GiveItem("", 0)
                ));

            Debug.Log($"총 {questQueue.Count}개의 퀘스트가 등록되었습니다.");
        }

        void GiveItem(string name, int amount)
        {
            //실제 보상 지급 로직
            Debug.Log($"{name} {amount}개 획득!");
        }
        void GiveCurrency(string name, int amount)
        {
            //실제 보상 지급 로직
            Debug.Log($"{name} {amount}개 획득!");
        }
        */

        /*
        async Task RunTutorialQuestLoop()
        {
            if (doingQuest) return;
            doingQuest = true;

            //큐가 빌 때까지 반복
            while (questQueue.Count > 0)
            {
                IQuestStep currentStep = questQueue.Dequeue(); //다음 퀘스트 꺼내기

                //UI 표시, 연출
                UpdateQuestUI(currentStep.Description);
                currentStep.OnStartQuest();

                await currentStep.ExecuteStepAsync(); //퀘스트 대기

                currentStep.OnCompleteQuest();
                //await Task.Delay(500); //퀘스트 넘어가기 전에 연출이 필요하신가요?
            }

            // 모든 튜토리얼 종료
            TutorialPanel.SetActive(false);
            Debug.Log("모든 튜토리얼이 완료되었습니다!");
            doingQuest = false;
        }
        */
        /*
        async Task RunCycleQuestLoop()
        {
            while (true) // 무한 루프
            {
                IQuestStep currentStep = activeQuests[currentIndex];

                currentStep.OnStartQuest();
                await currentStep.ExecuteStepAsync();
                currentStep.OnCompleteQuest();
                currentIndex = (currentIndex + 1) % activeQuests.Count;

                Debug.Log("--------------------------------------");
                Debug.Log("즉시 다음 퀘스트로 넘어갑니다.");

                await Task.Delay(500); //시각적 구분을 위해 아주 짧은 딜레이
            }
        }
        */


        /*
        public async void AddTutorialQuest(IQuestStep newQuest)
        {
            questQueue.Enqueue(newQuest);
            if (!doingQuest) await RunTutorialQuestLoop();
        }
        */
    }

}
