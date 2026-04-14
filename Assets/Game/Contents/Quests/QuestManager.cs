using Base.Data;
using Base.Managers;
using Base.Save;
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
        public int currentStep; //반복 퀘스트의 현재 회차
        public QuestStatus questStatus;
        public bool isCompleted;
        public bool isLocked;
        public string lockMessage;
        public int RuntimeTargetValue; //지금 퀘스트의 실제 목표치
        public int RuntimeTargetID;
        public string RuntimeDescription; //수치가 치환된 설명

        public ActiveQuest(QuestDataReader data, int startValue, bool isLocked = false)
        {
            //isCompleted = false;

            this.Data = data;
            this.StartValue = startValue;
            this.CurrentValue = 0; // 초기값, 이후 매니저가 계산
            //this.isCompleted = false;
            this.isLocked = isLocked;
            //this.lockMessage = lockMessage;
            this.RuntimeTargetValue = data.targetValue;
            //this.RuntimeTargetID = data.targetID;
            this.RuntimeDescription = data.description;
        }

        public QuestStatus GetCurrentStatus()
        {
            if (isCompleted) return QuestStatus.Completable;
            else if (isLocked) return QuestStatus.BeforeStart;
            return QuestStatus.Ongoing;
            //Clear 상태의 사용 여부, 사용 조건은 논의 필요
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
            GiveRewardsToPlayer(quest.Data.rewardGroupID);

            if (!suppressPopup && rewardPopup != null)
            {
                var rewards = GetRewardsByGroupID(quest.Data.rewardGroupID);
                if (rewards.Count > 0) rewardPopup.ShowRewards(rewards);
            }

            //퀘스트 완료 및 다음 퀘스트 처리 구간
            QuestCategory currentCategory = quest.Data.CategoryEnum;
            int qID = quest.Data.questID;

            if (currentCategory == QuestCategory.Recurring)
            {
                questStartPoints[qID] += quest.RuntimeTargetValue;
            }
            else if (quest.Data.isInfinite)
            {
                //회차 누적(RefreshQuests가 이 값을 보고 다음 수치를 계산함)
                if (!questSeriesSteps.ContainsKey(qID)) questSeriesSteps[qID] = 1;

                //시작점 전진(상대적 목표일 경우 초과분 이월)
                if (!quest.Data.isAbsoluteGoal)
                {
                    questStartPoints[qID] += quest.RuntimeTargetValue;
                }
                //무한 퀘스트는 completedQuestIds.Add(qID)를 하지 않음
                questSeriesSteps[qID]++;
                Debug.Log($"<color=cyan>무한 퀘스트: {questSeriesSteps[qID]}단계로 진입</color>");
            }
            else //선형, 데일리, 일반 업적: 완료 후 삭제
            {
                completedQuestIds.Add(qID);
            }

            if(currentCategory == QuestCategory.Recurring)
            {
                completedQuestIds.Remove(quest.Data.nextQuestID);
                completedQuestIds.Add(quest.Data.questID);
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
            EventHub.QuestProgressUpdated();
            FindObjectOfType<QuestUIManager>()?.RefreshQuestBox();
            SaveProgress();
            
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
                //순환형 퀘스트는 무조건 통과
                if (data.CategoryEnum != QuestCategory.Recurring && completedQuestIds.Contains(data.questID)) continue;

                if (data.CategoryEnum == QuestCategory.Daily)
                {
                    if (currentDailyID == 0) PickRandomDailyQuest();
                    //뽑히지 않은 퀘스트는 무시
                    if (data.questID != currentDailyID) continue;
                }

                if (IsQuestUnlocked(data))
                {
                    //해당 ID의 퀘스트가 리스트에 있다면 먼저 제거(상태 갱신용),
                    //이후 현재 진행해야 할 단계 생성 (InProgress)
                    activeQuests.RemoveAll(q => q.Data.questID == data.questID);
                    ActiveQuest current = CreateQuestObject(data, false);
                    activeQuests.Add(current);

                    //무한 퀘스트라면 딱 '다음 단계' 하나만 잠금 상태로 추가
                    if (data.isInfinite)
                    {
                        ActiveQuest nextLocked = CreateQuestObject(data, true);
                        activeQuests.Add(nextLocked);
                    }

                    isChanged = true;
                }
            }
            //수락하자마자 완료가 가능한가?
            if (isChanged)
            {
                CheckCompleteCondition();
                EventHub.QuestProgressUpdated();
                FindObjectOfType<QuestUIManager>()?.RefreshQuestBox();
            }
        }
        int GetCalculatedValue(ActiveQuest quest, int currentGlobalStat)
        {
            //절대 목표는 전체 통계값 그대로 사용, 상대 목표는 (현재 - 시작점)
            if (quest.Data.isAbsoluteGoal) return currentGlobalStat;

            int startPoint = questStartPoints.ContainsKey(quest.Data.questID) ? questStartPoints[quest.Data.questID] : 0;
            int resultValue = currentGlobalStat - startPoint;
            return resultValue < 0 ? 0 : resultValue;
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
        ActiveQuest CreateQuestObject(QuestDataReader data, bool isLocked)
        {
            string statKey = data.goalType.ToString();
            int currentGlobalStat = globalStats.ContainsKey(statKey) ? globalStats[statKey] : 0;

            if (!questStartPoints.ContainsKey(data.questID))
                questStartPoints[data.questID] = data.isAbsoluteGoal ? 0 : currentGlobalStat;

            ActiveQuest newQuest = new ActiveQuest(data, questStartPoints[data.questID], isLocked);
            //유저가 깨야 할 단계, 프리팹에 표시할 단계
            int currentStep = questSeriesSteps.ContainsKey(data.questID) ? questSeriesSteps[data.questID] : 1;

            int displayStep = isLocked ? currentStep + 1 : currentStep;
            newQuest.currentStep = currentStep; //회차 저장

            if (data.isInfinite)
            {
                if (data.GoalTypeEnum == GoalType.StageClear)
                {
                    newQuest.RuntimeTargetID = displayStep;
                    newQuest.RuntimeTargetValue = data.targetValue;
                    newQuest.RuntimeDescription = string.Format(data.description, displayStep);
                }
                else
                {
                    newQuest.RuntimeTargetValue = data.targetValue + (data.valueModifier * (displayStep - 1));
                    newQuest.RuntimeDescription = string.Format(data.description, newQuest.RuntimeTargetValue);
                }

                if (isLocked)
                {
                    string prevStepText = "";
                    if (data.GoalTypeEnum == GoalType.StageClear)
                        prevStepText = string.Format(data.description, currentStep);
                    else
                        prevStepText = string.Format(data.description, data.targetValue + (data.valueModifier * (currentStep - 1)));

                    newQuest.lockMessage = $"{prevStepText} 완료 후 해금";
                }
            }
            else //일반 퀘스트 처리
            {
                newQuest.RuntimeTargetValue = data.targetValue;
                newQuest.RuntimeDescription = data.description;
                if (isLocked) newQuest.lockMessage = "이전 퀘스트 완료 후 해금";
            }

            //진행도 및 완료 여부 계산(잠기지 않은 경우에만)
            if (!isLocked)
            {
                newQuest.CurrentValue = GetCalculatedValue(newQuest, currentGlobalStat);
                if (newQuest.CurrentValue >= newQuest.RuntimeTargetValue) newQuest.isCompleted = true;
            }
            else
            {
                newQuest.CurrentValue = 0;
                newQuest.isCompleted = false;
            }

            return newQuest;
        }
        bool IsQuestUnlocked(QuestDataReader data)
        {
            //순환형은 완료 기록이 없어야만 생성
            //if (data.CategoryEnum == QuestCategory.Recurring && completedQuestIds.Contains(data.prevQuestID)) return false;
            //선행 퀘스트가 없거나, 완료 기록이 없으면 생성
            if (data.prevQuestID == 0) return true;
            if (completedQuestIds.Contains(data.prevQuestID)) return true;

            //선행 퀘스트가 '무한 퀘스트'일 경우의 비교 로직
            //선행 퀘스트의 회차가 내 퀘스트의 회차보다 크면 해금
            if (questSeriesSteps.ContainsKey(data.prevQuestID))
            {
                int prevQuestStep = questSeriesSteps[data.prevQuestID];
                int myStep = questSeriesSteps.ContainsKey(data.questID) ? questSeriesSteps[data.questID] : 1;

                if (prevQuestStep > myStep) return true;
            }

            return false;
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
        private int _testChapter = 1;
        private int _testStage = 0;
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
            if (Input.GetKeyDown(KeyCode.PageUp))
            {
                if (eventHub != null)
                {
                    _testStage++;
                    if (_testStage > 5)
                    {
                        _testChapter++;
                        _testStage = 1;
                    }
                    StageSO dummyStage = ScriptableObject.CreateInstance<StageSO>();
                    dummyStage.chapter = _testChapter;
                    dummyStage.stage = _testStage;

                    Debug.Log($"<color=orange>[테스트] 가짜 스테이지 클리어 신호 발사! (챕터: {dummyStage.chapter}, 스테이지: {dummyStage.stage})</color>");

                    eventHub.StageCleared(dummyStage);
                    Destroy(dummyStage);
                }
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
