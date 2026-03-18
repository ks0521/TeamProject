using Battle;
using Cysharp.Threading.Tasks;
using Growth.Skill;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
namespace Personal.HagYun
{
    public enum ESCEventType
    {
        CooltimeStart,
        CooltimeUpdate,
        CooltimeEnd,
        CastingStart,
        CastingEnd
    }
    public class EquipSkillControllerEvent
    {
        public event Action OnCastingStart;
        public event Action OnCastingEnd;
        public void RaiseCastingStart() => OnCastingStart?.Invoke();
        public void RaiseCastingEnd() => OnCastingEnd?.Invoke();
    }
    public class EquipSkillController : MonoBehaviour
    {
        // test
        public static EquipSkillController esc;
        public SpriteRenderer sr;

        // skill pool for get skill
        [SerializeField] SkillPool skillPool;

        // equip skill
        [Serializable]
        struct EquipSkillSet
        {
            //public readonly int num;
            EquipSkill eSkill;
            public EquipSkill ESkill => eSkill;
            public Priority priority;
            public bool isEquipped;
            //public bool IsSkillUsePossible => eSkill != null && !eSkill.IsCooltime;
            public bool IsSkillUsePossible => isEquipped && !eSkill.IsCooltime;
            public Skill Skill => eSkill.Skill;
            //public EquipSkillSet(int index)
            //{
            //    num = index;
            //    eSkill = new EquipSkill();
            //    isEquipped = false;
            //    priority = Priority.Mid;
            //}
            public void Init()
            {
                eSkill = new EquipSkill();
            }
        }
        [SerializeField] private EquipSkillSet[] equipSkillArr;
        public EquipSkill this[int index] => equipSkillArr[index].ESkill;
        int skillCnt;
        // skill event
        private EquipSkillControllerEvent eventSet = new EquipSkillControllerEvent();

        // skill ready
        public bool IsSkillReady { get; private set; }
        [SerializeField] CircleCollider2D col;

        [Range(0f, 1f)] private float skillFireTimeValue = 0.5f;
        public bool IsCasting { get; private set; }
        //private Player owner;
        public void PlOwnerSet(Player pl) => Skill.SetPlOwner(pl);
        private void Awake()
        {
            esc = this;
        }
        void Start()
        {
            Init(GetComponent<Player>());
        }
        private void OnDestroy()
        {
            UnsubscribeUseSkillPossibleCntAll();
        }
        public void Init(Character cha)
        {
            switch (cha)
            {
                case Player pl:
                    PlOwnerSet(pl);
                    if (Skill.PlOwner == null) Debug.LogWarning("skill에 플레이어 주입 안 됨");
                    SkillEquipInit();
                    AutoSkillUsePossibleCntInit();
                    break;
                    //case Monster mon:
                    //    break;
                    //case null:
                    //    break;
                    //default:
                    //    break;
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
        private void Update()
        {
            SkillInput();
        }
        void ColliderSizeChange(int index)
        {
            float colRadius = equipSkillArr[index].Skill.Data.range / 2;
            col.radius = colRadius;
        }
        public void SkillReady(int index)
        {
            ColliderSizeChange(index);
        }
        // test용 bool
        public bool isTestAutoSkill;
        public void SkillInput()
        {
            // test
            if(Input.GetKeyDown(KeyCode.Return))
            {
                if (!equipSkillArr[2].isEquipped && skillPool.TryGetSkill(2, out Skill skill))
                {
                    SkillEquip(2, skill);
                    Debug.Log($"skillPool에서 {2}번 스킬 장착");
                }
            }
            if(Input.GetKeyDown(KeyCode.Space))
            {
                isTestAutoSkill = !isTestAutoSkill;
            }

            if(isTestAutoSkill)
            {
                AutoSkillUse();
            }
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                TrySkillUse(0);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                TrySkillUse(1);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                TrySkillUse(2);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                TrySkillUse(3);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                TrySkillUse(4);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                TrySkillUse(5);
            }
        }

        public void SkillEquip(int index, Skill targetSkill, bool isInit = false)
        {
            equipSkillArr[index].ESkill.SkillSet(targetSkill, isInit);
            equipSkillArr[index].priority = Priority.Mid;
            equipSkillArr[index].isEquipped = true;
            skillCnt++;
            SetUseSkillPossibleCnt();
            DecreaseUseSkillPossibleCnt();
        }
        public void SkillUnequip(int index)
        {
            equipSkillArr[index].ESkill.SkillUnset();
            equipSkillArr[index].isEquipped = false;
            skillCnt--;
            SetUseSkillPossibleCnt();
        }
        async UniTaskVoid CastingStartTask(int index, Character cha)
        {
            eventSet.RaiseCastingStart();
            IsCasting = true;

            sr.color = Color.blue;

            float baseCastingTime = equipSkillArr[index].Skill.Data.castingTime;
            float curCastingTime = baseCastingTime;
            float castingTimeValue = 1f;

            while (skillFireTimeValue < castingTimeValue)
            {
                castingTimeValue = curCastingTime / baseCastingTime;
                curCastingTime -= Time.deltaTime; // * owner의 캐스팅 시간 감소 속도
                await UniTask.Yield(Skill.PlOwner.GetCancellationTokenOnDestroy());
                if (Skill.PlOwner == null) return;
            }

            sr.color = Color.yellow;
            equipSkillArr[index].ESkill.SkillUse(cha);

            while (0 < castingTimeValue)
            {
                castingTimeValue = curCastingTime / baseCastingTime;
                curCastingTime -= Time.deltaTime; // * owner의 캐스팅 시간 감소 속도
                await UniTask.Yield(Skill.PlOwner.GetCancellationTokenOnDestroy());
                if (Skill.PlOwner == null) return;
            }

            sr.color = Color.white;

            eventSet.RaiseCastingEnd();
            IsCasting = false;
        }
        bool CheckSkillUsePossible(int index)
        {
            if (!equipSkillArr[index].IsSkillUsePossible)
            {
                Debug.LogWarning($"{index}번 자리에 장착된 스킬 없음 or 쿨타임");
                return false;
            }
            else if (IsCasting)
            {
                Debug.LogWarning("캐스팅중");
                return false;
            }
            //else if (equipSkillArr[index].ESkill.IsCooltime)
            //{
            //    Debug.LogWarning($"{index}번 자리에 장착된 스킬 쿨타임");
            //    return false;
            //}
            else return true;

        }
        bool TryGetTarget(int skillIndex, out Monster mon)
        {
            Skill tSkill = equipSkillArr[skillIndex].Skill;
            Vector2 plPos = tSkill.OwnerPos;
            int getNearMonCnt = OverlapChecker.GetCircleTargetsCount(plPos, tSkill.Data.range, tSkill.TargetMask);
            if (OverlapChecker.TryGetNearTarget(plPos, getNearMonCnt, out Collider2D targetCol))
            {
                mon = targetCol.GetComponent<Monster>();
                return mon != null;
            }
            mon = null;
            return false;
        }
        void SkillUse(int index, Monster mon)
        {
            CastingStartTask(index, mon).Forget();
        }
        public bool TrySkillUse(int index)
        {
            if (!CheckSkillUsePossible(index)) return false;
            else if (TryGetTarget(index, out Monster mon))
            {
                SkillUse(index, mon);
                return true;
            }
            return false;
        }
        //public bool IsForceSkillSelect { get; private set; }
        // auto skill use
        [SerializeField] int autoSkillUsePossibleCnt = 0;
        void DecreaseUseSkillPossibleCnt() => autoSkillUsePossibleCnt--;
        void EncreaseUseSkillPossibleCnt() => autoSkillUsePossibleCnt++;
        void SetUseSkillPossibleCnt() => autoSkillUsePossibleCnt = skillCnt;
        public void AutoSkillUsePossibleCntInit()
        {
            PrisoritySkillNumSetInitAll();
            SubscribeUseSkillPossibleCntAll();
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
            if(!set.IsInSkill || !set.isEquipArr[index])
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
                    if (TryGetTarget(autoSkillUseOrderNum, out Monster mon))
                    {
                        // 몬스터가 있을 시 스킬 사용 + auto skill 순서를 다음 번호로 변경
                        SkillUse(autoSkillUseOrderNum++, mon);
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
                        if (TrySkillUse(autoSkillUseOrderNum))
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

        // event subscription Func for external use  
        public void SkillEquip1(Skill skill) => SkillEquip(0, skill);
        public void SkillEquip2(Skill skill) => SkillEquip(1, skill);
        public void SkillEquip3(Skill skill) => SkillEquip(2, skill);
        public void SkillEquip4(Skill skill) => SkillEquip(3, skill);
        public void SkillEquip5(Skill skill) => SkillEquip(4, skill);
        public void SkillEquip6(Skill skill) => SkillEquip(5, skill);
        public void SkillUnequip1() => SkillUnequip(0);
        public void SkillUnequip2() => SkillUnequip(1);
        public void SkillUnequip3() => SkillUnequip(2);
        public void SkillUnequip4() => SkillUnequip(3);
        public void SkillUnequip5() => SkillUnequip(4);
        public void SkillUnequip6() => SkillUnequip(5);
        public void SkillUse1() => TrySkillUse(0);
        public void SkillUse2() => TrySkillUse(1);
        public void SkillUse3() => TrySkillUse(2);
        public void SkillUse4() => TrySkillUse(3);
        public void SkillUse5() => TrySkillUse(4);
        public void SkillUse6() => TrySkillUse(5);

        // event add/remove for external use
        public void AddEventCastingStart(Action func) => eventSet.OnCastingStart += func;
        public void AddEventCastingEnd(Action func) => eventSet.OnCastingEnd += func;
        public void RemoveEventCastingStart(Action func) => eventSet.OnCastingStart -= func;
        public void RemoveEventCastingEnd(Action func) => eventSet.OnCastingEnd -= func;
    }
}