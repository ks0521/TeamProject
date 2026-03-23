using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Stone_Set : MonoBehaviour
{
    //재화 표기하는 부분은 많이 있음(메인 / 상점 / 팝업 / ..... 재화 사용하는 대부분의 공간)
    //
    [Header("성장석 Text")]
    [SerializeField] TextMeshProUGUI growthStoneText;

    [SerializeField] private bool Auto;
    
    public void SetGrowthStone(int stone)
    {
        growthStoneText.text = stone.ToString();
    }
}
