using System;
using Base.Data;
using Base.Managers;
using Battle;
using Growth.StatUpgrade;
using UnityEngine;

namespace Base.Save
{
    [Serializable]
    public struct StageProgress
    {
        public int selectedNormalChapter;
        public int selectedNormalStage;
        public int nextChallengeChapter;
        public int nextChallengeStage;
    }
    /// <summary> 실제 런타임 데이터를 보유 / 저장 / 로드하는 데이터 매니저  </summary>
    public class PlayerProgressManager : MonoBehaviour, IManager
    {
        public static PlayerProgressManager Instance;
        public RuntimeProgressState progress; //현재 플레이어의 정보를 전부 저장하고 있는 데이터
        public StatusSO statUpgradeConfig; //UI파트에서 바꾸면 지울 예정
        public RuntimeProgressState GetProgress() => progress;
        public bool IsLoaded() => progress != null;
        [SerializeField] private StatusCalculator playerStatCalculator;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        public void Init()
        {
            LoadProgress();
            statUpgradeConfig = GameDataProvider.Instance.hub.statusTable;
            Debug.Log($"상태 계산중 {playerStatCalculator == null}");
            playerStatCalculator?.Calculate(progress);
        }

        public int GetOrder()
        {
            return 1;
        }

        /// <summary> 런타임 데이터 기기에 저장</summary>
        public void SaveProgress()
        {
            Debug.Log("GameDataManager : 진행 상황을 저장합니다. ");
            SaveManager.Save(DataConverter.RuntimeToSave(progress));
        }
        /// <summary> 저장된 데이터 런타임 데이터형식으로 불러오기</summary>
        public void LoadProgress()
        {
            progress = DataConverter.SaveToRuntime(SaveManager.Load());
        }
    }
}