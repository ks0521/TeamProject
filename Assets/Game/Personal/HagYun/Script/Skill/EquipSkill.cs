using Cysharp.Threading.Tasks;
using Growth.Skill;
using System;
using System.Collections.Generic;
using UnityEngine;
using Battle;
using Base.Data;
using Base.Managers;
namespace Personal.HagYun
{
    [Serializable]
    public class EquipSkill
    {
        // Owner
        Character owner;
        // Event
        EventHub eventHub;
        // EquipSkill index
        int eSkillIndex;
        // Equiped Skill
        [SerializeField] ActiveSkill skill;
        public ActiveSkill Skill => skill;
        [SerializeField] private int equippedSkillKey;
        public int EquippedSkillKey => equippedSkillKey;
        [SerializeField] SkillPool skillPool;
        // Cooltime Check
        public float CurCooltime { get; private set; }
        public float MaxCooltime { get; private set; }
        [field: SerializeField] public bool IsCooltime { get; private set; }

        // Current priority
        public Priority priority;
        // Equip State
        public bool isEquipped;
        // Skill Use Possible Check
        public bool IsSkillUsePossible => isEquipped && !IsCooltime;


        public void Init(Character owner, int index)
        {
            this.owner = owner;
            eventHub = GameManager.Instance.GetGameSystem<EventHub>();
            eSkillIndex = index;
        }
        public void SkillEquip(ActiveSkill skill, bool isInit = false)
        {
            this.skill = skill;
            skill.Init(owner);
            MaxCooltime = skill.ActiveSkillData.coolDown;

            if (!isInit) CooltimeStart();
        }
        public void SkillEquipByKey(int key, bool isInit = false)
        {
            if (!skillPool.TryGetActiveSkillToKey(key, out ActiveSkill aSkill)) return;
            equippedSkillKey = key;
            aSkill.Init(owner);
            skill = aSkill;
            MaxCooltime = aSkill.ActiveSkillData.coolDown;

            if (!isInit) CooltimeStart();
        }
        public void SkillUnequip()
        {
            skill = null;
            IsCooltime = false;
        }
        public void SkillUnequipByKey()
        {
            equippedSkillKey = -1;
        }
        public void SkillUse(Character target)
        {
            if (skill == null)
            {
                Debug.LogWarning("스킬 없음");
                return;
            }
            else if (IsCooltime)
            {
                //Debug.Log($"{skill.name} 스킬 쿨타임");
                return;
            }
            else if (target == null)
            {
                Debug.LogWarning("타겟 없음");
                return;
            }
            switch (skill.ActiveSkillData.Targeting)
            {
                case TargetingMode.Self:
                    skill.SkillUseTargeting(new TargetChecker(skill.OwnerPos));
                    break;
                case TargetingMode.Homing:
                    skill.SkillUseTargeting(new TargetChecker(target));
                    break;
                case TargetingMode.GroundTarget:
                    skill.SkillUseTargeting(new TargetChecker(target.transform.position));
                    break;
            }
            CooltimeStart();
        }
        async UniTaskVoid CooltimeStartTask()
        {
            IsCooltime = true;
            eventHub.SkillUsed(eSkillIndex);
            CurCooltime = MaxCooltime;
            while (0 < CurCooltime)
            {
                CurCooltime -= Time.deltaTime; // 쿨타임 감소 속도 증가 시, 해당 값 곱하기
                await UniTask.Yield(owner.GetCancellationTokenOnDestroy());
                if (owner == null || !isEquipped) return;
            }
            IsCooltime = false;
            eventHub.SkillCoolEnd(eSkillIndex);
        }
        public void CooltimeSet(float cooltime)
        {
            CurCooltime = cooltime;
        }
        public void ColltimeAdd(float cooltime)
        {
            CurCooltime += cooltime;
            if (!IsCooltime) CooltimeStartTask().Forget();
        }
        public void CooltimeStart()
        {
            CooltimeSet(skill.ActiveSkillData.coolDown);
            CooltimeStartTask().Forget();
        }
    }
}