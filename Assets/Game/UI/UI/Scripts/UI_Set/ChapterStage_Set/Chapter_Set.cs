using Base.Data;
using Base.Managers;
using Base.Save;

using Personal_Jongjun;
using System;
using System.Collections;
using System.Collections.Generic;
using UI.Scripts.Stage;
using UnityEngine;


public class Chapter_Set : MonoBehaviour
{
    [SerializeField] int chapterNum;

    [SerializeField] Transform stageContent;

    [SerializeField] Stage_Set [] stages;

    
    public void SetChapter(StageManager stageManager)
    {
       
        int count = stageContent.childCount;

        stages = new Stage_Set[count];

        for (int i = 0; i <  count; i++)
        {
            stages[i] = stageContent.GetChild(i).GetComponent<Stage_Set>();
        }

        AllChapter_Set allChap = gameObject.GetComponentInParent<AllChapter_Set>();
        Debug.Log(allChap);

        for (int i = 0; i < stages.Length; i++)
        {
            int stageNum = i + 1;

            var entry = stageManager.GetStageEntry(chapterNum, stageNum);

            Stage_Set currentStage = stages[i];
            stages[i].Bind(() => allChap.EnterStage(chapterNum, stageNum , currentStage));
            stages[i].SetStage(entry);
        }
    }

  
}




