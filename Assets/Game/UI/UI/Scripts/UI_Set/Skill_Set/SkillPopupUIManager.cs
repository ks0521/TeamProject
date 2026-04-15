using Base.Data;
using Base.Managers;
using Battle;
using Growth.Skill;
using Unity.VisualScripting;
using UnityEngine;

namespace UI.Skill_Set
{
    public struct UIPresenterInitData
    {
        public SkillManager skillMgr;
        public EventHub hub;
        public UIPresenterInitData(SkillManager skillMgr, EventHub hub)
        {
            this.skillMgr = skillMgr;
            this.hub = hub;
        }
    }
    public class SkillPopupUIManager : MonoBehaviour
    {
        private SkillManager skillMgr;
        public SkillManager SkillMgr => skillMgr;
        // private Player pl;
        // private SkillPool pool;
        // public PlayerEquipSkillController PEquipSkillController { get; private set; }
        private EventHub hub;
        // private IReadOnlyList<SkillDatas> allSkillList;

        [SerializeField] private SkillTreePresenter skillTreePresenter;
        [SerializeField] private SkillDetailPresenter skillDetailPresenter;
        [SerializeField] private SkillEquipChangePresenter skillEquipChangePresenter;
        // public int curPoint;
        // public int maxPoint;
        void OnEnable()
        {
            Init();
        }
        public void Init()
        {
            skillMgr = GameManager.Instance.GetGameSystem<SkillManager>();
            hub = GameManager.Instance.GetGameSystem<EventHub>();

            UIPresenterInitData initData = new UIPresenterInitData(skillMgr, hub);

            skillTreePresenter.Init(initData);
            skillTreePresenter.LvResetBtnInteractable(skillMgr.IsSkillResetPossible);
            skillTreePresenter.SetSkillPointText(skillMgr.PlayerSkillPoint);

            skillDetailPresenter.Init(initData);
            skillDetailPresenter.gameObject.SetActive(false);

            skillEquipChangePresenter.Init(initData);
            skillEquipChangePresenter.gameObject.SetActive(false);
            AllPresenterAddListner();

        }
        void AllPresenterAddListner()
        {
            hub.OnSkillLevelChange += SkillLevelUpdate;
            hub.OnLevelChange += LockImgSet;
            hub.OnEquipSkillPriorityChange += SkillPriorityChangeFeat;
            hub.OnSkillEquipComplete += SkillPriorityChangeFeat;
            hub.OnSkillUnset += SkillPriorityChangeBtnUnset;

            skillTreePresenter.BtnEventAddListner(SkillDetailSet, SkillReset);

            skillDetailPresenter.BtnEventAddListner(
                () => SkillLevelOneUp(showSkillKey),
                () => SkillLevelMaxUp(showSkillKey),
                SkillEquipChangePopupShow,
                SkillPriorityChangeInSkillDetailView);

            skillEquipChangePresenter.BtnEventAddListner(SkillPriorityChangeInSkillEquipChangePopup);
        }
        void OnDestroy()
        {
            Debug.Log("DestroyFeat");
            OnDestroyFeat();
        }
        public void OnDestroyFeat()
        {
            hub.OnSkillLevelChange -= SkillLevelUpdate;
            hub.OnLevelChange -= LockImgSet;
            hub.OnEquipSkillPriorityChange -= SkillPriorityChangeFeat;
            hub.OnSkillEquipComplete -= SkillPriorityChangeFeat;
            hub.OnSkillUnset -= SkillPriorityChangeBtnUnset;

            skillTreePresenter.OnDestroyFeat();
            skillDetailPresenter.OnDestroyFeat();
            skillEquipChangePresenter.OnDestroyFeat();
        }
        int showSkillKey;
        void LockImgSet(int plLv)
        {
            var skillTreeBtnSetList = skillTreePresenter.SkillTreeUISetList;
            for (int i = 0; i < skillTreeBtnSetList.Count; i++)
            {
                int skillKey = skillTreeBtnSetList[i].SkillKey;
                bool isUnlock = skillMgr.IsSkillUnlock(skillKey);
                skillTreePresenter.SetSkillTreeBtnLockImg(skillKey, isUnlock);
                if (showSkillKey == skillKey) skillDetailPresenter.SkillLockImageSet(isUnlock);
            }
        }
        void SkillDetailSet(int key)
        {
            showSkillKey = key;
            skillDetailPresenter.SkillDetailDataSetToSkillChange(key);
        }
        void SkillLvUpBtnInteractable(int key)
        {
            bool isInteractable = skillMgr.TryGetSkillSO(key, out var so) && skillMgr.IsSkillLvUpPossibe(so);
            skillDetailPresenter.SkillLvUpBtnInteractable(isInteractable);
            skillTreePresenter.LvResetBtnInteractable(skillMgr.IsSkillResetPossible);
        }
        void SkillLevelOneUp(int key)
        {
            hub.SkillLevelOneUpInput(key);
            SkillLvUpBtnInteractable(key);
        }
        void SkillLevelMaxUp(int key)
        {
            hub.SkillLevelMaxUpInput(key);
            SkillLvUpBtnInteractable(key);
        }
        void SkillLevelUpdate(Skill skill)
        {
            int key = skill.SkillData.key;

            skillTreePresenter.SetSkillPointText(skillMgr.PlayerSkillPoint);
            skillTreePresenter.SkillLevelTextChange(key, skill.CurLv, skill.MaxLv);
            skillDetailPresenter.SkillDetailDataSetToSkillLvEnhance(key);
        }
        void SkillReset()
        {
            hub.SkillLevelResetInput();
            skillTreePresenter.LvResetBtnInteractable(skillMgr.IsSkillResetPossible);
        }
        void SkillEquipChangePopupShow()
        {
            skillEquipChangePresenter.SkillEquipChangePopupShow(showSkillKey);
        }
        public void SkillPriorityChangeFeat(int slotNum, ActiveSkill aSkill)
        {
            if (!skillMgr.TryGetSkillPriorityBySlotNum(slotNum, out Priority pri)) return;

            SkillPriorityChangeFeat(slotNum, pri);
        }
        void SkillPriorityChangeBtnUnset(int slotNum)
        {
            if (skillMgr.CheckEquipSkillKeyIsTargetKey(slotNum, showSkillKey)
                    && skillMgr.IsSkillEquippedBySlotNum(slotNum))
                skillDetailPresenter.SkillPriorityBtnUnset();
            if (skillEquipChangePresenter.gameObject.activeSelf)
                skillEquipChangePresenter.EquipSkillPriorityBtnUnset(slotNum);
        }
        void SkillPriorityChangeFeat(int slotNum, Priority pri)
        {
            if (skillMgr.CheckEquipSkillKeyIsTargetKey(slotNum, showSkillKey)
                    && skillMgr.IsSkillEquippedBySlotNum(slotNum))
                skillDetailPresenter.SkillPriorityBtnSet(pri);
            if (skillEquipChangePresenter.gameObject.activeSelf)
                skillEquipChangePresenter.EquipSkillPriorityBtnSet(slotNum, pri);
        }
        bool TryGetPriorityFromPriorityBtnClick(EquipSkill eSkill, out Priority pri)
        {
            pri = Priority.Low;
            if (!eSkill.isEquipped) return false;
            switch (eSkill.priority)
            {
                case Priority.High:
                    pri = Priority.Low;
                    break;
                case Priority.Mid:
                    pri = Priority.High;
                    break;
                case Priority.Low:
                    pri = Priority.Mid;
                    break;
            }
            return true;
        }
        void SkillPriorityChangeInSkillDetailView()
        {
            if (!skillMgr.TryGetEquipSkillByKey(showSkillKey, out var eSkill, out int slotNum))
                return;

            TryGetPriorityFromPriorityBtnClick(eSkill, out Priority changePri);
            // skillDetailPresenter.SkillPriorityBtnChange(changePri);
            hub.EquipSkillPriorityChange(slotNum, changePri);
        }
        void SkillPriorityChangeInSkillEquipChangePopup(int slotNum)
        {
            if (!skillMgr.TryGetEquipSkillBySlotNum(slotNum, out var eSkill)) return;

            TryGetPriorityFromPriorityBtnClick(eSkill, out Priority changePri);
            // skillDetailPresenter.SkillPriorityBtnChange(changePri);
            hub.EquipSkillPriorityChange(slotNum, changePri);
        }
    }
}