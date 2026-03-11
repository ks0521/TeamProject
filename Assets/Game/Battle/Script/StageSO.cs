using System;
using System.Collections.Generic;
using Battle;
using UnityEngine;

namespace Battle
{
    [Serializable]
    public struct MonsterPreset
    {
        public MonsterSO monster;
        [Header("몬스터 등장 비중")]public int weights;
    }
    [CreateAssetMenu(menuName = "Game/Battle/Stage")]
    public class StageSO : ScriptableObject
    {
        [Header("스테이지 이름")]public string stageName;
        [Header("스테이지 식별용 키")] public int stageKey;
        [Header("스테이지")] public int stage;
        [Header("챕터")] public int chapter;
        [Header("스테이지 타입(일반 / 돌파)")] public bool isChallengeStage;

        [Header("스테이지 돌파 전용")] 
        [Header("보스전투 여부")] public bool isBossStage;
        [Header("제한시간")] public int deadLine;
        [Header("목표 처치 수")] public int targetKillScore;
        [Header("몬스터 프리셋")] public List<MonsterPreset> preset;
    }
}