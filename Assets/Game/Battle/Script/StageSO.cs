using System;
using System.Collections.Generic;
using Battle;
using UnityEngine;

namespace Battle
{
    public enum StageType
    {
        Normal, Challenge, Boss, Locked
    }
    [Serializable]
    public struct MonsterPreset
    {
        public MonsterSO monster;
        [Header("몬스터 등장 비중")]public int weights;
    }
    [CreateAssetMenu(menuName = "Game/Battle/Stage")]
    public class StageSO : ScriptableObject
    {
        [Header("공용")]
        [Tooltip("스테이지 식별용 키")] public int stageKey; 
        [Tooltip("스테이지 이름")]public string stageName; 
        [Tooltip("스테이지")] public int stage;
        [Tooltip("챕터")] public int chapter;
        [Tooltip("드랍 테이블")] public DropTableSO dropTable;
        [Tooltip("몬스터 프리셋")] public List<MonsterPreset> preset;
        [Tooltip("스테이지 타입(일반 / 돌파 / 보스)")] public StageType type;

        [Header("스테이지 돌파 전용")] 
        [Tooltip("제한시간")] public int deadLine;
        [Tooltip("목표 처치 수")] public int targetKillScore;
    }
}