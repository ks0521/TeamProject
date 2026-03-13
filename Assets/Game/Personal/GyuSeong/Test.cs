using System;
using System.Collections;
using System.Collections.Generic;
using Base.Managers;
using Base.Save;
using Growth.StatUpgrade;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class Test : MonoBehaviour
{
    // Update is called once per frame
    [SerializeField]private GameSaveData saveData;
    [SerializeField] private RuntimeData runData;
    [SerializeField] private PlayerRuntimeStatus runStat;
    [SerializeField] private StatusCalculator calc;
    private void Start()
    {
        Debug.Log("1. 저장 / 2. 불러오기 / 3. 저장파일 삭제 \nF1 . 저장데이터 런타임 데이터로 변환");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("저장 입력");
            GameDataManager.Instance.Save();
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("불러오기 실행");
            GameDataManager.Instance.Load();
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Debug.Log("저장파일 삭제");
            SaveManager.DeleteSaveFile();
        }

        if (Input.GetKeyDown(KeyCode.F1))
        {
            runData = DataConverter.SaveToRuntime(saveData);
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            GameDataManager.Instance.Load();
        }

        if (Input.GetKeyDown(KeyCode.F3))
        {
            //GameDataManager.Instance.RequestStatEnhance(StatusType.Atk, 5);
        }
    }
}
