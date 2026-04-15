using Base.Data;
using Base.Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
//using (UnityWebRequest webRequest = UnityWebRequest.Get("..."));

// 일일 퀘스트의 기준이 되는 서버 시간을 담당
// 안드로이드 빌드 시 Project Settings > Player > Android
// > Internet Access를 Require로 설정해야 통신이 가능
public class ServerTimeManager : MonoBehaviour, IManager
{
    private DateTime serverDateTime;
    private float lastSyncTime;
    private string lastCheckedDate = "";
    private TimeSpan debugTimeOffset = TimeSpan.Zero; //강제로 시간을 변환할 오프셋
    private bool isTimeLoaded = false;
    public bool IsSyncedTime => isTimeLoaded;

    // 대안 1: TimeAPI
    private const string PrimaryApiUrl = "https://www.timeapi.io/api/Time/current/zone?timeZone=Asia/Seoul";
    // 대안 2: Google (헤더 추출용)
    // private const string BackupUrl = "https://www.google.com";
    public void Init()
    {
        isTimeLoaded = false;
        lastSyncTime = 0;
        AsyncInitialize();
        Debug.Log($"[ServerTimeManager] 초기화 완료 (Order: {GetOrder()})");
    }
    public int GetOrder() => 333; //일일 퀘스트를 확인하는 로직은 이 이후

    async void AsyncInitialize() //Init()을 async와 안전하게 연결하기 위한 함수
    {
        try
        {
            await SyncServerTime();

            // 동기화가 완료된 후 필요한 추가 로직이 있다면 여기서 처리
            Debug.Log("[ServerTimeManager] 비동기 초기화 및 동기화 완료.");
        }
        catch (System.Exception e)
        {
            //async void 함수는 예외가 발생하면 앱이 크래시될 수 있으므로 반드시 예외 처리가 필요합니다.
            Debug.LogError($"[ServerTimeManager] 초기화 중 예외 발생: {e.Message}");
        }
    }
    public async Task SyncServerTime()
    {
        bool isSynced = await TryGetTimeFromAPI();
        if(isSynced)
        {
            lastSyncTime = Time.unscaledTime;
            isTimeLoaded = true;
        }
        else
        {
            Debug.LogWarning("서버 동기화에 실패하여 로컬 시간을 사용합니다.");
            serverDateTime = DateTime.Now;
            lastSyncTime = Time.unscaledTime;
            isTimeLoaded = true;
        }
        TryMidnightCheck();
    }

    async Task<bool> TryGetTimeFromAPI()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(PrimaryApiUrl))
        {
            var operation = request.SendWebRequest();
            while (!operation.isDone) await Task.Yield();

            if (request.result == UnityWebRequest.Result.Success)
            {
                var json = request.downloadHandler.text;
                string dtStr = ExtractJsonValue(json, "dateTime");
                serverDateTime = DateTime.Parse(dtStr);
                if (DateTime.TryParse(dtStr, CultureInfo.InvariantCulture, DateTimeStyles.None, out serverDateTime))
                {
                    Debug.Log($"[ServerTimeManager] 서버 동기화 성공: {serverDateTime}");
                    return true;
                }
            }
        }
        return false;
    }
    /*
    async Task<bool> TryGetTimeFromGoogleHeader()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Head(BackupUrl))
        {
            var operation = webRequest.SendWebRequest();
            while (!operation.isDone) await Task.Yield();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                // 헤더에서 "date" 값을 가져옴 (예: "Wed, 05 Feb 2025 01:30:00 GMT")
                string dateStr = webRequest.GetResponseHeader("date");
                if (!string.IsNullOrEmpty(dateStr))
                {
                    // GMT(UTC) 시간을 파싱한 뒤 한국 시간(+9)으로 변환
                    DateTime utcTime = DateTime.ParseExact(dateStr, "ddd, dd MMM yyyy HH:mm:ss 'GMT'",
                                        CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
                    serverDateTime = utcTime.AddHours(9);
                    Debug.Log($"[GoogleHeader] 동기화 성공: {serverDateTime}");
                    return true;
                }
            }
        }
        return false;
    }
    */
    public DateTime GetCurrentServerTime()
    {
        if (!IsSyncedTime) return DateTime.Now; //로딩 미완료 시에는 로컬 시간 반환
        float elapsed = Time.unscaledTime - lastSyncTime;
        DateTime actualServerTime = serverDateTime.AddSeconds(elapsed);
        //return serverDateTime.AddSeconds(elapsed); //실제로 사용하는 코드
        return actualServerTime.Add(debugTimeOffset); //테스트용 코드
    }
    public void Test_AddHours(float h)
    {
        debugTimeOffset = debugTimeOffset.Add(TimeSpan.FromHours(h));
        Debug.Log($"현재 시간(조작됨): {GetCurrentServerTime(): yyyy-MM-dd HH:mm:ss}");
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F12)) //8시간씩 미래로 이동
        {
            Test_AddHours(8f);
        }
    }
    async void TryMidnightCheck() //AsyncInitialize와 같은 원리
    {
        try
        {
            await StartMidnightCheckLoop();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[ServerTimeManager] 자정 감지 루프 중 에러: {e.Message}");
        }
    }
    async Task StartMidnightCheckLoop()
    {
        // 최초 실행 시 현재 날짜를 기록
        lastCheckedDate = GetCurrentServerTime().ToString("yyyy-MM-dd");
        Debug.Log($"[ServerTimeManager] 자정 체크 루프 시작. 현재 기준 날짜: {lastCheckedDate}");
        while (true)
        {
            // 1. 자정 여부 체크
            CheckMidnight();

            // 2. 5초 대기(간격은 나중에 수정)
            await Task.Delay(5000);

            // 유니티 오브젝트가 파괴되면 루프를 종료하도록 안전장치 추가
            if (this == null) break;
        }
    }
    void CheckMidnight()
    {
        if (!isTimeLoaded) return;

        DateTime now = GetCurrentServerTime();
        string todayStr = now.ToString("yyyy-MM-dd");

        // 저장된 날짜와 현재 날짜가 다르면 자정(날짜 변경)으로 간주
        if (todayStr != lastCheckedDate)
        {
            Debug.Log($"<color=cyan>[ServerTimeManager] 자정 감지됨! {lastCheckedDate} -> {todayStr}</color>");

            // 1. 기준 날짜 갱신
            lastCheckedDate = todayStr;

            // 2. 전역 이벤트 발생
            EventHub.NewDayStarted(todayStr);
        }
    }
    string ExtractJsonValue(string json, string key)
    {
        string search = $"\"{key}\":\"";
        int start = json.IndexOf(search) + search.Length;
        if (start < search.Length) return ""; //key를 못 찾으면 빈 값 반환

        int end = json.IndexOf("\"", start);
        return json.Substring(start, end - start);
    }
}
