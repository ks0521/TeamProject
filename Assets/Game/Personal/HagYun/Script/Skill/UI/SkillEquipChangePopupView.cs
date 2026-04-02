using Personal.HagYun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillEquipChangePopupView : MonoBehaviour
{
    [SerializeField] private Image targetSkillImage;
    public Button[] equipSkillSlotBtnArr;
    [SerializeField] private Image[] equippedSkillImage;
    public void BtnEventUnsubscribe()
    {
        foreach (Button btn in equipSkillSlotBtnArr)
        {
            btn.onClick.RemoveAllListeners();
        }
    }
    public void EquipSkillShow(ActiveSkill equipTargetSkill, ActiveSkill[] skillArr)
    {
        equipTargetSkill.SkillImgSet(targetSkillImage);
        for (int i = 0; i < 6; i++)
        {
            if (skillArr[i] is ActiveSkill aSkill)
            {
                aSkill.SkillImgSet(equippedSkillImage[i]);
            }
            else
            {
                Skill.SkillImgUnset(equippedSkillImage[i]);
            }
        }
    }
    public void BtnEventSubscribe(int slotIndex, Action func)
    {
        equipSkillSlotBtnArr[slotIndex].onClick.AddListener(() => func());
    }
}
