using Base.Data;
using Base.Managers;
using Battle;
using Cysharp.Threading.Tasks;
using Growth.Equipment;
using Growth.Skill;
using System;
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
    public class SkillPopupPresenter : MonoBehaviour, IMemberReceiver
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
            skillDetailView.BtnEventUnsubscribe();
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
            selectSkillUINum = value;
            GameObject skillDetailViewObject = skillDetailView.gameObject;
            if (!skillDetailViewObject.activeSelf)
            {
                skillDetailViewObject.SetActive(true);
            }
            ref var skill = ref skillLvSetArr[value].skill;
            if (skill is ActiveSkill aSkill)
                equipTargetSkill = aSkill;
            SkillDataShow(skill);

        }
        public void SkillDataShow(Skill skill)
        {
            if (skill == null) return;
            var data = skill.SkillData;
            skillDetailView.SkillNameChange(data.skillName);
            skillDetailView.SkillLevelChange(skill.CurLv, skill.MaxLv);
            if (data.Type == SkillType.Active)
            {
                skillDetailView.SkillDetailViewUIShowAndHide(true);
                
                var aSkill = (ActiveSkill)skill;

                skillDetailView.SkillCooltimeChange(aSkill.ActiveSkillData.coolDown);
                skillDetailView.SkillImgChange(data.skillIcon, aSkill.IsHomingSkill);

                ActiveSkillValueTextSet(aSkill);
                skillDetailView.SkillDescriptionChange(data.description);
            }
            else
            {
                skillDetailView.SkillDetailViewUIShowAndHide(false);
                skillDetailView.SkillImgChange(data.skillIcon, false);

                PassiveSkillValueTextSet((PassiveSkill)skill);

            }
        }
        void ActiveSkillValueTextSet(ActiveSkill aSkill)
        {
            skillDetailView.ActiveSkillStatValueTextInit();
            skillDetailView.SkillStatValueTextBuild(ActiveSkillDamageValueToString(aSkill.ResultDamage), false);
            skillDetailView.SkillStatValueTextChange();
        }
        void PassiveSkillValueTextSet(PassiveSkill pSkill)
        {
            skillDetailView.PassiveSkillStatValueTextInit();
            MemberExtractor<StatIncrease>.ExtractAll(pSkill.ResultSkillData, this);
            skillDetailView.SkillStatValueTextChange();
        }
        string ActiveSkillDamageValueToString(float damage) => $"{damage * 100}%";
        public void Receive(string name, int value) { }
        public void Receive(string name, float value)
        {
            if (value == 0) return;
            string contents = PassiveStatValueToString(name, value);
            if (!string.IsNullOrEmpty(contents)) skillDetailView.SkillStatValueTextBuild(contents, true);
        }
        public void Receive(string name, string value) { }
        public void ReceiveOther(string name, object value) { }
        string PassiveStatValueToString(string name, float value)
        {
            switch (name)
            {
                // 공격력 증가(상수)
                case nameof(StatIncrease.atk):
                    return $"공격력 + {value}";
                // 공격력 % 증가
                case nameof(StatIncrease.atkRate):
                    return ($"공격력 + {value * 100}%");
                // 피해량 증가
                case nameof(StatIncrease.damageDealtRate):
                    return ($"피해량 + {value * 100}%");
                // 치명타 확률 증가
                case nameof(StatIncrease.critChance):
                    return ($"치명타확률 + {value * 100}%");
                // 치명타 피해량 증가
                case nameof(StatIncrease.critDamage):
                    return ($"치명타피해량 + {value * 100}%");
                // HP 증가(상수)
                case nameof(StatIncrease.maxHp):
                    return ($"최대체력 + {value}");
                // HP % 증가
                case nameof(StatIncrease.maxHpRate):
                    return ($"최대체력 + {value * 100}%");
                // 방어력 비율 감소
                case nameof(StatIncrease.def):
                    return ($"방어력 + {value * 100}%");
                // 받는 피해 비율 감소
                case nameof(StatIncrease.damageReduction):
                    return ($"피해감소율 + {value * 100}%");
                // 이동속도 증가
                case nameof(StatIncrease.moveSpeed):
                    return ($"이동속도 + {value * 100}%");
                // 공격속도 증가
                case nameof(StatIncrease.atkSpeed):
                    return ($"공격속도 + {value * 100}%");
                // 아이템 드랍률 증가
                case nameof(StatIncrease.itemDropRate):
                    return ($"아이템드랍률 + {value * 100}%");
                // 골드 획득량 증가
                case nameof(StatIncrease.goldGain):
                    return ($"골드획득률 + {value * 100}%");
                // 경험치 획득량 증가
                case nameof(StatIncrease.expGain):
                    return ($"경험치획득률 + {value * 100}%");
                // 스탯 강화석 획득량 증가
                case nameof(StatIncrease.statStoneGain):
                    return ($"스탯강화석획득률 + {value * 100}%");
                default:
                    return null;
            }
        }
        #endregion
        #region SkillDetailView 내부 UI 기능
        public void SkillLevelUpBtnInteractable(bool isLevelUpBtnOn)
        {
            skillDetailView.SkillLevelUpBtnInteractable(isLevelUpBtnOn);
        }
        public void SkillLevelChangeUpdate(int skillUIIndex, bool isInit = false)
        {
            if (skillLvSetArr.Length <= skillUIIndex) return;
            var skill = skillLvSetArr[skillUIIndex].skill;
            int curLv = skill.CurLv;
            int maxLv = skill.MaxLv;
            if (skillTreeUISetArr[skillUIIndex] is SkillTreeUISetView stSet)
            {
                stSet.SetLvText(curLv, maxLv);
            }
            SetSkillPointText();
            if (!isInit && skillDetailView != null && skillDetailView.gameObject.activeSelf)
            {
                skillDetailView.SkillLevelChange(curLv, maxLv);
            }
            if (skill is ActiveSkill aSkill) ActiveSkillValueTextSet(aSkill);
            else if (skill is PassiveSkill pSkill) PassiveSkillValueTextSet(pSkill);
        }
        void InitSkillLevelUpdate()
        {
            for (int i = 0; i < skillTreeUISetArr.Length; i++)
            {
                int index = i;
                SkillLevelChangeUpdate(index, false);
            }
        }
        public void SkillLevelUp()
        {
            if (skillPoint.curPoint <= 0) return;
            else if (skillLvSetArr[selectSkillUINum].skill.TryLevelOneUp())
            {
                skillPoint.curPoint--;
                skillPoint.usePoint++;
                SkillLevelChangeUpdate(selectSkillUINum);
            }
        }
        public void SkillLevelUpMax()
        {
            ref int curPoint = ref skillPoint.curPoint;
            if (curPoint <= 0) return;
            else if (skillLvSetArr[selectSkillUINum].skill.TryLevelMaxUp(curPoint, out int lvUpCnt))
            {
                curPoint -= lvUpCnt;
                skillPoint.usePoint += lvUpCnt;
                SkillLevelChangeUpdate(selectSkillUINum);
            }
        }
        //public void SetSkillLevel(int setLv)
        //{
        //    ref int curPoint = ref skillPoint.curPoint;
        //    if (curPoint <= 0) return;
        //    else if (skillLvSetArr[selectSkillUINum].skill.TryLevelSet(curPoint, setLv, out int lvChangeCnt))
        //    {
        //        curPoint -= lvChangeCnt;
        //        skillPoint.usePoint += lvChangeCnt;
        //        SkillLevelChangeUpdate(selectSkillUINum);
        //    }
        //}
        public void SkillLevelReset(int index)
        {
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
                SkillLevelChangeUpdate(index);
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