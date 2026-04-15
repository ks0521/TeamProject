using Base.Data;
using Base.Managers;
using Battle;
using Growth.Skill;
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
            hub.OnEquipSkillPriorityChange += PriorityChange;

            skillTreePresenter.BtnEventAddListner(SkillDetailSet, SkillReset);

            skillDetailPresenter.BtnEventAddListner(
                () => SkillLevelOneUp(showSkillKey),
                () => SkillLevelMaxUp(showSkillKey),
                SkillEquipChangePopupShow,
                SkillPriorityChangeInSkillDetailView);

            skillEquipChangePresenter.BtnEventAddListner();
        }
        void SkillPriorityChangeInSkillDetailView()
        {
            var eSkill = skillMgr.PlayerEquipSkillList[showSkillKey];
            if (!eSkill.isEquipped) return;

            Priority changePri = Priority.Low;

            switch (eSkill.priority)
            {
                case Priority.High:
                    changePri = Priority.Low;
                    break;
                case Priority.Mid:
                    changePri = Priority.High;
                    break;
                case Priority.Low:
                    changePri = Priority.Mid;
                    break;
            }
            PriorityChange(showSkillKey, changePri);
        }
        void PriorityChange(int slotNum, Priority pri)
        {
            skillMgr.PlayerEquipSkillList[slotNum].priority = pri;
            skillMgr.TryChangeSkillPriority(slotNum, pri);
            if (slotNum == showSkillKey) skillDetailPresenter.SkillPriorityBtnChange(pri);
            if (skillEquipChangePresenter.gameObject.activeSelf) skillEquipChangePresenter.SkillPriorityChange(slotNum, pri);
        }
        void SkillPriorityChangeInEquipChangeView(int slotNum)
        {
            var eSkill = skillMgr.PlayerEquipSkillList[slotNum];
            if (!eSkill.isEquipped) return;

            Priority changePri = Priority.Low;

            switch (eSkill.priority)
            {
                case Priority.High:
                    changePri = Priority.Low;
                    break;
                case Priority.Mid:
                    changePri = Priority.High;
                    break;
                case Priority.Low:
                    changePri = Priority.Mid;
                    break;
            }
            PriorityChange(showSkillKey, changePri);
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
            hub.OnEquipSkillPriorityChange -= PriorityChange;

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
    }
}