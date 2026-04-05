using Growth.Equipment;
using Growth.Skill;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
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

        // skill value change용 StringBuilder
        StringBuilder sb = new StringBuilder();

        public void SkillDetailViewUIShowAndHide(bool isActive)
        {
            if (isActive)
            {
                skillDescriptionText.gameObject.SetActive(true);
                skillCooltimeText.gameObject.SetActive(true);
                equipBtn.gameObject.SetActive(true);
                priorityChangeBtn.gameObject.SetActive(true);
            }
            else
            {
                skillDescriptionText.gameObject.SetActive(false);
                skillCooltimeText.gameObject.SetActive(false);
                equipBtn.gameObject.SetActive(false);
                priorityChangeBtn.gameObject.SetActive(false);
            }
        }
        public void SkillImgChange(Sprite sp, bool isHoming)
        {
            skillImg.sprite = sp;
            if (isHoming) skillImg.rectTransform.localEulerAngles = new Vector3(0, 0, 135f);
            else skillImg.rectTransform.localEulerAngles = Vector3.zero;
        }
        public void SkillNameChange(string skillName) => nameText.text = skillName;
        public void SkillLevelChange(int curLv, int maxLv) => levelText.text = $"Lv : {curLv} / {maxLv}";
        public void ActiveSkillStatValueTextInit()
        {
            sb.Clear();
            sb.Append("배율 : ");
        }
        public void PassiveSkillStatValueTextInit()
        {
            sb.Clear();
            sb.AppendLine("증가 스탯");
        }
        public void SkillStatValueTextBuild(string contents, bool isEnter)
        {
            if (isEnter) sb.Append($"\n{contents}");
            else sb.Append(contents);
        }
        public void SkillStatValueTextChange() => skillValueText.text = sb.ToString();
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