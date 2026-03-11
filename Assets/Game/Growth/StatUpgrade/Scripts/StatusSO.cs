using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Growth.StatUpgrade
{
    //실제 SO 내 순서맞춰야 합니다!
    public enum StatusType
    {
        Atk,MaxHp,Def,AtkSpeed,CritChance,CritDmg,MoveSpeed, GoldRate, ExpRate, ItemDropRate
    }

    [Serializable]
    public struct StatEntry
    {
        [Tooltip("스탯강화 타입")]public StatusType type;
        [Tooltip("강화당 증가수치")]public float increasePerEnhance;
        [Tooltip("해금 레벨")] public int unlockLevel;
        [Tooltip("레벨당 강화 비용")] public int enhanceCost;
        [Tooltip("최대 레벨")] public int maxLevel;
    }
    [CreateAssetMenu(menuName = "Game/Growth/Status")]
    public class StatusSO : ScriptableObject
    {
        public List<StatEntry> stats; //스탯 정보 모아놓은 리스트
        Dictionary<StatusType, StatEntry> statDic;
        void MakeDictionary()
        {
            statDic = new Dictionary<StatusType, StatEntry>();
            foreach (var stat in stats)
            {
                statDic.Add(stat.type, stat);
            }
        }
        
        public bool TryGetStatEntry(StatusType key, out StatEntry stat)
        {
            if (statDic == null)
            {
                MakeDictionary();
                Debug.Log("딕셔너리를 생성했습니다. ");
            }

            if (!statDic.TryGetValue(key, out stat))
            {
                Debug.LogWarning("키에 해당하는 스탯이 없습니다. ");
                return false;
            }

            Debug.Log($"{stat.type}");
            return true;
        }
    }
}