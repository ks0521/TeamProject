using Personal.HagYun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Skill_Set
{
    public class SkillEquipChangePopupView : MonoBehaviour
    {
        [Serializable]
        struct EquipSkillPopupBtnSet
        {
            public Button btn;
            public Image img;
        }
        [SerializeField] private Image targetSkillImage;
        [SerializeField] private EquipSkillPopupBtnSet[] equipSkillBtnSet;
        [SerializeField] private Button closeBtn;
        public void BtnEventUnsubscribe()
        {
            foreach (EquipSkillPopupBtnSet btnSet in equipSkillBtnSet)
            {
                btnSet.btn.onClick.RemoveAllListeners();
            }
            closeBtn.onClick.RemoveAllListeners();
        }
<<<<<<< HEAD:Assets/Game/Personal/HagYun/Script/Skill/UI/SkillEquipChangePopupView.cs
        public void EquipSkillShow(ActiveSkill equipTargetSkill, ActiveSkill[] skillArr)
=======
        public void TargetSkillImgSet(Sprite sp) => targetSkillImage.sprite = sp;
        public void SkillSlotBtnImgSet(int index, Sprite sp)
>>>>>>> main:Assets/Game/UI/UI/Scripts/UI_Set/Skill_Set/SkillEquipChangePopupView.cs
        {
            equipTargetSkill.SkillImgSet(targetSkillImage);
            for (int i = 0; i < 6; i++)
            {
                if (skillArr[i] is ActiveSkill aSkill)
                {
                    aSkill.SkillImgSet(equipSkillBtnSet[i].img);
                }
                else
                {
                    Skill.SkillImgUnset(equipSkillBtnSet[i].img);
                }
            }
<<<<<<< HEAD:Assets/Game/Personal/HagYun/Script/Skill/UI/SkillEquipChangePopupView.cs
=======
            equipSkillBtnSet[index].img.SkillImgSetting(sp);
>>>>>>> main:Assets/Game/UI/UI/Scripts/UI_Set/Skill_Set/SkillEquipChangePopupView.cs
        }
        public void EquipSlotSelectBtnEventSubscribe(int slotIndex, Action func)
        {
            if (slotIndex < 0 || 6 <= slotIndex) return;
            equipSkillBtnSet[slotIndex].btn.onClick.AddListener(() => func());
        }
        public void BtnEventSubscribe(Action<int> func)
        {
            for (int i = 0; i < 6; i++)
            {
                int index = i;
                equipSkillBtnSet[i].btn.onClick.AddListener(() => func(index));
            }
            closeBtn.onClick.AddListener(() => gameObject.SetActive(false));
        }
        // public Transform targetParentObj;
        // public string imgObjectName;

        // [ContextMenu("Button Set Auto Bind UI")]
        // public void BindBtn()
        // {
        //     equipSkillBtnSet = new EquipSkillPopupBtnSet[6];
        //     var allBtns = targetParentObj.GetComponentsInChildren<Button>(true);
        //     for (int i = 0; i < equipSkillBtnSet.Length; i++)
        //     {
        //         var btn = allBtns[i];
        //         var bSet = new EquipSkillPopupBtnSet();
        //         bSet.btn = btn;

        //         if (btn.transform.TryGetChildren(imgObjectName, out Image img, false))
        //             bSet.img = img;

        //         equipSkillBtnSet[i] = bSet;
        //     }
        // }
    }
}