using Battle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Growth.Skill;

namespace Personal.HagYun
{
    public class PlayerEquipSkillController : EquipSkillController
    {
        //public bool IsForceSkillSelect { get; private set; }
        // auto skill use
        [SerializeField] int autoSkillUsePossibleCnt = 0;
        [SerializeField] bool isTestAutoSkill;
        void DecreaseUseSkillPossibleCnt() => autoSkillUsePossibleCnt--;
        void EncreaseUseSkillPossibleCnt() => autoSkillUsePossibleCnt++;
        void SetUseSkillPossibleCnt() => autoSkillUsePossibleCnt = skillCnt;
        private void OnDestroy()
        {
            UnsubscribeUseSkillPossibleCntAll();
        }
        public override void Init(Character cha)
        {
            if (cha is Player pl)
            {
                PlOwnerSet(pl);
                if (Skill.PlOwner == null) Debug.LogWarning("skill에 플레이어 주입 안 됨");
                SkillEquipInit();
                AutoSkillUsePossibleCntInit();
            }
        }
        void SkillEquipInit()
        {
            if (skillPool == null)
            {
                Debug.LogWarning("스킬풀 없음");
                return;
            }
            equipSkillArr = new EquipSkillSet[6];
            for (int i = 0; i < 6; i++)
            {
                equipSkillArr[i].Init();
                //test
                if (1 < i) continue;

                if (skillPool.TryGetSkill(i, out Skill skill))
                {
                    //equipSkillArr[i] = new EquipSkillSet(i);
                    SkillEquip(i, skill, true);
                    //Debug.Log($"{equipSkillArr[i].num}번호 부여\nskillPool에서 {i}번 스킬 장착");
                    Debug.Log($"skillPool에서 {i}번 스킬 장착");
                }
                else
                {
                    Debug.LogWarning("스킬 없음");
                    break;
                }
            }
            SetUseSkillPossibleCnt();
        }
        public void AutoSkillUsePossibleCntInit()
        {
            PrisoritySkillNumSetInitAll();
            SubscribeUseSkillPossibleCntAll();
        }
        protected override void UpdateFeat()
        {
            SkillInput();
        }
        public void SkillInput()
        {
            // test
            if (Input.GetKeyDown(KeyCode.Return))
            {
                if (!equipSkillArr[2].isEquipped && skillPool.TryGetSkill(2, out Skill skill))
                {
                    SkillEquip(2, skill);
                    Debug.Log($"skillPool에서 {2}번 스킬 장착");
                }
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                isTestAutoSkill = !isTestAutoSkill;
            }

            if (isTestAutoSkill)
            {
                AutoSkillUse();
            }
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
        public override void SkillEquip(int index, Skill targetSkill, bool isInit = false)
        {
            base.SkillEquip(index, targetSkill, isInit);
            SetUseSkillPossibleCnt();
            DecreaseUseSkillPossibleCnt();
        }
        public override void SkillUnequip(int index)
        {
            base.SkillUnequip(index);
            SetUseSkillPossibleCnt();
        }

        public void SubscribeUseSkillPossibleCnt(int index)
        {
            EquipSkill TESkill = equipSkillArr[index].ESkill;
            if (TESkill == null)
            {
                //Debug.LogWarning($"이벤트 구독할 {index}번 EquipSkill 없음");
                return;
            }
            TESkill.AddEventCooltimeStart(DecreaseUseSkillPossibleCnt);
            TESkill.AddEventCooltimeEnd(EncreaseUseSkillPossibleCnt);
        }
        public void SubscribeUseSkillPossibleCntAll()
        {
            for (int i = 0; i < 6; i++)
            {
                int index = i;
                SubscribeUseSkillPossibleCnt(index);
            }
        }
        public void UnsubscribeUseSkillPossibleCnt(int index)
        {
            EquipSkill TESkill = equipSkillArr[index].ESkill;
            if (TESkill == null)
            {
                //Debug.LogWarning($"이벤트 구독할 {index}번 EquipSkill 없음");
                return;
            }
            TESkill.RemoveEventCooltimeStart(DecreaseUseSkillPossibleCnt);
            TESkill.RemoveEventCooltimeEnd(EncreaseUseSkillPossibleCnt);
        }
        public void UnsubscribeUseSkillPossibleCntAll()
        {
            for (int i = 0; i < 6; i++)
            {
                int index = i;
                UnsubscribeUseSkillPossibleCnt(index);
            }
        }
        struct PrioritySkillNumSet
        {
            public bool[] isEquipArr;
            public int cnt;
            public int orderNum;
            public bool IsInSkill => 0 < cnt;
        }
        //PrioritySkillNumSet highPri;
        PrioritySkillNumSet middlePri;
        //PrioritySkillNumSet lowPri;
        [SerializeField] int autoSkillUseOrderNum = 0;
        void PrisoritySkillNumSetInit(ref PrioritySkillNumSet set, Priority pri)
        {
            set = new PrioritySkillNumSet();
            set.isEquipArr = new bool[6];
            for (int i = 0; i < 6; i++)
            {
                ref EquipSkillSet eSkill = ref equipSkillArr[i];
                if (eSkill.IsSkillUsePossible && eSkill.priority == pri)
                {
                    set.isEquipArr[i] = true;
                    set.cnt++;
                }
            }
        }
        void PrisoritySkillNumSetInitAll()
        {
            PrisoritySkillNumSetInit(ref middlePri, Priority.Mid);
        }
        bool CheckAutoSkillUseCheck(in PrioritySkillNumSet set, int index)
        {
            if (!set.IsInSkill || !set.isEquipArr[index])
            {
                return false;
            }
            //EquipSkill tESkill = equipSkillArr[index].ESkill;
            //if(!equipSkillArr[index].IsSkillUsePossible)
            //{
            //    return false;
            //}
            return true;
        }
        public void AutoSkillUse()
        {
            // 사용 가능한 스킬이 없을 때 return
            // 캐스팅 중일 때 return
            if (autoSkillUsePossibleCnt <= 0 || IsCasting) return;
            Debug.Log("사용 가능한 스킬 있음, 캐스팅 중이지 않음");
            // 스킬이 장착되어 있는지 체크 (우선순위 따라 확인)
            if (CheckAutoSkillUseCheck(middlePri, autoSkillUseOrderNum))
            {
                Debug.Log($"{autoSkillUseOrderNum}번 스킬 자동 사용 가능 여부 체크");
                // auto skill 순서의 스킬이 사용 가능한지 여부 체크
                if (equipSkillArr[autoSkillUseOrderNum].IsSkillUsePossible)
                {
                    Debug.Log($"{autoSkillUseOrderNum}번 스킬 자동 사용 가능, 타겟 체크");
                    // 사용 가능하다면 주변에 타겟이 있는지 체크
                    if (TryGetMonsterTargetToAtk(autoSkillUseOrderNum, out Monster mon))
                    {
                        // 몬스터가 있을 시 스킬 사용 + auto skill 순서를 다음 번호로 변경
                        AtkSkillUse(autoSkillUseOrderNum++, mon);
                        Debug.Log($"{autoSkillUseOrderNum}번 스킬 자동 사용 성공");
                        if (6 <= autoSkillUseOrderNum) autoSkillUseOrderNum = 0;
                    }
                    // 타겟 없을 시 다른 동작 없음
                    else
                    {
                        Debug.Log($"{autoSkillUseOrderNum}번 스킬 자동 사용 가능, 타겟 없음");
                    }
                }
            }
            // 해당 우선순위에 auto skill 순서의 스킬이 장착되지 않았을 시 
            else
            {
                Debug.Log($"{autoSkillUseOrderNum}번 스킬 장착 안 됨, 다음 번호 체크");
                // 번호 증가시키면서 사용 가능한 스킬 확인, 만약을 대비해 6번 까지만 체크
                for (int i = 0; i < 6; i++)
                {
                    autoSkillUseOrderNum++;
                    if (6 <= autoSkillUseOrderNum) autoSkillUseOrderNum = 0;
                    // 해당 우선순위 스킬에 있으면서 해당 스킬이 사용 가능할 경우
                    if (CheckAutoSkillUseCheck(middlePri, autoSkillUseOrderNum) && equipSkillArr[autoSkillUseOrderNum].IsSkillUsePossible)
                    {
                        // 스킬 사용 시도
                        if (TryAtkSkillUseToMonster(autoSkillUseOrderNum))
                        {
                            Debug.Log($"{autoSkillUseOrderNum}번 스킬 자동 사용");
                            autoSkillUseOrderNum++;
                            if (6 <= autoSkillUseOrderNum) autoSkillUseOrderNum = 0;
                        }
                        // 실패 시 다른 동작 없음
                        else
                        {
                            Debug.Log($"{autoSkillUseOrderNum}번 스킬 자동 사용 최종 실패");
                        }
                    }
                }
            }

        }
    }
}