using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Personal.HagYun
{
    public enum SkillType { Passive, Active }
    public enum PassiveSkillType { Attack, Range, MultipleNormalAttack }
    public enum ActiveSkillType { FireBall, WaterBall, ElectricBall, FireCircle, IceFall, Lightning, }
    public class SkillTreePopupPresenter : MonoBehaviour
    {
        [Serializable]
        public struct SkillLvSet
        {
            public static SkillTreePopupPresenter owner;
            public int openLv;
            public int maxLv;
            public int curLv;
        }
        [SerializeField] private TextMeshProUGUI skillPointTxt;
        [SerializeField] private SkillTreeUISetView[] skillTreeUIArr;
        [SerializeField] private SkillLvSet[] skillLvSetArr;
        [SerializeField] private int curPlLv;

        public event Action<int> OnSkillLevelChange;
        public event Action<int, int> OnSkillLevelChangeUpdate;
        void Start()
        {
            SkillLvSet.owner = this;
            for (int i = 0; i < skillTreeUIArr.Length; i++)
            {
                int index = i;
                var skillTreeUI = skillTreeUIArr[index];
                ref var skillLvSet = ref skillLvSetArr[index];
                skillTreeUI.BtnEventSubscribe(() => SkillLvUp(index), () => SkillLvDown(index));
                SkillLvChangeUpdate(index);
            }
        }
        public void SkillLvChangeUpdate(int index)
        {
            ref SkillLvSet lvSet = ref skillLvSetArr[index];
            SkillTreeUISetView skillTreeUISet = skillTreeUIArr[index];
            if (lvSet.openLv < curPlLv)
            {
                skillTreeUISet.LvUpUIBtnUpdate(false);
                skillTreeUISet.LvDownUIBtnUpdate(false);
                return;
            }
            int curLv = lvSet.curLv;
            int maxLv = lvSet.maxLv;
            if (curLv <= 0)
            {
                skillTreeUISet.LvUpUIBtnUpdate(true);
                skillTreeUISet.LvDownUIBtnUpdate(false);
            }
            else if (maxLv <= curLv)
            {
                skillTreeUISet.LvUpUIBtnUpdate(false);
                skillTreeUISet.LvDownUIBtnUpdate(true);
            }
            else
            {
                skillTreeUISet.LvUpUIBtnUpdate(true);
                skillTreeUISet.LvDownUIBtnUpdate(true);
            }
            skillTreeUISet.LvTextSet(curLv, maxLv);
        }
        public void SkillLvUp(int index)
        {
            ref SkillLvSet lvSet = ref skillLvSetArr[index];
            if (lvSet.maxLv <= lvSet.curLv) return;
            lvSet.curLv++;
            SkillLvChangeUpdate(index);
        }
        public void SkillLvDown(int index)
        {
            ref SkillLvSet lvSet = ref skillLvSetArr[index];
            if (lvSet.curLv <= 0) return;
            lvSet.curLv--;
            SkillLvChangeUpdate(index);
        }
        public void SkillReset(int index)
        {
            skillLvSetArr[index].curLv = 0;
        }
        public void SetSkillLv(int index, int lv)
        {
            ref SkillLvSet lvSet = ref skillLvSetArr[index];
            if (lv < 0 || lvSet.maxLv < lv) return;
            lvSet.curLv = lv;
        }
        public void SkillPointUpdate(int curPoint, int maxPoint) => skillPointTxt.text = $"{curPoint} / {maxPoint}";


    }
}