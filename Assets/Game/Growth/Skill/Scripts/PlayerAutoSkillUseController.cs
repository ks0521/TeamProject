using System;
using Base.Utils;
using System.Collections.Generic;
using UnityEngine;
using Base.Data;

namespace Growth.Skill
{
    public struct NeedsDataFromAutoSkillUseController
    {
        public EquipSkillController equipSkillController;
        public IReadOnlyList<EquipSkill> equipSkills;
        public EventHub hub;
        public NeedsDataFromAutoSkillUseController(EquipSkillController equipSkillController,
        IReadOnlyList<EquipSkill> equipSkills, EventHub hub)
        {
            this.equipSkillController = equipSkillController;
            this.equipSkills = equipSkills;
            this.hub = hub;
        }
    }
    [Serializable]
    public class PlayerAutoSkillUseController
    {
        private SequenceNode rootSequence;

        private PlayerEquipSkillController playerEquipSkillController;
        private IReadOnlyList<EquipSkill> equipSkills;
        private EventHub hub;
        [SerializeField] private bool isAutoSkillUse;
        public bool IsAutoSkillUse => isAutoSkillUse;
        [SerializeField] private int curIndex;
        public PlayerAutoSkillUseController(NeedsDataFromAutoSkillUseController data)
        {
            if (data.equipSkillController is PlayerEquipSkillController playerEquipSkillController)
            {
                this.playerEquipSkillController = playerEquipSkillController;
            }
            else
            {
                Debug.LogWarning("AutoSkillController : equipSkillController 없음");
            }
            equipSkills = data.equipSkills;
            if (equipSkills == null)
            {
                Debug.LogWarning("AutoSkillController : equipSkills 없음");
            }
            hub = data.hub;

            curIndex = 0;
            isAutoSkillUse = false;

            EventSubscribe();
            ToggleAutoSkillUse();

            NodeSet();
        }
        public void ToggleAutoSkillUse()
        {
            isAutoSkillUse = !isAutoSkillUse;
            hub.SkillAutoToggle(isAutoSkillUse);
        }
        void NodeSet()
        {
            rootSequence = new SequenceNode();

            FirstAutoSkillUsePossibleCheckNodeSet();
            NextAutoSkillUsePossibeCheckNodeSet();
        }
        void EventSubscribe()
        {
            hub.OnSkillAutoToggleInput += ToggleAutoSkillUse;
        }
        void EventUnsubscribe()
        {
            hub.OnSkillAutoToggleInput -= ToggleAutoSkillUse;
        }
        public void DestroyFeat()
        {
            rootSequence.DestroyFeat();
            EventUnsubscribe();
        }
        private bool IsCastingStateReverse() => !playerEquipSkillController.IsCasting;
        private bool IsSkillEquipped() => 0 < playerEquipSkillController.SkillCnt;
        private void FirstAutoSkillUsePossibleCheckNodeSet()
        {
            // autoSkillUse 상태, 장착된 스킬 있는지 상태, 캐스팅 상태 먼저 체크
            rootSequence.AddNode(new ConditionNode(() => isAutoSkillUse));
            rootSequence.AddNode(new ConditionNode(IsSkillEquipped));
            rootSequence.AddNode(new ConditionNode(IsCastingStateReverse));
        }
        private void NextAutoSkillUsePossibeCheckNodeSet()
        {
            var PriorityCheckSelector = new SelectorNode();

            var highPrioritySequence = new SequenceNode();
            PriorityCheckSequenceNodeAdd(highPrioritySequence, Priority.High);

            var midPrioritySequence = new SequenceNode();
            PriorityCheckSequenceNodeAdd(midPrioritySequence, Priority.Mid);

            var lowPrioritySequence = new SequenceNode();
            PriorityCheckSequenceNodeAdd(lowPrioritySequence, Priority.Low);

            PriorityCheckSelector.AddNode(highPrioritySequence);
            PriorityCheckSelector.AddNode(midPrioritySequence);
            PriorityCheckSelector.AddNode(lowPrioritySequence);

            rootSequence.AddNode(PriorityCheckSelector);
        }
        private void PriorityCheckSequenceNodeAdd(SequenceNode node, Priority pri)
        {
            for (int i = 0; i < 6; i++)
            {
                var curSlotSkillUseSelector = new SelectorNode();
                // 
                var curSlotSkillUseSequence = new SequenceNode();

                curSlotSkillUseSequence.AddNode
                 (new ConditionNode(() => CheckCurSlotIndexSkillIsUsePossible(pri)));
                curSlotSkillUseSequence.AddNode(new ActionNode(CurSkillUse));

                curSlotSkillUseSelector.AddNode(curSlotSkillUseSequence);

                curSlotSkillUseSelector.AddNode(new ActionNode(CurSkillUseFail));

                node.AddNode(curSlotSkillUseSelector);
            }
        }
        private bool CheckCurSlotIndexSkillIsUsePossible(Priority pri)
        {
            var eSkill = equipSkills[curIndex];
            return (eSkill.priority == pri) && !eSkill.IsCooltime;
        }
        private NodeState CurSkillUse()
        {
            Debug.Log($"CurSkillUse : {curIndex}번 스킬 자동 사용 시도");
            if (playerEquipSkillController.TryAtkSkillUseToMonster(curIndex))
            {
                curIndex = curIndex < 5 ? curIndex + 1 : 0;
                Debug.Log($"CurSkillUse : {curIndex}번 스킬 자동 사용 성공");
                return NodeState.Success;
            }
            Debug.Log($"CurSkillUse : {curIndex}번 스킬 자동 사용 실패");
            return NodeState.Run;
        }
        private NodeState CurSkillUseFail()
        {
            Debug.Log($"CurSkillUseFail : {curIndex}번 스킬 자동 사용 실패");
            curIndex = curIndex < 5 ? curIndex + 1 : 0;
            return NodeState.Fail;
        }
        public void UpdateFeat()
        {
            rootSequence.Evaluate();
        }
    }
    /*    [Serializable]
        public struct EquipSkillCheckerByPriority
        {
            [SerializeField] bool[] isEquipBoolArr;
            public bool[] IsEquipBoolArr => isEquipBoolArr;
            public int Cnt { get; private set; }
            public bool IsInThisPrioritySkill => 0 < Cnt;
            public bool IsEquipedTargetIndexSkill(int index)
            {
                // index 범위가 0~5 사이일 때만 통과
                if (index < 0 || 6 <= index) return false;
                // 장착 여부 return
                return isEquipBoolArr[index];
            }
            public void BoolArrInit() => isEquipBoolArr = new bool[6];
            public void EquipSkillCheckerInit(IReadOnlyList<EquipSkill> eSkillArr, Priority pri = Priority.Low)
            {
                for (int i = 0; i < 6; i++)
                {
                    if (eSkillArr[i].isEquipped && eSkillArr[i].priority == pri)
                    {
                        SkillPriorityChangeTrueCheck(i);
                    }
                }
            }
            public void SkillPriorityChangeTrueCheck(int equipSkillIndex)
            {
                if (isEquipBoolArr[equipSkillIndex]) return;
                isEquipBoolArr[equipSkillIndex] = true;
                Cnt++;
            }
            public void SkillPriorityChangeFalseCheck(int equipSkillIndex)
            {
                if (!isEquipBoolArr[equipSkillIndex]) return;
                isEquipBoolArr[equipSkillIndex] = false;
                Cnt--;
            }
        }
        [Serializable]
        public class EquipSkillCheckerByPrioritySet
        {
            // 0 : Low, 1 : Mid, 2 : High
            [SerializeField] EquipSkillCheckerByPriority[] equipSkillChecker;
            public EquipSkillCheckerByPriority[] EquipSkillChecker => equipSkillChecker;
            public EquipSkillCheckerByPrioritySet()
            {
                equipSkillChecker = new EquipSkillCheckerByPriority[3];
                for (int i = 0; i < 3; i++)
                {
                    equipSkillChecker[i].BoolArrInit();
                }
            }
            public void PrioritySkillNumSetInitAll(IReadOnlyList<EquipSkill> eSkillArr)
            {
                for (int i = 0; i < 3; i++)
                {
                    equipSkillChecker[i].EquipSkillCheckerInit(eSkillArr, (Priority)i);
                }
            }
            /// <summary> 우선순위 변경 시 사용하는 함수.  </summary>
            /// <param name="equipSkillIndex"> 해당 스킬이 장착된 번호 </param>
            /// <param name="pri"> 대상 우선순위 </param>
            public void EquipSkillPriorityUpdate(int equipSkillIndex, Priority pri)
            {
                for (int i = 0; i < 3; i++)
                {
                    // 해당하는 priority checker에 true 체크
                    if (pri == (Priority)i)
                    {
                        equipSkillChecker[i].SkillPriorityChangeTrueCheck(equipSkillIndex);
                    }
                    // 해당하지 않는 priority checker에 false 체크
                    else
                    {
                        equipSkillChecker[i].SkillPriorityChangeFalseCheck(equipSkillIndex);
                    }
                }
            }
            /// <summary> 스킬 장착 해제 시 사용하는 함수.  </summary>
            /// <param name="equipSkillIndex"> 해당 스킬이 장착된 번호 </param>
            public void SkillUnequipUpdate(int equipSkillIndex)
            {
                for (int i = 0; i < 3; i++)
                {
                    //priority checker 전부 false 체크
                    equipSkillChecker[i].SkillPriorityChangeFalseCheck(equipSkillIndex);
                }
            }
        }

        [Serializable]
        public class PlayerAutoSkillUseController
        {
            [SerializeField] int autoSkillUseOrderNum = 0;
            [SerializeField] PlayerEquipSkillController pesc;
            [SerializeField] EquipSkillCheckerByPrioritySet eSkillCheckerSet;
            public EquipSkillCheckerByPrioritySet ESkillCheckerSet => eSkillCheckerSet;

            // suto skill 토글용 bool
            [SerializeField] public bool IsAutoSkillUse { get; private set; }
            public void ToggleAutoSkillUse() => IsAutoSkillUse = !IsAutoSkillUse;
            public PlayerAutoSkillUseController(PlayerEquipSkillController pesc)
            {
                this.pesc = pesc;
                eSkillCheckerSet = new EquipSkillCheckerByPrioritySet();
                eSkillCheckerSet.PrioritySkillNumSetInitAll(pesc.EquipSkillList);
            }
            public void EQuipAndPriorityUpdate(int index, Priority pri) => eSkillCheckerSet.EquipSkillPriorityUpdate(index, pri);
            public void UnequipUpdate(int index) => eSkillCheckerSet.SkillUnequipUpdate(index);
            void OrderNumUpdate()
            {
                autoSkillUseOrderNum++;
                if (6 <= autoSkillUseOrderNum) autoSkillUseOrderNum = 0;
            }
            /// <summary> autoSkillUseOrderNum 번호를 기준으로 자동 스킬 사용 여부 확인
            /// + 없을 시, 순회 하면서 사용 가능한 스킬이 있는지 체크하는 bool 함수 </summary>
            /// <param name="pri">확인할 우선순위</param>
            /// <returns>현재 </returns>
            bool CheckAutoSkillUsePossibleNumByPriority(Priority pri)
            {
                ref EquipSkillCheckerByPriority tESkillChecker = ref eSkillCheckerSet.EquipSkillChecker[(int)pri];
                // pri 우선순위에 스킬이 장착되어 있는지 체크
                if (!tESkillChecker.IsInThisPrioritySkill)
                {
                    // Debug.LogWarning($"{pri} 우선순위에 스킬 없음");
                    return false;
                }
                // 현재 순서의 스킬이 현재 우선순위에 있는지 체크
                else if (!tESkillChecker.IsEquipedTargetIndexSkill(autoSkillUseOrderNum))
                {
                    //Debug.Log($"{pri} 우선순위에서 스킬 찾지 못함, 다음 번호 확인\n현재 번호 : {autoSkillUseOrderNum}");
                    // 없을 시 순회하면서 다음 번호의 스킬이 있는지 체크
                    int i = 0;
                    // 6번 돌릴 시 최악의 경우에도 처음으로 돌아가, 이전 순서부터 시작 가능
                    // 0(시작) -> 1(1번) -> 2(2번) -> 3(3번) -> 4(4번) -> 5(5번) -> 0(6번)
                    for (; i < 6; i++)
                    {
                        OrderNumUpdate();
                        // 마지막 번호일 경우 IsEquipedSkill 시도하지 않고 넘기기
                        if (5 <= i) continue;
                        // 순회 중 해당되는 번호의 스킬이 있을 경우 break
                        else if (tESkillChecker.IsEquipedTargetIndexSkill(autoSkillUseOrderNum)) break;
                    }
                    // 해당되는 번호가 없을 경우 찾기 실패
                    if (6 <= i)
                    {
                        // Debug.LogWarning($"자동 스킬 사용\n{pri} 우선순위, 다음 번호 찾지 못함");
                        return false;
                    }
                    // Debug.Log($"자동 스킬 사용\n{pri} 우선순위, 다음 번호 : {autoSkillUseOrderNum}");
                }
                EquipSkill tESkill = pesc.EquipSkillList[autoSkillUseOrderNum];
                // 해당 순서의 스킬이 사용 가능한지 여부 체크
                return !tESkill.IsCooltime;
            }
            bool CheckAutoSkillUsePossibleNumByAllPriority()
            {
                if (!CheckAutoSkillUsePossibleNumByPriority(Priority.High))
                {
                    //Debug.Log("High 우선순위 스킬 찾지 못함");
                    if (!CheckAutoSkillUsePossibleNumByPriority(Priority.Mid))
                    {
                        //Debug.Log("Mid 우선순위 스킬 찾지 못함");
                        if (!CheckAutoSkillUsePossibleNumByPriority(Priority.Low))
                        {
                            //Debug.Log("Low 우선순위 스킬 찾지 못함");
                            return false;
                        }
                    }
                }
                return true;
            }
            public bool TryAutoSkillUse()
            {
                // 자동 스킬 사용 상태가 아닐 때 or 사용 가능한 스킬이 없을 때 or 캐스팅 중일 때 return
                if (!IsAutoSkillUse)
                {
                    // Debug.LogWarning("자동 스킬 사용 상태 아님");
                    return false;
                }
                else if (pesc.AutoSkillUsePossibleCnt <= 0)
                {
                    // Debug.LogWarning("사용 가능한 스킬 없음");
                    return false;
                }
                else if (pesc.IsCasting)
                {
                    // Debug.LogWarning("스킬 캐스팅 중");
                    return false;
                }
                // 우선순위에 스킬이 없을 때 return
                else if (!CheckAutoSkillUsePossibleNumByAllPriority())
                {
                    // Debug.LogWarning("자동 사용 가능한 스킬 없음");
                    return false;
                }
                pesc.td.ColliderRadiusChange(pesc.EquipSkillList[autoSkillUseOrderNum].Skill.ActiveSkillData.range);
                if (pesc.td.IsDetectedTarget)
                {
                    // 몬스터가 있을 시 스킬 사용 + auto skill 순서를 다음 번호로 변경
                    //pesc.AtkSkillUse(autoSkillUseOrderNum++, mon);
                    if (pesc.TryAtkSkillUseToMonster(autoSkillUseOrderNum))
                    {
                        // Debug.Log("자동 스킬 사용");
                    }
                    OrderNumUpdate();
                    // Debug.Log($"{autoSkillUseOrderNum}번 스킬 자동 사용 성공");
                    if (CheckAutoSkillUsePossibleNumByAllPriority())
                        pesc.td.ColliderRadiusChange(pesc.EquipSkillList[autoSkillUseOrderNum].Skill.ActiveSkillData.range);
                }
                return true;
            }
        }
        */
}