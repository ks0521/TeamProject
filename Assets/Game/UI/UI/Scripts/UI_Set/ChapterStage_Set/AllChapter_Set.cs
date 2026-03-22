using Base.Managers;
using Battle;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class AllChapter_Set : MonoBehaviour
{
    [Header("스테이지 매니저")]
    [SerializeField] StageManager stageManager;

    [Header("챕터")]
    [SerializeField] Chapter_Set [] chapter;

    [Header("챕터 이름")]
    [SerializeField] TextMeshProUGUI chapterName;

    [Header("챕터 이동 버튼")]
    [SerializeField] Button before;
    [SerializeField] Button after;
    private int currentChapter;

    [Header("스테이지 이동 버튼")]
    [SerializeField] Button enter;
    int enterChapter;
    int enterStage;

    private void Awake()
    {
        if (stageManager == null)
        {
            stageManager = FindFirstObjectByType<StageManager>();
        }
    }
    private void Start()
    {
        currentChapter = 0;
        if (enter != null)
        {
            enter.gameObject.SetActive(false);
        }
    }
    private void OnEnable()
    {
        AllChapter();
        ShowChapter();
    }
    
    
    public void EnterStage(int chatperNum , int stageNum)
    {
        enterChapter = chatperNum;
        enterStage = stageNum;
        
        //여기 나중에 몬스터 이미지 , 보상 목록 이미지 변경 추가 할 예정
        if (enter != null)
        {
            enter.gameObject.SetActive(true);
        }
    }
    public void  OnClickChangeStage()
    {
        stageManager.ChangeStage(enterChapter , enterStage);
    }
    void AllChapter()
    {
        for (int i = 0; i < chapter.Length; i++)
        {
            chapter[i].SetChapter(stageManager);
        } 
    }
    public void SetChapterName() //나중에 작업 예정
    {
       
    }
    public void OnClickBefore()
    {
        if (currentChapter == 0)
        {
            return;
        }

        currentChapter--;
        ShowChapter();

    }
    public void OnClickAfter()
    {
        if (currentChapter == chapter.Length - 1)
        {
            return;
        }

        currentChapter++;
        ShowChapter();

    }
    private void ShowChapter()
    {
        chapterName.text = currentChapter.ToString();

        if (chapter == null || chapter.Length == 0)//배열 확인용
        {
            return;
        }
        for (int i = 0; i < chapter.Length; i++)
        {
            chapter[i].gameObject.SetActive(i == currentChapter);//currentChapter 아닌것들은 false 하는 용도
        }

        if (after != null)
        {
            after.interactable = currentChapter < chapter.Length - 1;
        }
        if (before != null)
        {
            before.interactable = currentChapter > 0;
        }
    }

}
