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
        Dictionary<(int,int), StageSO> stageDic;

        void MakeDictionary()
        {
            stageDic = new Dictionary<(int,int), StageSO>();
            foreach (var stage in stageList)
            {
                stageDic.Add((stage.chapter,stage.stage), stage);
            }
        }

        public StageSO GetSO(int chapter, int stage)
        {
            if (stageDic == null)
            {
                MakeDictionary();
                Debug.Log("딕셔너리를 생성했습니다. ");
            }

            if (!stageDic.TryGetValue((chapter,stage), out var stageSo))
            {
                Debug.LogWarning("키에 해당하는 스테이지가 없습니다. ");
                return null;
            }

            Debug.Log($"{stageSo.stageName}");
            return stageSo;
        }
    }
}