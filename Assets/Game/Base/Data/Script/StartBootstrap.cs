using Base.Data;
using Base.Manager;
using Base.Managers;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class StartBootstrap : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private DataLoadManager dataLoadManager;
    [SerializeField] private GameDataDictionaries dic;
    [SerializeField] private GameObject loadingCanvas;
    private async UniTaskVoid Start()
    {
        loadingCanvas.SetActive(true);
        try
        {
            await dataLoadManager.InitAllData(dic);
            gameManager.InitAllManagers();
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"초기값 로딩 에러 : {e}");
            //게임 종료시키기
        }
        finally
        {
            Destroy(loadingCanvas);
        }
        // 1. 로딩창 띄우기
        // 2. DataLoadManager에서 Addressable 로드
        // 3. 필수SO(Stage,Equipment등..) MakeDictionary
        // 4. GameManager.InitAllManagers()호출
        // 5. 로딩창 제거
    }

}
