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
