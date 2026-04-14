using Base.Data;
using Base.Manager;
using Base.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class StartBootstrap : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private DataLoadManager dataLoadManager;
    [SerializeField] private GameDataProvider dic;
    [SerializeField] private GameObject loadingCanvas;
    void Start()
    {
        loadingCanvas.SetActive(true);
        // 1. 로딩창 띄우기
        // 2. DataLoadManager에서 Addressable 로드
        // 3. 필수SO(Stage,Equipment등..) MakeDictionary
        // 4. GameManager.InitAllManagers()호출
        // 5. 로딩창 제거
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
