using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using Battle;
using Base.Data;
using Base.Managers;
namespace Growth.Skill
{
    [Serializable]
    public class EquipSkill
    {
        // Owner
        // Character owner;
        EquipSkillController esController;
        // Event
        EventHub eventHub;
        // EquipSkill index
        int eSkillIndex;
        // Equiped Skill
        [SerializeField] ActiveSkill skill;
        public ActiveSkill Skill => skill;

        [SerializeField] private int equippedSkillKey;
        public int EquippedSkillKey => equippedSkillKey;
        SkillPool skillPool;
        SkillObjectPool skillObjPool;
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


        public void Init(EquipSkillController esController, int index, SkillPool skillPool, SkillObjectPool skillObjPool)
        {
            // this.owner = owner;
            this.esController = esController;
            this.skillPool = skillPool;
            this.skillObjPool = skillObjPool;
            // if(skillObjPool == null)Debug.LogWarning("skillobjpool 없음");
            eventHub = GameManager.Instance.GetGameSystem<EventHub>();
            eSkillIndex = index;
        }
        public void SkillEquip(ActiveSkill skill, bool isInit = false)
        {
            equippedSkillKey = skill.SkillData.key;
            this.skill = skill;
            // skill.Init(owner);
            MaxCooltime = skill.ActiveSkillData.coolDown;

            if (!isInit) CooltimeStart();
        }
        public bool TrySkillEquipByKey(int key, bool isInit = false)
        {
            if (!skillPool.TryGetActiveSkillByKey(key, out ActiveSkill aSkill)) return false;
            equippedSkillKey = key;
            skill = aSkill;
            // aSkill.Init(owner);
            MaxCooltime = aSkill.ActiveSkillData.coolDown;

            if (!isInit) CooltimeStart();

            return true;
        }
        public void SkillUnequip()
        {
            equippedSkillKey = -1;
            skill = null;
            IsCooltime = false;
        }
        public void SkillUse(Character target)
        {
            if (skill == null)
            {
                // Debug.LogWarning("스킬 없음");
                return;
            }
            else if (IsCooltime)
            {
                //Debug.Log($"{skill.name} 스킬 쿨타임");
                return;
            }
            else if (target == null)
            {
                // Debug.LogWarning("타겟 없음");
                return;
            }
            var skillObj = skillObjPool.GetActiveSkill(skill);
            switch (skill.ActiveSkillData.Targeting)
            {
                case TargetingMode.Self:
                    skillObj.SkillUseTargeting(new TargetChecker(skill.Owner.transform.position));
                    break;
                case TargetingMode.Homing:
                    skillObj.SkillUseTargeting(new TargetChecker(target));
                    break;
                case TargetingMode.GroundTarget:
                    skillObj.SkillUseTargeting(new TargetChecker(target.transform.position));
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
                await UniTask.Yield();
                if (esController == null || !isEquipped) return;
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