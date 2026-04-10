using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using Growth.Skill;

namespace UI.Skill_Set
{
    public class SkillTreeUISetView : MonoBehaviour
    {
        [SerializeField] private Button skillDetailsPopupBtn;
        [SerializeField] private TextMeshProUGUI lvTxt;
        [SerializeField] private Image skillImg;
        private int skillNum;
        public int SkillNum => skillNum;
        public void SetSkillDetailsBtn(Skill skill, int skillNum)
        {
<<<<<<< HEAD:Assets/Game/Personal/HagYun/Script/Skill/UI/SkillTreeUISetView.cs
            skillImg.sprite = skill.SkillData.skillIcon;
            if (skill is ActiveSkill aSkill && aSkill.IsHomingSkill)
                skillImg.rectTransform.localEulerAngles = new Vector3(0, 0, 135f);
            else
                skillImg.rectTransform.localEulerAngles = Vector3.zero;
            this.skillNum = skillNum;
        }
=======
            SetSkillKey(data.key);
            SetImg(data.skillImg);
            SetLvText(data.curLv, data.maxLv);
        }
        public void SetSkillKey(int key) => skillKey = key;
        public void SetImg(Sprite sp) => skillImg.sprite = sp;
>>>>>>> main:Assets/Game/UI/UI/Scripts/UI_Set/Skill_Set/SkillTreeUISetView.cs
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