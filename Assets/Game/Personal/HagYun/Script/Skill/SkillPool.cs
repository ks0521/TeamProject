using Growth.Skill;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Personal.HagYun
{
    public class SkillPool : MonoBehaviour
    {
        [SerializeField] public Skill[] allSkillObjPool;
        // 스킬 풀에 있는 스킬
        // [SerializeField] public List<ActiveSkill> activeSkillList;
        // [SerializeField] public List<PassiveSkill> passiveSkillList;
        [SerializeField] public List<ActiveSkill> activeSkillList;
        [SerializeField] public List<PassiveSkill> passiveSkillList;
        public void Init()
        {
            SkillAddInit(allSkillObjPool);

        }
        /// <summary>
        /// Player Equip Skill Controller 에서 스킬 장착시킬 때 사용할 함수
        /// </summary>
        /// <param name="index">skill 풀에 저장된 skill의 index (추후 key값으로 교체 예정)</param>
        /// <param name="getSkill">EquipSkill에 장착시킬 대상 스킬</param>
        /// <returns>장착시킬 스킬을 찾았는지 여부 return</returns>
        public bool TryGetActiveSkill(int index, out ActiveSkill getSkill)
        {
            if (index < 0 || activeSkillList.Count <= index)
            {
                getSkill = null;
                return false;
            }
            getSkill = activeSkillList[index];
            return true;
        }
        public bool TryGetPassiveSkill(int index, out PassiveSkill getSkill)
        {
            if (index < 0 || passiveSkillList.Count <= index)
            {
                getSkill = null;
                return false;
            }
            getSkill = passiveSkillList[index];
            return true;
        }
        public bool TryGetActiveSkillToKey(int key, out ActiveSkill getSkill)
        {
            key -= 1001;
            if (key < 0 || activeSkillList.Count <= key)
            {
                getSkill = null;
                return false;
            }
            getSkill = activeSkillList[key];
            return true;
        }
        public bool TryGetPassiveSkillToKey(int key, out PassiveSkill getSkill)
        {
            key -= 1501;
            if (key < 0 || passiveSkillList.Count <= key)
            {
                getSkill = null;
                return false;
            }
            getSkill = passiveSkillList[key];
            return true;
        }
        public void ActiveSkillAdd(ActiveSkill aSkill)
        {
            activeSkillList.Add(aSkill);
            activeSkillList.Sort(CheckSortNum);
        }
        public void SkillAddInit(Skill[] skillArr)
        {
            for (int i = 0; i < skillArr.Length; i++)
            {
                if (skillArr[i] is ActiveSkill aSkill)
                    activeSkillList.Add(aSkill);
                else if (skillArr[i] is PassiveSkill pSkill)
                    passiveSkillList.Add(pSkill);
            }
            activeSkillList.Sort(CheckSortNum);
            passiveSkillList.Sort(CheckSortNum);
        }

        int CheckSortNum(Skill a, Skill b)
        {
            int aKey = a.SkillData.key;
            int bKey = b.SkillData.key;
            int result = 0;
            if (aKey > bKey) result = 1;
            else if (aKey < bKey) result = -1;
            return result;
        }
        int CheckSortNumReverse(Skill a, Skill b)
        {
            int aKey = a.SkillData.key;
            int bKey = b.SkillData.key;
            int result = 0;
            if (aKey > bKey) result = -1;
            else if (aKey < bKey) result = 1;
            return result;
        }
        void SkillArrQSort(Skill[] arr, int left, int right)
        {
            var pivot = arr[left];
            var low = arr[left + 1];
            var high = arr[right];


        }
    }
}