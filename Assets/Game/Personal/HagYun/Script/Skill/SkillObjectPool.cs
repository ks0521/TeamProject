using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Personal.HagYun
{
    public class SkillObjectPool
    {
        private SkillPool pool;
        public Dictionary<int, ActiveSkillObject> ActiveSkillObjDic { get; private set; }
        public void Init(SkillPool pool)
        {
            this.pool = pool;
            ActiveSkillObjDic = new Dictionary<int, ActiveSkillObject>(pool.ActiveSkillCnt);
        }
        public ActiveSkillObject GetActiveSkill(ActiveSkill aSkill)
        {
            var so = aSkill.ActiveSkillData;
            int key = so.key;
            if (ActiveSkillObjDic.TryGetValue(key, out var obj))
            {
                Debug.Log($"{aSkill.SkillData.name} obj 반출");
                return obj;
            }
            Debug.Log($"{aSkill.SkillData.name} obj 생성");
            var insObj = Object.Instantiate(so.skillObj, pool.transform);
            insObj.Init(aSkill);
            ActiveSkillObjDic.Add(key, insObj);
            return insObj;
        }
    }
}