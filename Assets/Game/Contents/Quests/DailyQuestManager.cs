using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class DailyQuestManager : MonoBehaviour
{
    private const string LastResetKey = "LastResetDate";
    void Start()
    {
        CheckDailyReset();
    }

    public void CheckDailyReset()
    {
        // 1. 저장된 날짜 가져오기 (없으면 아주 오래전 날짜)
        string lastDateStr = PlayerPrefs.GetString(LastResetKey, "1900-01-01");
        DateTime lastDate = DateTime.Parse(lastDateStr);

        // 2. 현재 날짜 가져오기 (시간 제외, 날짜만)
        DateTime today = DateTime.Today;

        // 3. 비교
        if (today > lastDate)
        {
            DoReset(); //초기화 실행

            // 4. 오늘 날짜로 갱신 저장
            PlayerPrefs.SetString(LastResetKey, today.ToString("yyyy-MM-dd"));
            PlayerPrefs.Save();
        }
    }

    private void DoReset()
    {
        Debug.Log("<color=green>날짜가 변경되었습니다. 일일 퀘스트를 초기화합니다!</color>");

        //QuestManager.Instance.ResetDailyQuests();
    }
}
