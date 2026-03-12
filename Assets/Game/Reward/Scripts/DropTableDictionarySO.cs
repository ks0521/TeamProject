using System.Collections.Generic;
using UnityEngine;

namespace Reward
{
    [CreateAssetMenu(menuName = "Game/Reward/DropTableDictionary")]
    public class DropTableDictionarySO : ScriptableObject
    {
        [Tooltip("여기에 드랍테이블 다 넣기")] public List<DropTableSO> dropTableList;
        // 챕터와 스테이지 2개를 키로 설정
        Dictionary<(int,int), DropTableSO> dropTableDic;

        void MakeDictionary()
        {
            dropTableDic = new Dictionary<(int,int), DropTableSO>();
            foreach (var dropTable in dropTableList)
            {
                dropTableDic.Add((dropTable.chapter,dropTable.stage), dropTable);
            }
        }

        public DropTableSO GetSO(int chapter, int stage)
        {
            if (dropTableDic == null)
            {
                MakeDictionary();
                Debug.Log("딕셔너리를 생성했습니다. ");
            }

            if (!dropTableDic.TryGetValue((chapter,stage), out var dropTable))
            {
                Debug.LogWarning("키에 해당하는 스테이지가 없습니다. ");
                return null;
            }

            Debug.Log($"{dropTable.name}");
            return dropTable;
        }
    }
}