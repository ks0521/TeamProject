using Base.Data;
using Growth.Equipment;
using Growth.Skill;
using System;
using System.Text;
using Base.Utils;
using UnityEngine;

namespace UI.Skill_Set
{
    public class SkillDetailPresenter : MonoBehaviour, IMemberReceiver
    {
        private SkillManager skillMgr;
        private EventHub hub;
        [SerializeField] private SkillDetailView skillDetailView;
        public void Init(UIPresenterInitData data)
        {
            skillMgr = data.skillMgr;
            hub = data.hub;
            if (skillMgr.AllSkillSOList[0] is var so) SkillDetailDataSetToSkillChange(so.key);
        }
        public void SkillLevelUpBtnInteractable(bool isLevelUpBtnOn)
        {
            skillDetailView.SkillLevelUpBtnInteractable(isLevelUpBtnOn);
        }
        public void BtnEventAddListner(Action lvOneUpFunc, Action lvMaxUpFunc, Action equipFunc, Action priorityChangeFunc)
        {
            skillDetailView.BtnEventAddListner(lvOneUpFunc, lvMaxUpFunc, equipFunc, priorityChangeFunc);
        }
        public void OnDestroyFeat()
        {
            BtnEventRemoveListner();
        }
        void BtnEventRemoveListner() => skillDetailView.BtnEventRemoveAllListner();
        public void SkillPriorityBtnSet(Priority pri)
        {
            Color changeCol = Color.white;
            string changeTxt = "";
            switch (pri)
            {
                case Priority.High:
                    changeCol = Color.red;
                    changeTxt = "High";
                    break;
                case Priority.Mid:
                    changeCol = Color.blue;
                    changeTxt = "Mid";
                    break;
                case Priority.Low:
                    changeCol = Color.yellow;
                    changeTxt = "Low";
                    break;
            }

            skillDetailView.SkillPriorityBtnSet(changeCol, changeTxt);
        }
        public void SkillPriorityBtnUnset() => skillDetailView.SkillPriorityBtnUnset();
        void SkillPriorityBtnChange(int key)
        {
            skillMgr.TryGetSkillPriorityByKey(key, out var pri);
            SkillPriorityBtnSet(pri);
        }
        public void SkillDetailDataSetToSkillChange(int key)
        {
            gameObject.SetActive(true);
            if (!skillMgr.TryGetSkillSO(key, out var so)) return;
            SkillDetailDataSetToSkillChange(so);
            if(skillMgr.IsSkillEquippedByKey(key)) SkillPriorityBtnChange(key);
        }
        public void SkillDetailDataSetToSkillChange(SkillSO so)
        {
            skillMgr.TryGetSkillLevel(so, out int curLv);
            string value = null;
            SkillDetailViewNeedsNameAndImage niData = new(so, skillMgr.IsSkillUnlock(so));
            SkillDetailViewNeedsStatData data = new();
            if (so is ActiveSkillSO aSO)
            {
                ActiveSkillValueTextSet(aSO.ResultDamage(curLv), out value);

                data = new SkillDetailViewNeedsStatData(aSO, value, curLv, skillMgr.IsSkillEquippedByKey(aSO.key));
            }
            else if (so is PassiveSkillSO pSO)
            {
                PassiveSkillValueTextSet(pSO.ResultAddStat(curLv), out value);

                data = new SkillDetailViewNeedsStatData(pSO, value, curLv);
            }
            skillDetailView.SkillDetailViewSetToSkillChange(niData, data);
        }
        public void SkillDetailDataSetToSkillLvEnhance(int key)
        {
            if (!skillMgr.TryGetSkillSO(key, out var so)) return;
            SkillDetailDataSetToSkillLvEnhance(so);
        }
        public void SkillDetailDataSetToSkillLvEnhance(SkillSO so)
        {
            string value = null;
            SkillDetailViewNeedsStatData statData = new();
            skillMgr.TryGetSkillLevel(so, out int curLv);
            if (so is ActiveSkillSO aSO)
            {
                ActiveSkillValueTextSet(aSO.ResultDamage(curLv), out value);
                statData = new SkillDetailViewNeedsStatData(aSO, value, curLv, skillMgr.IsSkillEquippedByKey(aSO.key));

            }
            else if (so is PassiveSkillSO pSO)
            {
                PassiveSkillValueTextSet(pSO.ResultAddStat(curLv), out value);
                statData = new SkillDetailViewNeedsStatData(pSO, value, curLv);
            }
            skillDetailView.SkillDetailViewSetToLvChange(statData, skillMgr.IsSkillUnlock(so.key));

        }
        StringBuilder sb = new StringBuilder();
        public void SkillLvUpBtnInteractable(bool isSkillLvUpPossible) => skillDetailView.SkillLevelUpBtnInteractable(isSkillLvUpPossible);
        public void SkillLockImageSet(bool isUnlock) => skillDetailView.SkillLockImgSet(isUnlock);
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
            sb.Append("증가 스탯");
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