using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Growth.Equipment;
using System.Text;

public class RewardDetailPopup : MonoBehaviour
{
    //이 파일은 매니저급이 아니므로 인스턴스 만들었습니다
    public static RewardDetailPopup Instance { get; private set; }

    [Header("UI 요소 연결")]
    [SerializeField] private GameObject mainContainer; //패널을 포함한 전체 부모
    [SerializeField] private RectTransform infoPanelRect; //실제 보이는 패널
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI infoText;

    private RectTransform rectTransform;

    void Awake()
    {
        Instance = this;
        rectTransform = GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            Debug.LogError($"{gameObject.name}에 RectTransform이 없습니다!");
        }
        if (mainContainer != null) mainContainer.SetActive(false);
    }

    public void ShowDetail(RewardData data, Vector2 clickPos)
    {
        nameText.text = data.itemName;
        iconImage.sprite = data.icon;

        if (data.originalSO is EquipmentSO equipSO) //아이템 분류
        {
            infoText.text = GetStatDescription(equipSO.equipBaseIncrease);
        }
        else infoText.text = data.description;

        AdjustPosition(clickPos);
        mainContainer.SetActive(true);
    }

    public void HidePanel()
    {
        if (mainContainer != null) mainContainer.SetActive(false);
    }

    string GetStatDescription(StatIncrease stat)
    {
        StringBuilder sb = new StringBuilder();

        //0보다 큰 장비 능력치를 1줄씩 추가
        if (stat.atk > 0) sb.AppendLine($"공격력 +{stat.atk}");
        if (stat.atkRate > 0) sb.AppendLine($"공격력 +{stat.atkRate}%");
        if (stat.damageDealtRate > 0) sb.AppendLine($"피해량 +{stat.damageDealtRate}%");
        if (stat.critChance > 0) sb.AppendLine($"치명타 확률 +{stat.critChance}%");
        if (stat.critDamage > 0) sb.AppendLine($"치명타 피해량 +{stat.critDamage}%");

        if (stat.maxHp > 0) sb.AppendLine($"최대 HP +{stat.maxHp}");
        if (stat.maxHpRate > 0) sb.AppendLine($"최대 HP +{stat.maxHpRate}%");
        if (stat.def > 0) sb.AppendLine($"방어력 +{stat.def}");
        if (stat.damageReduction > 0) sb.AppendLine($"피해 감소량 +{stat.damageReduction}");

        if (stat.moveSpeed > 0) sb.AppendLine($"이동속도 +{stat.moveSpeed}");
        if (stat.atkSpeed > 0) sb.AppendLine($"공격 속도 +{stat.atkSpeed}");

        if (stat.itemDropRate > 0) sb.AppendLine($"아이템 드랍률 +{stat.itemDropRate}%");
        if (stat.goldGain > 0) sb.AppendLine($"골드 획득량 +{stat.goldGain}");
        if (stat.expGain > 0) sb.AppendLine($"경험치 획득량 +{stat.expGain}");
        if (stat.statStoneGain > 0) sb.AppendLine($"스탯 성장석 획득량 +{stat.statStoneGain}");

        return sb.ToString().TrimEnd();
    }

    //패널이 화면 밖으로 나가는 일 방지용
    void AdjustPosition(Vector2 screenPos)
    {
        if (infoPanelRect == null) return;

        infoPanelRect.localScale = Vector3.one;
        //마우스 위치가 화면 좌/우에 위치하는지 판단 후 그 반대로 패널 배치
        float pivotX = (screenPos.x > Screen.width * 0.5f) ? 1.05f : -0.05f;
        float pivotY = (screenPos.y > Screen.width * 0.5f) ? 1.05f : -0.05f;
        infoPanelRect.pivot = new Vector2(pivotX, pivotY);
        infoPanelRect.position = screenPos;
        Vector3 pos = infoPanelRect.localPosition;
        pos.z = 0;
        infoPanelRect.localPosition = pos;
    }
}
