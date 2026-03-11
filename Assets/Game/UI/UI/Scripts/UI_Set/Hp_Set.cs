using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
public class Hp_Set : MonoBehaviour
{
    [Header("HP")]
    [SerializeField] Image hp;
    [SerializeField] TextMeshProUGUI hpText;
    public void SetHp(float currenthp, float maxhp)
    {
        if (maxhp <= 0f)
        {
            Debug.Log("maxHp가 0이라 갱신 불가");
            return;
        }
        if (currenthp <= 0f)
        {
            hpText.text = "0";
        }
        else
        {
            hpText.text = currenthp.ToString("F0");
        }
        hp.fillAmount = (currenthp / maxhp); //Hp 수치 표시하기

    }
}
