using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Power_Set : MonoBehaviour
{
    [Header("ÀüÅõ·Â Text")]
    [SerializeField] TextMeshProUGUI powerText;
    public void SetPower(int power)
    {
        powerText.text = power.ToString();
    }
}
