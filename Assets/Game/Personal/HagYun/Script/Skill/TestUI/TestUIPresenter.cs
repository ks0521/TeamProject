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
    public class TestUIPresenter : MonoBehaviour
    {
        // test
        public static TestUIPresenter ins;
        [SerializeField] TestBtnView[] btnViewArr;
        // public EquipSkillController es;

        EventHub eventHub;
        EquipSkillController plEquipSkillController;
        EquipSkill[] plEquipSkill;
        Player pl;
        void Awake()
        {
            ins = this;
        }
        public void Init()
        {
            eventHub = GameManager.Instance.GetGameSystem<EventHub>();
            pl = GameManager.Instance.GetGameSystem<PlayerManager>().Player;
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
                btnViewArr[index].ButtonEventSet(() => plEquipSkillController.TryAtkSkillUseToMonster(index));
                CooltimeCheckTask(index).Forget();
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
            if (!plEquipSkill[index].IsCooltime) return;
            float cooltimeValue = plEquipSkill[index].CurCooltime / plEquipSkill[index].MaxCooltime;
            btnViewArr[index].CooltimeShowUpdate(cooltimeValue);
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
    }
}