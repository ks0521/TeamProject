using Base.Data;
using Base.Managers;
using Base.Save;
using Battle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class imsiQuestController : MonoBehaviour, IManager
{
    private EventHub eventHub;
    private  RuntimeProgressState progress;
    void Update()
    {
        // 1. Delete 키를 누르면 레벨을 강제로 1 올리고 이벤트를 쏩니다.
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            
            if (progress != null)
            {
                // 데이터 조작: PlayerRuntimeStatus의 레벨을 강제로 올림
                
                int currentLevel = ++progress.currency.level;
                Debug.Log($"<color=orange>[TEST] 레벨 강제 조작: {currentLevel}</color>");

                // 이벤트 발생: QuestManager가 이 소식을 듣고 퀘스트를 업데이트함
                eventHub.LevelChanged(currentLevel);
            }
        }

        // 2. End 키를 누르면 스킬 사용 이벤트를 쏩니다.
        if (Input.GetKeyDown(KeyCode.End))
        {
            Debug.Log("<color=orange>[TEST] 스킬 사용 이벤트 강제 발생</color>");
            eventHub.SkillUsed(1); // 1번 스킬 사용 가정
        }

        // 3. PageDown 키를 누르면 스테이지 클리어 이벤트를 쏩니다 (테스트용)
        if (Input.GetKeyDown(KeyCode.PageDown))
        {
            Debug.Log("<color=orange>[TEST] 스테이지 클리어 강제 발생</color>");
            // QuestManager의 UpdateQuest를 직접 부르거나, 
            // 가짜 StageSO 데이터를 만들어 EventHub를 통해 쏠 수 있습니다.
            // 여기서는 간단히 직접 호출 예시:
            // QuestManager.Instance.UpdateQuest(GoalType.StageClear, 101, 1);
        }
    }

    public int GetOrder() => 998;
    public void Init()
    {
        progress = GameManager.Instance.GetGameSystem<PlayerProgressManager>().Progress;
    }
}
