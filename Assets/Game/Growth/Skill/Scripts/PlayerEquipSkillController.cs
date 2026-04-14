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
        // auto skill use
        // [SerializeField] int autoSkillUsePossibleCnt = 0;
        // public int AutoSkillUsePossibleCnt => autoSkillUsePossibleCnt;
        [SerializeField] PlayerAutoSkillUseController autoSkillController;
        /// <summary> EquipSkill의 쿨타임 시점에 실행될 함수, 
        /// IsCooltime 이후에 실행될 것이기 때문에 해당 시점을 기준으로 AutoSkillCnt Update </summary>
        /// <param name="index">스킬 index</param>
        // void AutoSkillUseCntUpdate(int index)
        // {
        //     if (equipSkillArr == null) return;
        //     EquipSkill eSkill = equipSkillArr[index];
        //     AutoSkillUseCntUpdateFeat(eSkill);
        // }
        // void AutoSkillUseCntUpdateFeat(EquipSkill eSkill)
        // {
        //     if (eSkill == null) return;
        //     else if (eSkill.isEquipped)
        //     {
        //         if (!eSkill.IsCooltime)
        //             autoSkillUsePossibleCnt++;
        //         else
        //             autoSkillUsePossibleCnt--;
        //     }
        //     else
        //     {
        //         if (!eSkill.IsCooltime)
        //             autoSkillUsePossibleCnt--;
        //     }
        // }

        // void SetUseSkillPossibleCnt() => autoSkillUsePossibleCnt = skillCnt;
        private void OnDestroy()
        {
            EquipSkillSlotEventUnsbuscribe();
            EventUnsubscribe();
            autoSkillController.DestroyFeat();
        }
        public TargetDetectorUsingCircleCollider2D td;
        public override void Init(Character cha)
        {
            if (cha is Player pl)
            {
                OwnerSet(pl);
                td = GetComponent<TargetDetectorUsingCircleCollider2D>();
                eventHub = GameManager.Instance.GetGameSystem<EventHub>();
                EventSubscribe();
                skillMgr = GameManager.Instance.GetGameSystem<SkillManager>();
                skillObjPool = new SkillObjectPool();
                // SkillEquipInit();
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
            skillPool.Init(skillMgr);
            for (int i = 0; i < 6; i++)
            {
                // if (skillPool.TestTryGetSaveSkill(i, out int key) &&
                // skillPool.TryGetActiveSkillByKey(key, out var skill))
                // {
                //     SkillEquip(i, skill, true);
                // }

                if (skillMgr.TryGetSaveEquippedSkill(i, out var aSkill))
                    SkillEquip(i, aSkill, true);
            }
            EquipSkillSlotEventSubscribe();
            // SetUseSkillPossibleCnt();
            SkillRangeChange(2);
            skillObjPool.Init(skillPool);
            // AutoSkillUsePossibleCntInit();
            var data = new NeedsDataFromAutoSkillUseController(this, EquipSkillList, eventHub);
            autoSkillController = new PlayerAutoSkillUseController(data);
        }
        void EquipSkillSlotEventSubscribe()
        {
            eventHub.OnSkillEquip += SkillEquip;
        }
        void EquipSkillSlotEventUnsbuscribe()
        {
            eventHub.OnSkillEquip -= SkillEquip;
        }
        // public void AutoSkillUsePossibleCntInit()
        // {
        //     autoSkillController = new PlayerAutoSkillUseController(this);
        //     SubscribeUseSkillPossibleCntAll();
        // }
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

            // autoSkillController.TryAutoSkillUse();
            autoSkillController.UpdateFeat();
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                TryAtkSkillUseToMonster(0);
                // Debug.Log("1번 스킬 시도");
                // if (TryAtkSkillUseToMonster(0))
                //     Debug.Log("1번 스킬 사용");
                // else
                //     Debug.LogWarning("1번 스킬 사용 실패");
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
            // if (!isInit) AutoSkillUseCntUpdate(slotIndex);
        }
        public override void PriorityUpdate(int index, Priority pri)
        {
            equipSkillArr[index].priority = pri;
            // autoSkillController.EQuipAndPriorityUpdate(index, pri);
        }
        protected override void SkillUnequipFeat(int index, EquipSkill eSkill)
        {
            base.SkillUnequipFeat(index, eSkill);
            eventHub.SkillUnset(index);
            // AutoSkillUseCntUpdateFeat(eSkill);
            // UnequipUpdateToEquipSkillChecker(index);
        }
        void SkillUse(int index){Debug.Log($"버튼 입력으로 {index} slot 스킬 사용 시도");TryAtkSkillUseToMonster(index);}
        // public void UnequipUpdateToEquipSkillChecker(int index) => autoSkillController.UnequipUpdate(index);
         //eSkillCheckerSet.SkillUnequipUpdate(index);

        public void EventSubscribe()
        {
            eventHub.OnPlayerSkillUse += SkillUse;
            // eventHub.OnSkillUsed += AutoSkillUseCntUpdate;
            // eventHub.OnSkillCoolEnd += AutoSkillUseCntUpdate;
        }
        public void EventUnsubscribe()
        {
            eventHub.OnPlayerSkillUse -= SkillUse;
            // eventHub.OnSkillUsed -= AutoSkillUseCntUpdate;
            // eventHub.OnSkillCoolEnd -= AutoSkillUseCntUpdate;
        }
    }
}