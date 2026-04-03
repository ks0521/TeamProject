using System;
using Base.Data;
using UnityEngine;

[CreateAssetMenu(menuName = "Test/Base/Player")]
public class PlayerBaseStatusSO : ScriptableObject
{
    [Header("Base Stat")] public TotalStat total;
}