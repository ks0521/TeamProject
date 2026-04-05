using Battle;
using Growth.Skill;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Personal.HagYun
{
    public class SkillPool : MonoBehaviour
    {
        [SerializeField] private Skill[] allSkillObjPool;
        // 스킬 풀에 있는 스킬을 active/passive로 나눠서 담기
        [SerializeField] private Transform skillObjPoolTransform;
        private Dictionary<string, ActiveSkill> activeSkillObjPool;
        public Dictionary<string, ActiveSkill> ActiveSkillObjPool => activeSkillObjPool;
        private ActiveSkill[] activeSkillArr;
        public ActiveSkill[] ActiveSkillArr => activeSkillArr;
        private PassiveSkill[] passiveSkillArr;
        public PassiveSkill[] PassiveSkillArr => passiveSkillArr;
        private Dictionary<int, Skill> allSkillDic;
        private Dictionary<int, ActiveSkill> activeSkillDic;
        public Dictionary<int, ActiveSkill> ActiveSkillDic => activeSkillDic;
        private Dictionary<int, PassiveSkill> passiveSkillDic;
        public Dictionary<int, PassiveSkill> PassiveSkillDic => passiveSkillDic;
        public void Init()
        {
            SkillAddInit(allSkillObjPool);

        }
        /// <summary>
        /// Active Skill Pool에서 Active Skill을 찾을 때 사용하는 함수
        /// </summary>
        /// <param name="index">Active Skill Pool에 저장된 Active Skill의 index</param>
        /// <param name="aSkill">찾은 Active Skill</param>
        /// <returns>Active Skill을 찾았는지 여부 return</returns>
        public bool TryGetActiveSkill(int index, out ActiveSkill aSkill)
        {
            if (index < 0 || activeSkillArr.Length <= index)
            {
                aSkill = null;
                return false;
            }
            aSkill = activeSkillArr[index];
            return true;
        }
        /// <summary>
        /// Passive Skill Pool에서 Passive Skill을 찾을 때 사용하는 함수
        /// </summary>
        /// <param name="index">Passive Skill Pool에 저장된 Passive Skill의 index</param>
        /// <param name="pSkill">찾은 Passive Skill</param>
        /// <returns>Passive Skill을 찾았는지 여부 return</returns>
        public bool TryGetPassiveSkill(int index, out PassiveSkill pSkill)
        {
            if (index < 0 || passiveSkillArr.Length <= index)
            {
                pSkill = null;
                return false;
            }
            pSkill = passiveSkillArr[index];
            return true;
        }

        public bool TryGetSkillToKey(int key, out Skill skill)
        {
            skill = null;
            if (TryGetActiveSkillToKey(key, out ActiveSkill aSkill))
            {
                skill = aSkill;
                return true;
            }
            else if (TryGetPassiveSkillToKey(key, out PassiveSkill pSkill))
            {
                skill = pSkill;
                return true;
            }
            return false;
        }
        public bool TryGetActiveSkillToKey(int key, out ActiveSkill aSkill) => activeSkillDic.TryGetValue(key, out aSkill);
        public bool TryGetPassiveSkillToKey(int key, out PassiveSkill pSkill) => passiveSkillDic.TryGetValue(key, out pSkill);
        public void SkillAddInit(Skill[] skillArr)
        {
            int activeCnt = 0;
            int passiveCnt = 0;
            foreach (Skill s in skillArr)
            {
                if (s is ActiveSkill) activeCnt++;
                else if (s is PassiveSkill) passiveCnt++;
            }
            activeSkillArr = new ActiveSkill[activeCnt];
            activeSkillDic = new Dictionary<int, ActiveSkill>(activeCnt);
            passiveSkillArr = new PassiveSkill[passiveCnt];
            passiveSkillDic = new Dictionary<int, PassiveSkill>(passiveCnt);
            allSkillDic = new Dictionary<int, Skill>(activeCnt + passiveCnt);
            activeCnt = 0;
            passiveCnt = 0;
            foreach (Skill s in skillArr)
            {
                int key = s.SkillData.key;
                allSkillDic.Add(key, s);
                if (s is ActiveSkill aSkill)
                {
                    activeSkillArr[activeCnt++] = aSkill;
                    activeSkillDic.Add(key, aSkill);
                }
                else if (s is PassiveSkill pSkill)
                {
                    passiveSkillArr[passiveCnt++] = pSkill;
                    passiveSkillDic.Add(key, pSkill);
                }
            }

            SkillArrQSort(activeSkillArr, activeSkillArr.Length);
            SkillArrQSort(passiveSkillArr, passiveSkillArr.Length);
        }
        void SkillArrQSort(Skill[] arr, int length) => ArrSorter<Skill>.StartArrSort(arr, length, (a, b) => a.SkillData.key.CompareTo(b.SkillData.key));

    }
}