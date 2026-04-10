using Base.Data;
using Base.Managers;
using Battle;
using Cysharp.Threading.Tasks;
using Growth.Equipment;
using Growth.Skill;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Personal.HagYun
{
    public class FindChildObject
    {
        readonly Transform parentTransform;
        public FindChildObject(Transform parentTransform)
        {
            this.parentTransform = parentTransform;
        }
        public bool TryFindChild<T>(out T[] childs)
        {
            childs = parentTransform.GetComponentsInChildren<T>();
            if (childs == null)
            {
                Debug.LogWarning($"{typeof(T)} 컴포넌트를 가진 자식 오브젝트 없음");
                return false;
            }
            return true;
        }
    }
    public enum PassiveSkillType { Attack, Range, MultipleNormalAttack }
    public enum ActiveSkillType { FireBall, WaterBall, ElectricBall, FireCircle, IceFall, Lightning, }
    public class SkillPopupPresenter : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI skillPointTxt;
        [SerializeField] private SkillTreeUISetView[] skillTreeUISetArr;
        [SerializeField] private Transform skillTreeUISetParentTransform;
        [SerializeField] private SkillDetailView skillDetailView;
        [SerializeField] private SkillEquipChangePopupView skillEquipChangePopupView;
        [SerializeField] private Button resetPointBtn;
        // test
        [Serializable]
        public struct SkillLvSet
        {
            public Skill skill;
            public SkillSO Data => skill.SkillData;
            public int openLv;
            public int openPointCnt;
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
        private Player pl;
        private EventHub eventHub;

        public Skill[] saveSkill;
        public Transform skillEmptyTransform;
        // test
        // slot index, skill
        public event Action<int, ActiveSkill> OnSkillEquip;
        public event Action<int> OnSkillSlotUnequip;
        public static SkillPopupPresenter instanse;
        public ActiveSkill[] activeSkillArr = new ActiveSkill[6];
        void SkillEquip(int slotIndex)
        {
            int slotIndexInSkill = equipTargetSkill.EquipSlotIndex;
            if (slotIndexInSkill == slotIndex) return;
            else if (slotIndexInSkill != -1) SkillUnequip(slotIndexInSkill);
            activeSkillArr[slotIndex] = equipTargetSkill;
            equipTargetSkill.EquipSkillSlotIndexUpdate(slotIndex);
            OnSkillEquip?.Invoke(slotIndex, equipTargetSkill);
            SkillEquipPopupClose();
        }
        void SkillUnequip(int slotIndex)
        {
            if (activeSkillArr == null) return;
            if (activeSkillArr[slotIndex] == null) return;
            activeSkillArr[slotIndex].EquipSkillSlotIndexUpdate(-1);
            activeSkillArr[slotIndex] = null;
        }
        public void Init()
        {
            instanse = this;
            // pl = GameManager.Instance.GetGameSystem<PlayerManager>().Player;
            // eventHub = GameManager.Instance.GetGameSystem<EventHub>();

            FindChildObject findChild = new FindChildObject(skillTreeUISetParentTransform);
            findChild.TryFindChild(out skillTreeUISetArr);


            int lvSetCnt = saveSkill.Length;
            skillLvSetArr = new SkillLvSet[lvSetCnt];
            for (int i = 0; i < skillTreeUISetArr.Length; i++)
            {
                if (lvSetCnt <= i)
                {
                    Debug.Log($"{i}번까지 버튼 셋팅 완료");
                    break;
                }
                int index = i;
                var skillTreeUI = skillTreeUISetArr[index];
                //test
                if (skillEmptyTransform != null)
                {
                    ref var skill = ref skillLvSetArr[index].skill;
                    skill = Instantiate(saveSkill[index], skillEmptyTransform);
                    skill.Init(null);
                }
                //
                skillTreeUI.SetSkillDetailsBtn(skillLvSetArr[index].skill, index);
                skillTreeUI.BtnEventSubscribe(() => SkillDetailsPopupShow(index));
            }
            InitSkillLevelUpdate();
            EventSubscribe();
        }
        void EventSubscribe()
        {
            skillDetailView.BtnEventSubscribe(SkillLevelUp, SkillLevelUpMax, SkillEquipPopupShow);
            resetPointBtn.onClick.AddListener(() => SkillLevelResetAll());
            for (int i = 0; i < 6; i++)
            {
                int index = i;
                skillEquipChangePopupView.EquipSlotSelectBtnEventSubscribe(index, () => SkillEquip(index));

            }

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
            skillEquipChangePopupView.BtnEventUnsubscribe();
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
            if(skillLvSetArr == null || skillLvSetArr.Length <= selectSkillUINum)return;
            var skill = skillLvSetArr[selectSkillUINum].skill;
            if(skill == null)return;
            selectSkillUINum = value;
            GameObject skillDetailViewObject = skillDetailView.gameObject;
            if (!skillDetailViewObject.activeSelf)
            {
                skillDetailViewObject.SetActive(true);
            }
            // ref var skill = ref skillLvSetArr[value].skill;
            skillDetailView.SkillDataShow(skill);
            if (skill is ActiveSkill aSkill)
                equipTargetSkill = aSkill;

        }
        #endregion
        #region SkillDetailView 내부 UI 기능
        public void SkillLevelUpBtnInteractable(bool isLevelUpBtnOn)
        {
            skillDetailView.SkillLevelUpBtnInteractable(isLevelUpBtnOn);
        }
        public void SkillLevelChangeUpdate(bool isInit = false)
        {
            // ref SkillLvSet lvSet = ref skillLvSetArr[selectSkillUINum];
            if(skillLvSetArr == null || skillLvSetArr.Length <= selectSkillUINum)return;
            var skill = skillLvSetArr[selectSkillUINum].skill;
            if(skill == null)return;
            if (skillTreeUISetArr[selectSkillUINum] is SkillTreeUISetView stSet)
            {
                stSet.SetLvText(skill.CurLv, skill.MaxLv);
            }
            SetSkillPointText();
            if (!isInit && skillDetailView != null && skillDetailView.gameObject.activeSelf)
            {
                skillDetailView.SkillValueChange(skill);
            }
        }
        void InitSkillLevelUpdate()
        {
            for (int i = 0; i < skillTreeUISetArr.Length; i++)
            {
                selectSkillUINum = i;
                SkillLevelChangeUpdate(false);
            }
            selectSkillUINum = 0;
        }
        public void SkillLevelUp()
        {
            if(skillLvSetArr == null || skillLvSetArr.Length <= selectSkillUINum)return;
            var skill = skillLvSetArr[selectSkillUINum].skill;
            if(skill == null)return;
            if (skillPoint.curPoint <= 0) return;
            else if (skillLvSetArr[selectSkillUINum].skill.TryLevelOneUp())
            {
                skillPoint.curPoint--;
                skillPoint.usePoint++;
                SkillLevelChangeUpdate();
            }
        }
        public void SkillLevelUpMax()
        {
            if(skillLvSetArr == null || skillLvSetArr.Length <= selectSkillUINum)return;
            var skill = skillLvSetArr[selectSkillUINum].skill;
            if(skill == null)return;
            if (skillPoint.curPoint <= 0) return;
            else if (skillLvSetArr[selectSkillUINum].skill.TryLevelMaxUp(out int lvUpCnt))
            {
                skillPoint.curPoint -= lvUpCnt;
                skillPoint.usePoint += lvUpCnt;
                SkillLevelChangeUpdate();
            }
        }
        public void SetSkillLevel(int lv)
        {
            if(skillLvSetArr == null || skillLvSetArr.Length <= selectSkillUINum)return;
            var skill = skillLvSetArr[selectSkillUINum].skill;
            if(skill == null)return;
            if (skillPoint.curPoint <= 0) return;
            else if (skillLvSetArr[selectSkillUINum].skill.TryLevelSet(lv, out int lvChangeCnt))
            {
                skillPoint.curPoint -= lvChangeCnt;
                skillPoint.usePoint += lvChangeCnt;
                SkillLevelChangeUpdate();
            }
        }
        public void SkillLevelReset(int index)
        {
            if(skillLvSetArr == null || skillLvSetArr.Length <= selectSkillUINum)return;
            var skill = skillLvSetArr[selectSkillUINum].skill;
            if(skill == null)return;
            if (skillPoint.maxPoint <= skillPoint.curPoint)
            {
                return;
            }
            else if (skillLvSetArr[index].skill.TryLevelReset(out int lvResetCnt))
            {
                skillPoint.curPoint += lvResetCnt;
                skillPoint.usePoint -= lvResetCnt;
                if (skillPoint.maxPoint < skillPoint.curPoint)
                {
                    Debug.LogWarning($"스킬 포인트가 최대치를 {skillPoint.curPoint - skillPoint.maxPoint} 만큼 넘김, 최대치로 맞춤");
                    skillPoint.curPoint = skillPoint.maxPoint;
                    skillPoint.usePoint = 0;
                }
                SkillLevelChangeUpdate();
            }
        }
        public void SkillLevelReset() => SkillLevelReset(selectSkillUINum);


        public void SkillEquipPopupShow()
        {
            skillEquipChangePopupView.EquipSkillShow(equipTargetSkill, activeSkillArr);
            skillEquipChangePopupView.gameObject.SetActive(true);
        }
        #endregion
        #region Skill Equip State
        ActiveSkill equipTargetSkill;
        bool isPopupClose;
        void PopupCloseTrigger() => isPopupClose = true;
        public void SkillEquipPopupClose()
        {
            skillEquipChangePopupView.gameObject.SetActive(false);
        }
        #endregion
    }
}