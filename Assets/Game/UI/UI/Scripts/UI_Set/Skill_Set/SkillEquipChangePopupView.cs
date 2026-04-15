using Base.Utils;
using System;
using TMPro;
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
            public TextMeshProUGUI priText;
            public Image img;
        }
        [SerializeField] private Image targetSkillImage;
        [SerializeField] private EquipSkillPopupBtnSet[] equipSkillBtnSet;
        [SerializeField] private Button closeBtn;
        public void BtnEventRemoveAllListner()
        {
            foreach (EquipSkillPopupBtnSet btnSet in equipSkillBtnSet)
            {
                btnSet.btn.onClick.RemoveAllListeners();
            }
            closeBtn.onClick.RemoveAllListeners();
        }
        public void TargetSkillImgSet(Sprite sp) => targetSkillImage.sprite = sp;
        public void SkillSlotBtnImgSet(int index, Sprite sp)
        {
            if (index < 0 || 6 <= index)
            {
                Debug.LogWarning("ui : equip skill slot index는 0~6 사이만 가능");
                return;
            }
            equipSkillBtnSet[index].img.SkillImgSetting(sp);
        }
        public void SkillPriorityBtnSet(int index, Color col, string priValue)
        {
            if (index < 0 || 6 <= index)
            {
                Debug.LogWarning("ui : equip skill slot index는 0~6 사이만 가능");
                return;
            }
            var curEquipSkillBtnSet = equipSkillBtnSet[index];
            curEquipSkillBtnSet.btn.image.color = col;
            curEquipSkillBtnSet.priText.text = priValue;
        }
        public void SkillSlotBtnImgUnset(int index) => equipSkillBtnSet[index].img.SkillImgUnsetting();
        public void EquipSlotSelectBtnEventSubscribe(int slotIndex, Action func)
        {
            if (slotIndex < 0 || 6 <= slotIndex) return;
            equipSkillBtnSet[slotIndex].btn.onClick.AddListener(() => func());
        }
        public void BtnEventAddListner(Action<int> func)
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

        //         if (btn.transform.TryGetChildrenByName(imgObjectName, out Image img, false))
        //             bSet.img = img;

        //         equipSkillBtnSet[i] = bSet;
        //     }
        // }
    }
}