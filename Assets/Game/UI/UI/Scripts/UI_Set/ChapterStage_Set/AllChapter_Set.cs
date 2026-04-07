using Base.Data;
using Base.Managers;
using Battle;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UI.Scripts.Stage;
using UI.Scripts;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;

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
    int currentChapter = 0;

    [Header("스테이지 이동 버튼")]
    [SerializeField] Button enter;
    int enterChapter;
    int enterStage;
    private Stage_Set lastSpotligh;

    [SerializeField] StageMonster_Set stageMon;
    [SerializeField] Reward_Set reward;

    [SerializeField] private StageEntry stageEntry;
    private EventHub hub;

    private void Awake()
    {
        BindButton();
        Debug.Log("버튼 함수 넣기");
    }
    private void OnEnable()
    {
        stageManager = GameManager.Instance.GetGameSystem<StageManager>();
        hub = GameManager.Instance.GetGameSystem<EventHub>();

        AllChapter();
        ShowChapter();
        
        enter.interactable = false;
        if (hub != null)
        {
            hub.OnClearStage += EventChain;
            hub.OnChangeStage += UpdateStageChange;
            Debug.Log("스테이지창 이벤트 구독!");
        }
    }
    private void OnDisable()
    {
        if (hub == null) return;
        hub.OnClearStage -= EventChain;
        hub.OnChangeStage -= UpdateStageChange;
        Debug.Log("스테이지창 이벤트 구독 해제");
    }
    


    /// <summary> 특정 스테이지의 버튼을 눌렀을 때 스테이지(적 정보, 보상정보, 진입버튼 눌렀을 시 이동하는 스테이지) 정보 출력</summary>
    public void EnterStage(int chatperNum, int stageNum, Stage_Set clickStage)
    {
        if (lastSpotligh != null)
        {
            lastSpotligh.Spotlight(false);
        }

        clickStage.Spotlight(true);
        lastSpotligh = clickStage;

        enterChapter = chatperNum; //진입버튼 누를 때 이동할 챕터 설정
        enterStage = stageNum; //진입버튼 누를 때 이동할 스테이지 설정

        stageEntry = stageManager.GetStageEntry(enterChapter, enterStage); //스테이지 엔트리 불러오기

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
    void EventChain(StageSO stage)//이벤트 연결용
    {
        AllChapter();
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
        after.onClick.RemoveAllListeners();
        before.onClick.RemoveAllListeners();
        enter.onClick.RemoveAllListeners(); //리스너 중복구독 방지

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
    /// <summary> 버튼 클릭시 스테이지 변경 명령을 보내는 메서드 </summary>
    void OnClickChangeStage()
    {
        Debug.Log($"OnClickChangeStage 호출 / chapter:{enterChapter} stage:{enterStage} frame:{Time.frameCount}");
        stageManager.ChangeStage(enterChapter, enterStage);
        Destroy(gameObject);
    }
    /// <summary> 스테이지 매니저에서 변경 완료된 스테이지 정보 확인해서 챌린지용 UI 키고 끄기
    /// 꼭 StageSO를 안받고 해당 코드의 StageEntry를 이용해도 괜찮을듯함</summary>
    public void UpdateStageChange(StageSO stageSo)
    {
        if (stageSo.type == StageType.Normal)
        {
            
            var popup = GameManager.Instance.GetGameSystem<UI.Scripts.PopupManager>();
            popup.CloseBossUI();
            popup.CloseMonsterKill();
            popup.CloseTimer();
            return;
        }
        if (stageSo.type == StageType.Challenge)
        {
            var popup = GameManager.Instance.GetGameSystem<UI.Scripts.PopupManager>();
            popup.CloseBossUI();

            popup.OpenTimer();
            popup.OpenMonsterKill();
            return;
        }
        if (stageSo.type == StageType.Boss)
        {
            var popup = GameManager.Instance.GetGameSystem<UI.Scripts.PopupManager>();
            popup.CloseMonsterKill();

            popup.OpenTimer();
            popup.OpenBossUI();
            return;
        }
    }
    //챌린지 UI 활성 / 비활성화 경로
    //1. OnClickChangeStage -> StageManager.ChangeStage -> StageManager.EventHub.StageChangeClear -> UpdateStageChange
    // -> UIPresenter.SetChallengeUI
}
