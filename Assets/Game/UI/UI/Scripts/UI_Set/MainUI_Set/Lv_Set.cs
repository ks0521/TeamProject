using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Lv_Set : MonoBehaviour
{
    [Header("Lv Text")]
    [SerializeField] TextMeshProUGUI LvText;
    public void SetLv(int Lv)
    {
        LvText.text = Lv.ToString();
    }
}
