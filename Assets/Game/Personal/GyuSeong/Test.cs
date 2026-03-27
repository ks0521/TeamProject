using Base.Managers;
using Base.Save;
using Growth.StatUpgrade;
using UnityEngine;
using UnityEngine.Serialization;

public class Test : MonoBehaviour
{
    // Update is called once per frame
    [SerializeField]private GameSaveData saveData;
    [FormerlySerializedAs("runData")] [SerializeField] private RuntimeProgressState runProgressState;
    [SerializeField] private PlayerRuntimeStatus runStat;
    [SerializeField] private StatusCalculator calc;
    private void Start()
    {
        Debug.Log("1. 저장 / 2. 불러오기 / 3. 저장파일 삭제 / 4 . 저장데이터 런타임 데이터로 변환");
    }

    void Update()
    {
        #if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("저장 입력");
            PlayerProgressManager.Instance.SaveProgress();
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("불러오기 실행");
            PlayerProgressManager.Instance.LoadProgress();
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Debug.Log("저장파일 삭제");
            SaveManager.DeleteSaveFile();
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            runProgressState = DataConverter.SaveToRuntime(saveData);
        }
        #endif
    }
}
