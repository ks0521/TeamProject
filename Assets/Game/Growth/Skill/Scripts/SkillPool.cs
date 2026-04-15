using Base.Utils;
using Base.Managers;
using Battle;
using System.Collections.Generic;
using UnityEngine;
using Base.Data;

namespace Growth.Skill
{
    public class SkillPool : MonoBehaviour
    {
        private SkillManager skillMgr;
        [SerializeField] private SkillSO[] allSkillData;
        public Skill[] AllSkillArr { get; private set; }
        private Dictionary<int, ActiveSkill> activeSkillDic;
        public int ActiveSkillCnt => activeSkillDic.Count;
        private Dictionary<int, PassiveSkill> passiveSkillDic;
        public int PassiveSkillCnt => passiveSkillDic.Count;
        public void Init(SkillManager skillMgr)
        {
            this.skillMgr = skillMgr;
            var allSkillArr = skillMgr.AllSkillSOList;
            int cnt = allSkillData.Length;
            AllSkillArr = new Skill[cnt];
            Player pl = GameManager.Instance.GetGameSystem<PlayerManager>().Player;
            for (int i = 0; i < allSkillArr.Count; i++)
            {
                var so = allSkillArr[i];
                PlayerSkillLevelProvider lvProvider = new PlayerSkillLevelProvider(skillMgr, so.key);
                if (so is ActiveSkillSO aso)
                {
                    var aSkill = new ActiveSkill(pl, aso, lvProvider);
                    AllSkillArr[i] = aSkill;
                }
                else if (so is PassiveSkillSO pso)
                {
                    var pSkill = new PassiveSkill(pl, pso, lvProvider);
                    AllSkillArr[i] = pSkill;
                }
            }
            ArrSorter<Skill>.ArrSortStart(AllSkillArr, (a, b) => a.SkillData.key.CompareTo(b.SkillData.key));
            SkillAddInit(AllSkillArr);
        }
        /// <summary>
        /// Active Skill Dic에서 Active Skill을 찾을 때 사용하는 함수
        /// </summary>
        /// <param name="key">Active Skill Dic에 저장된 Active Skill의 key</param>
        /// <param name="aSkill">찾은 Active Skill</param>
        /// <returns>Active Skill을 찾았는지 여부 return</returns>
        public bool TryGetActiveSkillByKey(int key, out ActiveSkill aSkill) => activeSkillDic.TryGetValue(key, out aSkill);
        /// <summary>
        /// Passive Skill Dic에서 Passive Skill을 찾을 때 사용하는 함수
        /// </summary>
        /// <param name="key">Passive Skill Dic에 저장된 Passive Skill의 key</param>
        /// <param name="pSkill">찾은 Passive Skill</param>
        /// <returns>Passive Skill을 찾았는지 여부 return</returns>
        public bool TryGetPassiveSkillByKey(int key, out PassiveSkill pSkill) => passiveSkillDic.TryGetValue(key, out pSkill);
        /// <summary>
        /// Active + Passive Dic에서 Skill을 찾을 때 사용하는 함수, Skill 정보만 찾고자 할 때 사용
        /// </summary>
        /// <param name="key">Active + Passive Skill Dic에 저장된 Skill의 key</param>
        /// <param name="pSkill">찾은 Skill</param>
        /// <returns>Skill을 찾았는지 여부 return</returns>
        public bool TryGetSkillByKey(int key, out Skill skill)
        {
            skill = null;
            if (TryGetActiveSkillByKey(key, out ActiveSkill aSkill))
            {
                skill = aSkill;
                return true;
            }
            else if (TryGetPassiveSkillByKey(key, out PassiveSkill pSkill))
            {
                skill = pSkill;
                return true;
            }
            return false;
        }
        public void SkillAddInit(Skill[] skillArr)
        {
            AllSkillArr = skillArr;
            int activeCnt = 0;
            int passiveCnt = 0;
            foreach (Skill s in skillArr)
            {
                if (s is ActiveSkill) activeCnt++;
                else if (s is PassiveSkill) passiveCnt++;
            }
            activeSkillDic = new Dictionary<int, ActiveSkill>(activeCnt);
            passiveSkillDic = new Dictionary<int, PassiveSkill>(passiveCnt);
            foreach (Skill s in skillArr)
            {
                int key = s.SkillData.key;
                if (s is ActiveSkill aSkill)
                {
                    activeSkillDic.Add(key, aSkill);
                }
                else if (s is PassiveSkill pSkill)
                {
                    passiveSkillDic.Add(key, pSkill);
                }
            }
        }
    }
}