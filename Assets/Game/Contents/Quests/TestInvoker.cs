using Base.Data;
using Base.Managers;
using Battle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestInvoker : MonoBehaviour
{
    /*
    private int _currentLevel = 1;
    private int _testChapter = 1;
    private int _testStage = 0;
    public EventHub eventHub;

    void Update()
    {
        // Del키를 누르면 레벨업 이벤트 발생
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            _currentLevel++;
            Debug.Log($"[테스트] 레벨업 버튼 클릭! 새 레벨: {_currentLevel}");
            eventHub.LevelChanged(_currentLevel);
        }

        // End키를 누르면 스킬 사용 이벤트 발생 (1~6번 중 랜덤)
        if (Input.GetKeyDown(KeyCode.End))
        {
            int randomSkill = Random.Range(1, 7);
            Debug.Log($"[테스트] 스킬 {randomSkill}번 버튼 클릭!");
            eventHub.SkillUsed(randomSkill);
        }
        if (Input.GetKeyDown(KeyCode.PageUp))
        {
            if (eventHub != null)
            {
                _testStage++;
                if (_testStage > 5)
                {
                    _testChapter++;
                    _testStage = 1;
                }
                StageSO dummyStage = ScriptableObject.CreateInstance<StageSO>();
                dummyStage.chapter = _testChapter;
                dummyStage.stage = _testStage;

                Debug.Log($"[테스트] 가짜 스테이지 클리어 신호 발사! (챕터: {dummyStage.chapter}, 스테이지: {dummyStage.stage})");

                eventHub.StageCleared(dummyStage);
                Destroy(dummyStage);
            }
        }
    }
    */
}
