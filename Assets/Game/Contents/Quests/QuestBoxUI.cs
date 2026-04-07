using QuestSystem;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestBoxUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private Image background; // 하이라이트용 배경
    [SerializeField] private GameObject redDot; // 박스 내부 레드닷

    private Color highlightColor;
    private Color normalColor;

    private ActiveQuest _quest;
    private QuestUIManager _uiManager;
    public ActiveQuest GetQuest() => _quest;
    void Awake()
    {
        ColorUtility.TryParseHtmlString("#74A9CD", out highlightColor);
        ColorUtility.TryParseHtmlString("#668499", out normalColor);
    }
    public void Setup(ActiveQuest quest, QuestUIManager uiManager)
    {
        if (quest == null) return;

        _quest = quest;
        _uiManager = uiManager;

        titleText.text = quest.RuntimeDescription;
        progressText.text = $"{quest.CurrentValue} / {quest.RuntimeTargetValue}";

        GetComponent<Button>().onClick.AddListener(() => _uiManager.SelectQuest(_quest));

        if (redDot != null) RefreshVisuals();
    }
    public void RefreshVisuals()
    {
        if (_quest == null) return;

        //진행도 텍스트 갱신 (예: 10/100)
        if (progressText != null)
            progressText.text = $"{_quest.CurrentValue} / {_quest.RuntimeTargetValue}";
        //퀘스트 진행 상태
        if (statusText != null) statusText.text = $"({_quest.GetStatusText()})";
        //완료 여부에 따른 레드닷 표시
        if (redDot != null)
            redDot.SetActive(_quest.isCompleted);
    }

    public void SetHighlight(bool isSelected)
    {
        if (background != null)
            background.color = isSelected ? highlightColor : normalColor;
    }
}
