using Base.Data;
using Growth.Equipment;
using Growth.Skill;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Personal.HagYun
{
    public class SkillDetailPresenter : MonoBehaviour, IMemberReceiver
    {
        private PlayerSkillUIManager owner;
        // private SkillManager skillMgr;
        private SkillPool pool;
        private EventHub hub;
        public int SelectedSkillKey { get; private set; }
        [SerializeField] private SkillDetailView skillDetailView;
        public void Init(UIPresenterInitData data)
        {
            owner = data.owner;
            pool = data.pool;
            hub = data.hub;
            // skillMgr = owner.SkillMgr;
            // var firstSkill = skillMgr.GetAllSkillInfo()[0];
            // SkillDetailDataSetToSkillChange(firstSkill.so.key);
            if (pool.AllSkillArr[0] is var skill) SkillDetailDataSetToSkillChangeByPool(skill.SkillData.key);
        }
        public void SkillLevelUpBtnInteractable(bool isLevelUpBtnOn)
        {
            skillDetailView.SkillLevelUpBtnInteractable(isLevelUpBtnOn);
        }
        public void BtnEventAddListner(Action lvOneUpFunc, Action lvMaxUpFunc, Action equipFunc)
        {
            skillDetailView.BtnEventAddListner(lvOneUpFunc, lvMaxUpFunc, equipFunc);
        }
        public void OnDestroyFeat()
        {
            BtnEventRemoveListner();
        }
        void BtnEventRemoveListner() => skillDetailView.BtnEventRemoveAllListner();

        public void SkillEquipPopupShow()
        {
            // skillEquipChangePopupView.EquipSkillShow(equipTargetSkill, activeSkillArr);
            // skillEquipChangePopupView.gameObject.SetActive(true);
        }
        public void SkillDetailDataSetToSkillChangeByPool(int key)
        {
            gameObject.SetActive(true);
            SelectedSkillKey = key;
            if (!pool.TryGetSkillByKey(key, out var skill)) return;
            int curLv = skill.CurLv;
            var so = skill.SkillData;
            string value = null;
            SkillDetailViewNeedsNameAndImage niData = new();
            SkillDetailViewNeedsStatData data = new();
            if (so is ActiveSkillSO aSO)
            {
                ActiveSkillValueTextSet(aSO.ResultDamage(curLv), out value);

                niData = new SkillDetailViewNeedsNameAndImage(aSO);
                data = new SkillDetailViewNeedsStatData(aSO, value, curLv);
            }
            else if (so is PassiveSkillSO pSO)
            {
                PassiveSkillValueTextSet(pSO.ResultAddStat(curLv), out value);

                niData = new SkillDetailViewNeedsNameAndImage(pSO);
                data = new SkillDetailViewNeedsStatData(pSO, value, curLv);
            }
            skillDetailView.SkillDetailViewSetToSkillChange(niData, data);
        }
        public void SkillDetailDataSetToSkillLvEnhanceByPool(int key)
        {
            if (!pool.TryGetSkillByKey(key, out var skill)) return;
            if (skill == null) return;
            string value = null;
            SkillDetailViewNeedsStatData statData = new();
            int curLv = skill.CurLv;
            var so = skill.SkillData;
            if (so is ActiveSkillSO aSO)
            {
                ActiveSkillValueTextSet(aSO.ResultDamage(curLv), out value);
                statData = new SkillDetailViewNeedsStatData(aSO, value, curLv);

            }
            else if (so is PassiveSkillSO pSO)
            {
                PassiveSkillValueTextSet(pSO.ResultAddStat(skill.CurLv), out value);
                statData = new SkillDetailViewNeedsStatData(pSO, value, curLv);
            }
            skillDetailView.SkillDetailViewSetToLvEnhance(statData);

        }
        // public void SkillLevelChangeUpdate(int key)
        // {
        //     SkillDetailDataSetToSkillLvEnhance(key);
        // }
        // public void SkillLevelUp(int key)
        // {
        //     SkillLevelChangeUpdate(key);
        // }
        // public void SkillLevelUpMax(int key)
        // {
        //     SkillLevelChangeUpdate(key);
        // }
        // public void SkillDetailDataSetToSkillChange(int key)
        // {
        //     gameObject.SetActive(true);
        //     SelectedSkillKey = key;
        //     var skill = skillMgr.GetSkill(key);
        //     int curLv = skillMgr.GetSkillLevel(key);
        //     string value = null;
        //     SkillDetailViewNeedsNameAndImage niData = new();
        //     SkillDetailViewNeedsStatData data = new();
        //     if (skill is ActiveSkillSO aSO)
        //     {
        //         ActiveSkillValueTextSet(aSO.ResultDamage(curLv), out value);

        //         niData = new SkillDetailViewNeedsNameAndImage(aSO);
        //         data = new SkillDetailViewNeedsStatData(aSO, value, curLv);
        //     }
        //     else if (skill is PassiveSkillSO pSO)
        //     {
        //         PassiveSkillValueTextSet(pSO.ResultAddStat(curLv), out value);

        //         niData = new SkillDetailViewNeedsNameAndImage(pSO);
        //         data = new SkillDetailViewNeedsStatData(pSO, value, curLv);
        //     }
        //     skillDetailView.SkillDetailViewSetToSkillChange(niData, data);
        // }
        // public void SkillDetailDataSetToSkillLvEnhance(int key)
        // {
        //     // var skill = skillMgr.GetSkill(key);
        //     // int curLv = skillMgr.GetSkillLevel(key);
        //     var skill = skillMgr.GetSkill(key);
        //     int curLv = skillMgr.GetSkillLevel(key);
        //     string value = null;
        //     SkillDetailViewNeedsStatData statData = new();
        //     if (skill is ActiveSkillSO aSO)
        //     {
        //         ActiveSkillValueTextSet(aSO.ResultDamage(curLv), out value);
        //         statData = new SkillDetailViewNeedsStatData(aSO, value, curLv);

        //     }
        //     else if (skill is PassiveSkillSO pSO)
        //     {
        //         PassiveSkillValueTextSet(pSO.ResultAddStat(curLv), out value);
        //         statData = new SkillDetailViewNeedsStatData(pSO, value, curLv);
        //     }
        //     skillDetailView.SkillDetailViewSetToLvEnhance(statData);
        // }
        // public void SkillDetailDataSetToSkillLvEnhance() => SkillDetailDataSetToSkillLvEnhance(SelectedSkillKey);
        StringBuilder sb = new StringBuilder();
        public void SkillStatValueTextBuild(string contents, bool isEnter)
        {
            if (isEnter) sb.Append($"\n{contents}");
            else sb.Append(contents);
        }
        void ActiveSkillValueTextSet(float resultDamage, out string valueText)
        {
            sb.Clear();
            sb.Append("배율 : ");
            SkillStatValueTextBuild($"{resultDamage * 100}%", false);
            valueText = sb.ToString();
        }
        void PassiveSkillValueTextSet(StatIncrease stat, out string valueText)
        {
            sb.Clear();
            sb.AppendLine("증가 스탯");
            MemberExtractor<StatIncrease>.ExtractAll(stat, this);
            valueText = sb.ToString();
        }
        public void Receive(string name, int value) { }
        public void Receive(string name, float value)
        {
            if (value == 0) return;
            string contents = PassiveStatValueToString(name, value);
            if (!string.IsNullOrEmpty(contents)) SkillStatValueTextBuild(contents, true);
        }
        public void Receive(string name, string value) { }
        public void ReceiveOther(string name, object value) { }
        string PassiveStatValueToString(string name, float value)
        {
            switch (name)
            {
                // 공격력 증가(상수)
                case nameof(StatIncrease.atk):
                    return $"공격력 + {value}";
                // 공격력 % 증가
                case nameof(StatIncrease.atkRate):
                    return ($"공격력 + {value * 100}%");
                // 피해량 증가
                case nameof(StatIncrease.damageDealtRate):
                    return ($"피해량 + {value * 100}%");
                // 치명타 확률 증가
                case nameof(StatIncrease.critChance):
                    return ($"치명타확률 + {value * 100}%");
                // 치명타 피해량 증가
                case nameof(StatIncrease.critDamage):
                    return ($"치명타피해량 + {value * 100}%");
                // HP 증가(상수)
                case nameof(StatIncrease.maxHp):
                    return ($"최대체력 + {value}");
                // HP % 증가
                case nameof(StatIncrease.maxHpRate):
                    return ($"최대체력 + {value * 100}%");
                // 방어력 비율 감소
                case nameof(StatIncrease.def):
                    return ($"방어력 + {value * 100}%");
                // 받는 피해 비율 감소
                case nameof(StatIncrease.damageReduction):
                    return ($"피해감소율 + {value * 100}%");
                // 이동속도 증가
                case nameof(StatIncrease.moveSpeed):
                    return ($"이동속도 + {value * 100}%");
                // 공격속도 증가
                case nameof(StatIncrease.atkSpeed):
                    return ($"공격속도 + {value * 100}%");
                // 아이템 드랍률 증가
                case nameof(StatIncrease.itemDropRate):
                    return ($"아이템드랍률 + {value * 100}%");
                // 골드 획득량 증가
                case nameof(StatIncrease.goldGain):
                    return ($"골드획득률 + {value * 100}%");
                // 경험치 획득량 증가
                case nameof(StatIncrease.expGain):
                    return ($"경험치획득률 + {value * 100}%");
                // 스탯 강화석 획득량 증가
                case nameof(StatIncrease.statStoneGain):
                    return ($"스탯강화석획득률 + {value * 100}%");
                default:
                    return null;
            }
        }
    }
}