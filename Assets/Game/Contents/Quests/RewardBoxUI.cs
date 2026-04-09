using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RewardBoxUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI amountText;

    private RewardData rData;
    public void Setup(RewardData data)
    {
        rData = data;
        iconImage.sprite = data.icon;
        amountText.text = data.amount.ToString();
    }

    //프리팹 자체의 버튼 이벤트로 연결 (인스펙터에서)
    public void OnClickShowDetail()
    {
        // 팁: 여기서 상세 정보 팝업을 띄우거나 전역 이벤트를 호출합니다.
        Debug.Log($"아이템 상세 정보: {rData.itemName}");
    }
}
