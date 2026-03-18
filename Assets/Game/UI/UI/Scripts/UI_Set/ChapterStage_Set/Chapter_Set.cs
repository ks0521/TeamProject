using Base.Data;
using Base.Managers;
using Base.Save;

using Personal_Jongjun;
using System.Collections;
using System.Collections.Generic;
using UI.Scripts.Stage;
using UnityEngine;


public class Chapter_Set : MonoBehaviour
{
    [SerializeField] int chapterNum;
    [SerializeField] Stage_Set[] stages;
    public void SetChapter(StageManager stageManager)
    {
        for (int i = 0; i < stages.Length; i++)
        {
            int stageNum = i + 1;

            Base.Managers.StageEntry entry = stageManager.GetStageEntry(chapterNum, stageNum);
            stages[i].SetStage(entry);
        }
    }
}


