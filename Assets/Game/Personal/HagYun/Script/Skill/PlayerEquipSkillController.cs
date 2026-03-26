using Base.Data;
using Base.Managers;
using Battle;
using Growth.Skill;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Personal.HagYun
{
    [Serializable]
    public struct EquipSkillCheckerByPriority
    {
        [SerializeField] bool[] isEquipBoolArr;
        public bool[] IsEquipBoolArr => isEquipBoolArr;
        [SerializeField] int cnt;
        public bool IsInThisPrioritySkill => 0 < cnt;
        public bool IsEquipedTargetIndexSkill(int index)
        {
            // index 범위가 0~5 사이일 때만 통과
            if (index < 0 || 6 <= index) return false;
            // 장착 여부 return
            return isEquipBoolArr[index];
        }
        public void BoolArrInit() => isEquipBoolArr = new bool[6];
        public void EquipSkillCheckerInit(in EquipSkill[] eSkillArr, Priority pri = Priority.Low)
        {
            //if (isEquipBoolArr == null) isEquipBoolArr = new bool[6];
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
            cnt++;
        }
        public void SkillPriorityChangeFalseCheck(int equipSkillIndex)
        {
            if (!isEquipBoolArr[equipSkillIndex]) return;
            isEquipBoolArr[equipSkillIndex] = false;
            cnt--;
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
        public void PrioritySkillNumSetInitAll(in EquipSkill[] eSkillArr)
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

        // suto skill 토글용 bool
        [SerializeField] public bool IsAutoSkillUse { get; private set; }
        public void ToggleAutoSkillUse() => IsAutoSkillUse = !IsAutoSkillUse;
        public PlayerAutoSkillUseController(PlayerEquipSkillController pesc)
        {
            this.pesc = pesc;
            eSkillCheckerSet = pesc.ESkillCheckerSet;
        }
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
            if (!tESkillChecker.IsInThisPrioritySkill) return false;
            // 현재 순서의 스킬이 현재 우선순위에 있는지 체크
            if (!tESkillChecker.IsEquipedTargetIndexSkill(autoSkillUseOrderNum))
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
            // ref EquipSkillSet tESkillSet = ref pesc.EquipSkillSetArr[autoSkillUseOrderNum];
            ref EquipSkill tESkillSet = ref pesc.EquipSkillArr[autoSkillUseOrderNum];
            // 해당 순서의 스킬이 사용 가능한지 여부 체크
            return tESkillSet.IsSkillUsePossible;
            // if(tESkillSet.IsSkillUsePossible)
            // {
            //    Debug.Log($"자동 스킬 사용\n{autoSkillUseOrderNum}번 스킬 사용 가능");
            //    return true;
            // }
            // else
            // {
            //    Debug.LogWarning($"자동 스킬 사용\n{autoSkillUseOrderNum}번 스킬 사용 불가");
            //    return false;
            // }
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
            pesc.td.ColliderRadiusChange(pesc.EquipSkillArr[autoSkillUseOrderNum].Skill.Data.range);
            if (pesc.td.IsDetectedTarget)
            {
                // 몬스터가 있을 시 스킬 사용 + auto skill 순서를 다음 번호로 변경
                //pesc.AtkSkillUse(autoSkillUseOrderNum++, mon);
                pesc.TryAtkSkillUseToMonster(autoSkillUseOrderNum++);
                if (6 <= autoSkillUseOrderNum) autoSkillUseOrderNum = 0;
                // Debug.Log($"{autoSkillUseOrderNum}번 스킬 자동 사용 성공");
                if (CheckAutoSkillUsePossibleNumByAllPriority())
                    pesc.td.ColliderRadiusChange(pesc.EquipSkillArr[autoSkillUseOrderNum].Skill.Data.range);
            }
            return true;
        }
    }
    public class PlayerEquipSkillController : EquipSkillController
    {
        //public bool IsForceSkillSelect { get; private set; }
        // auto skill use
        [SerializeField] int autoSkillUsePossibleCnt = 0;
        public int AutoSkillUsePossibleCnt => autoSkillUsePossibleCnt;
        // auto 스킬 사용 시 우선순위 설정용
        [SerializeField] EquipSkillCheckerByPrioritySet eSkillCheckerSet;
        public EquipSkillCheckerByPrioritySet ESkillCheckerSet => eSkillCheckerSet;
        // auto 스킬 사용 용 클래스
        [SerializeField] PlayerAutoSkillUseController autoSkillController;

        // void DecreaseUseSkillPossibleCnt() => autoSkillUsePossibleCnt--;
        // void EncreaseUseSkillPossibleCnt() => autoSkillUsePossibleCnt++;
        void DecreaseUseSkillPossibleCnt(int temp) => autoSkillUsePossibleCnt--;
        void EncreaseUseSkillPossibleCnt(int temp) => autoSkillUsePossibleCnt++;
        void SetUseSkillPossibleCnt() => autoSkillUsePossibleCnt = skillCnt;
        private void OnDestroy()
        {
            UnsubscribeUseSkillPossibleCntAll();
        }
        public TargetDetectorUsingCircleCollider2D td;
        public override void Init(Character cha)
        {
            if (cha is Player pl)
            {
                OwnerSet(pl);
                if (owner == null) Debug.LogWarning("플레이어 주입 안 됨");
                td = GetComponent<TargetDetectorUsingCircleCollider2D>();
                eventHub = GameManager.Instance.GetGameSystem<EventHub>();
                SkillEquipInit();
                AutoSkillUsePossibleCntInit();
            }
        }
        void SkillEquipInit()
        {
            // equipSkillSetArr = new EquipSkillSet[6];
            equipSkillArr = new EquipSkill[6];
            int index = 0;
            for (int i = 0; i < 6; i++)
            {
                index = i;
                // equipSkillSetArr[index].Init(owner, index);
                equipSkillArr[index] = new EquipSkill();
                equipSkillArr[index].Init(owner, index);
            }
            // test
            TestUIPresenter.ins.Init();

            if (skillPool == null)
            {
                Debug.LogWarning("스킬풀 없음");
            }
            else
            {
                for (int i = 0; i < 6; i++)
                {
                    index = i;
                    if (skillPool.TryGetSkill(index, out Skill skill))
                    {
                        Debug.Log($"{index}번 스킬 장착 시도");
                        SkillEquip(index, skill, true);
                        Debug.Log($"skillPool에서 {i}번 스킬 장착");
                    }
                    else
                    {
                        Debug.LogWarning("스킬 없음");
                        break;
                    }
                }
            }
            SetUseSkillPossibleCnt();
        }
        void EquipSkillPrioritySetInit()
        {
            eSkillCheckerSet = new EquipSkillCheckerByPrioritySet();
            // eSkillCheckerSet.PrioritySkillNumSetInitAll(equipSkillSetArr);
            eSkillCheckerSet.PrioritySkillNumSetInitAll(equipSkillArr);
        }
        public void AutoSkillUsePossibleCntInit()
        {
            EquipSkillPrioritySetInit();
            autoSkillController = new PlayerAutoSkillUseController(this);
            SubscribeUseSkillPossibleCntAll();
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

            autoSkillController.TryAutoSkillUse();
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
                // Debug.Log("2번 스킬 시도");
                // if (TryAtkSkillUseToMonster(1))
                //     Debug.Log("2번 스킬 사용");
                // else
                //     Debug.LogWarning("2번 스킬 사용 실패");
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                TryAtkSkillUseToMonster(2);
                // Debug.Log("3번 스킬 시도");
                // if (TryAtkSkillUseToMonster(2))
                //     Debug.Log("3번 스킬 사용");
                // else
                //     Debug.LogWarning("3번 스킬 사용 실패");
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                TryAtkSkillUseToMonster(3);
                // Debug.Log("4번 스킬 시도");
                // if (TryAtkSkillUseToMonster(3))
                //     Debug.Log("4번 스킬 사용");
                // else
                //     Debug.LogWarning("4번 스킬 사용 실패");
            }
            else if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                TryAtkSkillUseToMonster(4);
                // Debug.Log("5번 스킬 시도");
                // if (TryAtkSkillUseToMonster(4))
                //     Debug.Log("5번 스킬 사용");
                // else
                //     Debug.LogWarning("5번 스킬 사용 실패");
            }
            else if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                TryAtkSkillUseToMonster(5);
                // Debug.Log("6번 스킬 시도");
                // if (TryAtkSkillUseToMonster(5))
                //     Debug.Log("6번 스킬 사용");
                // else
                //     Debug.LogWarning("6번 스킬 사용 실패");
            }
        }
        public override void SkillEquip(int index, Skill targetSkill, bool isInit = false)
        {
            base.SkillEquip(index, targetSkill, isInit);
            SetUseSkillPossibleCnt();
            DecreaseUseSkillPossibleCnt(index);
        }
        public override void PriorityUpdate(int index, Priority pri)
        {
            equipSkillArr[index].priority = pri;
            eSkillCheckerSet.EquipSkillPriorityUpdate(index, pri);
        }
        public override void SkillUnequip(int index)
        {
            base.SkillUnequip(index);
            SetUseSkillPossibleCnt();
            UnequipUpdateToEquipSkillChecker(index);
        }
        public void UnequipUpdateToEquipSkillChecker(int index) => eSkillCheckerSet.SkillUnequipUpdate(index);

        public void SubscribeUseSkillPossibleCnt(int index)
        {
            EquipSkill TESkill = equipSkillArr[index];
            if (TESkill == null)
            {
                //Debug.LogWarning($"이벤트 구독할 {index}번 EquipSkill 없음");
                return;
            }
            // TESkill.AddEventCooltimeStart(DecreaseUseSkillPossibleCnt);
            // TESkill.AddEventCooltimeEnd(EncreaseUseSkillPossibleCnt);

            eventHub.OnSkillUsed += DecreaseUseSkillPossibleCnt;
            eventHub.OnSkillCoolEnd += EncreaseUseSkillPossibleCnt;
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
            EquipSkill tESkill = equipSkillArr[index];
            if (tESkill == null)
            {
                //Debug.LogWarning($"이벤트 구독할 {index}번 EquipSkill 없음");
                return;
            }
            // tESkill.RemoveEventCooltimeStart(DecreaseUseSkillPossibleCnt);
            // tESkill.RemoveEventCooltimeEnd(EncreaseUseSkillPossibleCnt);

            eventHub.OnSkillUsed -= DecreaseUseSkillPossibleCnt;
            eventHub.OnSkillCoolEnd -= EncreaseUseSkillPossibleCnt;
        }
        public void UnsubscribeUseSkillPossibleCntAll()
        {
            for (int i = 0; i < 6; i++)
            {
                int index = i;
                UnsubscribeUseSkillPossibleCnt(index);
            }
        }
    }
}