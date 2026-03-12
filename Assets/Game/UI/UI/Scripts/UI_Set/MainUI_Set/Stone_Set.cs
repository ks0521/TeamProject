using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Stone_Set : MonoBehaviour
{
    [Header("º∫¿ÂºÆ Text")]
    [SerializeField] TextMeshProUGUI growthStoneText;
    public void SetGrowthStone(int stone)
    {
        growthStoneText.text = stone.ToString();
    }
}
