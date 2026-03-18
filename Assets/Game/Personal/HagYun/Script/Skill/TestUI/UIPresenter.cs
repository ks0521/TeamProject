using Personal.HagYun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Personal.HagYun
{
    public class UIPresenter : MonoBehaviour
    {
        public BtnView[] btnArr;
        private void Start()
        {
            EquipSkillController.esc[0].AddEventCooltimeUpdate(btnArr[0].BtnImageUpdate);
            EquipSkillController.esc[1].AddEventCooltimeUpdate(btnArr[1].BtnImageUpdate);
            BtnEventSet(0, EquipSkillController.esc.SkillUse1);
            BtnEventSet(1, EquipSkillController.esc.SkillUse2);
        }
        public void BtnEventSet(int index, Action func) => btnArr[index].ButtonEventSet(func);
    }
}