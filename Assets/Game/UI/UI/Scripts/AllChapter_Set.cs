using Base.Managers;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AllChapter_Set : MonoBehaviour
{
    [Header("챕터")]
    [SerializeField] Chapter_1_Set chapter1;
    [SerializeField] Chapter_2_Set chapter2;
    [SerializeField] Chapter_3_Set chapter3;
    [SerializeField] Chapter_4_Set chapter4;

    [Header("스테이지 매니저")]
    [SerializeField] StageManager stageManager;

    private void Awake()
    {
        if (stageManager == null)
        {
            stageManager = FindFirstObjectByType<StageManager>();
        }
    }
    private void OnEnable()
    {
        AllChapter();
    }
    void AllChapter()
    {
        chapter1.SetChapter1(stageManager,1);
        

    }

}
