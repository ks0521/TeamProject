using Base.Data;
using Base.Managers;
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

    private QuestManager questManager;
    private List<QuestBoxUI> instantiatedBoxes = new List<QuestBoxUI>();
    private ActiveQuest currentSelectedQuest;

    public void Init()
    {
        questManager = FindObjectOfType<QuestManager>();
        EventHub.OnQuestProgressUpdated += RealtimeRefreshVisuals;
        OnClickBookmark(0);
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

        // 1. 기존에 생성된 박스 제거
        foreach (var box in instantiatedBoxes)
        {
            //지금은 파괴지만 나중에 오브젝트 풀링으로 바꿀 가능성 있음
            if (box != null) Destroy(box.gameObject);
        }
        instantiatedBoxes.Clear();

        if (questManager == null) return;

        // 2. 현재 선택된 카테고리의 퀘스트만 가져오기(LINQ 활용)
        QuestCategory currentCategory = (QuestCategory)currentTabIndex;
        var filteredQuests = questManager.GetActiveQuests()
            .Where(q => q.Data.CategoryEnum == currentCategory)
            .OrderBy(q => q.Data.questID)
            .ToList();

        // 3. 프리팹 생성 및 데이터 주입
        foreach (var quest in filteredQuests)
        {
            GameObject go = Instantiate(questBoxPrefab, questListContainer);
            QuestBoxUI boxScript = go.GetComponent<QuestBoxUI>();
            if (boxScript != null)
            {
                boxScript.Setup(quest, this);
                instantiatedBoxes.Add(boxScript);
            }
        }
        UpdateReceiveAllButtonState();
        // 4. 리스트가 갱신되면 첫 번째 퀘스트를 자동으로 선택해줌 (상세창 공백 방지)
        if (instantiatedBoxes.Count > 0)
        {
            SelectQuest(filteredQuests.First());
        }
        else
        {
            ClearDetails();
        }
    }
    void ClearDetails()
    {
        questDescription.text = "진행 중인 퀘스트가 없습니다.";
        questStatus.text = "";
        questProgress.text = "- / -";
        receiveButton.interactable = false;
        receiveAllButton.interactable = false;
    }

    //클릭된 퀘스트 박스의 하이라이트 효과, 퀘스트 상세 정보 등
    public void SelectQuest(ActiveQuest quest)
    {
        if (quest == null) return;
        currentSelectedQuest = quest;

        //우측의 상세 정보
        questDescription.text = quest.Data.description;
        questStatus.text = quest.isCompleted ? "<color=green>완료 가능</color>" : "<color=white>진행 중</color>";
        questProgress.text = $"{quest.CurrentValue} / {quest.Data.targetValue}";

        receiveButton.interactable = quest.isCompleted;

        //박스 하이라이트
        foreach(var box in instantiatedBoxes)
        {
            box.SetHighlight(box.GetQuest() == quest);
        }

        // 보상 아이템 목록 갱신 (추후 보상 시스템 연동)
        RefreshRewardIcons(quest.Data.rewardGroupID);
    }
    //모두 완료 버튼 활성화 여부 체크
    void UpdateReceiveAllButtonState()
    {
        if (receiveAllButton == null) return;
        QuestCategory currentCat = (QuestCategory)currentTabIndex;

        //리스트 내의 퀘스트 중 하나라도 isCompleted가 true라면 버튼 활성화
        //bool canReceiveAny = currentQuests.Any(q => q.isCompleted);
        bool canReceive = questManager.GetActiveQuests()
           .Any(q => q.Data.CategoryEnum == currentCat && q.isCompleted);
        receiveAllButton.interactable = canReceive;
    }

    void RefreshRewardIcons(int groupID)
    {
        // 보상 프리팹 생성 로직 (현재는 생략)
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

    //퀘스트 모두 완료 버튼
    public void OnClickQuestAllClear()
    {
        if (questManager == null) return;

        QuestCategory currentCat = (QuestCategory)currentTabIndex;

        // 현재 카테고리 중 완료 가능한 것들만 추출
        var targetQuests = questManager.GetActiveQuests()
            .Where(q => q.Data.CategoryEnum == currentCat && q.isCompleted)
            .ToList();

        foreach (var q in targetQuests)
        {
            questManager.TryCompleteQuest(q);
        }

        RefreshQuestBox();
        UpdateTabVisuals();
    }

    //실시간 데이터 동기화
    void RealtimeRefreshVisuals()
    {
        // 1. 왼쪽 리스트 박스들 수치 갱신
        foreach (var box in instantiatedBoxes)
        {
            if (box != null) box.RefreshVisuals();
        }

        // 2. 오른쪽 상세창 수치 및 버튼 상태 갱신
        if (currentSelectedQuest != null)
        {
            questProgress.text = $"{currentSelectedQuest.CurrentValue} / {currentSelectedQuest.Data.targetValue}";
            receiveButton.interactable = currentSelectedQuest.isCompleted;
            questStatus.text = currentSelectedQuest.isCompleted ? "<color=green>완료 가능</color>" : "<color=white>진행 중</color>";
        }

        // 3. 책갈피 알림(레드닷) 및 모두 완료 버튼 상태 갱신
        UpdateTabVisuals();
        UpdateReceiveAllButtonState();
    }

    void OnDestroy()
    {
        EventHub.OnQuestProgressUpdated -= RealtimeRefreshVisuals;
    }
}
