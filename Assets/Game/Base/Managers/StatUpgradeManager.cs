using Base.Data;
using Base.Save;
using Battle;
using Growth.StatUpgrade;
using UnityEngine;

namespace Base.Managers
{
    public class StatUpgradeManager : MonoBehaviour, IManager
    {
        //UI파트보다 먼저 초기화되어야함
        private RuntimeProgressState Progress => PlayerProgressManager.Instance.progress;
        private StatusCalculator calculator;
        private StatusSO statUpgradeConfig;
        private EventHub eventHub;

        public void Init()
        {
            statUpgradeConfig = GameDataProvider.Instance.hub.statusTable;
            calculator = GameManager.Instance.GetGameSystem<StatusCalculator>();
            eventHub = GameManager.Instance.GetGameSystem<EventHub>();
        }

        public int GetOrder()
        {
            return 99; 
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
                requireCost += (Progress.statUpgrades.upgradeLevelsByType[statType] + i) * statEntry.enhanceCost;
            }

            Debug.Log($"Need Cost : {requireCost}");
            if (requireCost > Progress.currency.statStone)
            {
                Debug.Log($"{statType}스텟강화에 필요한 골드가 부족합니다(요구 {requireCost}강화석 / 소지 {Progress.currency.statStone})");
                return false;
            }

            return true;
        }

        /// <summary> 특정 스탯의 강화 수치를 반환하는 메서트</summary>
        /// <param name="statType">강화정도를 알고싶은 스탯 타입</param>
        /// <returns>강화 횟수</returns>
        public int GetStatUpgradeLevel(StatusType statType)
        {
            return Progress.statUpgrades.upgradeLevelsByType[statType];
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
                requireCost += (Progress.statUpgrades.upgradeLevelsByType[statType] + i) * statEntry.enhanceCost;
            }
            //업그레이드 횟수 늘리고 재화 차감
            Progress.statUpgrades.upgradeLevelsByType[statType] += upgradeCount;
            Progress.currency.statStone -= requireCost;
            eventHub.CurrencyChange(CurrencyType.STATSTONE,Progress.currency.statStone);
            
            Debug.Log($"{statType}스탯 {upgradeCount}번 강화, {requireCost}강화석 사용, 남은 강화석 : {Progress.currency.statStone} " +
                      $"\n {statType}스탯 강화횟수 : {Progress.statUpgrades.upgradeLevelsByType[statType]}(+{upgradeCount})");
            //강화결과 실제 스탯에 반영
            calculator.Calculate(Progress);
            //최대체력 증가 스탯일 경우에는 증가한 최대체력만큼 HP 회복
            if (statType == StatusType.MaxHp)
            {
                int healAmount = (int)(upgradeCount * statEntry.increasePerEnhance);

                if (GameManager.Instance.TryGetGameSystem<PlayerManager>(out var playerManager) && playerManager != null)
                {
                    playerManager.Player.RecoveryHP(healAmount);
                }
            }
            return true;
        }
    }
}