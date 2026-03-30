using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Personal.HagYun
{
    public class SkillButtonView : MonoBehaviour
    {
        private Button btn;
        [SerializeField] private Image cooltimeMask;
        [SerializeField] private Image skillImg;
        // 0 : 기본, 1 : 선택됨
        [SerializeField] private Sprite[] borderArr;
        public bool IsCooltimeMaskActiveState => cooltimeMask.gameObject.activeSelf;
        public bool IsCooltimeCheck { get; private set; }
        private void OnEnable()
        {
            btn = GetComponent<Button>();
        }
        private void OnDestroy()
        {
            btn.onClick.RemoveAllListeners();
        }
        public void SkillIconImageChange(Sprite sp, bool isHomingSkill)
        {
            skillImg.sprite = sp;
            if (isHomingSkill) skillImg.rectTransform.localEulerAngles = new Vector3(0, 0, 135f);
            else skillImg.rectTransform.localEulerAngles = Vector3.zero;
        }
        public void CooltimeStart()
        {
            IsCooltimeCheck = true;
            cooltimeMask.fillAmount = 1;
            cooltimeMask.gameObject.SetActive(true);
        }
        public void CooltimeEnd()
        {
            IsCooltimeCheck = false;
            cooltimeMask.fillAmount = 0;
            cooltimeMask.gameObject.SetActive(false);
        }
        public void BorderChange(bool isSelected)
        {
            if (isSelected) btn.image.sprite = borderArr[1];
            else btn.image.sprite = borderArr[0];
        }
        public void ButtonEventSet(Action func) => btn.onClick.AddListener(() => func());
        // public void BtnImageUpdate(float value) => btn.image.fillAmount = value;
        public void CooltimeShowUpdate(float value) => cooltimeMask.fillAmount = value;
    }
}