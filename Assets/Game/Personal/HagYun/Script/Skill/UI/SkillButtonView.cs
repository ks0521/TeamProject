using System;
using UnityEngine;
using UnityEngine.UI;

namespace Personal.HagYun
{
    [Serializable]
    public class SkillButtonView
    {
        [SerializeField] private Button btn;
        [SerializeField] private Image cooltimeMask;
        [SerializeField] private Image skillImg;
        // 0 : 기본, 1 : 선택됨
        // [SerializeField] private Sprite[] borderArr;
        public bool IsCooltimeMaskActiveState => cooltimeMask.gameObject.activeSelf;
        // public void BorderSpriteSet(Sprite[] borderArr) => this.borderArr = borderArr;
        public void OnDestroyFeat()
        {
            ButtonEventUnsubscribe();
        }
        public void SkillIconImageChange(Sprite sp, bool isHomingSkill) => skillImg.SkillImgSetting(sp, isHomingSkill);
        public void SkillIconImageUnset() => skillImg.SkillImgUnsetting();
        public void CooltimeStart()
        {
            cooltimeMask.fillAmount = 1;
            cooltimeMask.gameObject.SetActive(true);
        }
        public void CooltimeEnd()
        {
            cooltimeMask.fillAmount = 0;
            cooltimeMask.gameObject.SetActive(false);
        }
        public bool IsSelected { get; private set; }
        public void SkillSelect(Sprite borderImg)
        {
            IsSelected = true;
            btn.image.sprite = borderImg;
        }
        public void SkillUnset(Sprite borderImg)
        {
            IsSelected = false;
            btn.image.sprite = borderImg;
        }
        public void ButtonEventSubscribe(Action func) => btn.onClick.AddListener(() => func());
        public void ButtonEventUnsubscribe() => btn.onClick.RemoveAllListeners();
        // public void BtnImageUpdate(float value) => btn.image.fillAmount = value;
        public void CooltimeShowUpdate(float value) => cooltimeMask.fillAmount = value;
    }
}