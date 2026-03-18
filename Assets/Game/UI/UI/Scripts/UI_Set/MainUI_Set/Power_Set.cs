using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Power_Set : MonoBehaviour
{
    [Header("전투력 Text")]
    [SerializeField] TextMeshProUGUI powerText;
    public void SetPower(int power)
    {
        powerText.text = power.ToString();
    }
}
