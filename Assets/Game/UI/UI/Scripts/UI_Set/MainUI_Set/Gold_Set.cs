using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Gold_Set : MonoBehaviour
{
    [Header("골드 Text")]
    [SerializeField] TextMeshProUGUI goldText;
    public void SetGold(int gold)
    {
        goldText.text = gold.ToString();
    }
}
