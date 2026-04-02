using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using QuestSystem; //QuestManager 호출용

//ServerTimeManager를 바탕으로 자정 때 일일 퀘스트 리셋
public class DailyQuestManager : MonoBehaviour
{
    private const string LastResetKey = "LastResetDate";
    private bool isInitialized = false;
    private QuestManager questManager;
    private ServerTimeManager timeManager;
    void Start()
    {
        StartCoroutine(WaitAndCheckReset());
    }
    IEnumerator WaitAndCheckReset()
    {
        //ServerTimeManager가 시간을 가져올 때까지 프레임 단위로 대기
        while (timeManager == null || !timeManager.IsSyncedTime)
        {
            yield return null; 
        }
        Debug.Log("[DailyQuestManager] 서버 시간 준비 완료. 리셋 체크를 시작합니다.");

        CheckDailyReset();

        while (true) //5초 단위로 자정 체크
        {
            yield return new WaitForSeconds(5f);
            CheckDailyReset();
        }
    }
    public void CheckDailyReset()
    {
        //저장된 날짜 가져오기(없다면 1900-01-01을 반환)
        string lastDateStr = PlayerPrefs.GetString(LastResetKey, "1900-01-01");
        if (!DateTime.TryParse(lastDateStr, out DateTime lastDate))
        {
            lastDate = new DateTime(1900, 1, 1);
        }

        DateTime nowServerTime = timeManager.GetCurrentServerTime();
        DateTime today = nowServerTime.Date;

        //날짜 비교(서버 상의 오늘이 마지막 리셋 날짜보다 큰가?)
        if (today > lastDate)
        {
            DoReset(); //초기화 실행

            //오늘 날짜로 갱신 저장
            PlayerPrefs.SetString(LastResetKey, today.ToString("yyyy-MM-dd"));
            PlayerPrefs.Save();
            Debug.Log($"[DailyQuestManager] 오늘의 날짜: {today:yyyy-MM-dd}");
        }
    }

    void DoReset()
    {
        Debug.Log("<color=green>날짜가 변경되었습니다. 일일 퀘스트를 초기화합니다!</color>");

        if (questManager != null) questManager.ResetDailyQuests();
    }
}
