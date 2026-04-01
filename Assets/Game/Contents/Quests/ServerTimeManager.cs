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

// 안드로이드 빌드 시 Project Settings > Player > Android
// > Internet Access를 Require로 설정해야 통신이 가능
public class ServerTimeManager : MonoBehaviour, IManager
{
    private DateTime serverDateTime;
    private float lastSyncTime;
    private bool isTimeLoaded = false;

    // 대안 1: TimeAPI
    private const string PrimaryApiUrl = "https://www.timeapi.io/api/Time/current/zone?timeZone=Asia/Seoul";
    // 대안 2: Google (헤더 추출용)
    // private const string BackupUrl = "https://www.google.com";
    public void Init()
    {
        
    }
    public int GetOrder() => 399;
    async void Start()
    {
        await SyncServerTime();
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
                Debug.Log($"[TimeAPI] 동기화 성공: {serverDateTime}");
                return true;
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
        if (!isTimeLoaded) return DateTime.Now;
        float elapsed = Time.unscaledTime - lastSyncTime;
        return serverDateTime.AddSeconds(elapsed);
    }
    string ExtractJsonValue(string json, string key)
    {
        string search = $"\"{key}\":\"";
        int start = json.IndexOf(search) + search.Length;
        int end = json.IndexOf("\"", start);
        return json.Substring(start, end - start);
    }
}
