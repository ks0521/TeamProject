using Base.Utils;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

namespace UI.Skill_Set
{
    public struct SkillTreeUISetViewNeedsImageData
    {
        public int key;
        public bool isSkillUnlock;
        public int curLv;
        public int maxLv;
        public Sprite skillImg;
        public bool isHomingSkill;
        public SkillTreeUISetViewNeedsImageData(int key, bool isSkillUnlock, int curLv, int maxLv, Sprite img, bool isHomingSkill)
        {
            this.key = key;
            this.isSkillUnlock = isSkillUnlock;
            this.curLv = curLv;
            this.maxLv = maxLv;
            skillImg = img;
            this.isHomingSkill = isHomingSkill;
        }
    }
    public class SkillTreeUISetView : MonoBehaviour
    {
        private int skillKey = -1;
        public int SkillKey => skillKey;
        [SerializeField] private Button skillDetailsPopupBtn;
        [SerializeField] private TextMeshProUGUI lvTxt;
        [SerializeField] private Image skillImg;
        [SerializeField] private Image skillLockImg;
        public void SkillTreerUISet(SkillTreeUISetViewNeedsImageData data)
        {
            SetSkillKey(data.key);
            SetLockImg(data.isSkillUnlock);
            SetImg(data.skillImg);
            SetLvText(data.curLv, data.maxLv);
        }
        public void SetSkillKey(int key) => skillKey = key;
        public void SetLockImg(bool isUnlock) => skillLockImg.gameObject.SetActive(!isUnlock);
        public void SetImg(Sprite sp) => skillImg.sprite = sp;
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