using Growth.Skill;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Personal.HagYun
{
    public enum PassiveSkillType { Attack, Range, MultipleNormalAttack }
    public enum ActiveSkillType { FireBall, WaterBall, ElectricBall, FireCircle, IceFall, Lightning, }
    public class SkillPopupPresenter : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI skillPointTxt;
        [SerializeField] private SkillTreeUISetView[] skillTreeUISetArr;
        [SerializeField] private SkillDetailView skillDetailView;
        [SerializeField] private Button resetPointBtn;
        // test
        [Serializable]
        public struct SkillLvSet
        {
            public SkillSO data;
            public int openLv;
            public int openPointCnt;
            public int maxLv;
            public int curLv;
        }
        [Serializable]
        public struct SkillPointSet
        {
            public int curPoint;
            public int usePoint;
            public int maxPoint;
        }
        public SkillLvSet[] skillLvSetArr;
        public SkillPointSet skillPoint;
        public int curPlLv;
        [SerializeField] private int selectSkillUINum;
        public event Action<int> OnSkillLevelChange;
        public event Action<int, int> OnSkillLevelChangeUpdate;
        public void Init()
        {
            for (int i = 0; i < skillTreeUISetArr.Length; i++)
            {
                int index = i;
                var skillTreeUI = skillTreeUISetArr[index];
                ref var skillLvSet = ref skillLvSetArr[index];
                skillTreeUI.SetSkillDetailsBtn(skillLvSet.data, index);
                skillTreeUI.BtnEventSubscribe(() => SkillDetailsPopupShow(index));
            }
            InitSkillLevelUpdate();
            EventSubscribe();
        }
        void EventSubscribe()
        {
            skillDetailView.BtnEventSubscribe(SkillLevelUp, SkillLevelUpMax);
            resetPointBtn.onClick.AddListener(() => SkillLevelResetAll());
        }
        public void OnDestroyFeat()
        {
            EventUnsubscribe();
        }
        void EventUnsubscribe()
        {
            foreach (SkillTreeUISetView skillTreeUI in skillTreeUISetArr)
            {
                skillTreeUI.BtnEventUnsubscribe();
            }
            resetPointBtn.onClick.RemoveAllListeners();
        }
        #region SkillPopupPresenter 내부 UI 기능
        public void SkillLevelResetAll()
        {
            for (int i = 0; i < skillLvSetArr.Length; i++)
            {
                SkillLevelReset(i);
            }
        }
        public void SetSkillPointText()
         => skillPointTxt.text = $"보유 SP : {skillPoint.curPoint} / {skillPoint.maxPoint}";
        #endregion

        #region SkillTreeUISetView 내부 UI 기능
        public void SkillDetailsPopupShow(int value)
        {
            selectSkillUINum = value;
            GameObject skillDetailViewObject = skillDetailView.gameObject;
            if (!skillDetailViewObject.activeSelf)
            {
                skillDetailViewObject.SetActive(true);
            }
            ref var skillLvSet = ref skillLvSetArr[value];
            skillDetailView.SkillDataShow(skillLvSet.data, skillLvSet.curLv, skillLvSet.maxLv);
        }
        #endregion
        #region SkillDetailView 내부 UI 기능
        public void SkillLevelUpBtnInteractable(bool isLevelUpBtnOn)
        {
            skillDetailView.SkillLevelUpBtnInteractable(isLevelUpBtnOn);
        }
        public void SkillLevelChangeUpdate(bool isInit = false)
        {
            ref SkillLvSet lvSet = ref skillLvSetArr[selectSkillUINum];
            int curLv = lvSet.curLv;
            int maxLv = lvSet.maxLv;
            if (skillTreeUISetArr[selectSkillUINum] is SkillTreeUISetView stSet)
                stSet.SetLvText(curLv, maxLv);
            SetSkillPointText();
            if (!isInit && skillDetailView != null)
                skillDetailView.SkillLevelShow(curLv, maxLv);
        }
        void InitSkillLevelUpdate()
        {
            for (int i = 0; i < skillTreeUISetArr.Length; i++)
            {
                selectSkillUINum = i;
                SkillLevelChangeUpdate(true);
            }
            selectSkillUINum = 0;
        }
        public void SkillLevelUp()
        {
            if (skillPoint.curPoint <= 0) return;
            ref SkillLvSet lvSet = ref skillLvSetArr[selectSkillUINum];
            if (lvSet.maxLv <= lvSet.curLv) return;
            lvSet.curLv++;
            skillPoint.curPoint--;
            skillPoint.usePoint++;
            SkillLevelChangeUpdate();
        }
        public void SkillLevelUpMax()
        {
            if (skillPoint.curPoint <= 0) return;
            ref SkillLvSet lvSet = ref skillLvSetArr[selectSkillUINum];
            if (lvSet.maxLv <= lvSet.curLv) return;
            int maxLvCnt = lvSet.maxLv - lvSet.curLv;
            if (skillPoint.curPoint < maxLvCnt) maxLvCnt = skillPoint.curPoint;
            lvSet.curLv += maxLvCnt;
            skillPoint.curPoint -= maxLvCnt;
            skillPoint.usePoint += maxLvCnt;
            SkillLevelChangeUpdate();
        }
        public void SetSkillLevel(int lv)
        {
            if (skillPoint.curPoint <= 0) return;
            ref SkillLvSet lvSet = ref skillLvSetArr[selectSkillUINum];
            if (lv < 0 || lvSet.maxLv < lv) return;

            else if (skillPoint.curPoint < lv)
                lv = skillPoint.curPoint;
            lvSet.curLv = lv;
            skillPoint.curPoint -= lv;
            skillPoint.usePoint += lv;
            SkillLevelChangeUpdate();
        }
        public void SkillLevelReset(int index)
        {
            if (skillPoint.maxPoint <= skillPoint.curPoint)
            {
                return;
            }
            ref SkillLvSet lvSet = ref skillLvSetArr[index];
            if (lvSet.curLv <= 0) return;
            skillPoint.curPoint += lvSet.curLv;
            skillPoint.usePoint -= lvSet.curLv;
            lvSet.curLv = 0;
            if (skillPoint.maxPoint < skillPoint.curPoint)
            {
                Debug.LogWarning($"스킬 포인트가 최대치를 {skillPoint.curPoint - skillPoint.maxPoint} 만큼 넘김, 최대치로 맞춤");
                skillPoint.curPoint = skillPoint.maxPoint;
                skillPoint.usePoint = 0;
            }
            SkillLevelChangeUpdate();
        }
        public void SkillLevelReset() => SkillLevelReset(selectSkillUINum);


        #endregion
    }
}