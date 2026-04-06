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
    private int currentTabIndex = 0;

    [Header("퀘스트 & 리워드 프리팹")]
    [SerializeField] private GameObject questBoxPrefab;
    [SerializeField] private Transform questListContainer;
    [SerializeField] private GameObject rewardItemPrefab;
    [SerializeField] private Transform rewardContainer;

    [Header("퀘스트 상세 정보")]
    [SerializeField] private TextMeshProUGUI questDescription;
    [SerializeField] private TextMeshProUGUI questProgress;

    [Header("완료 버튼")]
    [SerializeField] private Button receiveButton;
    [SerializeField] private Button receiveAllButton;

    private QuestManager questManager;
    private List<QuestBoxUI> instantiatedBoxes = new List<QuestBoxUI>();
    private ActiveQuest currentSelectedQuest;

    public void Init()
    {
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
            //선택된 탭은 왼쪽으로 -20만큼 더 튀어나오게
            rt.anchoredPosition = new Vector2(i == currentTabIndex ? -20f : 0f, rt.anchoredPosition.y);

            bool hasRedDot = CheckRedDot((QuestCategory)i);
            Transform dot = questBookmarks[i].transform.Find("Red Dot");
            if (dot != null) dot.gameObject.SetActive(hasRedDot);
        }
    }
    
    //왼편의 퀘스트 박스 프리팹 목록을 갱신
    public void RefreshQuestBox()
    {
        // 1. 기존에 생성된 박스 제거
        foreach (var box in instantiatedBoxes)
        {
            if (box != null) Destroy(box.gameObject);
        }
        instantiatedBoxes.Clear();

        // 2. 현재 선택된 카테고리의 퀘스트만 가져오기 (LINQ 활용)
        QuestCategory currentCategory = (QuestCategory)currentTabIndex;
        var filteredQuests = questManager.GetActiveQuests() // QuestManager에 GetActiveQuests() 함수가 있다고 가정
            .Where(q => q.Data.CategoryEnum == currentCategory)
            .OrderBy(q => q.Data.questID);

        // 3. 프리팹 생성 및 데이터 주입
        foreach (var quest in filteredQuests)
        {
            GameObject go = Instantiate(questBoxPrefab, questListContainer);
            QuestBoxUI boxScript = go.GetComponent<QuestBoxUI>();
            boxScript.Setup(quest, this);
            instantiatedBoxes.Add(boxScript);
        }

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
        questProgress.text = "- / -";
        receiveButton.interactable = false;
    }

    //클릭된 퀘스트 박스의 하이라이트 효과, 퀘스트 상세 정보 등
    public void SelectQuest(ActiveQuest quest)
    {
        currentSelectedQuest = quest;

        //우측의 상세 정보
        questDescription.text = quest.Data.description;
        questProgress.text = $"{quest.CurrentValue} / {quest.Data.targetValue}";

        receiveButton.interactable = quest.isCompleted;

        //박스 하이라이트
        foreach(var box in instantiatedBoxes)
        {
            // 내가 들고 있는 퀘스트 정보가 선택된 퀘스트와 같으면 하이라이트
            // (QuestBoxUI 내부에 public ActiveQuest GetQuest() { return _quest; } 추가 필요)
            // 간단하게는 박스 스크립트에서 직접 처리하도록 설계
        }

        // 보상 아이템 목록 갱신 (추후 보상 시스템 연동)
        RefreshRewardIcons(quest.Data.rewardGroupID);
    }
    void RefreshRewardIcons(int groupID)
    {
        // 보상 프리팹 생성 로직 (현재는 생략)
    }
    //완료 가능한 퀘스트 표시
    bool CheckRedDot(QuestCategory qc)
    {
        return questManager.GetActiveQuests()
            .Any(q => q.Data.CategoryEnum == qc && q.isCompleted);
    }
    //레드닷 (비)활성화 : 이 코드는 필요 없을 수도 있음
    void RefreshRedDot()
    {

    }

    //퀘스트 완료 버튼
    public void OnClickQuestClear()
    {
        //ㅁㄴㅇㄹ
        RefreshQuestBox();
        RefreshRedDot();
    }

    //퀘스트 모두 완료 버튼
    public void OnClickQuestAllClear()
    {
        QuestCategory currentCat = (QuestCategory)currentTabIndex;


        RefreshQuestBox();
        RefreshRedDot();
    }
}
