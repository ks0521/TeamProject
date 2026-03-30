using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

namespace Personal.HagYun
{
    public class SkillTreeUISetView : MonoBehaviour
    {
        [SerializeField] private Button lvUpBtn;
        [SerializeField] private Button lvDownBtn;
        [SerializeField] private TextMeshProUGUI lvTxt;
        public void LvUpUIBtnUpdate(bool isOn) => lvUpBtn.interactable = isOn;
        public void LvDownUIBtnUpdate(bool isOn) => lvDownBtn.interactable = isOn;
        public void LvTextSet(int curLv, int maxLv) => lvTxt.text = $"{curLv} / {maxLv}";
        public void BtnEventSubscribe(Action lvUpFunc, Action lvDownFunc)
        {
            lvUpBtn.onClick.AddListener(() => lvUpFunc());
            lvDownBtn.onClick.AddListener(() => lvDownFunc());
        }
        public void BtnEventUnsubscribe()
        {
            lvUpBtn.onClick.RemoveAllListeners();
            lvDownBtn.onClick.RemoveAllListeners();
        }
    }
}