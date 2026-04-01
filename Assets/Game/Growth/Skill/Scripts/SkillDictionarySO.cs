using System.Collections.Generic;
using Growth.Skill;
using UnityEngine;

namespace Base.Data
{
    [CreateAssetMenu(menuName = "Game/Growth/SkillDictionary")]
    public class SkillDictionarySO : ScriptableObject
    {
        [Header("스킬 전체")]
        [SerializeField] private List<SkillSO> allSkills = new();
        Dictionary<int, SkillSO> skillDic;

        void MakeDictionary()
        {
            if (skillDic != null) return;
            skillDic = new Dictionary<int, SkillSO>();
            foreach (var skill in allSkills)
            {
                skillDic.Add(skill.key, skill);
            }
            Debug.Log("딕셔너리를 생성했습니다. ");
        }

        public SkillSO[] GetAll() => allSkills.ToArray();
        
        public SkillSO GetSO(int key)
        {
            MakeDictionary();
            if (!skillDic.TryGetValue(key, out var skill))
            {
                Debug.LogWarning("키에 해당하는 아이템이 없습니다. ");
                return null;
            }

            Debug.Log($"{skill.name}");
            return skill;
        }
    }
}