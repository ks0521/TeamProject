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
        amountText.text = GetAbbreviatedValue(data.amount);
        //장비 아이템은 수량 표기 없음, 그 외에는 2 이상의 수량만 노출
        bool isEquipment = data.originalSO is Growth.Equipment.EquipmentSO;
        bool shouldShowAmount = !isEquipment && data.amount > 1;
        if (amountText != null)
        {
            amountText.text = GetAbbreviatedValue(data.amount);
            amountText.gameObject.SetActive(shouldShowAmount);
        }

        Button btn = GetComponent<Button>();
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() => {
            if (RewardDetailPopup.Instance == null)
            {
                Debug.LogError("RewardDetailPopup 인스턴스가 씬에 없습니다! " +
                    "하이어라키에 오브젝트가 있는지, Awake에서 Instance = this를 했는지 확인하세요.");
                return;
            }
            if (rData == null)
            {
                Debug.LogError("전달된 RewardData가 null입니다.");
                return;
            }

            // 정상 실행 (함수 이름이 ShowDetail인지 Show인지 확인 필수!)
            RewardDetailPopup.Instance.ShowDetail(rData, Input.mousePosition);
        });
    }
    //숫자를 k, m, b 단위로 변환하는 함수
    string GetAbbreviatedValue(int amount)
    {
        float value = amount;
        string unit = "";

        if (amount >= 1000000000) //10억 이상(B)
        {
            value /= 1000000000f;
            unit = "b";
        }
        else if (amount >= 1000000) //100만 이상(M)
        {
            value /= 1000000f;
            unit = "m";
        }
        else if (amount >= 1000) //1,000 이상(K)
        {
            value /= 1000f;
            unit = "k";
        }
        //1,000 미만은 그대로 반환
        else return amount.ToString();

        //소수점 셋째 자리까지 표시하되, .0인 경우 생략
        return value.ToString("0.###") + unit;
    }

    //프리팹 자체의 버튼 이벤트로 연결 (인스펙터에서)
    public void OnClickShowDetail()
    {
        if (rData == null) return;

        if (RewardDetailPopup.Instance != null)
        {
            RewardDetailPopup.Instance.ShowDetail(rData, Input.mousePosition);
        }
    }
}
