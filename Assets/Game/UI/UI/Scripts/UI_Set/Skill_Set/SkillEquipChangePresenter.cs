using Base.Data;
using Base.Managers;
using Battle;
using Growth.Skill;
using System;
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

            EventSubscribe();
        }
        public void OnDestroyFeat()
        {
            BtnEventRemoveListner();
            EventUnsubscribe();
        }
        public void EquipSkillShow(int targetSkillKey)
        {
            if (!skillMgr.TryGetActiveSkill(targetSkillKey, out var aSkill)) return;
            this.targetSkillKey = targetSkillKey;
            skillEquipChangePopupView.TargetSkillImgSet(aSkill.SkillData.skillIcon);

            for (int i = 0; i < 6; i++)
            {
                int index = i;
                int curKey = eSkillList[index].EquippedSkillKey;
                EquipSkillPriorityBtnSet(index, curKey);
                if (!skillMgr.TryGetActiveSkill(curKey, out var tASkill))
                {
                    Debug.LogWarning($"{index}번 슬롯에 스킬 없음");
                    skillEquipChangePopupView.SkillSlotBtnImgUnset(index);
                    continue;
                }
                var data = tASkill.ActiveSkillData;
                skillEquipChangePopupView.SkillSlotBtnImgSet(index, data.skillIcon);
            }
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
        public void EquipSkillPriorityBtnSet(int slotNum, Priority pri)
        {
            if (!skillMgr.PlayerEquipSkillList[slotNum].isEquipped) return;
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
        public void EquipSkillPriorityBtnUnset(int slotNum)
        {
            skillEquipChangePopupView.SkillPriorityBtnUnset(slotNum);
        }
        public void EquipSkillPriorityBtnSet(int slotNum, int skillKey)
        {
            if (slotNum < 0 || 6 <= slotNum) return;
            var eSkill = skillMgr.PlayerEquipSkillList[slotNum];
            if (eSkill.isEquipped && eSkill.EquippedSkillKey == skillKey)
            {
                EquipSkillPriorityBtnSet(slotNum, eSkill.priority);
                Debug.Log($"SkillEquipList : {slotNum}에 스킬 있음 & 동일, priority change");
            }
            else
            {
                EquipSkillPriorityBtnUnset(slotNum);
                Debug.LogWarning($"SkillEquipList : {slotNum}에 스킬 없음 or 맞지 않음, priority unset");
            }
        }
        void EventSubscribe()
        {
            hub.OnEquipSkillPriorityChange += EquipSkillPriorityBtnSet;
            hub.OnSkillUnset += EquipSkillPriorityBtnUnset;
        }
        public void BtnEventAddListner(Action<int> equipSkillPriorityChangeFunc)
        {
            skillEquipChangePopupView.BtnEventAddListner(SkillEquipChangeSelect, equipSkillPriorityChangeFunc);
        }
        void EventUnsubscribe()
        {
            hub.OnEquipSkillPriorityChange -= EquipSkillPriorityBtnSet;
            hub.OnSkillUnset -= EquipSkillPriorityBtnUnset;
        }
        void BtnEventRemoveListner() { skillEquipChangePopupView.BtnEventRemoveAllListner(); }
    }
}