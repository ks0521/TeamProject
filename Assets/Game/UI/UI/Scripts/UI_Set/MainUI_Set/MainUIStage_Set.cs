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
        if (stageSO.type == StageType.Challenge || stageSO.type == StageType.Boss)
        {
            stageText.text = $"{stageSO.chapter}-{stageSO.stage}C";
            return;
        }
        //3.23(규성) 챌린지맵 테스트때문에 잠깐 수정했습니다. 
        stageText.text = $"{stageSO.chapter}-{stageSO.stage}";
    }
}
