using Base.Data;
using Base.Managers;
using Battle;
using Growth.Skill;
using System.Collections.Generic;
using UnityEngine;

namespace UI.Skill_Set
{
    public class SkillEquipChangePresenter : MonoBehaviour
    {
        private SkillManager skillMgr;
        private EventHub hub;
        private int targetSkillKey;
        [SerializeField] private SkillEquipChangePopupView skillEquipChangePopupView;
        private IReadOnlyList<EquipSkill> eSkillList;
        public void Init(UIPresenterInitData data)
        {
            skillMgr = data.skillMgr;
            hub = data.hub;
            eSkillList = skillMgr.PlayerEquipSkillList;
        }
        public void OnDestroyFeat()
        {
            BtnEventRemoveListner();
        }
        public void EquipSkillShow(int targetSkillKey)
        {
            if (!skillMgr.TryGetActiveSkill(targetSkillKey, out var aSkill)) return;
            this.targetSkillKey = targetSkillKey;
            skillEquipChangePopupView.TargetSkillImgSet(aSkill.SkillData.skillIcon);

            for (int i = 0; i < 6; i++)
            {
                int curKey = eSkillList[i].EquippedSkillKey;
                if(!skillMgr.TryGetActiveSkill(curKey, out var tASkill))
                {
                    Debug.LogWarning($"{i}번 슬롯에 스킬 없음");
                    skillEquipChangePopupView.SkillSlotBtnImgUnset(i);
                    continue;
                }
                var data = tASkill.ActiveSkillData;
                skillEquipChangePopupView.SkillSlotBtnImgSet(i, data.skillIcon);
            }
        }
        public void SkillPriorityChange(int slotNum, Priority pri)
        {
            Color changeCol = Color.white;
            string changeTxt = "";
            switch (pri)
            {
                case Priority.High:
                    changeCol = Color.red;
                    changeTxt = "High";
                    break;
                case Priority.Mid:
                    changeCol = Color.blue;
                    changeTxt = "Mid";
                    break;
                case Priority.Low:
                    changeCol = Color.yellow;
                    changeTxt = "Low";
                    break;
            }
            skillEquipChangePopupView.SkillPriorityBtnSet(slotNum, changeCol, changeTxt);
        }
        public void SkillEquipChangePopupShow(int key)
        {
            gameObject.SetActive(true);
            EquipSkillShow(key);
        }
        public void SkillEquipChangeSelect(int slotIndex)
        {
            hub.SkillEquip(slotIndex, targetSkillKey);
            gameObject.SetActive(false);
        }
        public void BtnEventAddListner()
        {
            skillEquipChangePopupView.BtnEventAddListner(SkillEquipChangeSelect);
        }
        void BtnEventRemoveListner() => skillEquipChangePopupView.BtnEventRemoveAllListner();
    }
}