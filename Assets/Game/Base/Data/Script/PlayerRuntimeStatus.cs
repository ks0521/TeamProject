using Base.Data;
using Base.Managers;
using UnityEngine;

public class PlayerRuntimeStatus : MonoBehaviour, IGameSystem
{
    public static PlayerRuntimeStatus Instance; //MVP종료후 제거
    public PlayerBaseStatusSO baseStat;
    public BattleStat finalBattleStatus;
    public RewardStat finalRewardStatus;
    public int Level;
    public float finalRange;
    public float finalAttackSpeed;
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

    public int GetOrder() => 2; //PlayerProgressManager(1) 실행이 보장되어야 함
}
