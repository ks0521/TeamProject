using Base.Data;
using Base.Managers;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary> 게임 플레이시 사용되는 플레이어 최종스탯 </summary>
public class PlayerRuntimeStatus : MonoBehaviour, IGameSystem
{
    public static PlayerRuntimeStatus Instance; //MVP종료후 제거
    public PlayerBaseStatusSO baseStat;
    public TotalStat finalStatus;
    public BattleStat finalBattleStatStatus;
    public RewardStat finalRewardStatStatus;
    // Start is called before the first frame update
    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public int GetOrder() => 3; //PlayerProgressManager(1) 실행이 보장되어야 함
}
