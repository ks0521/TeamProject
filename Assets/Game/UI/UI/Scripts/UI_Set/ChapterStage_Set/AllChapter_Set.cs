using Base.Managers;
using Battle;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UI.Scripts.Stage;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class AllChapter_Set : MonoBehaviour
{
    [Header("스테이지 매니저")]
    [SerializeField] StageManager stageManager;

    [Header("챕터")]
    [SerializeField] Chapter_Set[] chapter;

    [Header("챕터 이름")]
    [SerializeField] TextMeshProUGUI chapterName;

    [Header("챕터 이동 버튼")]
    [SerializeField] Button before;
    [SerializeField] Button after;
    int currentChapter;

    [Header("스테이지 이동 버튼")]
    [SerializeField] Button enter;
    int enterChapter;
    int enterStage;
    private Stage_Set lastSpotligh;

    [SerializeField] StageMonster_Set stageMon;
    [SerializeField] Reward_Set reward;

    private void OnEnable()
    {
        AllChapter();
        ShowChapter();
        enter.interactable = false;
    }

    public void Init()
    {
        stageManager = GameManager.Instance.GetGameSystem<StageManager>();
        currentChapter = 0;
        BindButton();
        AllChapter();
       
        gameObject.SetActive(false);
        enter.interactable = false;
    }
    public void EnterStage(int chatperNum, int stageNum, Stage_Set clickStage)
    {
        if (lastSpotligh != null)
        {
            lastSpotligh.Spotlight(false);
        }

        clickStage.Spotlight(true);
        lastSpotligh = clickStage;

        enterChapter = chatperNum;
        enterStage = stageNum;

        StageEntry stageEntry = stageManager.GetStageEntry(enterChapter, enterStage);

        //여기 나중에 몬스터 이미지 , 보상 목록 이미지 변경 추가 할 예정
        if (reward != null)
        {
            reward.SetReward(stageEntry);
        }
        if (stageMon != null)
        {
            stageMon.SetMonster(stageEntry);
        }
        if (enter != null)
        {
            enter.interactable = true;
        }
    }
    void AllChapter()
    {
        for (int i = 0; i < chapter.Length; i++)
        {
            chapter[i].SetChapter(stageManager);
        }
    }
    void SetChapterName() //나중에 작업 예정
    {

    }
    void ShowChapter()
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


    void BindButton()
    {
        after.onClick.AddListener(() => OnClickAfter());
        before.onClick.AddListener(() => OnClickBefore());
        enter.onClick.AddListener(() => OnClickChangeStage());
    }
    void OnClickBefore()
    {
        if (currentChapter == 0)
        {
            return;
        }

        currentChapter--;
        ShowChapter();

    }
    void OnClickAfter()
    {
        if (currentChapter == chapter.Length - 1)
        {
            return;
        }

        currentChapter++;
        ShowChapter();

    }
    void OnClickChangeStage()
    {
        stageManager.ChangeStage(enterChapter, enterStage);

        gameObject.SetActive(false);
    }

}
