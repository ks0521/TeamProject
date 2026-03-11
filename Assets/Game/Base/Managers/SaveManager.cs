using System;
using System.IO;
using Base.Save;
using UnityEngine;

namespace Base.Managers
{
    /// <summary> 게임 시작 시 데이터 불러오기 + GameSaveData 관리</summary>
    public static class SaveManager
    {
        //안드로이드용, 테스트 후 휴대폰 테스트시 해당 설정 사용
        private static string SavePath => Path.Combine(Application.persistentDataPath, "SaveData.json");
        private static int number => 5;
        /// <summary> 데이터 저장하기 </summary>
        public static void Save(GameSaveData data)
        {
            data.lastAccess.lastConnectTime = DateTime.Now.ToBinary(); //최종 접속시간 저장
            
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(SavePath,json);
            Debug.Log($"파일 저장 완료 , 경로 : {Application.persistentDataPath}");
        }

        /// <summary> 데이터 불러오기 </summary>
        public static GameSaveData Load()
        {
            if (!File.Exists(SavePath))
            {
                Debug.Log("불러올 세이브파일 없음. 신규 생성");
                return MakeDefaultSaveData();
            }
            
            string json = File.ReadAllText(SavePath);
            GameSaveData data = JsonUtility.FromJson<GameSaveData>(json);
            if (data is null) return MakeDefaultSaveData();
            Debug.Log($"세이브파일 로드 완료 , 경로 : {Application.persistentDataPath}");
            return data;
        }

        public static void DeleteSaveFile()
        {
            if (!File.Exists(SavePath))
            {
                Debug.LogWarning("삭제할 저장파일이 없습니다. ");
                return;
            }
            File.Delete(SavePath);
            Debug.Log($"세이브파일 삭제 완료 , 경로 : {Application.persistentDataPath}");
        }
        /// <summary> 게임 처음 시작했을 때의 기본 데이터 설정</summary>
        public static GameSaveData MakeDefaultSaveData()
        {
            GameSaveData data =  new GameSaveData()
            {
                stageProgress = new StageProgressData()
                {
                    curNormalStage = 1,
                    curNormalChapter = 1,
                    maxClearStage = 1,
                    maxClearChapter = 1
                },
                currencyData = new PlayerCurrencyData()
                {
                    level = 0,
                    exp = 0,
                    gold = 0,
                    statStone = 0
                },
                //equipmentInventory = new PlayerEquipmentInventoryData(),
                //equipment = new PlayerEquipmentData(),
                itemInventory = new PlayerItemInventoryData(),
                statUpgrade = new PlayerStatUpgradeData(),
                skill = new PlayerSkillData(),
                lastAccess = new PlayerAccessTimeData()
                {
                    lastConnectTime = DateTime.Now.ToBinary() //최종 접속시간 저장
                }
            };
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(SavePath,json);
            Debug.Log("새 저장파일 생성 완료");
            return data;
        }
    }
}