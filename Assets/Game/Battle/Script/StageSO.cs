using System;
using System.Collections.Generic;
using Battle;
using UnityEngine;
using UnityEngine.Serialization;

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
    //몬스터 생성 타입(무한, 보스단일, 웨이브 식)
    public enum SpawnType
    {
        Endless, Boss, Wave
    }
    //클리어 조건(특정 마릿수, 보스처치, 생존)
    public enum ClearType
    {
        None,KillCount, BossKill, Survival
    }
    //보상 방식(아이템 필드 드랍, 클리어시 한번에)
    public enum RewardType
    {
        ItemDrop, ClearReward
    }
    [CreateAssetMenu(menuName = "Game/Battle/Stage")]
    public class StageSO : ScriptableObject
    {
        [Header("공용")]
        [Tooltip("스테이지 식별용 키")] public int stageKey; 
        [Tooltip("스테이지 이름")][TextArea(2,5)]public string stageName; 
        [Tooltip("챕터")] public int chapter;
        [Tooltip("스테이지")] public int stage;
        [Tooltip("드랍 테이블")] public DropTableSO dropTable; //일반스테이지
        [Tooltip("보상 테이블")] public RewardTableSO rewardTable; //도전 스테이지
        [Tooltip("몬스터 프리셋")] public List<MonsterPreset> preset;
        [Tooltip("스테이지 타입(일반 / 돌파 / 보스)")] public StageType stageType;
        [Tooltip("스폰 형식")]public SpawnType spawnType;
        [Tooltip("클리어 조건")]public ClearType clearType;
        [Tooltip("보상 방식")]public RewardType rewardType;
        [Header("스테이지 돌파 전용")] 
        [Tooltip("제한시간")] public float deadLine;
        [Tooltip("목표 처치 수")] public int targetKillScore;
    }
}