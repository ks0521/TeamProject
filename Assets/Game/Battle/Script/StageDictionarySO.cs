using System.Collections;
using System.Collections.Generic;
using Growth.Skill;
using UnityEngine;

namespace Battle
{
    
    [CreateAssetMenu(menuName = "Game/Battle/StageDictionary")]
    public class StageDictionarySO : ScriptableObject
    {
        [Tooltip("여기에 스테이지 다 넣기")]public List<StageSO> stageList;
        // 챕터와 스테이지 2개를 키로 설정
        Dictionary<(int,int,StageType), StageSO> stageDic;

        void MakeDictionary()
        {
            stageDic = new Dictionary<(int,int,StageType), StageSO>();
            foreach (var stage in stageList)
            {
                stageDic.Add((stage.chapter,stage.stage,stage.type), stage);
            }
        }
        /// <summary> 특정 챕터의 스테이지 정보를 불러오는 메서드</summary>
        /// <param name="chapter">찾으려는 챕터</param>
        /// <param name="stage">찾으려는 스테이지</param>
        /// <param name="stageType">찾으려는 스테이지의 전투정보</param>
        /// <returns>해당 챕터 스테이지의 StageSO</returns>
        public StageSO GetSO(int chapter, int stage, StageType stageType)
        {
            if (stageDic == null)
            {
                MakeDictionary();
                Debug.Log("딕셔너리를 생성했습니다. ");
            }

            if (!stageDic.TryGetValue((chapter,stage, stageType), out var stageSo))
            {
                Debug.LogWarning("키에 해당하는 스테이지가 없습니다. ");
                return null;
            }

            Debug.Log($"{stageSo.stageName}");
            return stageSo;
        }
    }
}