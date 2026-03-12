using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Stage_Set : MonoBehaviour
{
    [Header("스테이지 Text")]
    [SerializeField] TextMeshProUGUI stageText;

    public void SetStage(int chapter , int stage)
    {
        stageText.text = $"{chapter}-{stage}";
    }
}
