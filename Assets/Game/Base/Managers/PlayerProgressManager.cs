using System;
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
    public class PlayerProgressManager : MonoBehaviour
    {
        public static PlayerProgressManager Instance;
        [SerializeField] private StatusCalculator playerStatCalculator;
        public RuntimeProgressState progress; //현재 플레이어의 정보를 전부 저장하고 있는 데이터
        public StatusSO statUpgradeConfig;
        public RuntimeProgressState GetProgress() => progress;
        public bool IsLoaded() => progress != null;

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
            Debug.Log($"상태 계산중 {playerStatCalculator == null}");
            playerStatCalculator?.Calculate(progress);
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
        
        public StageProgress GetStageProgress()
        {
            return new StageProgress()
            {
                selectedNormalChapter = progress.stage.selectedNormalChapter,
                selectedNormalStage = progress.stage.selectedNormalStage,
                nextChallengeChapter = progress.stage.nextChallangeChapter,
                nextChallengeStage = progress.stage.nextChallangeStage
            };
        }
        /// <summary> 노말 스테이지 변경 </summary>
        /// <returns> 변경된 런타임 스테이지 데이터</returns>
        public StageProgress SelectNormalStage(int changeChapter,int changeStage)
        {
            progress.stage.selectedNormalChapter = changeChapter;
            progress.stage.selectedNormalStage = changeStage;
            SaveManager.Save(DataConverter.RuntimeToSave(progress));
            return new StageProgress()
            {
                selectedNormalChapter = progress.stage.selectedNormalChapter,
                selectedNormalStage = progress.stage.selectedNormalStage,
                nextChallengeChapter = progress.stage.nextChallangeChapter,
                nextChallengeStage = progress.stage.nextChallangeStage
            };
        }

        public StageProgress ProgressChallengeStage(StageSO clearStage)
        {
            //보스를 잡았으면 다음 챕터 2-1
            if (clearStage.type == StageType.Boss)
            {
                progress.stage.nextChallangeChapter = clearStage.chapter + 1;
                progress.stage.nextChallangeStage = 2;
            }
            //아니면 스테이지 +1만
            else
            {
                progress.stage.nextChallangeStage++;
            }
            SaveManager.Save(DataConverter.RuntimeToSave(progress));
            return new StageProgress()
            {
                selectedNormalChapter = progress.stage.selectedNormalChapter,
                selectedNormalStage = progress.stage.selectedNormalStage,
                nextChallengeChapter = progress.stage.nextChallangeChapter,
                nextChallengeStage = progress.stage.nextChallangeStage
            };
        } 
        /// <summary> 현재 재화로 스탯 강화가 가능한지 확인하는 함수</summary>
        /// <param name="statType">강화하고 싶은 스탯 타입</param>
        /// <param name="upgradeCount">강화 횟수</param>
        /// <returns>현재 강화석으로 강화가 가능하면 true, 불가능하면 false</returns>
        public bool CanUpgradeStat(StatusType statType, int upgradeCount)
        {
            statUpgradeConfig.TryGetStatEntry(statType, out var statEntry);
            int requireCost = 0;
            for (int i = 1; i <= upgradeCount; i++)
            {
                requireCost += (progress.statUpgrades.upgradeLevelsByType[statType] + i) * statEntry.enhanceCost;
            }
            Debug.Log($"Need Cost : {requireCost}");
            if (requireCost > progress.currency.statStone)
            {
                Debug.Log($"{statType}스텟강화에 필요한 골드가 부족합니다(요구 {requireCost}강화석 / 소지 {progress.currency.statStone})");
                return false;
            }
            return true;
        }
        /// <summary> 특정 스탯의 강화 수치를 반환하는 메서트</summary>
        /// <param name="statType">강화정도를 알고싶은 스탯 타입</param>
        /// <returns>강화 횟수</returns>
        public int GetStatUpgradeLevel(StatusType statType)
        {
            return progress.statUpgrades.upgradeLevelsByType[statType];
            //GameDataManager.instance.runtimedata.stat.upgrade[type.atk]
        }
        /// <summary> 강화를 시도하는 메서드</summary>
        /// <param name="statType">강화 타입</param>
        /// <param name="upgradeCount">강화 횟수</param>
        /// <returns> 재화 소모해서 스탯 강화 성공 여부, 재화가 부족해서 실패했으면 false</returns>
        public bool TryUpgradeStat(StatusType statType, int upgradeCount)
        {
            if (!CanUpgradeStat(statType, upgradeCount))
            {
                return false;
            }
            statUpgradeConfig.TryGetStatEntry(statType, out var statEntry);
            int requireCost = 0;
            for (int i = 1; i <= upgradeCount; i++)
            {
                requireCost += (progress.statUpgrades.upgradeLevelsByType[statType] + i) * statEntry.enhanceCost;
            }

            progress.statUpgrades.upgradeLevelsByType[statType] += upgradeCount;
            progress.currency.statStone -= requireCost;
            Debug.Log($"{statType}스탯 {upgradeCount}번 강화, {requireCost}강화석 사용, 남은 강화석 : {progress.currency.statStone} " +
                      $"\n {statType}스탯 강화횟수 : {progress.statUpgrades.upgradeLevelsByType[statType]}(+{upgradeCount})");
            return true;
        }
    }
}