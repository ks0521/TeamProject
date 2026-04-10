using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Reward/RewardTable")]
public class RewardTableSO : ScriptableObject
{
    [Header("장비 보상 테이블")]
    public List<DropReward> rewardList = new();
}
