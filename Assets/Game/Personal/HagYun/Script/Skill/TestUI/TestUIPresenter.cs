using Personal.HagYun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Personal.HagYun
{
    public class TestUIPresenter : MonoBehaviour
    {
        public BtnView[] btnArr;
        public EquipSkillController es;
        private void Start()
        {
            BtnEventSet(0, es.SkillUse1);
            BtnEventSet(1, es.SkillUse2);
        }
        void Update()
        {
            for (int i = 0; i < btnArr.Length; i++)
            {
                BtnValueUpdate(i);
            }
        }
        public void BtnValueUpdate(int index)
        {
            if (!es[index].IsCooltime)
            {
                btnArr[index].BtnImageUpdate(1);
                return;
            }
            float curCool = es[index].CurCooltime;
            float maxCool = es[index].MaxCooltime;
            float value = 1 - (curCool / maxCool);
            // Debug.Log($"현재 쿨:{curCool}, 맥스 쿨 : {maxCool}\n결과 쿨 : {value}");
            btnArr[index].BtnImageUpdate(value);
        }
        public void BtnEventSet(int index, Action func) => btnArr[index].ButtonEventSet(func);
    }
}