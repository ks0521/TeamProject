using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Personal.HagYun
{
    public class BtnView : MonoBehaviour
    {
        [SerializeField] Button btn;
        private void OnDestroy()
        {
            btn.onClick.RemoveAllListeners();
        }
        public void ButtonEventSet(Action func) => btn.onClick.AddListener(() => func());
        public void BtnImageUpdate(float value) => btn.image.fillAmount = value;
    }
}