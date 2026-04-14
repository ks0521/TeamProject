using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace QuestSystem
{
    [CreateAssetMenu(fileName = "QuestDatabase", menuName = "Quest/Database")]
    public class QuestDatabaseSO : ScriptableObject
    {
        public List<QuestDataReader> allQuests = new List<QuestDataReader>();
        private Dictionary<int, QuestDataReader> questDic;

        //퀘스트 딕셔너리 초기화
        void InitQuestDictionary()
        {
            if (questDic != null && questDic.Count == allQuests.Count) return;

            questDic = new Dictionary<int, QuestDataReader>();
            foreach(var quest in allQuests)
            {
                if(quest != null && !questDic.ContainsKey(quest.questID))
                {
                    questDic.Add(quest.questID, quest);
                }
            }
        }

        public List<QuestDataReader> GetAllQuests() => allQuests;

        public QuestDataReader GetQuestByID(int id)
        {
            InitQuestDictionary();

            if (questDic.TryGetValue(id, out var quest)) return quest;

            Debug.LogWarning($"[QuestDatabase] ID {id}번에 해당하는 퀘스트를 찾을 수 없습니다.");
            return null;
        }
    }
}

