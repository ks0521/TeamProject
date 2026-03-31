using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using Growth.Skill;

namespace Personal.HagYun
{
    public class SkillTreeUISetView : MonoBehaviour
    {
        [SerializeField] private Button skillDetailsPopupBtn;
        [SerializeField] private TextMeshProUGUI lvTxt;
        [SerializeField] private Image skillImg;
        [SerializeField] private int skillNum;
        public int SkillNum => skillNum;
        public void SetSkillDetailsBtn(SkillSO skillData, int skillNum)
        {
            skillImg.sprite = skillData.skillIcon;
            if (skillData is ActiveSkillSO aSkillData && aSkillData.Targeting == TargetingMode.Homing)
                skillImg.rectTransform.localEulerAngles = new Vector3(0, 0, 135f);
            else
                skillImg.rectTransform.localEulerAngles = Vector3.zero;
            this.skillNum = skillNum;
        }
        public void SetLvText(int curLv, int maxLv) => lvTxt.text = $"{curLv} / {maxLv}";
        public void BtnEventSubscribe(Action func)
        {
            skillDetailsPopupBtn.onClick.AddListener(() => func?.Invoke());
        }
        public void BtnEventUnsubscribe()
        {
            skillDetailsPopupBtn.onClick.RemoveAllListeners();
        }
    }
}