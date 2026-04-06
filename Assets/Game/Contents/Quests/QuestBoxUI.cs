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
    public void Setup(ActiveQuest quest, QuestUIManager uiManager)
    {
        _quest = quest;
        _uiManager = uiManager;

        // 데이터 반영
        titleText.text = quest.Data.description;
        progressText.text = $"{quest.CurrentValue}/{quest.Data.targetValue}";

        // 버튼 클릭 이벤트 연결
        GetComponent<Button>().onClick.AddListener(() => _uiManager.SelectQuest(_quest));

        RefreshRedDot();
    }

    public void RefreshRedDot()
    {
        // 완료 가능 상태면 레드닷 표시
        if (redDot != null) redDot.SetActive(_quest.isCompleted);
    }

    public void SetHighlight(bool isSelected)
    {
        // 선택되면 밝게, 아니면 어둡게 (색상은 기획에 맞게 조절)
        background.color = isSelected ? new Color(0.8f, 0.8f, 0.8f) : Color.white;
    }
}
