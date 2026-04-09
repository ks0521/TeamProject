using Growth.Skill;
using System;
using Base.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Personal.HagYun
{
    public struct SkillDetailViewNeedsNameAndImage
    {
        public string name;
        public Sprite sp;
        public bool isHomingSkill;
        public SkillDetailViewNeedsNameAndImage(string name, Sprite sp, bool isHomingSkill)
        {
            this.name = name;
            this.sp = sp;
            this.isHomingSkill = isHomingSkill;
        }
        public SkillDetailViewNeedsNameAndImage(SkillSO so)
        {
            name = so.name;
            sp = so.skillIcon;
            if (so is ActiveSkillSO aSo)
                isHomingSkill = aSo.Targeting == TargetingMode.Homing;
            else
                isHomingSkill = false;
        }
        public SkillDetailViewNeedsNameAndImage(ActiveSkillSO so)
        {
            name = so.name;
            sp = so.skillIcon;
            isHomingSkill = so.Targeting == TargetingMode.Homing;
        }
        public SkillDetailViewNeedsNameAndImage(PassiveSkillSO so)
        {
            name = so.name;
            sp = so.skillIcon;
            isHomingSkill = false;
        }
    }
    public struct SkillDetailViewNeedsStatData
    {
        public int curLv;
        public int maxLv;
        public string skillValueText;
        public float cooltimeValue;
        public string description;
        public bool isActiveSkill;
        public SkillDetailViewNeedsStatData(int curLv, int maxLv,
        string skillValueText, float cooltimeValue, string description, bool isActiveSkill)
        {
            this.curLv = curLv;
            this.maxLv = maxLv;
            this.skillValueText = skillValueText;
            this.cooltimeValue = cooltimeValue;
            this.description = description;
            this.isActiveSkill = isActiveSkill;
        }
        public SkillDetailViewNeedsStatData(SkillSO so, string skillValueText, int curLv)
        {
            maxLv = so.maxLv;
            description = so.description;
            this.curLv = curLv;
            this.skillValueText = skillValueText;
            if (so is ActiveSkillSO aSO)
            {
                cooltimeValue = aSO.coolDown;
                isActiveSkill = true;
            }
            else
            {
                cooltimeValue = 0;
                isActiveSkill = false;
            }
        }
        public SkillDetailViewNeedsStatData(ActiveSkillSO so, string skillValueText, int curLv)
        {
            maxLv = so.maxLv;
            description = so.description;
            cooltimeValue = so.coolDown;
            isActiveSkill = true;
            this.curLv = curLv;
            this.skillValueText = skillValueText;
        }
        public SkillDetailViewNeedsStatData(PassiveSkillSO so, string skillValueText, int curLv)
        {
            maxLv = so.maxLv;
            description = so.description;
            cooltimeValue = 0;
            isActiveSkill = false;
            this.curLv = curLv;
            this.skillValueText = skillValueText;
        }
    }
    public class SkillDetailView : MonoBehaviour
    {
        [Header("UI 표시")]
        [SerializeField, Tooltip("스킬 이미지")] private Image skillImg;
        [SerializeField, Tooltip("스킬 이름")] private TextMeshProUGUI nameText;
        [SerializeField, Tooltip("스킬 레벨(현재/최대)")] private TextMeshProUGUI levelText;
        [SerializeField, Tooltip("스킬 데미지/스탯 정보")] private TextMeshProUGUI skillValueText;
        [SerializeField, Tooltip("스킬 쿨타임")] private TextMeshProUGUI skillCooltimeText;
        [SerializeField, Tooltip("스킬 설명")] private TextMeshProUGUI skillDescriptionText;
        [Header("버튼")]

        [SerializeField, Tooltip("스킬 레벨 업 버튼")] private Button lvUpBtn;
        [SerializeField, Tooltip("스킬 Max 레벨 업 버튼")] private Button lvUpMaxBtn;
        [SerializeField, Tooltip("스킬 장착 버튼")] private Button equipBtn;
        [SerializeField, Tooltip("장착 스킬 우선순위 변경 버튼")] private Button priorityChangeBtn;
        public void SkillDetailViewSetToSkillChange(SkillDetailViewNeedsNameAndImage niData, SkillDetailViewNeedsStatData statData)
        {
            SkillNameChange(niData.name);
            SkillImgChange(niData.sp, niData.isHomingSkill);

            SkillDetailViewSetToLvEnhance(statData);
        }
        public void SkillDetailViewSetToLvEnhance(SkillDetailViewNeedsStatData statData)
        {
            SkillDetailViewUIShowAndHide(statData.isActiveSkill);
            SkillLevelChange(statData.curLv, statData.maxLv);
            SkillStatValueTextChange(statData.skillValueText);
            SkillCooltimeChange(statData.cooltimeValue);
            SkillDescriptionChange(statData.description);
        }
        public void SkillDetailViewUIShowAndHide(bool isActive)
        {
            if (isActive)
            {
                skillCooltimeText.gameObject.SetActive(true);
                equipBtn.gameObject.SetActive(true);
                priorityChangeBtn.gameObject.SetActive(true);
            }
            else
            {
                skillCooltimeText.gameObject.SetActive(false);
                equipBtn.gameObject.SetActive(false);
                priorityChangeBtn.gameObject.SetActive(false);
            }
        }
        public void SkillImgChange(Sprite sp, bool isHoming) => skillImg.SkillImgSetting(sp, isHoming);
        public void SkillNameChange(string skillName) => nameText.text = skillName;
        public void SkillLevelChange(int curLv, int maxLv) => levelText.text = $"Lv : {curLv} / {maxLv}";
        // public void SkillStatValueTextChange() => skillValueText.text = sb.ToString();
        public void SkillStatValueTextChange(string valueText) => skillValueText.text = valueText;
        public void SkillCooltimeChange(float cooltime) => skillCooltimeText.text = $"쿨타임 : {cooltime}초";
        public void SkillDescriptionChange(string skillDescription) => skillDescriptionText.text = skillDescription;
        public void SkillLevelUpBtnInteractable(bool isInteractable)
        {
            lvUpBtn.interactable = isInteractable;
            lvUpMaxBtn.interactable = isInteractable;
        }
        public void BtnEventAddListner(Action lvOneUpFunc, Action maxLvUpFunc, Action equipFunc)
        {
            lvUpBtn.onClick.AddListener(() => lvOneUpFunc());
            lvUpMaxBtn.onClick.AddListener(() => maxLvUpFunc());
            equipBtn.onClick.AddListener(() => equipFunc());
            // priorityChangeBtn.onClick.AddListener(() => ());
        }
        public void BtnEventRemoveAllListner()
        {
            lvUpBtn.onClick.RemoveAllListeners();
            lvUpMaxBtn.onClick.RemoveAllListeners();
            equipBtn.onClick.RemoveAllListeners();
            // priorityChangeBtn.onClick.RemoveAllListeners();
        }
    }
}