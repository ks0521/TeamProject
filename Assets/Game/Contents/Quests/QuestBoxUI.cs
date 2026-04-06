using QuestSystem;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestBoxUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private Image background; // 하이라이트용 배경
    [SerializeField] private GameObject redDot; // 박스 내부 레드닷

    private ActiveQuest _quest;
    private QuestUIManager _uiManager;
    public ActiveQuest GetQuest() => _quest;
    public void Setup(ActiveQuest quest, QuestUIManager uiManager)
    {
        if (quest == null) return;

        _quest = quest;
        _uiManager = uiManager;

        // 데이터 반영
        titleText.text = quest.Data.description;
        progressText.text = $"{quest.CurrentValue}/{quest.Data.targetValue}";

        // 버튼 클릭 이벤트 연결
        GetComponent<Button>().onClick.AddListener(() => _uiManager.SelectQuest(_quest));

        if (redDot != null) RefreshVisuals();
    }
    public void RefreshVisuals()
    {
        if (_quest == null) return;

        // 1. 진행도 텍스트 갱신 (예: 10/100)
        if (progressText != null)
            progressText.text = $"{_quest.CurrentValue} / {_quest.Data.targetValue}";

        // 2. 완료 여부에 따른 레드닷 표시
        if (redDot != null)
            redDot.SetActive(_quest.isCompleted);
    }

    public void SetHighlight(bool isSelected)
    {
        if (background != null)
            background.color = isSelected ? new Color(0.7f, 0.9f, 0.7f) : Color.white;
    }
}
