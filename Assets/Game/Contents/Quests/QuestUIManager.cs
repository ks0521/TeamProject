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

    public void Init()
    {

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
            questBookmarks[i].transform.Find("RedDot").gameObject.SetActive(hasRedDot);
        }
    }
    
    //왼편의 퀘스트 박스 프리팹 목록을 갱신
    public void RefreshQuestBox()
    {

    }

    //클릭된 퀘스트 박스의 하이라이트 효과, 퀘스트 상세 정보 등
    public void SelectQuest(ActiveQuest quest)
    {
        
    }
    //완료 가능한 퀘스트 표시
    bool CheckRedDot(QuestCategory qc)
    {
        return false;
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
