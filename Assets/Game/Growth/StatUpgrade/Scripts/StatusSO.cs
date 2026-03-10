using UnityEngine;

namespace Growth.StatUpgrade
{
    public enum StatusType
    {
        Atk,MaxHp,Def,AtkSpeed,CritChance,CritDmg,MoveSpeed
    }
    [CreateAssetMenu(menuName = "Game/Growth/Status")]
    public class StatusSO : ScriptableObject
    {
        [Header("스탯강화 타입")]public StatusType type;
        [Header("강화당 증가수치")]public int increasePerEnhance;
        [Header("해금 레벨")] public int unlockLevel;
        [Header("최대 레벨")] public int maxLevel;
    }
}