using Base.Data;
using Growth.Skill;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

namespace UI.Skill_Set
{
    public class SkillTreePresenter : MonoBehaviour
    {
        private SkillPopupUIManager owner;
        // private SkillManager skillMgr;
        private SkillPool pool;
        private EventHub hub;
        [SerializeField] private TextMeshProUGUI skillPointTxt;
        [SerializeField] private SkillTreeUISetView[] skillTreeUISetArr;
        [SerializeField] private SkillTreeUISetView skillTreeUISetPrefab;
        [SerializeField] private Button closeBtn;
        private Dictionary<int, SkillTreeUISetView> skillTreeUISetDic;
        [SerializeField] private Transform skillTreeUISetParentTransform;
        [SerializeField] private Button resetPointBtn;
        [SerializeField] private GridLayoutGroup layoutGroup;
        [SerializeField] private ContentSizeFitter sizeFitter;
        public void Init(UIPresenterInitData data)
        {
            owner = data.owner;
            pool = data.pool;
            hub = data.hub;
            // skillMgr = owner.SkillMgr;

            // ObjectSettingHelper.TryFindChild(skillTreeUISetParentTransform, out skillTreeUISetArr);
            SkillTreeBtnSetting();
            LayoutGroupAndSizeFitterOff();
        }
        void SkillTreeBtnSetting()
        {
            var allSkillArr = pool.AllSkillArr;
            int length = allSkillArr.Length;
            int activeCnt = pool.ActiveSkillCnt;
            skillTreeUISetArr = new SkillTreeUISetView[length];
            // layoutGroup.enabled = true;
            // sizeFitter.enabled = true;
            // 패시브부터 주입
            for(int i = activeCnt; i < length; i++)
            {
                skillTreeUISetArr[i] = Instantiate(skillTreeUISetPrefab, skillTreeUISetParentTransform);
            }
            // 액티브 주입
            for(int i = 0; i < activeCnt; i++)
            {
                skillTreeUISetArr[i] = Instantiate(skillTreeUISetPrefab, skillTreeUISetParentTransform);
            }
        }
        async UniTaskVoid LayoutGroupAndSizeFitterOffTask()
        {
            layoutGroup.enabled = true;
            sizeFitter.enabled = true;
            await UniTask.NextFrame();
            layoutGroup.enabled = false;
            sizeFitter.enabled = false;
        }
        public void LayoutGroupAndSizeFitterOff()
        {
            LayoutGroupAndSizeFitterOffTask().Forget();
        }
        public void SkillTreeUISetByPool(int key)
        {
            if (!pool.TryGetSkillByKey(key, out var skill)) return;
            var so = skill.SkillData;
            bool isHomingSkill = (so is ActiveSkillSO aSO) && (aSO.Targeting == TargetingMode.Homing);
            SkillTreeUISetViewNeedsImageData skillTreeUIData
             = new SkillTreeUISetViewNeedsImageData(so.key, skill.CurLv, so.maxLv, so.skillIcon, isHomingSkill);

            skillTreeUISetDic[key].SkillTreerUISet(skillTreeUIData);
        }
        // public void BtnEventAddListner(Action<int> skillDetailShowFunc, Action skillLevelResetFunc, Action skillPopupClose)
        public void BtnEventAddListner(Action<int> skillDetailShowFunc, Action skillLevelResetFunc)
        {
            var skillDatas = pool.AllSkillArr;
            int skillsCnt = skillDatas.Length;
            int skillTreeLength = skillTreeUISetArr.Length;

            skillTreeUISetDic = new Dictionary<int, SkillTreeUISetView>(skillTreeLength);
            for (int i = 0; i < skillTreeLength; i++)
            {
                if (skillsCnt <= i) break;
                int index = i;
                var skill = skillDatas[index];
                var skillTreeUI = skillTreeUISetArr[index];
                int key = skill.SkillData.key;
                skillTreeUISetDic.Add(key, skillTreeUI);
                SkillTreeUISetByPool(key);
                skillTreeUI.BtnEventAddListner(() => skillDetailShowFunc(key));
            }
            resetPointBtn.onClick.AddListener(() => skillLevelResetFunc());
            // closeBtn.onClick.AddListener(() => skillPopupClose());
        }
        public void SkillTreeUISet(int key, SkillDatas data)
        {
            var so = data.so;
            bool isHomingSkill = (so is ActiveSkillSO aSO) && (aSO.Targeting == TargetingMode.Homing);
            SkillTreeUISetViewNeedsImageData skillTreeUIData
             = new SkillTreeUISetViewNeedsImageData(so.key, data.level, so.maxLv, so.skillIcon, isHomingSkill);


            skillTreeUISetDic[key].SkillTreerUISet(skillTreeUIData);
        }
        public void SkillLevelTextChange(int key, int curLv, int maxLv) => skillTreeUISetDic[key].SetLvText(curLv, maxLv);
        // public void BtnSetInitAndEventSubscribe(Action<int> skillDetailShowFunc, Action skillLevelResetFunc)
        // {
        //     var skillDatas = skillMgr.GetAllSkillInfo();
        //     int forCnt = 0;
        //     int skillsCnt = skillDatas.Count;
        //     int skillTreeLength = skillTreeUISetArr.Length;

        //     if (skillsCnt < skillTreeLength)
        //     {
        //         Debug.Log("저장된 스킬보다 스킬트리 버튼이 많음");
        //         forCnt = skillsCnt;
        //     }
        //     else if (skillsCnt > skillTreeLength)
        //     {
        //         Debug.Log("스킬트리 버튼보다 저장된 스킬이 많음");
        //         forCnt = skillTreeLength;
        //     }
        //     skillTreeUISetDic = new Dictionary<int, SkillTreeUISetView>(skillTreeLength);
        //     for (int i = 0; i < forCnt; i++)
        //     {
        //         int index = i;
        //         var skillData = skillDatas[index];
        //         var skillTreeUI = skillTreeUISetArr[index];
        //         int key = skillData.so.key;
        //         skillTreeUISetDic.Add(key, skillTreeUI);
        //         SkillTreeUISet(key, skillData);
        //         // 버튼 클릭 -> detail view에 상세 내용 노출
        //         // -> 사이 : 해당 스킬의 key를 확인, key를 통해 skill so 추출, skill so를 통해 필요한 내용들 추출
        //         skillTreeUI.BtnEventSubscribe(() => skillDetailShowFunc(key));
        //     }
        //     resetPointBtn.onClick.AddListener(() => skillLevelResetFunc());
        // }
        public void OnDestroyFeat()
        {
            BtnEventRemoveListner();
        }
        void BtnEventRemoveListner()
        {
            foreach (SkillTreeUISetView skillTreeUI in skillTreeUISetArr)
            {
                skillTreeUI.BtnEventRemoveAllListner();
            }
            resetPointBtn.onClick.RemoveAllListeners();
        }
        #region SkillPopupPresenter 내부 UI 기능
        public void SetSkillPointText(int curLv, int maxLv)
         => skillPointTxt.text = $"보유 SP\n{curLv} / {maxLv}";
        #endregion
    }
}