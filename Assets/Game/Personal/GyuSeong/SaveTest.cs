using System.Collections;
using System.Collections.Generic;
using Base.Managers;
using Base.Save;
using UnityEngine;

public class SaveTest : MonoBehaviour
{
    // Update is called once per frame
    [SerializeField]private GameSaveData data;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("저장 입력");
            SaveManager.Save(data);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("불러오기 실행");
            data = SaveManager.Load();
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Debug.Log("저장파일 삭제");
            SaveManager.DeleteSaveFile();
        }
    }
}
