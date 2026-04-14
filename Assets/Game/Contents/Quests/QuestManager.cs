using Base.Data;
using Base.Managers;
using Battle;
using Growth.Equipment;
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
        //public bool isLocked;
        public string lockMessage;
        public int RuntimeTargetValue; //지금 퀘스트의 실제 목표치
        public int RuntimeTargetID;
        public string RuntimeDescription; //수치가 치환된 설명

        //현재는 1회성 목표 달성이 n레벨 달성뿐임을 전제로 함
        public ActiveQuest(QuestDataReader data, int startValue)
        {
            //isCompleted = false;

            this.Data = data;
            this.StartValue = startValue;
            this.CurrentValue = 0; // 초기값, 이후 매니저가 계산
            //this.isCompleted = false;
            //this.isLocked = isLocked;
            //this.lockMessage = lockMessage;
            this.RuntimeTargetValue = data.targetValue;
            this.RuntimeTargetID = data.targetID;
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
        /*
        public void UnlockQuest(int startValue, int curGlobalStat)
        {
            this.isLocked = false;
            this.StartValue = startValue;

            if (this.Data.isAbsoluteGoal)
                this.CurrentValue = curGlobalStat;
            else
                this.CurrentValue = curGlobalStat - startValue;

            if (this.CurrentValue >= this.RuntimeTargetValue)
                this.isCompleted = true;
        }
        */
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
        //[SerializeField] private QuestUIManager questUIManager;

        //모든 누적 수치 기록
        private Dictionary<string, int> globalStats = new Dictionary<string, int>();
        //퀘스트가 시작됐을 때의 기준점
        private Dictionary<int, int> questStartPoints = new Dictionary<int, int>();

        [Header("퀘스트 데이터")]
        [SerializeField] private QuestDatabaseSO questDatabase;
        //[SerializeField] private TextAsset questJsonFile;

        [Header("튜토리얼용 오브젝트들")]
        public Button autoHuntButton;
        public Button bossMapButton;
        public Button newContentButton;
        public GameObject guideArrow;
        public GameObject TutorialPanel; //튜토리얼 설명 텍스트가 있는 패널
        public Text TutorialDescText;

        //[SerializeField] private RewardItemRegistrySO itemDatabase;
        //[SerializeField] private QuestRewardSO rewardDatabase;
        //[SerializeField] private RewardPopupUI rewardPopup;
        [Header("보상 설정")]
        [SerializeField] private ItemDropManager itemDropManager;
        [SerializeField] private RewardPopupUI rewardPopup;

        // [중요] ID와 RewardTableSO 에셋을 매칭하기 위한 딕셔너리
        // 인스펙터에서 설정할 수 있도록 리스트 형태로 먼저 선언합니다.
        [System.Serializable]
        public struct RewardTableMap
        {
            public int groupID;
            public RewardTableSO table;
        }

        [SerializeField] private List<RewardTableMap> rewardTableMaps;

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
        }

        public void Init()
        {
            eventHub = FindObjectOfType<Base.Data.EventHub>();
            player = GameManager.Instance.GetGameSystem<PlayerManager>().GetComponent<Player>();

            if (eventHub != null)
            {
                eventHub.OnLevelChange += (level) => OnActivity(GoalType.LevelUp, 0, level);
                eventHub.OnSkillUsed += (skillID) => OnActivity(GoalType.SkillUse, skillID, 1);
                eventHub.OnClearStage += (stageSO) =>
                {
                    //5번째 스테이지를 클리어했을 때만 퀘스트 진행
                    if (stageSO.stage == 5)
                    {
                        OnActivity(GoalType.StageClear, stageSO.chapter, 1);
                    }
                };
                EventHub.OnNewDayStarted += (dateStr) => ResetDailyQuests();
                RefreshQuests(); //초기 퀘스트
                EventHub.QuestProgressUpdated();
            }
            //if (questJsonFile != null) questDatabase.LoadFromJson(questJsonFile.text);
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
        #endregion

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

                int requiredID = quest.RuntimeTargetID;
                if (requiredID != 0 && requiredID != targetID) continue;

                string statKey = $"{type}_{targetID}";
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
            //1. 실제 아이템 지급(팀원 시스템 호출)
            GiveRewardsToPlayer(quest.Data.rewardGroupID);

            //2. 보상 팝업 표시(사용자 UI 시스템)
            if (!suppressPopup && rewardPopup != null)
            {
                var rewards = GetRewardsByGroupID(quest.Data.rewardGroupID);
                if (rewards.Count > 0) rewardPopup.ShowRewards(rewards);
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
            FindObjectOfType<QuestUIManager>()?.RefreshQuestBox();
            SaveProgress();
            EventHub.QuestProgressUpdated();
        }

        //rewardGroupID에 따른 보상 목록 반환
        public List<RewardData> GetRewardsByGroupID(int groupID)
        {
            List<RewardData> result = new List<RewardData>();

            var map = rewardTableMaps.Find(x => x.groupID == groupID);
            if (map.table == null) return result;

            //보상 그룹 중에서 필요한 목록 검색
            foreach (var drop in map.table.rewardList)
            {
                // [로직 추가] 아이템(장비) 타입인 경우 수량만큼 반복해서 리스트에 추가
                if (drop.rewardType == DropRewardType.Item)
                {
                    for (int i = 0; i < drop.amount; i++)
                    {
                        result.Add(CreateRewardData(drop, 1)); //수량을 무조건 1로 세팅
                    }
                }
                else // 재화(Currency)인 경우 기존처럼 한꺼번에 추가
                {
                    result.Add(CreateRewardData(drop, drop.amount));
                }
            }
            return result;
        }
        // 중복 코드를 방지하기 위한 데이터 생성 헬퍼 함수
        private RewardData CreateRewardData(DropReward drop, int finalAmount)
        {
            RewardData uiData = new RewardData();
            uiData.amount = finalAmount;
            uiData.currencyType = drop.currencyType;

            if (drop.rewardType == DropRewardType.Currency)
            {
                uiData.itemName = drop.currencySO.currencyName;
                uiData.icon = drop.currencySO.icon;
                uiData.originalSO = drop.currencySO;
                uiData.description = drop.currencySO.explain;
            }
            else
            {
                uiData.itemName = drop.itemSO.itemName;
                uiData.icon = drop.itemSO.icon;
                uiData.originalSO = drop.itemSO;
                uiData.description = "";
            }
            return uiData;
        }
        //실제 아이템 지급을 요청할 때 사용
        public void GiveRewardsToPlayer(int groupID)
        {
            var map = rewardTableMaps.Find(x => x.groupID == groupID);
            if (map.table != null && itemDropManager != null)
            {
                //팀원이 만든 실제 지급 메서드 호출
                itemDropManager.GetRewards(map.table.rewardList, true);
            }
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

                // [수정] 리스트에 이미 있는지 확인하고 객체를 미리 가져옵니다.
                //ActiveQuest existingQuest = activeQuests.FirstOrDefault(q => q.Data.questID == data.questID);

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

                    if (!questStartPoints.ContainsKey(data.questID))
                    {
                        questStartPoints[data.questID] = data.isAbsoluteGoal ? 0 : currentGlobalStat;
                    }

                    // 단순 생성 로직
                    ActiveQuest newQuest = new ActiveQuest(data, questStartPoints[data.questID]);
                    if (data.isInfinite)
                    {
                        if (!questSeriesSteps.ContainsKey(data.questID)) questSeriesSteps[data.questID] = 1;
                        int currentStep = questSeriesSteps[data.questID];

                        if (data.GoalTypeEnum == GoalType.StageClear)
                        {
                            newQuest.RuntimeTargetID = currentStep;
                            newQuest.RuntimeTargetValue = data.targetValue;
                            newQuest.RuntimeDescription = string.Format(data.description, currentStep);
                        }
                        else
                        {
                            newQuest.RuntimeTargetValue = data.targetValue + (data.valueModifier * (currentStep - 1));
                            newQuest.RuntimeDescription = string.Format(data.description, newQuest.RuntimeTargetValue);
                        }
                    }

                    newQuest.CurrentValue = GetCalculatedValue(newQuest, currentGlobalStat);
                    if (newQuest.CurrentValue >= newQuest.RuntimeTargetValue) newQuest.isCompleted = true;

                    activeQuests.Add(newQuest);
                    isChanged = true;
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

        void OnDestroy()
        {
            eventHub.OnLevelChange -= (level) => OnActivity(GoalType.LevelUp, 0, level);
            eventHub.OnSkillUsed -= (skillID) => OnActivity(GoalType.SkillUse, skillID, 1);
            eventHub.OnClearStage -= (stageSO) => OnActivity(GoalType.StageClear, stageSO.stageKey, 1);
            EventHub.OnNewDayStarted -= (dateStr) => ResetDailyQuests();
        }
    }
}
