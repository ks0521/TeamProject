using Base.Data;
using Base.Managers;
using Battle;
using System;
using Base.Utils;
using UnityEngine;
using System.Collections.Generic;

namespace Growth.Skill
{
    public class PlayerEquipSkillController : EquipSkillController
    {
        private SkillManager skillMgr;
        public bool IsForceSkillSelect { get; private set; }
        [SerializeField] PlayerAutoSkillUseController autoSkillController;
        private void OnDestroy()
        {
            EventUnsubscribe();
            autoSkillController.DestroyFeat();
        }
        public override void Init(Character cha)
        {
            if (cha is Player pl)
            {
                OwnerSet(pl);
                eventHub = GameManager.Instance.GetGameSystem<EventHub>();
                skillMgr = GameManager.Instance.GetGameSystem<SkillManager>();
                skillObjPool = new SkillObjectPool();
            }
        }
        public void SkillEquipInit()
        {
            equipSkillArr = new EquipSkill[6];
            for (int i = 0; i < 6; i++)
            {
                var eSkill = new EquipSkill();
                eSkill.Init(this, i, skillPool, skillObjPool);
                equipSkillArr[i] = eSkill;
            }
            for (int i = 0; i < 6; i++)
            {
                if (skillMgr.TryGetSaveEquippedSkill(i, out var aSkill))
                    SkillEquip(i, aSkill, true);
            }
            EventSubscribe();
            SkillRangeChange(2);
            skillObjPool.Init(skillPool);
            var data = new NeedsDataFromAutoSkillUseController(this, EquipSkillList, eventHub);
            autoSkillController = new PlayerAutoSkillUseController(data);
        }
        protected override void UpdateFeat()
        {
            SkillInput();
        }
        public void SkillInput()
        {
            if (!owner.canAtk) return;
            // test
            if (Input.GetKeyDown(KeyCode.Return))
            {
                PriorityUpdate(1, Priority.High);
                PriorityUpdate(0, Priority.Mid);
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                autoSkillController.ToggleAutoSkillUse();
            }

            autoSkillController.UpdateFeat();
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                TryAtkSkillUseToMonster(0);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                TryAtkSkillUseToMonster(1);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                TryAtkSkillUseToMonster(2);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                TryAtkSkillUseToMonster(3);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                TryAtkSkillUseToMonster(4);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                TryAtkSkillUseToMonster(5);
            }
        }
        protected override void SkillEquipFeat(int slotIndex, ActiveSkill targetSkill, bool isInit = false)
        {
            base.SkillEquipFeat(slotIndex, targetSkill, isInit);
            eventHub.SkillEquipComplete(slotIndex, targetSkill);
        }
        public override void PriorityUpdate(int index, Priority pri)
        {
            equipSkillArr[index].priority = pri;
        }
        protected override void SkillUnequipFeat(int index, EquipSkill eSkill)
        {
            base.SkillUnequipFeat(index, eSkill);
            eventHub.SkillUnset(index);
        }
        void SkillUse(int index)
        {
            // Debug.Log($"버튼 입력으로 {index} slot 스킬 사용 시도");
            TryAtkSkillUseToMonster(index);
        }

        public void EventSubscribe()
        {
            eventHub.OnPlayerSkillUse += SkillUse;
            eventHub.OnSkillEquip += SkillEquip;
        }
        public void EventUnsubscribe()
        {
            eventHub.OnPlayerSkillUse -= SkillUse;
            eventHub.OnSkillEquip -= SkillEquip;
        }
    }
}