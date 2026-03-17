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
        public RuntimeData runtimeData;
        public StageProgressData StageProgressData => runtimeData.stageProgress;
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
            calculator?.Calculate(runtimeData);
        }

        /// <summary> 런타임 데이터 기기에 저장</summary>
        public void Save()
        {
            Debug.Log("GameDataManager : 진행 상황을 저장합니다. ");
            JJ_SaveManager.Save(DataConverter.RuntimeToSave(runtimeData));
        }

        /// <summary> 저장된 데이터 런타임 데이터형식으로 불러오기</summary>
        public void Load()
        {
            runtimeData = DataConverter.SaveToRuntime(JJ_SaveManager.Load());
        }

        public RuntimeData GetData() => runtimeData;
        public bool HasData() => runtimeData != null;

        public StageInfo GetStageInfo()
        {
            return new StageInfo()
            {
                selectedChapter = runtimeData.stageProgress.selectedNormalChapter,
                selectedStage = runtimeData.stageProgress.selectedNormalStage,
                nextChallengeChapter = runtimeData.stageProgress.nextChallangeChapter,
                nextChallengeStage = runtimeData.stageProgress.nextChallangeStage
            };
        }

        public bool RequestStatEnhance(StatusType type, int count)
        {
            statusConfig.TryGetStatEntry(type, out var stat);
            int totalCost = 0;
            for (int i = 1; i <= count; i++)
            {
                totalCost = (runtimeData.stat.upgrade[type] + i) * stat.enhanceCost;
            }

            Debug.Log($"Need Cost : {totalCost}");
            return true;
        }
    }
}