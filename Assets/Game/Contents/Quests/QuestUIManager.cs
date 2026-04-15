using Base.Data;
using Base.Managers;
using Base.Save;
using QuestSystem;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//퀘스트 창의 로직을 관리
public class QuestUIManager : MonoBehaviour, IManager
{
    [Header("퀘스트 분류")]
    [SerializeField] private List<Button> questBookmarks;
    [SerializeField] private float tabBaseX = -180f; //평소 위치
    [SerializeField] private float tabHighlightOffset = -20f; //선택 시 더 튀어나올 양
    private int currentTabIndex = 0;

    [Header("퀘스트 & 리워드 프리팹")]
    [SerializeField] private GameObject questBoxPrefab;
    [SerializeField] private Transform questListContainer;
    [SerializeField] private GameObject rewardItemPrefab;
    [SerializeField] private Transform rewardContainer;

    [Header("퀘스트 상세 정보")]
    [SerializeField] private TextMeshProUGUI questDescription;
    [SerializeField] private TextMeshProUGUI questStatus;
    [SerializeField] private TextMeshProUGUI questProgress;

    [Header("완료 버튼")]
    [SerializeField] private Button receiveButton;
    [SerializeField] private Button receiveAllButton;

    [Header("보상 팝업")]
    [SerializeField] private RewardPopupUI rewardPopup;

    [Header("업적 점수")]
    [SerializeField] private TextMeshProUGUI fameText;

    private QuestManager questManager;
    private EventHub eventHub;
    private RuntimeProgressData runtimeProgress;
    private List<QuestBoxUI> instantiatedBoxes = new List<QuestBoxUI>();
    private List<QuestBoxUI> questBoxPool = new List<QuestBoxUI>(); //미사용 퀘스트 박스 프리팹 보관용
    private ActiveQuest currentSelectedQuest;

    public void Init()
    {
        questManager = FindObjectOfType<QuestManager>();
        eventHub = FindObjectOfType<EventHub>();
        OnClickBookmark(0);
        if (eventHub != null) eventHub.OnCurrencyChange += HandleFameChange;
        EventHub.OnQuestProgressUpdated += RealtimeRefreshVisuals;
        UpdateFameDisplay(GetCurrentFame());
    }
    public int GetOrder() => 340;

    //퀘스트 분류 클릭
    public void OnClickBookmark(int index)
    {
        currentTabIndex = index;
        UpdateTabVisuals();
        RefreshQuestBox();
    }
    void UpdateTabVisuals()
    {
        for (int i = 0; i < questBookmarks.Count; i++)
        {
            RectTransform rt = questBookmarks[i].GetComponent<RectTransform>();
            //선택된 탭(i)은 왼쪽으로 -20만큼 더 튀어나오게
            float targetX = (i == currentTabIndex) ? (tabBaseX + tabHighlightOffset) : tabBaseX;
            rt.anchoredPosition = new Vector2(targetX, rt.anchoredPosition.y);

            bool hasRedDot = CheckRedDot((QuestCategory)i);
            Transform dot = questBookmarks[i].transform.Find("Red Dot");
            if (dot != null) dot.gameObject.SetActive(hasRedDot);
        }
    }

    //왼편의 퀘스트 박스 프리팹 목록을 갱신
    public void RefreshQuestBox()
    {
        if (questBoxPrefab == null || questListContainer == null) return;

        //기존에 생성된 박스는 제거
        foreach (var box in instantiatedBoxes)
        {
            if (box != null)
            {
                box.gameObject.SetActive(false);
                questBoxPool.Add(box);
            }
        }
        instantiatedBoxes.Clear();

        if (questManager == null) return;

        //현재 선택된 카테고리의 퀘스트만 가져오기(LINQ 활용)
        QuestCategory currentCategory = (QuestCategory)currentTabIndex;
        var filteredQuests = questManager.GetActiveQuests()
            .Where(q => q.Data.CategoryEnum == currentCategory)
            .OrderBy(q => q.Data.questID)
            .ToList();

        //프리팹 생성 및 데이터 주입
        foreach (var quest in filteredQuests)
        {
            QuestBoxUI boxScript = GetBoxFromPool();
            if (boxScript != null)
            {
                boxScript.Setup(quest, this);
                //boxScript.RefreshVisuals();
                instantiatedBoxes.Add(boxScript);
            }
        }
        UpdateReceiveAllButtonState();

        //리스트가 갱신되면 1번째 퀘스트를 자동으로 선택해줌
        if (instantiatedBoxes.Count > 0)
        {
            SelectQuest(filteredQuests.First());
        }
        else ClearDetails();
    }
    void ClearDetails()
    {
        questDescription.text = "진행 중인 퀘스트가 없습니다.";
        questStatus.text = "";
        questProgress.text = "- / -";
        receiveButton.interactable = false;
        receiveAllButton.interactable = false;
    }
    QuestBoxUI GetBoxFromPool()
    {
        QuestBoxUI box;

        if (questBoxPool.Count > 0)
        {
            //풀에 노는 박스가 있다면 꺼내서 재사용
            box = questBoxPool[0];
            questBoxPool.RemoveAt(0);
            box.gameObject.SetActive(true);
        }
        else //풀이 비어있다면 새로 생성
        {
            GameObject go = Instantiate(questBoxPrefab, questListContainer);
            box = go.GetComponent<QuestBoxUI>();
        }

        return box;
    }

    //클릭된 퀘스트 박스의 하이라이트 효과, 퀘스트 상세 정보 등
    public void SelectQuest(ActiveQuest quest)
    {
        if (quest == null) return;
        currentSelectedQuest = quest;

        //우측의 상세 정보
        questDescription.text = quest.RuntimeDescription;
        questStatus.text = $"({quest.GetStatusText()})";
        questProgress.text = $"{quest.CurrentValue} / {quest.RuntimeTargetValue}";

        receiveButton.interactable = quest.isCompleted;

        //박스 하이라이트
        foreach (var box in instantiatedBoxes)
        {
            box.SetHighlight(box.GetQuest() == quest);
        }

        //보상 아이템 목록 갱신
        RefreshRewardIcons(quest.Data.rewardGroupID);
    }
    //모두 완료 버튼 활성화 여부 체크
    void UpdateReceiveAllButtonState()
    {
        if (receiveAllButton == null) return;
        QuestCategory currentCat = (QuestCategory)currentTabIndex;

        bool canReceive = questManager.GetActiveQuests()
           .Any(q => q.Data.CategoryEnum == currentCat && q.isCompleted);
        receiveAllButton.interactable = canReceive;
    }

    void RefreshRewardIcons(int groupID)
    {
        foreach (Transform child in rewardContainer)
        {
            Destroy(child.gameObject);
        }

        if (questManager == null) return;

        List<RewardData> rewards = questManager.GetRewardsByGroupID(groupID);

        foreach (var data in rewards)
        {
            GameObject go = Instantiate(rewardItemPrefab, rewardContainer);

            //좌표 및 스케일 초기화(위치 이탈 방지)
            RectTransform rt = go.GetComponent<RectTransform>();
            rt.localPosition = Vector3.zero;
            rt.localScale = Vector3.one;

            //데이터 주입
            RewardBoxUI boxUI = go.GetComponent<RewardBoxUI>();
            if (boxUI != null) boxUI.Setup(data);
        }
    }

    //완료 가능한 퀘스트 표시
    bool CheckRedDot(QuestCategory qc)
    {
        if (questManager == null) return false;

        var activeList = questManager.GetActiveQuests();
        if (activeList == null) return false;
        return activeList.Any(q => q != null &&
                               q.Data != null &&
                               q.Data.CategoryEnum == qc &&
                               q.isCompleted);
    }

    //퀘스트 완료 버튼
    public void OnClickQuestClear()
    {
        if (currentSelectedQuest != null && currentSelectedQuest.isCompleted)
        {
            questManager.TryCompleteQuest(currentSelectedQuest);

            //데이터 변경 후 UI 갱신
            RefreshQuestBox();
            UpdateTabVisuals();
        }
    }

    //퀘스트 모두 완료 버튼(반복성 퀘스트 한번에 클리어 기능 포함)
    public void OnClickQuestAllClear()
    {
        if (questManager == null) return;

        //재화 합산용 딕셔너리
        Dictionary<CurrencyType, RewardData> totalCurrencies = new Dictionary<CurrencyType, RewardData>();
        List<RewardData> uniqueRewards = new List<RewardData>();

        //완료 가능한 퀘스트 카테고리 추출
        var targetQuests = questManager.GetActiveQuests()
        .Where(q => q.isCompleted)
        .ToList();

        foreach (var q in targetQuests)
        {
            int completableCount = q.Data.isInfinite ? (q.CurrentValue / q.RuntimeTargetValue) : 1;
            if (completableCount < 1) completableCount = 1;

            for (int i = 0; i < completableCount; i++)
            {
                // i회차 단계의 보상을 계산하기 위한 가상 회차
                int stepForThisLoop = q.currentStep + i;
                var rewards = questManager.GetRewardsByGroupID(q.Data.rewardGroupID);

                foreach (var r in rewards)
                {
                    bool isEquipment = r.originalSO is Growth.Equipment.EquipmentSO;
                    if (isEquipment) uniqueRewards.Add(r);
                    else
                    {
                        // 재화는 회차(stepForThisLoop)만큼 곱함
                        int addedAmount = r.amount * stepForThisLoop;

                        if (totalCurrencies.ContainsKey(r.currencyType))
                        {
                            totalCurrencies[r.currencyType].amount += addedAmount;
                        }
                        else
                        {
                            totalCurrencies.Add(r.currencyType, new RewardData
                            {
                                itemName = r.itemName,
                                amount = addedAmount,
                                icon = r.icon,
                                description = r.description,
                                originalSO = r.originalSO,
                                currencyType = r.currencyType
                            });
                        }
                    }
                }
                // 실제 데이터 갱신 (TryCompleteQuest 내부에서 다음 단계로 넘김)
                questManager.TryCompleteQuest(q, true);
            }
        }

        List<RewardData> finalList = new List<RewardData>();
        finalList.AddRange(totalCurrencies.Values);
        finalList.AddRange(uniqueRewards);

        if (finalList.Count > 0 && rewardPopup != null)
        {
            rewardPopup.ShowRewards(finalList);
        }
        RefreshQuestBox();
        UpdateTabVisuals();
    }

    //실시간 데이터 동기화
    void RealtimeRefreshVisuals()
    {
        //왼쪽 박스들의 수치 갱신
        foreach (var box in instantiatedBoxes)
        {
            if (box != null) box.RefreshVisuals();
        }

        //오른쪽 상세창 수치 및 버튼 상태 갱신
        if (currentSelectedQuest != null)
        {
            var updatedQuest = questManager.GetActiveQuests().FirstOrDefault(q => q.Data.questID == currentSelectedQuest.Data.questID);

            if (updatedQuest != null)
            {
                currentSelectedQuest = updatedQuest; //최신 객체로 갱신
                questProgress.text = $"{currentSelectedQuest.CurrentValue} / {currentSelectedQuest.RuntimeTargetValue}";
                questStatus.text = currentSelectedQuest.isCompleted ? "<color=green>완료 가능</color>" : "<color=white>진행 중</color>";
            }
        }

        //레드닷, 모두 완료 버튼 상태 갱신
        UpdateTabVisuals();
        UpdateReceiveAllButtonState();
    }

    void HandleFameChange(CurrencyType type, int currentAmount)
    {
        if (type == CurrencyType.FAME)
        {
            Debug.Log($"<color=cyan>[FAME 수신]</color> 타입: {type}, 새로운 수치: {currentAmount}");
            UpdateFameDisplay(currentAmount);
        }
    }
    void UpdateFameDisplay(int amount)
    {
        if (fameText != null) fameText.text = amount.ToString("N0");
    }
    int GetCurrentFame()
    {
        if (runtimeProgress != null)
        {
            return runtimeProgress.currency.fame;
        }
        return 0;
    }

    void OnDestroy()
    {
        EventHub.OnQuestProgressUpdated -= RealtimeRefreshVisuals;
        eventHub.OnCurrencyChange -= HandleFameChange;
    }
}
