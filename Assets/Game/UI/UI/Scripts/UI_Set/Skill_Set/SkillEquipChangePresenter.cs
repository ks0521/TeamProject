using Base.Data;
using Base.Managers;
using Battle;
using Growth.Skill;
using UnityEngine;

namespace UI.Skill_Set
{
    public class SkillEquipChangePresenter : MonoBehaviour
    {
        private SkillPopupUIManager owner;
        // private SkillManager skillMgr;
        private SkillPool pool;
        private EventHub hub;
        private int targetSkillKey;
        [SerializeField] private SkillEquipChangePopupView skillEquipChangePopupView;
        private EquipSkill[] eSkillArr;
        public void Init(UIPresenterInitData data)
        {
            owner = data.owner;
            pool = data.pool;
            hub = data.hub;
            eSkillArr = GameManager.Instance.GetGameSystem<PlayerManager>().Player.ESController.EquipSkillArr;

            // hub.OnSkillEquip += EquippedKeySetting;
            // hub.OnSkillUnset += UnequipKeySetting;
        }
        public void OnDestroyFeat()
        {
            BtnEventRemoveListner();
            // hub.OnSkillEquip -= EquippedKeySetting;
            // hub.OnSkillUnset -= UnequipKeySetting;
        }
        public void EquipSkillShow(int targetSkillKey)
        {
            if (!pool.TryGetActiveSkillByKey(targetSkillKey, out var aSkill)) return;
            this.targetSkillKey = targetSkillKey;
            bool isHomingSkill = aSkill.ActiveSkillData.Targeting == Growth.Skill.TargetingMode.Homing;
            skillEquipChangePopupView.TargetSkillImgSet(aSkill.SkillData.skillIcon, isHomingSkill);

            for (int i = 0; i < 6; i++)
            {
                int curKey = eSkillArr[i].EquippedSkillKey;
                if(!pool.TryGetActiveSkillByKey(curKey, out var tASkill))
                {
                    Debug.LogWarning($"{i}번 슬롯에 스킬 없음");
                    skillEquipChangePopupView.SkillSlotBtnImgUnset(i);
                    continue;
                }
                var data = tASkill.ActiveSkillData;
                bool isTHomingSkill = data.Targeting == Growth.Skill.TargetingMode.Homing;
                skillEquipChangePopupView.SkillSlotBtnImgSet(i, data.skillIcon, isTHomingSkill);
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
        public void BtnEventAddListner()
        {
            skillEquipChangePopupView.BtnEventAddListner(SkillEquipChangeSelect);
        }
        void BtnEventRemoveListner() => skillEquipChangePopupView.BtnEventRemoveAllListner();
    }
}