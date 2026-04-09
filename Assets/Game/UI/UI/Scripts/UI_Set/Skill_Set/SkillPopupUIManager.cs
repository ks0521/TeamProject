using Base.Data;
using Base.Managers;
using Battle;
using Growth.Skill;
using UnityEngine;

namespace UI.Skill_Set
{
    public struct UIPresenterInitData
    {
        public SkillPopupUIManager owner;
        public SkillPool pool;
        public EventHub hub;
        public UIPresenterInitData(SkillPopupUIManager owner, SkillPool pool, EventHub hub)
        {
            this.owner = owner;
            this.pool = pool;
            this.hub = hub;
        }
    }
    public class SkillPopupUIManager : MonoBehaviour
    {
        private SkillManager skillMgr;
        public SkillManager SkillMgr => skillMgr;
        private Player pl;
        private SkillPool pool;
        // public PlayerEquipSkillController PEquipSkillController { get; private set; }
        private EventHub hub;
        // private IReadOnlyList<SkillDatas> allSkillList;

        [SerializeField] private SkillTreePresenter skillTreePresenter;
        [SerializeField] private SkillDetailPresenter skillDetailPresenter;
        [SerializeField] private SkillEquipChangePresenter skillEquipChangePresenter;
        public int curPoint;
        public int maxPoint;
        void OnEnable()
        {
            Init();
        }
        public void Init()
        {
            curPoint = maxPoint;
            skillMgr = GameManager.Instance.GetGameSystem<SkillManager>();
            pl = GameManager.Instance.GetGameSystem<PlayerManager>().Player;
            pool = pl.ESController.Pool;
            // PEquipSkillController = GameManager.Instance.GetGameSystem<PlayerManager>().Player.ESController;
            hub = GameManager.Instance.GetGameSystem<EventHub>();

            // allSkillList = skillMgr.GetAllSkillInfo();

            skillTreePresenter = GetComponentInChildren<SkillTreePresenter>();
            skillDetailPresenter = GetComponentInChildren<SkillDetailPresenter>(true);
            skillEquipChangePresenter = GetComponentInChildren<SkillEquipChangePresenter>(true);

            UIPresenterInitData initData = new UIPresenterInitData(this, pool, hub);

            skillTreePresenter.Init(initData);
            skillTreePresenter.SetSkillPointText(curPoint, maxPoint);

            skillDetailPresenter.Init(initData);
            skillDetailPresenter.gameObject.SetActive(false);

            skillEquipChangePresenter.Init(initData);
            skillEquipChangePresenter.gameObject.SetActive(false);
            AllPresenterAddListnerByPool();

        }
        // void AllPresenterAddListner()
        // {
        //     skillTreePresenter.BtnSetInitAndEventSubscribe(SkillDetailSet, SkillReset);
        //     var so = skillMgr.GetSkill(showSkillKey);

        //     eventHub.OnSkillEnhanced += skillTreePresenter.SetSkillPointText;
        // }
        void AllPresenterAddListnerByPool()
        {
            skillTreePresenter.BtnEventAddListner(SkillDetailSetByPool, SkillResetByPool);
            // skillTreePresenter.BtnEventAddListner(SkillDetailSetByPool, SkillResetByPool, () => Destroy(gameObject));

            skillDetailPresenter.BtnEventAddListner(
                () => SkillLevelOneUpByPool(showSkillKey),
                () => SkillLevelMaxUpByPool(showSkillKey),
                SkillEquipChangePopupShow);

            // skillEquipChangePresenter.BtnEventAddListner(SkillEquipChangeByPool);
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
        // void SkillEquipChangeByPool(int slot) => hub.SkillEquip(slot, showSkillKey);
        void SkillDetailSetByPool(int key)
        {
            showSkillKey = key;
            skillDetailPresenter.SkillDetailDataSetToSkillChangeByPool(key);
        }
        void SkillLevelOneUpByPool(int key)
        {
            if (!pool.TryGetSkillByKey(key, out var skill)) return;
            else if (skill.TryLevelOneUp(curPoint, out int lvUpCnt))
            {
                curPoint -= lvUpCnt;
                SkillLevelUpdateByPool(key);
            }
        }
        void SkillLevelMaxUpByPool(int key)
        {
            if (!pool.TryGetSkillByKey(key, out var skill)) return;
            else if (skill.TryLevelMaxUp(curPoint, out int lvUpCnt))
            {
                curPoint -= lvUpCnt;
                SkillLevelUpdateByPool(key);
            }
        }
        void SkillLevelUpdateByPool(int key)
        {
            if (!pool.TryGetSkillByKey(key, out var skill)) return;
            skillTreePresenter.SetSkillPointText(curPoint, maxPoint);
            skillTreePresenter.SkillLevelTextChange(key, skill.CurLv, skill.MaxLv);
            skillDetailPresenter.SkillDetailDataSetToSkillLvEnhanceByPool(key);
        }
        void SkillResetByPool()
        {
            var allSkills = pool.AllSkillArr;
            foreach (var skill in allSkills)
            {
                if (skill.TryLevelReset(out int resetPoint))
                {
                    curPoint += resetPoint;
                    SkillLevelUpdateByPool(skill.SkillData.key);
                }
            }
        }
        void SkillEquipChangePopupShow()
        {
            skillEquipChangePresenter.SkillEquipChangePopupShow(showSkillKey);
        }
    }
}