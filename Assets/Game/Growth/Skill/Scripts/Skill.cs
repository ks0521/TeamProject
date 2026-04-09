using Base.Data;
using Base.Managers;
using Battle;
using System;
using UnityEngine;

namespace Growth.Skill
{
    public abstract class Skill
    {
        public abstract SkillSO SkillData { get; }
        [SerializeField] protected Character owner;
        public Character Owner => owner;
        [SerializeField] protected int curLv;
        public int CurLv => curLv;
        public int MaxLv => SkillData.maxLv;
        public bool IsSkillLock => SkillData.openPlayerLv < skillMgr.PlayerLevel;
        private SkillManager skillMgr;
        private EventHub eventHub;
        public virtual void Init(Character owner)
        {
            if (owner != null && this.owner == null) this.owner = owner;

            skillMgr = GameManager.Instance.GetGameSystem<SkillManager>();
            eventHub = GameManager.Instance.GetGameSystem<EventHub>();
            int maxLv = SkillData.maxLv;
            curLv = skillMgr.GetSkillLevel(SkillData.key);

            if (curLv < 0) curLv = 0;
            else if (maxLv < curLv) curLv = maxLv;

            StatUpdate();
        }
        public abstract void StatUpdate();
        public bool TryLvUp(int curSkillPoint, int addLv)
        {
            int maxLv = MaxLv;
            // 현재 스킬 포인트가 0 이하일 때
            // 현재 스킬 레벨이 최대레벨 이상일 때
            if (curSkillPoint <= 0 || maxLv <= curLv)
            {
                return false;
            }
            // curSkillPoint가 작다면 curSkillPoint로, 아니라면 addLv로 lv up
            addLv = Math.Min(curSkillPoint, addLv);
            // maxLv - curLv : 레벨업 최대 횟수
            // addLv은 레벨업 횟수
            // addLv은 maxLv - curLv 과 addLv 중 작은 쪽으로 lvUp
            int lvUpCnt = Math.Min(addLv, maxLv - curLv);
            curLv += lvUpCnt;
            StatUpdate();
            // OnSkillLvEnhance?.Invoke(SkillData.key, lvUpCnt);
            
            return true;
        }
        public bool TryLevelOneUp(int curSkillPoint) => TryLvUp(curSkillPoint, 1);
        public bool TryLevelMaxUp(int curSkillPoint) => TryLvUp(curSkillPoint, SkillData.maxLv);
        public bool TryLvUp(int curSkillPoint, int addLv, out int lvUpCnt)
        {
            int maxLv = MaxLv;
            // 현재 스킬 포인트가 0 이하일 때
            // 현재 스킬 레벨이 최대레벨 이상일 때
            if (curSkillPoint <= 0 || maxLv <= curLv)
            {
                lvUpCnt= 0;
                return false;
            }
            // curSkillPoint가 작다면 curSkillPoint로, 아니라면 addLv로 lv up
            addLv = Math.Min(curSkillPoint, addLv);
            // maxLv - curLv : 레벨업 최대 횟수
            // addLv은 레벨업 횟수
            // addLv은 maxLv - curLv 과 addLv 중 작은 쪽으로 lvUp
            lvUpCnt = Math.Min(addLv, maxLv - curLv);
            curLv += lvUpCnt;
            StatUpdate();
            // OnSkillLvEnhance?.Invoke(SkillData.key, lvUpCnt);
            return true;
        }
        public bool TryLevelOneUp(int curSkillPoint, out int lvUpCnt) => TryLvUp(curSkillPoint, 1, out lvUpCnt);
        public bool TryLevelMaxUp(int curSkillPoint, out int lvUpCnt) => TryLvUp(curSkillPoint, SkillData.maxLv, out lvUpCnt);
        public bool TryLevelReset(out int lvResetCnt)
        {
            if (curLv <= 0)
            {
                lvResetCnt = 0;
                return false;
            }
            lvResetCnt = curLv;
            curLv = 0;
            StatUpdate();
            return true;
        }
    }
}