using Base.Data;
using Base.Managers;
using Battle;
using Cysharp.Threading.Tasks;
using Growth.Skill;
using Personal.HagYun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Personal.HagYun
{
    public class SkillButtonPresenter : MonoBehaviour
    {
        [SerializeField] private SkillButtonView[] btnViewArr;
        // public EquipSkillController es;

        private EventHub eventHub;
        private EquipSkillController plEquipSkillController;
        private EquipSkill[] plEquipSkill;
        private Player pl;
        public void Init()
        {
            eventHub = GameManager.Instance.GetGameSystem<EventHub>();
            if (eventHub == null)
            {
                Debug.LogWarning("TestUIPresenter에서 eventHub 찾지 못함");
                return;
            }
            pl = GameManager.Instance.GetGameSystem<PlayerManager>().Player;
            if (pl == null)
            {
                Debug.LogWarning("TestUIPresenter에서 Player 찾지 못함");
                return;
            }
            plEquipSkillController = pl.ESController;
            plEquipSkill = plEquipSkillController.EquipSkillArr;
            for (int i = 0; i < 6; i++)
            {
                if (plEquipSkill[i] == null)
                {
                    Debug.LogWarning($"{i}번째 EquipSkill 없음");
                    continue;
                }
                int index = i;
                SkillButtonView tBtnView = btnViewArr[index];
                if (btnViewArr[index] == null)
                {
                    Debug.LogWarning($"{index}번 TestBtnView 연결 안 됨");
                    continue;
                }
                tBtnView.ButtonEventSet(() => plEquipSkillController.TryAtkSkillUseToMonster(index));
                CooltimeCheckTask(index).Forget();
                SkillSO skillData = plEquipSkill[index].Skill.Data;
                tBtnView.SkillIconImageChange(skillData.skillIcon, skillData.Targeting == TargetingMode.Homing);
            }
            EquipSkillEventRemove();
            EquipSkillEventSet();
        }
        async UniTaskVoid CooltimeCheckTask(int index)
        {
            while (true)
            {
                CooltimeUpdate(index);
                await UniTask.DelayFrame(10);
                if (pl.IsDead) break;
            }
        }
        void CooltimeUpdate(int index)
        {
            EquipSkill tESkill = plEquipSkill[index];
            SkillButtonView tbv = btnViewArr[index];
            if (tESkill == null || tbv == null) return;
            else if (!tESkill.IsCooltime)
            {
                if (tbv.IsCooltimeMaskActiveState) tbv.CooltimeEnd();
                return;
            }
            if (!tbv.IsCooltimeMaskActiveState) tbv.CooltimeStart();
            float cooltimeValue = tESkill.CurCooltime / tESkill.MaxCooltime;
            tbv.CooltimeShowUpdate(cooltimeValue);

        }
        void BtnCooltimeStartEvent(int index) => btnViewArr[index].CooltimeStart();
        void BtnCooltimeEndEvent(int index) => btnViewArr[index].CooltimeEnd();
        void SkillIconSet(int index)
        {
            SkillSO tSkillData = plEquipSkill[index].Skill.Data;
            btnViewArr[index].SkillIconImageChange(tSkillData.skillIcon, tSkillData.Targeting == TargetingMode.Homing);
        }
        void SkillIconUnset(int index) => btnViewArr[index].SkillIconImageChange(null, false);
        void EquipSkillEventSet()
        {
            eventHub.OnSkillUsed += BtnCooltimeStartEvent;
            eventHub.OnSkillCoolEnd += BtnCooltimeEndEvent;

            eventHub.OnSkillSet += SkillIconSet;
            eventHub.OnSkillUnset += SkillIconUnset;
        }
        void EquipSkillEventRemove()
        {
            eventHub.OnSkillUsed -= BtnCooltimeStartEvent;
            eventHub.OnSkillCoolEnd -= BtnCooltimeEndEvent;

            eventHub.OnSkillSet -= SkillIconSet;
            eventHub.OnSkillUnset -= SkillIconUnset;
        }

        public int GetOrder() => 100;
    }
}