using Growth.Skill;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Skill_Set
{
<<<<<<< HEAD:Assets/Game/Personal/HagYun/Script/Skill/UI/SkillDetailView.cs
=======
    public struct SkillDetailViewNeedsNameAndImage
    {
        public string name;
        public Sprite sp;
        public bool isUnlock;
        public SkillDetailViewNeedsNameAndImage(string name, Sprite sp, bool isUnlock)
        {
            this.name = name;
            this.sp = sp;
            this.isUnlock = isUnlock;
        }
        public SkillDetailViewNeedsNameAndImage(SkillSO so, bool isUnlock)
        {
            name = so.skillName;
            sp = so.skillIcon;
            this.isUnlock = isUnlock;
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
>>>>>>> main:Assets/Game/UI/UI/Scripts/UI_Set/Skill_Set/SkillDetailView.cs
    public class SkillDetailView : MonoBehaviour
    {
        [Header("UI 표시")]
        [SerializeField, Tooltip("스킬 이미지")] private Image skillImg;
        [SerializeField, Tooltip("스킬 이름")] private TextMeshProUGUI nameText;
        [SerializeField, Tooltip("스킬 레벨(현재/최대)")] private TextMeshProUGUI levelText;
        [SerializeField, Tooltip("스킬 배율")] private TextMeshProUGUI skillValueText;
        [SerializeField, Tooltip("스킬 쿨타임")] private TextMeshProUGUI skillCooltimeText;
        [SerializeField, Tooltip("스킬 설명")] private TextMeshProUGUI skillDescriptionText;
        [SerializeField, Tooltip("스킬 잠금 Mask 이미지")] private Image lockImg;
        [Header("버튼")]

        [SerializeField, Tooltip("스킬 레벨 업 버튼")] private Button lvUpBtn;
        [SerializeField, Tooltip("스킬 Max 레벨 업 버튼")] private Button lvUpMaxBtn;
        [SerializeField, Tooltip("스킬 장착 버튼")] private Button equipBtn;
        [SerializeField, Tooltip("장착 스킬 우선순위 변경 버튼")] private Button priorityChangeBtn;
<<<<<<< HEAD:Assets/Game/Personal/HagYun/Script/Skill/UI/SkillDetailView.cs
=======
        public void SkillDetailViewSetToSkillChange(SkillDetailViewNeedsNameAndImage niData, SkillDetailViewNeedsStatData statData)
        {
            SkillNameChange(niData.name);
            SkillImgChange(niData.sp);
>>>>>>> main:Assets/Game/UI/UI/Scripts/UI_Set/Skill_Set/SkillDetailView.cs

        public void SkillDataShow(Skill skill)
        {
            if (skill == null) return;
            var data = skill.SkillData;
            SkillNameChange(data.skillName);
            if (data.Type == Growth.Skill.SkillType.Passive)
            {
                skillCooltimeText.gameObject.SetActive(false);
                equipBtn.gameObject.SetActive(false);
                priorityChangeBtn.gameObject.SetActive(false);
                SkillImgChange(data.skillIcon, false);
                PassiveSkill passiveSkill = (PassiveSkill)skill;
                SkillValueChange(passiveSkill);
            }
            else
            {
                var activeSkill = (ActiveSkill)skill;
                var activeData = activeSkill.ActiveSkillData;
                SkillCooltimeChange(activeData.coolDown);
                SkillImgChange(data.skillIcon, activeSkill.IsHomingSkill);
                SkillValueChange(activeSkill);
                skillCooltimeText.gameObject.SetActive(true);
                equipBtn.gameObject.SetActive(true);
                priorityChangeBtn.gameObject.SetActive(true);
            }
            SkillDescriptionChange(data.description);
        }
        public void SkillImgChange(Sprite sp, bool isHoming)
        {
            skillImg.sprite = sp;
            if (isHoming) skillImg.rectTransform.localEulerAngles = new Vector3(0, 0, 135f);
            else skillImg.rectTransform.localEulerAngles = Vector3.zero;
        }
<<<<<<< HEAD:Assets/Game/Personal/HagYun/Script/Skill/UI/SkillDetailView.cs
        public void SkillNameChange(string skillName) => nameText.text = skillName;
        public void SkillLevelChange(int curLv, int maxLv) => levelText.text = $"Lv : {curLv} / {maxLv}";
        public void SkillValueChange(Skill skill)
        {
            SkillLevelChange(skill.CurLv, skill.MaxLv);
            if(skill.SkillData.Type == Growth.Skill.SkillType.Passive)
            {
                SkillValueChange((PassiveSkill)skill);
            }
            else
            {
                SkillValueChange((ActiveSkill)skill);
            }
        }
        StringBuilder sb = new StringBuilder();
        public void SkillValueChange(ActiveSkill activeSkill)
        {
            skillValueText.text = $"배율 : {activeSkill.ResultDamage * 100}%";
        }
        public void SkillValueChange(PassiveSkill passiveSkill)
        {
            sb.Clear();
            sb.AppendLine("증가 스탯");
            foreach (var extractor in passiveSkill.Extractors)
            {
                var statData = passiveSkill.ResultSkillData;
                if (extractor.IsEffective(statData))
                {
                    extractor.GetValue(statData, PassiveValueStringSet);
                }
            }
            skillValueText.text = sb.ToString();
        }
        void PassiveValueStringSet(string name, string value)
        {
            switch (name)
            {
                // 공격력 증가(상수)
                case "flatAttack":
                    sb.AppendLine($"공격력 + {value}");
                    break;
                    // 공격력 % 증가
                case "attackRate":
                    sb.AppendLine($"공격력 + {value}%");
                    break;
                    // HP 증가(상수)
                case "flatMaxHp":
                    sb.AppendLine($"HP + {value}");
                    break;
                    // HP % 증가
                case "maxHpRate":
                    sb.AppendLine($"HP + {value}%");
                    break;
                    // 받는 피해 비율 감소
                case "damageReductionRate":
                    sb.AppendLine($"받는 피해 감소 + {value}%");
                    break;
                    // 아이템 드랍률 증가
                case "itemDropRateBonus":
                    sb.AppendLine($"아이템 드랍률 + {value}%");
                    break;
                    // 골드 획득량 증가
                case "goldGainRate":
                    sb.AppendLine($"골드 획득량 + {value}");
                    break;
                    // 경험치 획득량 증가
                case "expGainRate":
                    sb.AppendLine($"경험치 획득량 + {value}");
                    break;
                    // 스탯 강화석 획득량 증가
                case "statStoneGainRate":
                    sb.AppendLine($"스탯 강화석 + {value}");
                    break;
                    // 이동속도 증가
                case "moveSpeedRate":
                    sb.AppendLine($"이동속도 + {value}");
                    break;
                    // 공격속도 증가
                case "attackSpeedRate":
                    sb.AppendLine($"공격속도 + {value}");
                    break;
            }
        }
=======
        public void SkillImgChange(Sprite sp) => skillImg.sprite = sp;
        public void SkillNameChange(string skillName) => nameText.text = skillName;
        public void SkillLevelChange(int curLv, int maxLv) => levelText.text = $"{curLv} / {maxLv}";
        public void SkillStatValueTextChange(string valueText) => skillValueText.text = valueText;
>>>>>>> main:Assets/Game/UI/UI/Scripts/UI_Set/Skill_Set/SkillDetailView.cs
        public void SkillCooltimeChange(float cooltime) => skillCooltimeText.text = $"쿨타임 : {cooltime}초";
        public void SkillDescriptionChange(string skillDescription) => skillDescriptionText.text = skillDescription;
        public void SkillLevelUpBtnInteractable(bool isInteractable)
        {
            lvUpBtn.interactable = isInteractable;
            lvUpMaxBtn.interactable = isInteractable;
        }
        public void BtnEventSubscribe(Action lvUpFunc, Action maxLvUpFunc, Action equipFunc)
        {
            lvUpBtn.onClick.AddListener(() => lvUpFunc());
            lvUpMaxBtn.onClick.AddListener(() => maxLvUpFunc());
            equipBtn.onClick.AddListener(() => equipFunc());
            // priorityChangeBtn.onClick.AddListener(() => ());
        }
        public void BtnEventUnsubscribe()
        {
            lvUpBtn.onClick.RemoveAllListeners();
            lvUpMaxBtn.onClick.RemoveAllListeners();
            equipBtn.onClick.RemoveAllListeners();
            // priorityChangeBtn.onClick.RemoveAllListeners();
        }
    }
}