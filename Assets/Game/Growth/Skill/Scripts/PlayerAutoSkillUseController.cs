using System;
using Base.Utils;
using System.Collections.Generic;
using UnityEngine;
using Base.Data;
using Cysharp.Threading.Tasks;

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
        private void FirstAutoSkillUsePossibleCheckNodeSet()
        {
            // autoSkillUse 상태, 장착된 스킬 있는지 상태, 캐스팅 상태 먼저 체크
            rootSequence.AddNode(new ConditionNode(() => isAutoSkillUse));
            rootSequence.AddNode(new ConditionNode(() => 0 < playerEquipSkillController.SkillCnt));
            rootSequence.AddNode(new ConditionNode(() => !playerEquipSkillController.IsCasting));
        }
        private void NextAutoSkillUsePossibeCheckNodeSet()
        {
            var PriorityCheckSelector = new SelectorNode();

            PriorityCheckSelector.AddNode(PriorityCheckSequenceNodeAdd(Priority.High));
            PriorityCheckSelector.AddNode(PriorityCheckSequenceNodeAdd(Priority.Mid));
            PriorityCheckSelector.AddNode(PriorityCheckSequenceNodeAdd(Priority.Low));

            rootSequence.AddNode(PriorityCheckSelector);
        }
        private SequenceNode PriorityCheckSequenceNodeAdd(Priority pri)
        {
            var curSlotSkillUseSequence = new SequenceNode();

            curSlotSkillUseSequence.AddNode(new ConditionNode(() => UsePossibleSkillCheckInCurPriority(pri)));

            curSlotSkillUseSequence.AddNode(new ActionNode(CurSkillUse));

            return curSlotSkillUseSequence;
        }
        private bool UsePossibleSkillCheckInCurPriority(Priority pri)
        {
            for (int i = 0; i < 6; i++)
            {
                var eSkill = equipSkills[curIndex];
                // 현재 슬롯 스킬이 장착 상태일 때 && 현재 슬롯 스킬의 우선순위가 pri와 같을 때 && 현재 슬롯 스킬이 쿨타임 상태가 아닐 때 true
                if (eSkill.isEquipped && eSkill.priority == pri && !eSkill.IsCooltime)
                    return true;

                curIndex = curIndex < 5 ? curIndex + 1 : 0;
            }
            return false;
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
        private float curTime = 0f;
        private readonly float tickRate = 0.1f;
        public void UpdateFeat()
        {
            curTime += Time.deltaTime;
            if (curTime < tickRate) return;
            curTime = 0;
            rootSequence.Evaluate();
        }
    }
}