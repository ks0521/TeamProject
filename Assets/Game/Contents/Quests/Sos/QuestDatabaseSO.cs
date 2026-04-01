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
        private Dictionary<int, QuestDataReader> _questDic = new Dictionary<int, QuestDataReader>();

        public void LoadFromJson(string jsonText)
        {
            QuestDataWrapper wrapper = JsonUtility.FromJson<QuestDataWrapper>(jsonText);
            allQuests = wrapper.quests;
            _questDic.Clear();
            foreach (var entry in wrapper.quests)
            {
                if (!_questDic.ContainsKey(entry.questID))
                    _questDic.Add(entry.questID, entry);
            }
            Debug.Log($"[QuestDatabase] {allQuests.Count}개의 퀘스트 로드 완료.");
        }

        public List<QuestDataReader> GetAllQuests() => _questDic.Values.ToList();

        public QuestDataReader GetQuestByID(int id)
        {
            _questDic.TryGetValue(id, out var quest);
            return quest;
        }
    }
}
