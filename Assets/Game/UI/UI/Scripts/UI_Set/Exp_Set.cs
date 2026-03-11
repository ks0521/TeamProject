using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class Exp_Set : MonoBehaviour
{
    [Header("EXP")]
    [SerializeField] Slider expBar;
    [SerializeField] TextMeshProUGUI expPercent;
    // Start is called before the first frame update
    public void SetExp(float exp, float maxExp)
    {
        if (maxExp <= 0)
        {
            Debug.Log("maxExp 가 0 이라 갱신 불가");
        }
        else
        {
            expBar.value = exp / maxExp;
        }
        expPercent.text = (exp / maxExp).ToString("0.00%"); // % 로 표시하기
    }
   
}
