using Base.Data;
using Base.Managers;
using Battle;
using Cysharp.Threading.Tasks;
using Growth.Skill;
using System.Threading;
using UnityEngine;

namespace UI.Skill_Set
{
    public class SkillButtonPresenter : MonoBehaviour, IManager
    {
        [SerializeField] private SkillButtonView[] btnViewArr;

        private EventHub eventHub;
        private EquipSkillController plEquipSkillController;
        private EquipSkill[] plEquipSkill;
        private Player pl;
        // 0 : 기본, 1 : 선택됨
        [SerializeField] private Sprite[] borderArr;
        public int GetOrder() => 99; //UIPresnter와 겹쳐서 99로 조정했습니다
        public void Init()
        {
            eventHub = GameManager.Instance.GetGameSystem<EventHub>();
            if (eventHub == null)
            {
                // Debug.LogWarning("SkillButtonPresenter에서 eventHub 찾지 못함");
                return;
            }
            pl = GameManager.Instance.GetGameSystem<PlayerManager>().Player;
            if (pl == null)
            {
                // Debug.LogWarning("SkillButtonPresenter에서 Player 찾지 못함");
                return;
            }
            plEquipSkillController = pl.ESController;
            plEquipSkill = plEquipSkillController.EquipSkillArr;
            for (int i = 0; i < 6; i++)
            {
                if (plEquipSkill[i] == null)
                {
                    // Debug.LogWarning($"{i}번째 EquipSkill 없음");
                    continue;
                }
                int index = i;
                SkillButtonView tBtnView = btnViewArr[index];
                if (btnViewArr[index] == null)
                {
                    // Debug.LogWarning($"{index}번 TestBtnView 연결 안 됨");
                    continue;
                }
                tBtnView.ButtonEventSubscribe(() => SkillUseToMonster(index));
                CooltimeCheckTask(index).Forget();
                ActiveSkillSO skillData = plEquipSkill[index].Skill.ActiveSkillData;
<<<<<<< HEAD:Assets/Game/Personal/HagYun/Script/Skill/UI/SkillButtonPresenter.cs
                tBtnView.SkillIconImageChange(skillData.skillIcon, skillData.Targeting == TargetingMode.Homing);
=======
                tBtnView.SkillIconImageChange(skillData.skillIcon);
>>>>>>> main:Assets/Game/UI/UI/Scripts/UI_Set/Skill_Set/SkillButtonPresenter.cs
            }
            EquipSkillEventRemove();
            EquipSkillEventSet();
        }
        void SkillUseToMonster(int index) => eventHub.PlayerSkillUse(index);
        public void OnDestroyFeat()
        {
            EquipSkillEventRemove();
            foreach(SkillButtonView btnView in btnViewArr)
            {
                btnView.OnDestroyFeat();
            }
        }
        async UniTaskVoid CooltimeCheckTask(int index)
        {
            CancellationToken ct = this.GetCancellationTokenOnDestroy();
            while (true)
            {
                if (pl.IsDead)
                {
                    await UniTask.WaitUntil(() => !pl.IsDead, PlayerLoopTiming.Update, ct);
                }
                CooltimeUpdate(index);
                await UniTask.DelayFrame(10, PlayerLoopTiming.Update, ct);
                
            }
        }
        void CooltimeUpdate(int index)
        {
            EquipSkill tESkill = plEquipSkill[index];
            SkillButtonView tbv = btnViewArr[index];
            if (tESkill == null || tbv == null) return;
            else if (!tESkill.IsCooltime)
            {
                if (tbv.IsCooltimeMaskActiveState) tbv.CooltimeEnd();
                return;
            }
            if (!tbv.IsCooltimeMaskActiveState) tbv.CooltimeStart();
            float cooltimeValue = tESkill.CurCooltime / tESkill.MaxCooltime;
            tbv.CooltimeShowUpdate(cooltimeValue);

        }
        void BtnCooltimeStartEvent(int index) => btnViewArr[index].CooltimeStart();
        void BtnCooltimeEndEvent(int index) => btnViewArr[index].CooltimeEnd();
        void SkillIconSet(int index)
        {
<<<<<<< HEAD:Assets/Game/Personal/HagYun/Script/Skill/UI/SkillButtonPresenter.cs
            ActiveSkillSO tSkillData = plEquipSkill[index].Skill.ActiveSkillData;
            btnViewArr[index].SkillIconImageChange(tSkillData.skillIcon, tSkillData.Targeting == TargetingMode.Homing);
=======
            btnViewArr[slotIndex].SkillIconImageChange(aSkill.ActiveSkillData.skillIcon);
>>>>>>> main:Assets/Game/UI/UI/Scripts/UI_Set/Skill_Set/SkillButtonPresenter.cs
        }
        void SkillIconUnset(int index) => btnViewArr[index].SkillIconImageChange(null, false);
        int selectSkillNum = 0;
        void SkillSelect(int index)
        {
            SkillButtonView targetSkillBtnView = btnViewArr[index];
            if(!targetSkillBtnView.IsSelected)
            {
                btnViewArr[selectSkillNum].SkillUnset(borderArr[0]);
            }
            selectSkillNum=index;
            targetSkillBtnView.SkillSelect(borderArr[1]);
        }
        void SkillSelectCancel(int index)
        {
            SkillButtonView targetSkillBtnView = btnViewArr[index];
            if(targetSkillBtnView.IsSelected)
            {
                targetSkillBtnView.SkillUnset(borderArr[0]);
            }
        }
        void EquipSkillEventSet()
        {
            eventHub.OnSkillUsed += BtnCooltimeStartEvent;
            eventHub.OnSkillCoolEnd += BtnCooltimeEndEvent;

            eventHub.OnSkillSet += SkillIconSet;
            eventHub.OnSkillUnset += SkillIconUnset;
        }
        void EquipSkillEventRemove()
        {
            eventHub.OnSkillUsed -= BtnCooltimeStartEvent;
            eventHub.OnSkillCoolEnd -= BtnCooltimeEndEvent;

            eventHub.OnSkillSet -= SkillIconSet;
            eventHub.OnSkillUnset -= SkillIconUnset;
        }

    }
}