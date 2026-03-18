using System;
using Base.Managers;
using Base.Save;
using Growth.StatUpgrade;
using UnityEngine;

namespace Personal_Jongjun
{
    [Serializable]
    public struct StageInfo
    {
        public int selectedStage;
        public int selectedChapter;
        public int nextChallengeStage;
        public int nextChallengeChapter;
    }
    /// <summary> 실제 런타임 데이터를 보유 / 저장 / 로드하는 데이터 매니저  </summary>
    public class JJ_GameDataManager : MonoBehaviour
    {
        public static JJ_GameDataManager Instance;
        [SerializeField] private StatusCalculator calculator;
        public RuntimeProgressState runtimeProgressState;
        public StageProgressState StageState => runtimeProgressState.stage;
        public StatusSO statusConfig;

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
            Load();
            calculator?.Calculate(runtimeProgressState);
        }

        /// <summary> 런타임 데이터 기기에 저장</summary>
        public void Save()
        {
            Debug.Log("GameDataManager : 진행 상황을 저장합니다. ");
            JJ_SaveManager.Save(DataConverter.RuntimeToSave(runtimeProgressState));
        }

        /// <summary> 저장된 데이터 런타임 데이터형식으로 불러오기</summary>
        public void Load()
        {
            runtimeProgressState = DataConverter.SaveToRuntime(JJ_SaveManager.Load());
        }

        public RuntimeProgressState GetData() => runtimeProgressState;
        public bool HasData() => runtimeProgressState != null;

        public StageInfo GetStageInfo()
        {
            return new StageInfo()
            {
                selectedChapter = runtimeProgressState.stage.selectedNormalChapter,
                selectedStage = runtimeProgressState.stage.selectedNormalStage,
                nextChallengeChapter = runtimeProgressState.stage.nextChallangeChapter,
                nextChallengeStage = runtimeProgressState.stage.nextChallangeStage
            };
        }

        public bool RequestStatEnhance(StatusType type, int count)
        {
            statusConfig.TryGetStatEntry(type, out var stat);
            int totalCost = 0;
            for (int i = 1; i <= count; i++)
            {
                totalCost = (runtimeProgressState.statUpgrades.upgradeLevelsByType[type] + i) * stat.enhanceCost;
            }

            Debug.Log($"Need Cost : {totalCost}");
            return true;
        }
    }
}