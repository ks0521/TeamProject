using Base.Data;
using Base.Managers;
using Battle;
using System;
using Base.Utils;
using UnityEngine;

namespace Growth.Skill
{
    [Serializable]
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
        public void EquipSkillCheckerInit(EquipSkill[] eSkillArr, Priority pri = Priority.Low)
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
        public void PrioritySkillNumSetInitAll(EquipSkill[] eSkillArr)
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
            eSkillCheckerSet.PrioritySkillNumSetInitAll(pesc.EquipSkillArr);
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
            ref EquipSkill tESkill = ref pesc.EquipSkillArr[autoSkillUseOrderNum];
            // 해당 순서의 스킬이 사용 가능한지 여부 체크
            return !tESkill.IsCooltime;
            // if(!tESkill.IsCooltime)
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
            pesc.td.ColliderRadiusChange(pesc.EquipSkillArr[autoSkillUseOrderNum].Skill.ActiveSkillData.range);
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
                    pesc.td.ColliderRadiusChange(pesc.EquipSkillArr[autoSkillUseOrderNum].Skill.ActiveSkillData.range);
            }
            return true;
        }
    }
    public class PlayerEquipSkillController : EquipSkillController
    {
        public bool IsForceSkillSelect { get; private set; }
        // auto skill use
        [SerializeField] int autoSkillUsePossibleCnt = 0;
        public int AutoSkillUsePossibleCnt => autoSkillUsePossibleCnt;
        [SerializeField] PlayerAutoSkillUseController autoSkillController;
        /// <summary> EquipSkill의 쿨타임 시점에 실행될 함수, 
        /// IsCooltime 이후에 실행될 것이기 때문에 해당 시점을 기준으로 AutoSkillCnt Update </summary>
        /// <param name="index">스킬 index</param>
        void AutoSkillUseCntUpdate(int index)
        {
            if (equipSkillArr == null) return;
            EquipSkill eSkill = equipSkillArr[index];
            AutoSkillUseCntUpdateFeat(eSkill);
        }
        void AutoSkillUseCntUpdateFeat(EquipSkill eSkill)
        {
            if (eSkill == null) return;
            else if (eSkill.isEquipped)
            {
                if (!eSkill.IsCooltime)
                    autoSkillUsePossibleCnt++;
                else
                    autoSkillUsePossibleCnt--;
            }
            else
            {
                if (!eSkill.IsCooltime)
                    autoSkillUsePossibleCnt--;
            }
        }

        void SetUseSkillPossibleCnt() => autoSkillUsePossibleCnt = skillCnt;
        private void OnDestroy()
        {
            EquipSkillSlotEventUnsbuscribe();
            UnsubscribeUseSkillPossibleCntAll();
        }
        public TargetDetectorUsingCircleCollider2D td;
        public override void Init(Character cha)
        {
            if (cha is Player pl)
            {
                OwnerSet(pl);
                td = GetComponent<TargetDetectorUsingCircleCollider2D>();
                eventHub = GameManager.Instance.GetGameSystem<EventHub>();
                skillObjPool = new SkillObjectPool();
                // SkillEquipInit();
            }
        }
        public void SkillEquipInit()
        {
            equipSkillArr = new EquipSkill[6];
            int index = 0;
            for (int i = 0; i < 6; i++)
            {
                index = i;
                var eSkill = new EquipSkill();
                eSkill.Init(this, index, skillPool, skillObjPool);
                equipSkillArr[index] = eSkill;
            }
            skillPool.Init();
            for (int i = 0; i < 6; i++)
            {
                index = i;
                // if (skillPool.TryGetActiveSkillByKey(index, out ActiveSkill skill))
                if (skillPool.TestTryGetSaveSkill(i, out int key) &&
                skillPool.TryGetActiveSkillByKey(key, out var skill))
                {
                    SkillEquip(index, skill, true);
                }
            }
            EquipSkillSlotEventSubscribe();
            SetUseSkillPossibleCnt();
            SkillRangeChange(2);
            skillObjPool.Init(skillPool);
            AutoSkillUsePossibleCntInit();
        }
        void EquipSkillSlotEventSubscribe()
        {
            eventHub.OnSkillEquip += SkillEquip;
        }
        void EquipSkillSlotEventUnsbuscribe()
        {
            eventHub.OnSkillEquip -= SkillEquip;
        }
        public void AutoSkillUsePossibleCntInit()
        {
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
            if (!isInit) AutoSkillUseCntUpdate(slotIndex);
        }
        public override void PriorityUpdate(int index, Priority pri)
        {
            equipSkillArr[index].priority = pri;
            autoSkillController.EQuipAndPriorityUpdate(index, pri);
        }
        protected override void SkillUnequipFeat(int index, EquipSkill eSkill)
        {
            base.SkillUnequipFeat(index, eSkill);
            eventHub.SkillUnsetComplete(index);
            AutoSkillUseCntUpdateFeat(eSkill);
            UnequipUpdateToEquipSkillChecker(index);
        }
        void SkillUse(int index) => TryAtkSkillUseToMonster(index);
        public void UnequipUpdateToEquipSkillChecker(int index) => autoSkillController.UnequipUpdate(index); //eSkillCheckerSet.SkillUnequipUpdate(index);

        public void SubscribeUseSkillPossibleCntAll()
        {
            eventHub.OnPlayerSkillUse += SkillUse;
            eventHub.OnSkillUsed += AutoSkillUseCntUpdate;
            eventHub.OnSkillCoolEnd += AutoSkillUseCntUpdate;
        }
        public void UnsubscribeUseSkillPossibleCntAll()
        {
            eventHub.OnPlayerSkillUse -= SkillUse;
            eventHub.OnSkillUsed -= AutoSkillUseCntUpdate;
            eventHub.OnSkillCoolEnd -= AutoSkillUseCntUpdate;
        }
    }
}