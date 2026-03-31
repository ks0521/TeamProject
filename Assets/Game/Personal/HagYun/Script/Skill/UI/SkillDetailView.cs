using Growth.Skill;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Personal.HagYun
{
    public class SkillDetailView : MonoBehaviour
    {
        [Header("UI 표시")]
        [SerializeField, Tooltip("스킬 이미지")] private Image skillImg;
        [SerializeField, Tooltip("스킬 이름")] private TextMeshProUGUI nameText;
        [SerializeField, Tooltip("스킬 레벨(현재/최대)")] private TextMeshProUGUI levelText;
        [SerializeField, Tooltip("스킬 배율")] private TextMeshProUGUI skillValueText;
        [SerializeField, Tooltip("스킬 쿨타임")] private TextMeshProUGUI skillCooltimeText;
        [SerializeField, Tooltip("스킬 설명")] private TextMeshProUGUI skillDescriptionText;
        [Header("버튼")]

        [SerializeField, Tooltip("스킬 레벨 업 버튼")] private Button lvUpBtn;
        [SerializeField, Tooltip("스킬 Max 레벨 업 버튼")] private Button lvUpMaxBtn;
        [SerializeField, Tooltip("스킬 장착 버튼")] private Button equipBtn;
        [SerializeField, Tooltip("장착 스킬 우선순위 변경 버튼")] private Button priorityChangeBtn;

        public void SkillDataShow(SkillSO skillData, int curLv, int maxLv)
        {
            if (skillData == null) return;
            SkillNameShow(skillData.skillName);
            SkillLevelShow(curLv, maxLv);
            if (skillData.SkillType == Growth.Skill.Type.Passive)
            {
                skillCooltimeText.gameObject.SetActive(false);
                SkillImgShow(skillData.skillIcon, false);
            }
            else
            {
                ActiveSkillSO aSkillData = (ActiveSkillSO)skillData;
                SkillCooltimeShow(aSkillData.coolDown);
                skillCooltimeText.gameObject.SetActive(true);
                SkillImgShow(skillData.skillIcon, aSkillData.Targeting == TargetingMode.Homing);
            }
            SkillValueShow(skillData.baseValue);
            SkillDescriptionShow(skillData.description);
        }
        public void SkillImgShow(Sprite sp, bool isHoming)
        {
            skillImg.sprite = sp;
            if(isHoming) skillImg.rectTransform.localEulerAngles = new Vector3(0, 0, 135f);
            else  skillImg.rectTransform.localEulerAngles = Vector3.zero;
        }
        public void SkillNameShow(string skillName) => nameText.text = skillName;
        public void SkillLevelShow(int curLv, int maxLv) => levelText.text = $"Lv : {curLv} / {maxLv}";
        public void SkillValueShow(float value) => skillValueText.text = $"배율 : {value * 100}%";
        public void SkillCooltimeShow(float cooltime) => skillCooltimeText.text = $"쿨타임 : {cooltime}초";
        public void SkillDescriptionShow(string skillDescription) => skillDescriptionText.text = skillDescription;
        public void SkillLevelUpBtnInteractable(bool isInteractable)
        {
            lvUpBtn.interactable = isInteractable;
            lvUpMaxBtn.interactable = isInteractable;
        }
        public void BtnEventSubscribe(Action lvUpFunc, Action maxLvUpFunc)
        {
            lvUpBtn.onClick.AddListener(() => lvUpFunc());
            lvUpMaxBtn.onClick.AddListener(() => maxLvUpFunc());
            // equipBtn.onClick.AddListner(() => ());
            // priorityChangeBtn.onClick.AddListner(() => ());
        }
        public void BtnEventUnsubscribe()
        {
            lvUpBtn.onClick.RemoveAllListeners();
            lvUpMaxBtn.onClick.RemoveAllListeners();
            // equipBtn.onClick.RemoveAllListeners();
            // priorityChangeBtn.onClick.RemoveAllListeners();
        }
    }
}