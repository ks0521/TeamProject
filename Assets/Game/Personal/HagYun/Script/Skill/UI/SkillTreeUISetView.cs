using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using Growth.Skill;

namespace Personal.HagYun
{
    public struct SkillTreeUISetViewNeedsImageData
    {
        public int key;
        public int curLv;
        public int maxLv;
        public Sprite skillImg;
        public bool isHomingSkill;
        public SkillTreeUISetViewNeedsImageData(int key, int curLv, int maxLv, Sprite img, bool isHomingSkill)
        {
            this.key = key;
            this.curLv = curLv;
            this.maxLv = maxLv;
            skillImg = img;
            this.isHomingSkill = isHomingSkill;
        }
    }
    public class SkillTreeUISetView : MonoBehaviour
    {
        [SerializeField] private Button skillDetailsPopupBtn;
        [SerializeField] private TextMeshProUGUI lvTxt;
        [SerializeField] private Image skillImg;
        private int skillKey = -1;
        public int SkillKey => skillKey;
        public void SkillTreerUISet(SkillTreeUISetViewNeedsImageData data)
        {
            SetSkillKey(data.key);
            SetImg(data.skillImg, data.isHomingSkill);
            SetLvText(data.curLv, data.maxLv);
        }
        public void SetSkillKey(int key) => skillKey = key;
        public void SetImg(Sprite sp, bool isHoming) => skillImg.SkillImgSetting(sp, isHoming);
        public void SetLvText(int curLv, int maxLv) => lvTxt.text = $"{curLv} / {maxLv}";
        public void BtnEventAddListner(Action func)
        {
            skillDetailsPopupBtn.onClick.AddListener(() => func?.Invoke());
        }
        public void BtnEventRemoveAllListner()
        {
            skillDetailsPopupBtn.onClick.RemoveAllListeners();
        }
    }
}