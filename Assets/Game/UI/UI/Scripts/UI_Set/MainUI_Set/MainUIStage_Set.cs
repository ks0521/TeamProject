using Battle;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainUIStage_Set : MonoBehaviour
{
    [Header("스테이지 Text")]
    [SerializeField] TextMeshProUGUI stageText;

    public void SetStage(StageSO stageSO)
    {
        stageText.text = $"{stageSO.chapter}-{stageSO.stage}";
    }
}
