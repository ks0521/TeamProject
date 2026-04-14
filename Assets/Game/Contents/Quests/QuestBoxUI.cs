using QuestSystem;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//퀘스트 박스 프리팹이 어떻게 노출될 것인지를 결정
public class QuestBoxUI : MonoBehaviour
{
    [Header("프리팹 기본 구성")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private Image background; //하이라이트용 배경
    [SerializeField] private GameObject redDot; //박스 내부 레드닷

    [Header("잠금 표시")]
    [SerializeField] private GameObject hider;
    [SerializeField] private TextMeshProUGUI lockText;

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

        GetComponent<Button>().onClick.RemoveAllListeners();
        GetComponent<Button>().onClick.AddListener(() => _uiManager.SelectQuest(_quest));

        if (redDot != null) RefreshVisuals();
    }
    public void RefreshVisuals()
    {
        if (_quest == null) return;
        
        if (_quest.isLocked) //잠긴 퀘스트
        {
            hider.SetActive(true);
            GetComponent<Button>().interactable = false; //클릭 차단
            if (lockText != null) lockText.text = _quest.lockMessage;

            if (titleText != null) titleText.text = _quest.RuntimeDescription;
            if (progressText != null) progressText.text = "";
            if (statusText != null) statusText.text = $"({_quest.GetStatusText()})";
            if (redDot != null) redDot.SetActive(false);

            return; //잠겼다면 여기서 로직 종료
        }
        else //진행 중인 퀘스트(isLocked == false)
        {
            hider.SetActive(false);
            GetComponent<Button>().interactable = true;

            if (titleText != null) titleText.text = _quest.RuntimeDescription;
            if (progressText != null)
            {
                //값이 음수가 나오는 상황 방지
                int displayValue = _quest.CurrentValue < 0 ? 0 : _quest.CurrentValue;
                progressText.text = $"{_quest.CurrentValue} / {_quest.RuntimeTargetValue}";
            }
            if (statusText != null) statusText.text = $"({_quest.GetStatusText()})";
            if (redDot != null) redDot.SetActive(_quest.isCompleted);
        }
    }

    public void SetHighlight(bool isSelected)
    {
        if (background != null)
            background.color = isSelected ? highlightColor : normalColor;
    }
}
