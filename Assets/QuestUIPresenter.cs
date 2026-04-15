using Base.Data;
using Base.Managers;
using QuestSystem;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class QuestUIPresenter : MonoBehaviour, IManager
{
    [SerializeField] private QuestManager questManager;
    [SerializeField] private EventHub eventHub;
    private QuestUIManager view;
    private int currentTabIndex = 0;
    private int selectedQuestId = -1;
    public int GetOrder() => 335;
    public void Init()
    {
        if (questManager == null)
            questManager = GameManager.Instance.GetGameSystem<QuestManager>();
        if (eventHub == null)
            eventHub = GameManager.Instance.GetGameSystem<EventHub>();
        // QuestManager에서 데이터 변경 이벤트를 발행하도록 아래에서 추가함
        eventHub.OnQuestDataChanged += HandleQuestDataChanged;
        EventHub.OnQuestProgressUpdated += HandleQuestProgressUpdated;
    }
    private void OnDestroy()
    {
        if (eventHub != null)
            eventHub.OnQuestDataChanged -= HandleQuestDataChanged;
        EventHub.OnQuestProgressUpdated -= HandleQuestProgressUpdated;
    }
    public void AttachView(QuestUIManager targetView)
    {
        view = targetView;
        RefreshView();
    }
    public void DetachView(QuestUIManager targetView)
    {
        if (view == targetView) view = null;
    }
    public void OnClickTab(int tabIndex)
    {
        currentTabIndex = tabIndex;
        selectedQuestId = -1;
        RefreshView();
    }
    public void OnClickSelectQuest(int questId)
    {
        selectedQuestId = questId;
        RefreshView();
    }
    public void OnClickReceiveOne()
    {
        if (questManager == null) return;
        ActiveQuest selected = GetSelectedQuest();
        if (selected == null || !selected.isCompleted) return;
        questManager.TryCompleteQuest(selected);
        // TryCompleteQuest 내부에서 이벤트가 오므로 RefreshView 직접호출은 생략 가능
    }
    public void OnClickReceiveAll()
    {
        if (questManager == null) return;
        QuestCategory currentCat = (QuestCategory)currentTabIndex;
        List<ActiveQuest> targets = questManager.GetActiveQuests()
            .Where(q => q.Data.CategoryEnum == currentCat && q.isCompleted)
            .ToList();
        if (targets.Count == 0) return;
        foreach (var q in targets)
        {
            // 필요하면 suppressPopup=true 조합으로 일괄 처리 후 뷰에서 1회 팝업 띄우도록 확장 가능
            questManager.TryCompleteQuest(q);
        }
    }
    private void HandleQuestDataChanged()
    {
        RefreshView();
    }
    private void HandleQuestProgressUpdated()
    {
        RefreshView();
    }
    public void RefreshView()
    {
        if (view == null || questManager == null) return;
        QuestCategory category = (QuestCategory)currentTabIndex;
        List<ActiveQuest> list = questManager.GetActiveQuests()
            .Where(q => q.Data.CategoryEnum == category)
            .OrderBy(q => q.Data.questID)
            .ToList();
        // 선택 유지 로직
        if (selectedQuestId < 0 || !list.Any(q => q.Data.questID == selectedQuestId))
            selectedQuestId = list.Count > 0 ? list[0].Data.questID : -1;
        ActiveQuest selected = list.FirstOrDefault(q => q.Data.questID == selectedQuestId);
        bool canReceiveOne = selected != null && selected.isCompleted;
        bool canReceiveAll = list.Any(q => q.isCompleted);
        view.RenderTab(currentTabIndex, questManager);
        view.RenderQuestList(list, selectedQuestId);
        view.RenderQuestDetail(selected, questManager);
        view.RenderButtons(canReceiveOne, canReceiveAll);
    }
    private ActiveQuest GetSelectedQuest()
    {
        if (questManager == null || selectedQuestId < 0) return null;
        return questManager.GetActiveQuests().FirstOrDefault(q => q.Data.questID == selectedQuestId);
    }
}