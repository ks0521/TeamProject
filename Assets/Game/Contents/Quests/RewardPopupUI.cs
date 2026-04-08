using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//보상 수령 창과 프리팹 담당
public class RewardPopupUI : MonoBehaviour
{
    [SerializeField] private GameObject rewardBoxPrefab;
    [SerializeField] private Transform contentArea;
    [SerializeField] private GameObject popupPanel;

    // 보상 창 열기 (QuestManager 등에서 호출)
    public void ShowRewards(List<RewardData> rewards)
    {
        //기존의 박스 제거(나중에 오브젝트 풀링으로 변경?)
        foreach (Transform child in contentArea)
        {
            Destroy(child.gameObject);
        }

        //새로운 보상 생성
        foreach (var data in rewards)
        {
            GameObject go = Instantiate(rewardBoxPrefab, contentArea);
            RectTransform rt = go.GetComponent<RectTransform>();
            rt.localPosition = Vector3.zero;
            rt.localRotation = Quaternion.identity;
            rt.localScale = Vector3.one;
            go.GetComponent<RewardBoxUI>().Setup(data);
        }

        //팝업 활성화
        popupPanel.SetActive(true);
    }

    public void ClosePopup()
    {
        popupPanel.SetActive(false);
    }
}
