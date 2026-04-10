using Base.Data;
using Base.Managers;
using Battle;
<<<<<<< HEAD:Assets/Game/Personal/HagYun/Script/Skill/UI/PlayerSkillUIManager.cs
using System.Collections;
using System.Collections.Generic;
=======
using Growth.Skill;
>>>>>>> main:Assets/Game/UI/UI/Scripts/UI_Set/Skill_Set/SkillPopupUIManager.cs
using UnityEngine;

namespace UI.Skill_Set
{
<<<<<<< HEAD:Assets/Game/Personal/HagYun/Script/Skill/UI/PlayerSkillUIManager.cs
    public class PlayerSkillUIManager : MonoBehaviour, IManager
=======
    public struct UIPresenterInitData
    {
        public SkillPopupUIManager owner;
        public SkillManager skillMgr;
        public SkillPool pool;
        public EventHub hub;
        public UIPresenterInitData(SkillPopupUIManager owner, SkillManager skillMgr, SkillPool pool, EventHub hub)
        {
            this.owner = owner;
            this.skillMgr = skillMgr;
            this.pool = pool;
            this.hub = hub;
        }
    }
    public class SkillPopupUIManager : MonoBehaviour
>>>>>>> main:Assets/Game/UI/UI/Scripts/UI_Set/Skill_Set/SkillPopupUIManager.cs
    {
        [SerializeField] private SkillPopupPresenter skillTreePopupPresenter;

<<<<<<< HEAD:Assets/Game/Personal/HagYun/Script/Skill/UI/PlayerSkillUIManager.cs

        [SerializeField] private SkillPool skillPool;
        public SkillPool SkillPool => skillPool;

        Player pl;
        [SerializeField] private EquipSkillController esController;

        [SerializeField] EventHub eventHub;
        void Start()
=======
        [SerializeField] private SkillTreePresenter skillTreePresenter;
        [SerializeField] private SkillDetailPresenter skillDetailPresenter;
        [SerializeField] private SkillEquipChangePresenter skillEquipChangePresenter;
        public int curPoint;
        public int maxPoint;
        void OnEnable()
>>>>>>> main:Assets/Game/UI/UI/Scripts/UI_Set/Skill_Set/SkillPopupUIManager.cs
        {
            Init();
        }
        public void Init()
        {
<<<<<<< HEAD:Assets/Game/Personal/HagYun/Script/Skill/UI/PlayerSkillUIManager.cs
            if (skillTreePopupPresenter != null) skillTreePopupPresenter.Init();
            if (skillPool != null) skillPool.Init();
            if (esController != null) esController.Init(pl);
=======
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

            UIPresenterInitData initData = new UIPresenterInitData(this, skillMgr, pool, hub);

            skillTreePresenter.Init(initData);
            skillTreePresenter.SetSkillPointText(curPoint, maxPoint);

            skillDetailPresenter.Init(initData);
            skillDetailPresenter.gameObject.SetActive(false);

            skillEquipChangePresenter.Init(initData);
            skillEquipChangePresenter.gameObject.SetActive(false);
            AllPresenterAddListnerByPool();

        }
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
>>>>>>> main:Assets/Game/UI/UI/Scripts/UI_Set/Skill_Set/SkillPopupUIManager.cs
        }
        void OnDestroy()
        {
            if (skillTreePopupPresenter != null) skillTreePopupPresenter.OnDestroyFeat();
        }
<<<<<<< HEAD:Assets/Game/Personal/HagYun/Script/Skill/UI/PlayerSkillUIManager.cs
        public int GetOrder() => 100;
=======
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
>>>>>>> main:Assets/Game/UI/UI/Scripts/UI_Set/Skill_Set/SkillPopupUIManager.cs
    }
}