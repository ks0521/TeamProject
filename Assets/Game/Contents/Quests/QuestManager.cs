using Base.Data;
using Base.Managers;
using Battle;
using QuestSystem.TutorialSteps;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.UI;
using static QuestSystem.TutorialSteps.Tutorial_Click;

namespace QuestSystem
{
    [System.Serializable]
    public class ActiveQuest
    {
        public QuestDataReader Data { get; private set; } //연결된 원본 데이터
        public int StartValue; //퀘스트 시작 시점의 값
        public int CurrentValue; //현재 진행 수치
        public QuestStatus questStatus;
        public bool isCompleted;
        public int RuntimeTargetValue; //지금 퀘스트의 실제 목표치
        public string RuntimeDescription; //수치가 치환된 설명

        //현재는 1회성 목표 달성이 n레벨 달성뿐임을 전제로 함
        public ActiveQuest(QuestDataReader data, int startValue)
        {
            isCompleted = false;

            this.Data = data;
            this.StartValue = startValue;
            this.isCompleted = false;
            this.CurrentValue = 0; // 초기값, 이후 매니저가 계산
            this.RuntimeTargetValue = data.targetValue;
            this.RuntimeDescription = data.description;
        }

        public QuestStatus GetCurrentStatus()
        {
            if (isCompleted) return QuestStatus.Completable;
            return QuestStatus.Ongoing;
            //BeforeStart, Clear 상태의 사용 여부, 사용 조건은 논의 필요
        }
        public string GetStatusText()
        {
            switch (GetCurrentStatus())
            {
                case QuestStatus.BeforeStart: return "시작 전";
                case QuestStatus.Ongoing: return "진행 중";
                case QuestStatus.Completable: return "완료 가능";
                case QuestStatus.Clear: return "완료됨";
                default: return "";
            }
        }
    }

    [System.Serializable]
    public class QuestSaveData
    {
        //Dictionary를 저장하기 위한 리스트 구조
        public List<string> statKeys = new List<string>();
        public List<int> statValues = new List<int>();

        public List<int> startPointKeys = new List<int>();
        public List<int> startPointValues = new List<int>();

        public List<int> seriesStepKeys = new List<int>();
        public List<int> seriesStepValues = new List<int>();

        public List<int> completedQuestIds = new List<int>();
    }

    public class QuestManager : MonoBehaviour, IManager
    {
        public static QuestManager Instance { get; private set; }
        private Player player;
        [SerializeField] private EventHub eventHub;
        private QuestSO questSO;
        private List<ActiveQuest> activeQuests = new List<ActiveQuest>(); //진행 중
        private HashSet<int> completedQuestIds = new HashSet<int>(); //완료

        //모든 누적 수치 기록
        private Dictionary<string, int> globalStats = new Dictionary<string, int>();
        //퀘스트가 시작됐을 때의 기준점
        private Dictionary<int, int> questStartPoints = new Dictionary<int, int>();

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

        [Header("보상 설정")]
        [SerializeField] private RewardItemRegistrySO itemDatabase;
        [SerializeField] private QuestRewardSO rewardDatabase;
        [SerializeField] private RewardPopupUI rewardPopup;

        //무한 반복형 퀘스트에 사용할 단계 딕셔너리
        private Dictionary<int, int> questSeriesSteps = new Dictionary<int, int>();

        //퀘스트 데이터 저장 경로
        private string SavePath => Path.Combine(Application.persistentDataPath, "QuestSave.json");

        private int currentIndex = 0;
        private bool doingQuest = false;
        private const string SelectedDailyKey = "SelectedDailyQuestID";
        private int currentDailyID = 0; //0이면 아직 선택 안 됨

        void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            if (questJsonFile != null) questDatabase.LoadFromJson(questJsonFile.text);
        }

        public void Init()
        {
            eventHub = FindObjectOfType<Base.Data.EventHub>();
            player = GameManager.Instance.GetGameSystem<PlayerManager>().GetComponent<Player>();

            eventHub.OnLevelChange += (level) => OnActivity(GoalType.LevelUp, 0, level);
            eventHub.OnSkillUsed += (skillID) => OnActivity(GoalType.SkillUse, skillID, 1);
            eventHub.OnClearStage += (stageSO) => OnActivity(GoalType.StageClear, stageSO.stageKey, 1);
            EventHub.OnNewDayStarted += (dateStr) => ResetDailyQuests();
            RefreshQuests(); //초기 퀘스트
            EventHub.QuestProgressUpdated();

            if (questJsonFile != null) questDatabase.LoadFromJson(questJsonFile.text);
            LoadProgress();
            RefreshQuests();
        }

        public int GetOrder() => 300;

        #region 퀘스트 저장/불러오기/초기화
        //저장된 퀘스트 불러오기
        public void SaveProgress()
        {
            QuestSaveData saveData = new QuestSaveData();

            foreach (var kvp in globalStats)
            {
                saveData.statKeys.Add(kvp.Key);
                saveData.statValues.Add(kvp.Value);
            }
            foreach (var kvp in questStartPoints)
            {
                saveData.startPointKeys.Add(kvp.Key);
                saveData.startPointValues.Add(kvp.Value);
            }
            foreach (var kvp in questSeriesSteps)
            {
                saveData.seriesStepKeys.Add(kvp.Key);
                saveData.seriesStepValues.Add(kvp.Value);
            }

            saveData.completedQuestIds = new List<int>(completedQuestIds);

            string json = JsonUtility.ToJson(saveData, true);
            File.WriteAllText(SavePath, json);
            Debug.Log($"[QuestManager] 데이터 저장 완료: {SavePath}");
        }
        public void LoadProgress()
        {
            if (!File.Exists(SavePath))
            {
                Debug.Log("[QuestManager] 저장된 데이터가 없습니다. 새로 시작합니다.");
                return;
            }
            string json = File.ReadAllText(SavePath);
            QuestSaveData saveData = JsonUtility.FromJson<QuestSaveData>(json);

            globalStats.Clear();
            for (int i = 0; i < saveData.statKeys.Count; i++)
                globalStats[saveData.statKeys[i]] = saveData.statValues[i];
            questStartPoints.Clear();
            for (int i = 0; i < saveData.startPointKeys.Count; i++)
                questStartPoints[saveData.startPointKeys[i]] = saveData.startPointValues[i];
            questSeriesSteps.Clear();
            for (int i = 0; i < saveData.seriesStepKeys.Count; i++)
                questSeriesSteps[saveData.seriesStepKeys[i]] = saveData.seriesStepValues[i];

            completedQuestIds = new HashSet<int>(saveData.completedQuestIds);
            Debug.Log("[QuestManager] 데이터 로드 완료");
        }

        //테스트용: 저장된 퀘스트 내역 삭제 버튼
        [ContextMenu("Reset All Quest Progress")] // 유니티 인스펙터에서 우클릭으로 실행 가능
        public void ClearSaveData()
        {
            if (File.Exists(SavePath))
            {
                File.Delete(SavePath);
                Debug.Log("<color=red>퀘스트 세이브 파일이 삭제되었습니다.</color>");
            }

            //PlayerPrefs에 저장된 일퀘 정보 삭제
            PlayerPrefs.DeleteKey(SelectedDailyKey);
            PlayerPrefs.DeleteKey("LastResetDate"); //DailyQuestManager 등에서 쓰는 날짜 키
            PlayerPrefs.Save();

            //그 외의 데이터 초기화
            globalStats.Clear();
            questStartPoints.Clear();
            questSeriesSteps.Clear();
            completedQuestIds.Clear();
            activeQuests.Clear();

            RefreshQuests();
            EventHub.QuestProgressUpdated();
            FindObjectOfType<QuestUIManager>()?.RefreshQuestBox();
            Debug.Log("<color=yellow>모든 퀘스트 진행도가 초기화되었습니다!</color>");
        }
        #endregion

        //활성화된 모든 퀘스트 리스트 반환
        public List<ActiveQuest> GetActiveQuests()
        {
            return activeQuests;
        }
        //특정 카테고리의 활성 퀘스트 필터링
        public List<ActiveQuest> GetActiveQuestsByCategory(QuestCategory category)
        {
            return activeQuests.FindAll(q => q.Data.CategoryEnum == category);
        }

        //퀘스트 진척량이 누적될 때 호출(UpdateQuest 대신 사용)
        public void OnActivity(GoalType type, int targetID, int amount)
        {
            //실제로 발생한 targetID 기록
            string statKey = $"{type}_{targetID}";
            if (!globalStats.ContainsKey(statKey)) globalStats[statKey] = 0;

            if (type == GoalType.LevelUp)
                globalStats[statKey] = amount; //레벨은 갱신
            else
                globalStats[statKey] += amount; //사냥, 스킬 등은 누적

            //아무 스킬 시전에도 대응하려고 만든 부분
            if (targetID != 0)
            {
                string allKey = $"{type}_0";
                if (!globalStats.ContainsKey(allKey)) globalStats[allKey] = 0;

                if (type == GoalType.LevelUp) globalStats[allKey] = amount;
                else globalStats[allKey] += amount;
            }

            UpdateActiveQuestsProgress(type, targetID);
        }
        //모든 활성 퀘스트의 수치를 globalStats 기준으로 새로고침하는 함수
        void UpdateActiveQuestsProgress(GoalType type, int targetID)
        {
            foreach (var quest in activeQuests)
            {
                if (quest.Data.GoalTypeEnum != type) continue;
                if (quest.Data.targetID != 0 && quest.Data.targetID != targetID) continue;

                string statKey = $"{quest.Data.GoalTypeEnum}_{quest.Data.targetID}";
                int totalProgress = globalStats.ContainsKey(statKey) ? globalStats[statKey] : 0;
                int startPoint = questStartPoints.ContainsKey(quest.Data.questID) ? questStartPoints[quest.Data.questID] : 0;
                //진척도 계산: 절대 누적치와 상대치 구분
                quest.CurrentValue = quest.Data.isAbsoluteGoal ? totalProgress : totalProgress - startPoint;
                if (quest.CurrentValue >= quest.RuntimeTargetValue)
                {
                    quest.isCompleted = true;
                }
            }
            EventHub.QuestProgressUpdated();
        }
        public void OnClickComplete(string categoryString)
        {
            QuestCategory category = (QuestCategory)System.Enum.Parse(typeof(QuestCategory), categoryString);
            //카테고리 내의 퀘스트 중 완료 가능한 것 탐색
            var targetQuest = activeQuests.FirstOrDefault(q => q.Data.CategoryEnum == category && q.isCompleted);
            if (targetQuest != null) TryCompleteQuest(targetQuest);
        }
        public void TryCompleteQuest(ActiveQuest quest, bool suppressPopup = false)
        {
            //보상 리스트 확보 후 팝업 띄우기
            List<RewardData> rewards = GetRewardsByGroupID(quest.Data.rewardGroupID);
            //suppressPopup이 false = 개별 팝업
            if (rewardPopup != null && rewards.Count > 0 && !suppressPopup)
            {
                rewardPopup.ShowRewards(rewards);
            }

            QuestCategory currentCategory = quest.Data.CategoryEnum;
            int qID = quest.Data.questID;

            if (quest.Data.isInfinite) //무한 퀘스트(업적 등)일 때
            {
                // 1. 단계를 올립니다. (RefreshQuests가 이 값을 보고 다음 수치를 계산함)
                if (!questSeriesSteps.ContainsKey(qID)) questSeriesSteps[qID] = 1;

                // 2. 시작점 전진 (상대적 목표일 경우 초과분 이월)
                if (!quest.Data.isAbsoluteGoal)
                {
                    questStartPoints[qID] += quest.RuntimeTargetValue;
                }
                // [핵심] 무한 퀘스트는 completedQuestIds.Add(qID)를 하지 않습니다!
                // 그래야 RefreshQuests에서 "아직 완료 안 된 녀석"으로 인식되어 다시 생성됩니다.
                questSeriesSteps[qID]++;
                Debug.Log($"<color=cyan>무한 퀘스트: {questSeriesSteps[qID]}단계로 진입</color>");
            }
            else if (currentCategory == QuestCategory.Recurring && !quest.Data.isAbsoluteGoal)
            {
                //일반 순환 퀘스트 처리(고정 수치만큼 전진)
                questStartPoints[qID] += quest.Data.targetValue;
            }
            else
            {
                //선형, 데일리, 일반 업적: 완료 후 삭제
                completedQuestIds.Add(qID);
            }

            if (qID == 102)
            {
                completedQuestIds.Remove(101);
                completedQuestIds.Remove(102);
            }

            //활성 리스트에서 제거
            activeQuests.Remove(quest);
            bool isAllCleared = false;
            if (currentCategory == QuestCategory.Lineal || currentCategory == QuestCategory.Daily)
            {
                isAllCleared = !activeQuests.Any(q => q.Data.CategoryEnum == currentCategory);
            }
            eventHub.QuestCompleted(quest.Data, isAllCleared);

            RefreshQuests();
            SaveProgress();
            EventHub.QuestProgressUpdated();
        }
        //rewardGroupID에 따른 보상 목록 반환
        public List<RewardData> GetRewardsByGroupID(int groupID)
        {
            List<RewardData> rewards = new List<RewardData>();

            var group = rewardDatabase.GetGroup(groupID);
            if (group == null) return rewards;

            //보상 그룹 중에서 필요한 목록 검색
            foreach (var r in group.items)
            {
                var info = rewardDatabase.GetGroup(r.itemID);
                if (group == null) return rewards;
            }

            //그룹 내의 아이템들을 RewardData로 변환
            foreach (var r in group.items)
            {
                var info = itemDatabase.GetItem(r.itemID);
                if (info != null)
                {
                    rewards.Add(new RewardData
                    {
                        itemID = r.itemID,
                        itemName = info.itemName,
                        amount = r.amount,
                        icon = info.itemIcon
                    });
                }
            }
            return rewards;
        }

        void RefreshQuests()
        {
            bool isChanged = false;
            //저장된 일퀘 ID 호출(없다면 0으로)
            currentDailyID = PlayerPrefs.GetInt(SelectedDailyKey, 0);
            foreach (var data in questDatabase.allQuests)
            {
                if (completedQuestIds.Contains(data.questID)) continue;
                if (activeQuests.Any(q => q.Data.questID == data.questID)) continue;

                if (data.CategoryEnum == QuestCategory.Daily)
                {
                    if (currentDailyID == 0)
                    {
                        PickRandomDailyQuest();
                        //뽑은 직후 다시 루프를 돌거나 현재 data와 비교?
                    }
                    //뽑히지 않은 퀘스트는 무시
                    if (data.questID != currentDailyID) continue;
                }
                if (data.prevQuestID == 0 || completedQuestIds.Contains(data.prevQuestID))
                {
                    string statKey = $"{data.goalType}_{data.targetID}";
                    int currentGlobalStat = globalStats.ContainsKey(statKey) ? globalStats[statKey] : 0;
                    //이 퀘스트를 처음 접할 때만 현재 통계를 기록하고 저장
                    if (!questStartPoints.ContainsKey(data.questID))
                    {
                        //레벨 달성, 총 n골드 획득 등의 누적 퀘스트는 기준점이 0
                        questStartPoints[data.questID] = data.isAbsoluteGoal ? 0 : currentGlobalStat;
                    }

                    ActiveQuest newQuest = new ActiveQuest(data, questStartPoints[data.questID]);

                    if (data.isInfinite)
                    {
                        //퀘스트 단계 확인(없으면 1단계)
                        if (!questSeriesSteps.ContainsKey(data.questID)) questSeriesSteps[data.questID] = 1;
                        int currentStep = questSeriesSteps.ContainsKey(data.questID) ? questSeriesSteps[data.questID] : 1;

                        //이번 퀘스트의 목표치(시작값 + (증가량 * (단계-1)))
                        newQuest.RuntimeTargetValue = data.targetValue + (data.valueModifier * (currentStep - 1));
                        newQuest.RuntimeDescription = string.Format(data.description, newQuest.RuntimeTargetValue);
                    }
                    //ActiveQuest 생성 시 현재 진척도 계산
                    newQuest.CurrentValue = GetCalculatedValue(newQuest, currentGlobalStat);

                    //퀘스트 즉시 완료 여부 체크
                    if (newQuest.CurrentValue >= newQuest.RuntimeTargetValue)
                    {
                        newQuest.isCompleted = true;
                    }
                    activeQuests.Add(newQuest);
                    isChanged = true;
                }
            }
            // 수락하자마자 완료 조건인지 체크 (이미 레벨이 높은 경우 등)
            if (isChanged)
            {
                CheckCompleteCondition();
                EventHub.QuestProgressUpdated();
            }
        }
        int GetCalculatedValue(ActiveQuest quest, int currentGlobalStat)
        {
            //절대 목표는 전체 통계값 그대로 사용
            if (quest.Data.isAbsoluteGoal) return currentGlobalStat;

            //상대 목표는 (현재 - 시작점)
            return currentGlobalStat - quest.StartValue;
        }
        void CheckCompleteCondition()
        {
            for (int i = activeQuests.Count - 1; i >= 0; i--)
            {
                var quest = activeQuests[i];
                if (quest.CurrentValue >= quest.RuntimeTargetValue)
                {
                    quest.isCompleted = true;
                }
            }
        }

        public void ResetDailyQuests()
        {
            activeQuests.RemoveAll(q => q.Data.CategoryEnum == QuestCategory.Daily);
            //ID 900~999번은 일일 퀘스트라고 가정
            completedQuestIds.RemoveWhere(id => id >= 900 && id <= 999);

            //저장된 일퀘 초기화
            PlayerPrefs.DeleteKey(SelectedDailyKey);
            currentDailyID = 0;
            PickRandomDailyQuest();
            RefreshQuests();
            EventHub.QuestProgressUpdated();
        }

        void PickRandomDailyQuest()
        {
            //Daily 퀘스트만 필터링
            var dailyPool = questDatabase.allQuests
                .Where(q => q.CategoryEnum == QuestCategory.Daily)
                .ToList();

            if (dailyPool.Count > 0)
            {
                int randomIndex = UnityEngine.Random.Range(0, dailyPool.Count);
                currentDailyID = dailyPool[randomIndex].questID;

                //자정까지 선택된 퀘스트ID 저장
                PlayerPrefs.SetInt(SelectedDailyKey, currentDailyID);
                PlayerPrefs.Save();
                Debug.Log($"[QuestManager] 오늘의 일일 퀘스트: {currentDailyID}");
            }
        }


#if UNITY_EDITOR
        private int _debugLevel = 1;
        //Pull Request하기 전에 PlayerRuntimeStatus를 Player로 바꿀 것
        int GetCurrentPlayerLevel() //삭제 예정인 인스턴스에 의존하고 있음
        {
            if (RuntimeStatus.Instance != null) return player.Level;
            return 1; //인스턴스가 없다면 나오는 기본값
        }
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Delete))
            {
                _debugLevel++;
                Debug.Log($"<color=orange>[Test] 레벨업 조작! 현재 레벨: {_debugLevel}</color>");
                if (RuntimeStatus.Instance != null)
                    player.Level = _debugLevel;
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
                OnActivity(GoalType.Hunt, 901, 100);
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
        void OnDestroy()
        {
            eventHub.OnLevelChange -= (level) => OnActivity(GoalType.LevelUp, 0, level);
            eventHub.OnSkillUsed -= (skillID) => OnActivity(GoalType.SkillUse, skillID, 1);
            eventHub.OnClearStage -= (stageSO) => OnActivity(GoalType.StageClear, stageSO.stageKey, 1);
            EventHub.OnNewDayStarted -= (dateStr) => ResetDailyQuests();
        }
    }
}
