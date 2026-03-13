using System.Collections;
using System.Collections.Generic;
using Base.Data;
using UnityEngine;

public class PlayerRuntimeStatus : MonoBehaviour
{
    public static PlayerRuntimeStatus Instance;
    public PlayerBaseStatusSO baseStat;
    public BattleStat finalBattleStatus;
    public RewardStat finalRewardStatus;
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
}
