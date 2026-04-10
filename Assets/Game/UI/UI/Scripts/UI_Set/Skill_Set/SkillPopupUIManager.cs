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
            // pl = GameManager.Instance.GetGameSystem<PlayerManager>().Player;
            hub = GameManager.Instance.GetGameSystem<EventHub>();

            skillTreePresenter = GetComponentInChildren<SkillTreePresenter>();
            skillDetailPresenter = GetComponentInChildren<SkillDetailPresenter>(true);
            skillEquipChangePresenter = GetComponentInChildren<SkillEquipChangePresenter>(true);

            UIPresenterInitData initData = new UIPresenterInitData(skillMgr, hub);

            skillTreePresenter.Init(initData);
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
            
            skillTreePresenter.BtnEventAddListner(SkillDetailSet, SkillReset);

            skillDetailPresenter.BtnEventAddListner(
                () => SkillLevelOneUp(showSkillKey),
                () => SkillLevelMaxUp(showSkillKey),
                SkillEquipChangePopupShow);

            skillEquipChangePresenter.BtnEventAddListner();
        }
        void OnDestroy()
        {
            OnDestroyFeat();
        }
        public void OnDestroyFeat()
        {
            skillTreePresenter.OnDestroyFeat();
            skillDetailPresenter.OnDestroyFeat();
            skillEquipChangePresenter.OnDestroyFeat();
        }
        int showSkillKey;
        void SkillDetailSet(int key)
        {
            showSkillKey = key;
            skillDetailPresenter.SkillDetailDataSetToSkillChange(key);
        }
        void SkillLevelOneUp(int key) => hub.SkillLevelOneUpInput(key);
        void SkillLevelMaxUp(int key) => hub.SkillLevelMaxUpInput(key);
        void SkillLevelUpdate(Skill skill)
        {
            int key = skill.SkillData.key;
            skillTreePresenter.SetSkillPointText(skillMgr.PlayerSkillPoint);
            skillTreePresenter.SkillLevelTextChange(key, skill.CurLv, skill.MaxLv);
            skillDetailPresenter.SkillDetailDataSetToSkillLvEnhance(key);
        }
        void SkillReset() => hub.SkillLevelResetInput();
        void SkillEquipChangePopupShow()
        {
            skillEquipChangePresenter.SkillEquipChangePopupShow(showSkillKey);
        }
    }
}